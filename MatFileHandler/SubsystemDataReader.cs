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
            var rawVariables = ReadRawVariables(bytes, subsystemData);

            // Parse subsystem data.
            var mainVariable = rawVariables[0].DataElement as IStructureArray
                               ?? throw new HandlerException("Subsystem data must be a structure array.");
            var mcosData = mainVariable["MCOS", 0] as Opaque
                           ?? throw new HandlerException("MCOS data must be an opaque object.");
            var opaqueData = mcosData.RawData as ICellArray
                             ?? throw new HandlerException("Opaque data must be a cell array.");
            var info = (opaqueData[0] as IArrayOf<byte>)?.Data
                       ?? throw new HandlerException("Opaque data info must be a byte array.");
            var (offsets, position) = ReadOffsets(info, 0);
            var fieldNames = ReadFieldNames(info, position, offsets[1]);
            var numberOfClasses = ((offsets[3] - offsets[2]) / 16) - 1;
            var classIdToName = ReadClassIdToName(info, offsets, fieldNames, numberOfClasses);
            var numberOfEmbeddedObjects = (offsets[4] - offsets[3] - 8) / 16;
            var embeddedObjectPositionsToValues = ReadEmbeddedObjectPositionsToValues(info, offsets, numberOfEmbeddedObjects);
            var numberOfObjects = ((offsets[5] - offsets[4]) / 24) - 1;
            var objectClasses = ReadObjectClassInformations(info, offsets, numberOfObjects);
            var numberOfObjectPositions = objectClasses.NumberOfObjectPositions;
            var objectPositionsToValues = ReadObjectPositionsToValues(info, offsets, numberOfObjectPositions);
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

        private static Dictionary<int, Dictionary<int, int>> ReadObjectPositionsToValues(byte[] info, int[] offsets, int numberOfObjectPositions)
        {
            using var stream = new MemoryStream(info, offsets[5], offsets[6] - offsets[5]);
            using var reader = new BinaryReader(stream);
            return ReadObjectPositionsToValuesMapping(reader, numberOfObjectPositions);
        }

        private static ObjectClasses ReadObjectClassInformations(byte[] info, int[] offsets, int numberOfObjects)
        {
            using var stream = new MemoryStream(info, offsets[4], offsets[5] - offsets[4]);
            using var reader = new BinaryReader(stream);
            return ReadObjectClasses(reader, numberOfObjects);
        }

        private static Dictionary<int, Dictionary<int, int>> ReadEmbeddedObjectPositionsToValues(byte[] info, int[] offsets, int numberOfEmbeddedObjects)
        {
            using var stream = new MemoryStream(info, offsets[3], offsets[4] - offsets[3]);
            using var reader = new BinaryReader(stream);
            return ReadEmbeddedObjectPositionsToValuesMapping(reader, numberOfEmbeddedObjects);
        }

        private static Dictionary<int, string> ReadClassIdToName(byte[] info, int[] offsets, string[] fieldNames, int numberOfClasses)
        {
            using var stream = new MemoryStream(info, offsets[2], offsets[3] - offsets[2]);
            using var reader = new BinaryReader(stream);
            return ReadClassNames(reader, fieldNames, numberOfClasses);
        }

        private static List<RawVariable> ReadRawVariables(byte[] bytes, SubsystemData subsystemData)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            reader.ReadBytes(8);
            return MatFileReader.ReadRawVariables(reader, -1, subsystemData);
        }

        private static (Dictionary<int, SubsystemData.ClassInfo> classInfos,
            Dictionary<int, SubsystemData.ObjectInfo> objectInfos)
            GatherClassAndObjectInformation(
                Dictionary<int, string> classIdToName,
                string[] fieldNames,
                ObjectClasses objectClasses,
                Dictionary<int, Dictionary<int, int>> objectPositionsToValues,
                Dictionary<int, Dictionary<int, int>> embeddedObjectPositionsToValues)
        {
            var classInfos = new Dictionary<int, SubsystemData.ClassInfo>();
            foreach (var classId in classIdToName.Keys)
            {
                var className = classIdToName[classId];
                var fieldIds = new SortedSet<int>();
                foreach (var objectPosition in objectPositionsToValues.Keys)
                {
                    var foundClassId = objectClasses.GetClassIdByObjectPosition(objectPosition);
                    if (foundClassId != classId)
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
                    var foundClassId = objectClasses.GetClassIdByEmbeddedObjectPosition(objectPosition);
                    if (foundClassId != classId)
                    {
                        continue;
                    }

                    foreach (var fieldId in embeddedObjectPositionsToValues[objectPosition].Keys)
                    {
                        fieldIds.Add(fieldId);
                    }
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
                var foundKey = objectClasses.GetKeyByObjectPosition(objectPosition);
                objectInfos[foundKey] = new SubsystemData.ObjectInfo(objectPositionsToValues[objectPosition]);
            }

            foreach (var embeddedObjectPosition in embeddedObjectPositionsToValues.Keys)
            {
                var foundKey = objectClasses.GetKeyByEmbeddedObjectPosition(embeddedObjectPosition);
                objectInfos[foundKey] = new SubsystemData.ObjectInfo(embeddedObjectPositionsToValues[embeddedObjectPosition]);
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

        private static Dictionary<int, Dictionary<int, int>> ReadEmbeddedObjectPositionsToValuesMapping(
            BinaryReader reader, int numberOfObjects)
        {
            var result = new Dictionary<int, Dictionary<int, int>>();
            reader.ReadBytes(8);
            for (var objectPosition = 1; objectPosition <= numberOfObjects; objectPosition++)
            {
                var a = reader.ReadInt32();
                var fieldIndex = reader.ReadInt32();
                var c = reader.ReadInt32();
                var valueIndex = reader.ReadInt32();
                result[objectPosition] = new Dictionary<int, int> { [fieldIndex] = valueIndex };
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

        private static ObjectClasses ReadObjectClasses(
            BinaryReader reader,
            int numberOfObjects)
        {
            var result = new Dictionary<int, ObjectClassInformation>();
            var classIdFromObjectPosition = new Dictionary<int, int>();
            var classIdFromEmbeddedObjectPosition = new Dictionary<int, int>();
            var keyFromObjectPosition = new Dictionary<int, int>();
            var keyFromEmbeddedObjectPosition = new Dictionary<int, int>();
            reader.ReadBytes(24);
            var numberOfObjectPositions = 0;
            for (var i = 0; i < numberOfObjects; i++)
            {
                var classId = reader.ReadInt32();
                reader.ReadBytes(8);
                var embeddedObjectPosition = reader.ReadInt32();
                var objectPosition = reader.ReadInt32();
                var loadingOrder = reader.ReadInt32(); // Not used.
                classIdFromObjectPosition[objectPosition] = classId;
                classIdFromEmbeddedObjectPosition[embeddedObjectPosition] = classId;
                keyFromObjectPosition[objectPosition] = i + 1;
                keyFromEmbeddedObjectPosition[embeddedObjectPosition] = i + 1;
                if (objectPosition != 0)
                {
                    numberOfObjectPositions++;
                }
            }

            return new ObjectClasses(
                classIdFromObjectPosition,
                classIdFromEmbeddedObjectPosition,
                keyFromObjectPosition,
                keyFromEmbeddedObjectPosition,
                numberOfObjectPositions);
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
                        uintArray,
                        indexToObjectId,
                        classIndex,
                        subsystemData);
                }
            }

            return array;
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

        private class ObjectClasses
        {
            private readonly Dictionary<int, int> _classIdFromObjectPosition;
            private readonly Dictionary<int, int> _classIdFromEmbeddedObjectPosition;
            private readonly Dictionary<int, int> _keyFromObjectPosition;
            private readonly Dictionary<int, int> _keyFromEmbeddedObjectPosition;

            public ObjectClasses(
                Dictionary<int, int> classIdFromObjectPosition,
                Dictionary<int, int> classIdFromEmbeddedObjectPosition,
                Dictionary<int, int> keyFromObjectPosition,
                Dictionary<int, int> keyFromEmbeddedObjectPosition,
                int numberOfObjectPositions)
            {
                _classIdFromObjectPosition = classIdFromObjectPosition;
                _classIdFromEmbeddedObjectPosition = classIdFromEmbeddedObjectPosition;
                _keyFromObjectPosition = keyFromObjectPosition;
                _keyFromEmbeddedObjectPosition = keyFromEmbeddedObjectPosition;
                NumberOfObjectPositions = numberOfObjectPositions;
            }

            public int NumberOfObjectPositions { get; }

            public int GetClassIdByObjectPosition(int objectPosition)
                => _classIdFromObjectPosition[objectPosition];

            public int GetClassIdByEmbeddedObjectPosition(int embeddedObjectPosition)
                => _classIdFromEmbeddedObjectPosition[embeddedObjectPosition];

            public int GetKeyByObjectPosition(int objectPosition)
                => _keyFromObjectPosition[objectPosition];

            public int GetKeyByEmbeddedObjectPosition(int embeddedObjectPosition)
                => _keyFromEmbeddedObjectPosition[embeddedObjectPosition];
        }
    }
}