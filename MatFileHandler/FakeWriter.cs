// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MatFileHandler
{
    /// <summary>
    /// A simulated writer of .mat files that just calculate the length of data that would be written.
    /// </summary>
    internal class FakeWriter
    {
        /// <summary>
        /// Gets current position of the writer.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Write contents of a numerical array.
        /// </summary>
        /// <param name="array">A numerical array.</param>
        /// <param name="name">Name of the array.</param>
        public void WriteNumericalArrayContents(IArray array, string name)
        {
            WriteArrayFlags();
            WriteDimensions(array.Dimensions);
            WriteName(name);
            WriteNumericalArrayValues(array);
        }

        /// <summary>
        /// Write contents of a char array.
        /// </summary>
        /// <param name="charArray">A char array.</param>
        /// <param name="name">Name of the array.</param>
        public void WriteCharArrayContents(ICharArray charArray, string name)
        {
            WriteArrayFlags();
            WriteDimensions(charArray.Dimensions);
            WriteName(name);
            WriteDataElement(GetLengthOfByteArray<ushort>(charArray.String.Length));
        }

        /// <summary>
        /// Write contents of a sparse array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">A sparse array.</param>
        /// <param name="name">Name of the array.</param>
        public void WriteSparseArrayContents<T>(
            ISparseArrayOf<T> array,
            string name)
            where T : unmanaged, IEquatable<T>
        {
            (var rowsLength, var columnsLength, var dataLength, var nonZero) = PrepareSparseArrayData(array);
            WriteSparseArrayFlags();
            WriteDimensions(array.Dimensions);
            WriteName(name);
            WriteSparseArrayValues<T>(rowsLength, columnsLength, dataLength);
        }

        /// <summary>
        /// Write contents of a structure array.
        /// </summary>
        /// <param name="array">A structure array.</param>
        /// <param name="name">Name of the array.</param>
        public void WriteStructureArrayContents(IStructureArray array, string name)
        {
            WriteArrayFlags();
            WriteDimensions(array.Dimensions);
            WriteName(name);
            WriteFieldNames(array.FieldNames);
            WriteStructureArrayValues(array);
        }

        /// <summary>
        /// Write contents of a cell array.
        /// </summary>
        /// <param name="array">A cell array.</param>
        /// <param name="name">Name of the array.</param>
        public void WriteCellArrayContents(ICellArray array, string name)
        {
            WriteArrayFlags();
            WriteDimensions(array.Dimensions);
            WriteName(name);
            WriteCellArrayValues(array);
        }

        private void WriteTag()
        {
            Position += 8;
        }

        private void WriteShortTag()
        {
            Position += 4;
        }

        private void WriteWrappingContents<T>(T array, Action<FakeWriter> writeContents)
            where T : IArray
        {
            if (array.IsEmpty)
            {
                WriteTag();
                return;
            }

            WriteTag();
            writeContents(this);
        }

        private void WriteNumericalArrayValues(IArray value)
        {
            switch (value)
            {
                case IArrayOf<sbyte> sbyteArray:
                    WriteDataElement(GetLengthOfByteArray<sbyte>(sbyteArray.Data.Length));
                    break;
                case IArrayOf<byte> byteArray:
                    WriteDataElement(GetLengthOfByteArray<byte>(byteArray.Data.Length));
                    break;
                case IArrayOf<short> shortArray:
                    WriteDataElement(GetLengthOfByteArray<short>(shortArray.Data.Length));
                    break;
                case IArrayOf<ushort> ushortArray:
                    WriteDataElement(GetLengthOfByteArray<ushort>(ushortArray.Data.Length));
                    break;
                case IArrayOf<int> intArray:
                    WriteDataElement(GetLengthOfByteArray<int>(intArray.Data.Length));
                    break;
                case IArrayOf<uint> uintArray:
                    WriteDataElement(GetLengthOfByteArray<uint>(uintArray.Data.Length));
                    break;
                case IArrayOf<long> longArray:
                    WriteDataElement(GetLengthOfByteArray<long>(longArray.Data.Length));
                    break;
                case IArrayOf<ulong> ulongArray:
                    WriteDataElement(GetLengthOfByteArray<ulong>(ulongArray.Data.Length));
                    break;
                case IArrayOf<float> floatArray:
                    WriteDataElement(GetLengthOfByteArray<float>(floatArray.Data.Length));
                    break;
                case IArrayOf<double> doubleArray:
                    WriteDataElement(GetLengthOfByteArray<double>(doubleArray.Data.Length));
                    break;
                case IArrayOf<bool> boolArray:
                    WriteDataElement(boolArray.Data.Length);
                    break;
                case IArrayOf<ComplexOf<sbyte>> complexSbyteArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexSbyteArray.Data));
                    break;
                case IArrayOf<ComplexOf<byte>> complexByteArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexByteArray.Data));
                    break;
                case IArrayOf<ComplexOf<short>> complexShortArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexShortArray.Data));
                    break;
                case IArrayOf<ComplexOf<ushort>> complexUshortArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexUshortArray.Data));
                    break;
                case IArrayOf<ComplexOf<int>> complexIntArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexIntArray.Data));
                    break;
                case IArrayOf<ComplexOf<uint>> complexUintArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexUintArray.Data));
                    break;
                case IArrayOf<ComplexOf<long>> complexLongArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexLongArray.Data));
                    break;
                case IArrayOf<ComplexOf<ulong>> complexUlongArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexUlongArray.Data));
                    break;
                case IArrayOf<ComplexOf<float>> complexFloatArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexFloatArray.Data));
                    break;
                case IArrayOf<Complex> complexDoubleArray:
                    WriteComplexValues(GetLengthOfPairOfByteArrays(complexDoubleArray.Data));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void WriteName(string name)
        {
            var nameBytes = Encoding.ASCII.GetBytes(name);
            WriteDataElement(nameBytes.Length);
        }

        private void WriteArrayFlags()
        {
            WriteTag();
            Position += 8;
        }

        private void WriteDimensions(int[] dimensions)
        {
            var buffer = GetLengthOfByteArray<int>(dimensions.Length);
            WriteDataElement(buffer);
        }

        private unsafe int GetLengthOfByteArray<T>(int dataLength)
            where T : unmanaged
        {
            return dataLength * sizeof(T);
        }

        private unsafe int GetLengthOfPairOfByteArrays<T>(ComplexOf<T>[] data)
            where T : unmanaged
        {
            return data.Length * sizeof(T);
        }

        private unsafe int GetLengthOfPairOfByteArrays(Complex[] data)
        {
            return data.Length * sizeof(double);
        }

        private int CalculatePadding(int length)
        {
            var rem = length % 8;
            if (rem == 0)
            {
                return 0;
            }

            return 8 - rem;
        }

        private void WriteDataElement(int dataLength)
        {
            var maybePadding = 0;
            if (dataLength > 4)
            {
                WriteTag();
                Position += dataLength;
                maybePadding = CalculatePadding(dataLength + 8);
            }
            else
            {
                WriteShortTag();
                Position += 4;
            }

            Position += maybePadding;
        }

        private void WriteComplexValues(
            int dataLength)
        {
            WriteDataElement(dataLength);
            WriteDataElement(dataLength);
        }

        private void WriteSparseArrayValues<T>(int rowsLength, int columnsLength, int dataLength)
          where T : unmanaged
        {
            WriteDataElement(GetLengthOfByteArray<int>(rowsLength));
            WriteDataElement(GetLengthOfByteArray<int>(columnsLength));
            if (typeof(T) == typeof(double))
            {
                WriteDataElement(GetLengthOfByteArray<double>(dataLength));
            }
            else if (typeof(T) == typeof(Complex))
            {
                WriteDataElement(GetLengthOfByteArray<double>(dataLength));
                WriteDataElement(GetLengthOfByteArray<double>(dataLength));
            }
            else if (typeof(T) == typeof(bool))
            {
                WriteDataElement(dataLength);
            }
        }

        private (int rowIndexLength, int columnIndexLength, int dataLength, uint nonZero) PrepareSparseArrayData<T>(
            ISparseArrayOf<T> array)
            where T : struct, IEquatable<T>
        {
            var numberOfColumns = array.Dimensions[1];
            var numberOfElements = array.Data.Values.Count(value => !value.Equals(default));
            return (numberOfElements, numberOfColumns + 1, numberOfElements, (uint)numberOfElements);
        }

        private void WriteSparseArrayFlags()
        {
            WriteTag();
            Position += 8;
        }

        private void WriteFieldNames(IEnumerable<string> fieldNames)
        {
            var fieldNamesArray = fieldNames.Select(name => Encoding.ASCII.GetBytes(name)).ToArray();
            var maxFieldName = fieldNamesArray.Select(name => name.Length).Max() + 1;
            WriteDataElement(GetLengthOfByteArray<int>(1));
            var buffer = new byte[fieldNamesArray.Length * maxFieldName];
            var startPosition = 0;
            foreach (var name in fieldNamesArray)
            {
                for (var i = 0; i < name.Length; i++)
                {
                    buffer[startPosition + i] = name[i];
                }
                startPosition += maxFieldName;
            }
            WriteDataElement(buffer.Length);
        }

        private void WriteStructureArrayValues(IStructureArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                foreach (var name in array.FieldNames)
                {
                    WriteArray(array[name, i]);
                }
            }
        }

        private void WriteArray(IArray array, string variableName = "", bool isGlobal = false)
        {
            switch (array)
            {
                case ICharArray charArray:
                    WriteCharArray(charArray, variableName);
                    break;
                case ISparseArrayOf<double> doubleSparseArray:
                    WriteSparseArray(doubleSparseArray, variableName);
                    break;
                case ISparseArrayOf<Complex> complexSparseArray:
                    WriteSparseArray(complexSparseArray, variableName);
                    break;
                case ISparseArrayOf<bool> boolSparseArray:
                    WriteSparseArray(boolSparseArray, variableName);
                    break;
                case ICellArray cellArray:
                    WriteCellArray(cellArray, variableName);
                    break;
                case IStructureArray structureArray:
                    WriteStructureArray(structureArray, variableName);
                    break;
                default:
                    WriteNumericalArray(array, variableName);
                    break;
            }
        }

        private void WriteCharArray(ICharArray charArray, string name)
        {
            WriteWrappingContents(
                charArray,
                fakeWriter => fakeWriter.WriteCharArrayContents(charArray, name));
        }

        private void WriteSparseArray<T>(ISparseArrayOf<T> sparseArray, string name)
            where T : unmanaged, IEquatable<T>
        {
            WriteWrappingContents(
                sparseArray,
                fakeWriter => fakeWriter.WriteSparseArrayContents(sparseArray, name));
        }

        private void WriteCellArray(ICellArray cellArray, string name)
        {
            WriteWrappingContents(
                cellArray,
                fakeWriter => fakeWriter.WriteCellArrayContents(cellArray, name));
        }

        private void WriteCellArrayValues(ICellArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                WriteArray(array[i]);
            }
        }

        private void WriteStructureArray(
            IStructureArray structureArray,
            string name)
        {
            WriteWrappingContents(
                structureArray,
                fakeWriter => fakeWriter.WriteStructureArrayContents(structureArray, name));
        }

        private void WriteNumericalArray(
            IArray numericalArray,
            string name = "")
        {
            WriteWrappingContents(
                numericalArray,
                fakeWriter => fakeWriter.WriteNumericalArrayContents(numericalArray, name));
        }
    }
}