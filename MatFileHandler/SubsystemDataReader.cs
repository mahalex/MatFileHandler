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
        /// <returns>Subsystem data read.</returns>
        public static SubsystemData Read(byte[] bytes)
        {
            List<RawVariable> rawVariables = null;
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    reader.ReadBytes(8);
                    rawVariables = MatFileReader.ReadRawVariables(reader, -1);
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
            SubsystemData.ClassInfo[] classInformation = null;
            using (var stream = new MemoryStream(info, offsets[2], offsets[3] - offsets[2]))
            {
                using (var reader = new BinaryReader(stream))
                {
                    classInformation = ReadClassInformation(reader, fieldNames, numberOfClasses);
                }
            }
            var numberOfObjects = ((offsets[5] - offsets[4]) / 24) - 1;
            SubsystemData.ObjectInfo[] objectInformation = null;
            using (var stream = new MemoryStream(info, offsets[5], offsets[6] - offsets[5]))
            {
                using (var reader = new BinaryReader(stream))
                {
                    objectInformation = ReadObjectInformation(reader, numberOfObjects);
                }
            }

            var allFields = objectInformation.SelectMany(obj => obj.FieldLinks.Values);
            var data = new Dictionary<int, IArray>();
            foreach (var i in allFields)
            {
                data[i] = opaqueData[i + 2];
            }
            return new SubsystemData(classInformation, objectInformation, data);
        }

        private static SubsystemData.ObjectInfo ReadObjectInformation(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var fieldLinks = new Dictionary<int, int>();
            for (var i = 0; i < length; i++)
            {
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var index = x * y;
                var link = reader.ReadInt32();
                fieldLinks[index] = link;
            }
            return new SubsystemData.ObjectInfo(fieldLinks);
        }

        private static SubsystemData.ObjectInfo[] ReadObjectInformation(BinaryReader reader, int numberOfObjects)
        {
            var result = new SubsystemData.ObjectInfo[numberOfObjects];
            reader.ReadBytes(8);
            for (var objectIndex = 0; objectIndex < numberOfObjects; objectIndex++)
            {
                result[objectIndex] = ReadObjectInformation(reader);
                var position = reader.BaseStream.Position;
                if (position % 8 != 0)
                {
                    reader.ReadBytes(8 - (int)(position % 8));
                }
            }
            return result;
        }

        private static SubsystemData.ClassInfo[] ReadClassInformation(
            BinaryReader reader,
            string[] fieldNames,
            int numberOfClasses)
        {
            var result = new SubsystemData.ClassInfo[numberOfClasses];
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
                var numberOfFields = indices[i + 1] - indices[i] - 1;
                var names = new string[numberOfFields];
                Array.Copy(fieldNames, indices[i], names, 0, numberOfFields);
                var className = fieldNames[indices[i + 1] - 1];
                result[i] = new SubsystemData.ClassInfo(className, names);
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
    }
}
