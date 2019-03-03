// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatFileHandler
{
    /// <summary>
    /// Reader for "subsystem data" in .mat files.
    /// </summary>
    internal class SubsystemDataReader
    {
        /// <summary>
        /// Read subsystem data from a given byte array.
        /// </summary>
        /// <param name="bytes">Byte array with the data.</param>
        /// <param name="subsystemData">
        /// Link to the existing subsystem data; this will be put in nested OpaqueLink objects
        /// and later replaced with the subsystem data that we are currently reading.</param>
        /// <returns>Subsystem data read.</returns>
        public static SubsystemData Read(byte[] bytes, SubsystemData subsystemData)
        {
            List<RawVariable> rawVariables = null;
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    reader.ReadBytes(8);
                    rawVariables = MatFileReader.ReadRawVariables(reader, -1, subsystemData);
                }
            }

            // Parse subsystem data.
            var mainVariable = rawVariables[0].DataElement as IStructureArray;
            var mcosData = mainVariable["MCOS", 0] as Opaque;
            var opaqueData = mcosData.RawData as ICellArray;
            var info = (opaqueData[0] as IArrayOf<byte>).Data;
            var (offsets, position) = ReadOffsets(info, 0);
            var fieldNames = ReadFieldNames(info, position, offsets[1]);
            var numberOfClasses = ((offsets[3] - offsets[2]) / 16) - 1;
            Dictionary<int, string> classIdToName = null;
            using (var stream = new MemoryStream(info, offsets[2], offsets[3] - offsets[2]))
            {
                using (var reader = new BinaryReader(stream))
                {
                    classIdToName = ReadClassNames(reader, fieldNames, numberOfClasses);
                }
            }

            var numberOfEmbeddedObjects = (offsets[4] - offsets[3] - 8) / 16;
            Dictionary<int, EmbeddedObjectInformation> embeddedObjectPositionsToValues = null;
            using (var stream = new MemoryStream(info, offsets[3], offsets[4] - offsets[3]))
            {
                using (var reader = new BinaryReader(stream))
                {
                    embeddedObjectPositionsToValues =
                        ReadEmbeddedObjectPositionsToValuesMapping(reader, numberOfEmbeddedObjects);
                }
            }

            var numberOfObjects = ((offsets[5] - offsets[4]) / 24) - 1;
            Dictionary<int, ObjectClassInformation> objectClasses = null;
            using (var stream = new MemoryStream(info, offsets[4], offsets[5] - offsets[4]))
            {
                using (var reader = new BinaryReader(stream))
                {
                    objectClasses = ReadObjectClasses(reader, numberOfObjects);
                }
            }

            var numberOfObjectPositions = objectClasses.Values.Count(x => x.ObjectPosition != 0);

            Dictionary<int, Dictionary<int, int>> objectPositionsToValues = null;
            using (var stream = new MemoryStream(info, offsets[5], offsets[6] - offsets[5]))
            {
                using (var reader = new BinaryReader(stream))
                {
                    objectPositionsToValues = ReadObjectPositionsToValuesMapping(reader, numberOfObjectPositions);
                }
            }

            var (classInformation, objectInformation) =
                GatherClassAndObjectInformation(
                    classIdToName,
                    fieldNames,
                    objectClasses,
                    objectPositionsToValues,
                    embeddedObjectPositionsToValues);

            var allFields = objectInformation.Values.SelectMany(obj => obj.FieldLinks.Values);
            var data = new Dictionary<int, IArray>();
            foreach (var i in allFields)
            {
                data[i] = TransformOpaqueData(opaqueData[i + 2], subsystemData);
            }

            return new SubsystemData(classInformation, objectInformation, data);
        }

        private static (Dictionary<int, SubsystemData.ClassInfo>, Dictionary<int, SubsystemData.ObjectInfo>)
            GatherClassAndObjectInformation(
                Dictionary<int, string> classIdToName,
                string[] fieldNames,
                Dictionary<int, ObjectClassInformation> objectClasses,
                Dictionary<int, Dictionary<int, int>> objectPositionsToValues,
                Dictionary<int, EmbeddedObjectInformation> embeddedObjectPositionsToValues)
        {
            var classInfos = new Dictionary<int, SubsystemData.ClassInfo>();
            var newEmbeddedObjectPositionsToValues = new Dictionary<int, Dictionary<int, int>>();
            foreach (var classId in classIdToName.Keys)
            {
                var className = classIdToName[classId];
                var fieldIds = new SortedSet<int>();
                foreach (var objectPosition in objectPositionsToValues.Keys)
                {
                    var keyValuePair = objectClasses.First(pair => pair.Value.ObjectPosition == objectPosition);
                    if (keyValuePair.Value.ClassId != classId)
                    {
                        continue;
                    }

                    foreach (var fieldId in objectPositionsToValues[objectPosition].Keys)
                    {
                        fieldIds.Add(fieldId);
                    }
                }

                foreach (var objectPosition in embeddedObjectPositionsToValues.Keys)
                {
                    var keyValuePair = objectClasses.First(pair => pair.Value.EmbeddedObjectPosition == objectPosition);
                    if (keyValuePair.Value.ClassId != classId)
                    {
                        continue;
                    }

                    fieldIds.Add(embeddedObjectPositionsToValues[objectPosition].FieldIndex);
                    var d = new Dictionary<int, int>();
                    var embeddedInfo = embeddedObjectPositionsToValues[objectPosition];
                    d[embeddedInfo.FieldIndex] = embeddedInfo.ValueIndex;
                    newEmbeddedObjectPositionsToValues[objectPosition] = d;
                }

                var fieldToIndex = new Dictionary<string, int>();
                foreach (var fieldId in fieldIds)
                {
                    fieldToIndex[fieldNames[fieldId - 1]] = fieldId;
                }

                classInfos[classId] = new SubsystemData.ClassInfo(className, fieldToIndex);
            }

            var objectInfos = new Dictionary<int, SubsystemData.ObjectInfo>();
            foreach (var objectPosition in objectPositionsToValues.Keys)
            {
                var keyValuePair = objectClasses.First(pair => pair.Value.ObjectPosition == objectPosition);
                objectInfos[keyValuePair.Key] = new SubsystemData.ObjectInfo(objectPositionsToValues[objectPosition]);
            }

            foreach (var objectPosition in embeddedObjectPositionsToValues.Keys)
            {
                var keyValuePair = objectClasses.First(pair => pair.Value.EmbeddedObjectPosition == objectPosition);
                objectInfos[keyValuePair.Key] =
                    new SubsystemData.ObjectInfo(newEmbeddedObjectPositionsToValues[objectPosition]);
            }

            return (classInfos, objectInfos);
        }

        private static Dictionary<int, string> ReadClassNames(
            BinaryReader reader,
            string[] fieldNames,
            int numberOfClasses)
        {
            var result = new Dictionary<int, string>();
            var indices = new int[numberOfClasses + 1];
            for (var i = 0; i <= numberOfClasses; i++)
            {
                reader.ReadInt32();
                indices[i] = reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
            }

            for (var i = 0; i < numberOfClasses; i++)
            {
                result[i + 1] = fieldNames[indices[i + 1] - 1];
            }

            return result;
        }

        private static Dictionary<int, EmbeddedObjectInformation> ReadEmbeddedObjectPositionsToValuesMapping(
            BinaryReader reader, int numberOfObjects)
        {
            var result = new Dictionary<int, EmbeddedObjectInformation>();
            reader.ReadBytes(8);
            for (var objectPosition = 1; objectPosition <= numberOfObjects; objectPosition++)
            {
                var a = reader.ReadInt32();
                var fieldIndex = reader.ReadInt32();
                var c = reader.ReadInt32();
                var valueIndex = reader.ReadInt32();
                result[objectPosition] = new EmbeddedObjectInformation(fieldIndex, valueIndex);
            }

            return result;
        }

        private static string[] ReadFieldNames(byte[] bytes, int startPosition, int numberOfFields)
        {
            var result = new string[numberOfFields];
            var position = startPosition;
            for (var i = 0; i < numberOfFields; i++)
            {
                var list = new List<byte>();
                while (bytes[position] != 0)
                {
                    list.Add(bytes[position]);
                    position++;
                }

                result[i] = Encoding.ASCII.GetString(list.ToArray());
                position++;
            }

            return result;
        }

        private static Dictionary<int, int> ReadFieldToFieldDataMapping(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var result = new Dictionary<int, int>();
            for (var i = 0; i < length; i++)
            {
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var index = x * y;
                var link = reader.ReadInt32();
                result[index] = link;
            }

            return result;
        }

        private static Dictionary<int, ObjectClassInformation> ReadObjectClasses(
            BinaryReader reader,
            int numberOfObjects)
        {
            var result = new Dictionary<int, ObjectClassInformation>();
            reader.ReadBytes(24);
            for (var i = 0; i < numberOfObjects; i++)
            {
                var classId = reader.ReadInt32();
                reader.ReadBytes(8);
                var embeddedObjectPosition = reader.ReadInt32();
                var objectPosition = reader.ReadInt32();
                var loadingOrder = reader.ReadInt32();
                result[i + 1] =
                    new ObjectClassInformation(embeddedObjectPosition, objectPosition, loadingOrder, classId);
            }

            return result;
        }

        private static Dictionary<int, Dictionary<int, int>> ReadObjectPositionsToValuesMapping(
            BinaryReader reader,
            int numberOfObjects)
        {
            var result = new Dictionary<int, Dictionary<int, int>>();
            reader.ReadBytes(8);
            for (var objectPosition = 1; objectPosition <= numberOfObjects; objectPosition++)
            {
                result[objectPosition] = ReadFieldToFieldDataMapping(reader);
                var position = reader.BaseStream.Position;
                if (position % 8 != 0)
                {
                    reader.ReadBytes(8 - (int)(position % 8));
                }
            }

            return result;
        }

        private static (int[] offsets, int newPosition) ReadOffsets(byte[] bytes, int startPosition)
        {
            var position = startPosition;
            var offsets = new List<int>();
            while (true)
            {
                var next = BitConverter.ToInt32(bytes, position);
                position += 4;
                if (next == 0)
                {
                    if (position % 8 != 0)
                    {
                        position += 4;
                    }

                    break;
                }

                offsets.Add(next);
            }

            return (offsets.ToArray(), position);
        }

        private static IArray TransformOpaqueData(IArray array, SubsystemData subsystemData)
        {
            if (array is MatNumericalArrayOf<uint> uintArray)
            {
                if (uintArray.Data[0] == 3707764736u)
                {
                    var (dimensions, indexToObjectId, classIndex) = DataElementReader.ParseOpaqueData(uintArray.Data);
                    return new OpaqueLink(
                        uintArray.Name,
                        string.Empty,
                        string.Empty,
                        dimensions,
                        array as DataElement,
                        indexToObjectId,
                        classIndex,
                        subsystemData);
                }
            }

            return array;
        }

        private struct EmbeddedObjectInformation
        {
            public EmbeddedObjectInformation(int fieldIndex, int valueIndex)
            {
                FieldIndex = fieldIndex;
                ValueIndex = valueIndex;
            }

            public int FieldIndex { get; }

            public int ValueIndex { get; }
        }

        private struct ObjectClassInformation
        {
            public ObjectClassInformation(int embeddedObjectPosition, int objectPosition, int loadingOrder, int classId)
            {
                EmbeddedObjectPosition = embeddedObjectPosition;
                ObjectPosition = objectPosition;
                LoadingOrder = loadingOrder;
                ClassId = classId;
            }

            public int ClassId { get; }

            public int EmbeddedObjectPosition { get; }

            public int LoadingOrder { get; }

            public int ObjectPosition { get; }
        }
    }
}