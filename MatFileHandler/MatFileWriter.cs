// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MatFileHandler
{
    /// <summary>
    /// Class for writing .mat files.
    /// </summary>
    public class MatFileWriter
    {
        private readonly MatFileWriterOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileWriter"/> class with a stream and default options.
        /// </summary>
        /// <param name="stream">Output stream.</param>
        public MatFileWriter(Stream stream)
        {
            Stream = stream;
            _options = MatFileWriterOptions.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileWriter"/> class with a stream.
        /// </summary>
        /// <param name="stream">Output stream.</param>
        /// <param name="options">Options to use for file writing.</param>
        public MatFileWriter(Stream stream, MatFileWriterOptions options)
        {
            Stream = stream;
            _options = options;
        }

        private Stream Stream { get; }

        /// <summary>
        /// Writes a .mat file.
        /// </summary>
        /// <param name="file">A file to write.</param>
        public void Write(IMatFile file)
        {
            var header = Header.CreateNewHeader();
            using (var writer = new BinaryWriter(Stream))
            {
                WriteHeader(writer, header);
                foreach (var variable in file.Variables)
                {
                    switch (_options.UseCompression)
                    {
                        case CompressionUsage.Always:
                            if (Stream.CanSeek)
                            {
                                WriteCompressedVariableToSeekableStream(writer, variable);
                            }
                            else
                            {
                                WriteCompressedVariableToUnseekableStream(writer, variable);
                            }

                            break;
                        case CompressionUsage.Never:
                            WriteVariable(writer, variable);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private static uint CalculateAdler32Checksum(Stream stream)
        {
            uint s1 = 1;
            uint s2 = 0;
            const uint bigPrime = 0xFFF1;
            const int bufferSize = 2048;
            var buffer = new byte[bufferSize];
            while (true)
            {
                var bytesRead = stream.Read(buffer, 0, bufferSize);
                for (var i = 0; i < bytesRead; i++)
                {
                    s1 = (s1 + buffer[i]) % bigPrime;
                    s2 = (s2 + s1) % bigPrime;
                }
                if (bytesRead < bufferSize)
                {
                    break;
                }
            }
            return (s2 << 16) | s1;
        }

        private void WriteHeader(BinaryWriter writer, Header header)
        {
            writer.Write(Encoding.UTF8.GetBytes(header.Text));
            writer.Write(header.SubsystemDataOffset);
            writer.Write((short)header.Version);
            writer.Write((short)19785); // Magic number, 'IM'.
        }

        private void WriteTag(BinaryWriter writer, Tag tag)
        {
            writer.Write((int)tag.Type);
            writer.Write(tag.Length);
        }

        private void WriteShortTag(BinaryWriter writer, Tag tag)
        {
            writer.Write((short)tag.Type);
            writer.Write((short)tag.Length);
        }

        private void WritePadding(BinaryWriter writer)
        {
            var positionMod8 = writer.BaseStream.Position % 8;
            if (positionMod8 != 0)
            {
                writer.Write(new byte[8 - positionMod8]);
            }
        }

        private void WriteDataElement(BinaryWriter writer, DataType type, byte[] data)
        {
            if (data.Length > 4)
            {
                WriteTag(writer, new Tag(type, data.Length));
                writer.Write(data);
                var rem = data.Length % 8;
                if (rem > 0)
                {
                    var padding = new byte[8 - rem];
                    writer.Write(padding);
                }
            }
            else
            {
                WriteShortTag(writer, new Tag(type, data.Length));
                writer.Write(data);
                if (data.Length < 4)
                {
                    var padding = new byte[4 - data.Length];
                    writer.Write(padding);
                }
            }
        }

        private void WriteDimensions(BinaryWriter writer, int[] dimensions)
        {
            var buffer = ConvertToByteArray(dimensions);
            WriteDataElement(writer, DataType.MiInt32, buffer);
        }

        private byte[] ConvertToByteArray<T>(T[] data)
          where T : struct
        {
            int size;
            if (typeof(T) == typeof(sbyte))
            {
                size = sizeof(sbyte);
            }
            else if (typeof(T) == typeof(byte))
            {
                size = sizeof(byte);
            }
            else if (typeof(T) == typeof(short))
            {
                size = sizeof(short);
            }
            else if (typeof(T) == typeof(ushort))
            {
                size = sizeof(ushort);
            }
            else if (typeof(T) == typeof(int))
            {
                size = sizeof(int);
            }
            else if (typeof(T) == typeof(uint))
            {
                size = sizeof(uint);
            }
            else if (typeof(T) == typeof(long))
            {
                size = sizeof(long);
            }
            else if (typeof(T) == typeof(ulong))
            {
                size = sizeof(ulong);
            }
            else if (typeof(T) == typeof(float))
            {
                size = sizeof(float);
            }
            else if (typeof(T) == typeof(double))
            {
                size = sizeof(double);
            }
            else
            {
                throw new NotSupportedException();
            }
            var buffer = new byte[data.Length * size];
            Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        private (byte[] real, byte[] imaginary) ConvertToPairOfByteArrays<T>(ComplexOf<T>[] data)
          where T : struct
        {
            return (ConvertToByteArray(data.Select(x => x.Real).ToArray()),
                ConvertToByteArray(data.Select(x => x.Imaginary).ToArray()));
        }

        private (byte[] real, byte[] imaginary) ConvertToPairOfByteArrays(Complex[] data)
        {
            return (ConvertToByteArray(data.Select(x => x.Real).ToArray()),
                ConvertToByteArray(data.Select(x => x.Imaginary).ToArray()));
        }

        private void WriteComplexValues(BinaryWriter writer, DataType type, (byte[] real, byte[] complex) data)
        {
            WriteDataElement(writer, type, data.real);
            WriteDataElement(writer, type, data.complex);
        }

        private void WriteArrayFlags(BinaryWriter writer, ArrayFlags flags)
        {
            var flag = (byte)flags.Variable;
            WriteTag(writer, new Tag(DataType.MiUInt32, 8));
            writer.Write((byte)flags.Class);
            writer.Write(flag);
            writer.Write(new byte[] { 0, 0, 0, 0, 0, 0 });
        }

        private void WriteSparseArrayFlags(BinaryWriter writer, SparseArrayFlags flags)
        {
            var flag = (byte)flags.ArrayFlags.Variable;
            WriteTag(writer, new Tag(DataType.MiUInt32, 8));
            writer.Write((byte)flags.ArrayFlags.Class);
            writer.Write(flag);
            writer.Write(new byte[] { 0, 0 });
            writer.Write(flags.NzMax);
        }

        private void WriteName(BinaryWriter writer, string name)
        {
            var nameBytes = Encoding.ASCII.GetBytes(name);
            WriteDataElement(writer, DataType.MiInt8, nameBytes);
        }

        private void WriteNumericalArrayValues(BinaryWriter writer, IArray value)
        {
            switch (value)
            {
                case IArrayOf<sbyte> sbyteArray:
                    WriteDataElement(writer, DataType.MiInt8, ConvertToByteArray(sbyteArray.Data));
                    break;
                case IArrayOf<byte> byteArray:
                    WriteDataElement(writer, DataType.MiUInt8, ConvertToByteArray(byteArray.Data));
                    break;
                case IArrayOf<short> shortArray:
                    WriteDataElement(writer, DataType.MiInt16, ConvertToByteArray(shortArray.Data));
                    break;
                case IArrayOf<ushort> ushortArray:
                    WriteDataElement(writer, DataType.MiUInt16, ConvertToByteArray(ushortArray.Data));
                    break;
                case IArrayOf<int> intArray:
                    WriteDataElement(writer, DataType.MiInt32, ConvertToByteArray(intArray.Data));
                    break;
                case IArrayOf<uint> uintArray:
                    WriteDataElement(writer, DataType.MiUInt32, ConvertToByteArray(uintArray.Data));
                    break;
                case IArrayOf<long> longArray:
                    WriteDataElement(writer, DataType.MiInt64, ConvertToByteArray(longArray.Data));
                    break;
                case IArrayOf<ulong> ulongArray:
                    WriteDataElement(writer, DataType.MiUInt64, ConvertToByteArray(ulongArray.Data));
                    break;
                case IArrayOf<float> floatArray:
                    WriteDataElement(writer, DataType.MiSingle, ConvertToByteArray(floatArray.Data));
                    break;
                case IArrayOf<double> doubleArray:
                    WriteDataElement(writer, DataType.MiDouble, ConvertToByteArray(doubleArray.Data));
                    break;
                case IArrayOf<bool> boolArray:
                    WriteDataElement(
                        writer,
                        DataType.MiUInt8,
                        boolArray.Data.Select(element => element ? (byte)1 : (byte)0).ToArray());
                    break;
                case IArrayOf<ComplexOf<sbyte>> complexSbyteArray:
                    WriteComplexValues(writer, DataType.MiInt8, ConvertToPairOfByteArrays(complexSbyteArray.Data));
                    break;
                case IArrayOf<ComplexOf<byte>> complexByteArray:
                    WriteComplexValues(writer, DataType.MiUInt8, ConvertToPairOfByteArrays(complexByteArray.Data));
                    break;
                case IArrayOf<ComplexOf<short>> complexShortArray:
                    WriteComplexValues(writer, DataType.MiInt16, ConvertToPairOfByteArrays(complexShortArray.Data));
                    break;
                case IArrayOf<ComplexOf<ushort>> complexUshortArray:
                    WriteComplexValues(writer, DataType.MiUInt16, ConvertToPairOfByteArrays(complexUshortArray.Data));
                    break;
                case IArrayOf<ComplexOf<int>> complexIntArray:
                    WriteComplexValues(writer, DataType.MiInt32, ConvertToPairOfByteArrays(complexIntArray.Data));
                    break;
                case IArrayOf<ComplexOf<uint>> complexUintArray:
                    WriteComplexValues(writer, DataType.MiUInt32, ConvertToPairOfByteArrays(complexUintArray.Data));
                    break;
                case IArrayOf<ComplexOf<long>> complexLongArray:
                    WriteComplexValues(writer, DataType.MiInt64, ConvertToPairOfByteArrays(complexLongArray.Data));
                    break;
                case IArrayOf<ComplexOf<ulong>> complexUlongArray:
                    WriteComplexValues(writer, DataType.MiUInt64, ConvertToPairOfByteArrays(complexUlongArray.Data));
                    break;
                case IArrayOf<ComplexOf<float>> complexFloatArray:
                    WriteComplexValues(writer, DataType.MiSingle, ConvertToPairOfByteArrays(complexFloatArray.Data));
                    break;
                case IArrayOf<Complex> complexDoubleArray:
                    WriteComplexValues(writer, DataType.MiDouble, ConvertToPairOfByteArrays(complexDoubleArray.Data));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private ArrayFlags GetArrayFlags(IArray array, bool isGlobal)
        {
            var variableFlags = isGlobal ? Variable.IsGlobal : 0;
            switch (array)
            {
                case IArrayOf<sbyte> _:
                    return new ArrayFlags(ArrayType.MxInt8, variableFlags);
                case IArrayOf<byte> _:
                    return new ArrayFlags(ArrayType.MxUInt8, variableFlags);
                case IArrayOf<short> _:
                    return new ArrayFlags(ArrayType.MxInt16, variableFlags);
                case IArrayOf<ushort> _:
                    return new ArrayFlags(ArrayType.MxUInt16, variableFlags);
                case IArrayOf<int> _:
                    return new ArrayFlags(ArrayType.MxInt32, variableFlags);
                case IArrayOf<uint> _:
                    return new ArrayFlags(ArrayType.MxUInt32, variableFlags);
                case IArrayOf<long> _:
                    return new ArrayFlags(ArrayType.MxInt64, variableFlags);
                case IArrayOf<ulong> _:
                    return new ArrayFlags(ArrayType.MxUInt64, variableFlags);
                case IArrayOf<float> _:
                    return new ArrayFlags(ArrayType.MxSingle, variableFlags);
                case IArrayOf<double> _:
                    return new ArrayFlags(ArrayType.MxDouble, variableFlags);
                case IArrayOf<bool> _:
                    return new ArrayFlags(ArrayType.MxUInt8, variableFlags | Variable.IsLogical);
                case IArrayOf<ComplexOf<sbyte>> _:
                    return new ArrayFlags(ArrayType.MxInt8, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<byte>> _:
                    return new ArrayFlags(ArrayType.MxUInt8, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<short>> _:
                    return new ArrayFlags(ArrayType.MxInt16, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<ushort>> _:
                    return new ArrayFlags(ArrayType.MxUInt16, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<int>> _:
                    return new ArrayFlags(ArrayType.MxInt32, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<uint>> _:
                    return new ArrayFlags(ArrayType.MxUInt32, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<long>> _:
                    return new ArrayFlags(ArrayType.MxInt64, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<ulong>> _:
                    return new ArrayFlags(ArrayType.MxUInt64, variableFlags | Variable.IsComplex);
                case IArrayOf<ComplexOf<float>> _:
                    return new ArrayFlags(ArrayType.MxSingle, variableFlags | Variable.IsComplex);
                case IArrayOf<Complex> _:
                    return new ArrayFlags(ArrayType.MxDouble, variableFlags | Variable.IsComplex);
                case IStructureArray _:
                    return new ArrayFlags(ArrayType.MxStruct, variableFlags);
                case ICellArray _:
                    return new ArrayFlags(ArrayType.MxCell, variableFlags);
                default:
                    throw new NotSupportedException();
            }
        }

        private SparseArrayFlags GetSparseArrayFlags<T>(ISparseArrayOf<T> array, bool isGlobal, uint nonZero)
            where T : struct
        {
            var flags = GetArrayFlags(array, isGlobal);
            return new SparseArrayFlags
            {
                ArrayFlags = new ArrayFlags
                {
                    Class = ArrayType.MxSparse,
                    Variable = flags.Variable,
                },
                NzMax = nonZero,
            };
        }

        private ArrayFlags GetCharArrayFlags(bool isGlobal)
        {
            return new ArrayFlags(ArrayType.MxChar, isGlobal ? Variable.IsGlobal : 0);
        }

        private void WriteWrappingContents<T>(
            BinaryWriter writer,
            T array,
            Action<FakeWriter> lengthCalculator,
            Action<BinaryWriter> writeContents)
            where T : IArray
        {
            if (array.IsEmpty)
            {
                WriteTag(writer, new Tag(DataType.MiMatrix, 0));
                return;
            }

            var fakeWriter = new FakeWriter();
            lengthCalculator(fakeWriter);
            var calculatedLength = fakeWriter.Position;
            WriteTag(writer, new Tag(DataType.MiMatrix, calculatedLength));
            writeContents(writer);
        }

        private void WriteNumericalArrayContents(BinaryWriter writer, IArray array, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetArrayFlags(array, isGlobal));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteNumericalArrayValues(writer, array);
        }

        private void WriteNumericalArray(
            BinaryWriter writer,
            IArray numericalArray,
            string name = "",
            bool isGlobal = false)
        {
            WriteWrappingContents(
                writer,
                numericalArray,
                fakeWriter => fakeWriter.WriteNumericalArrayContents(numericalArray, name),
                contentsWriter => { WriteNumericalArrayContents(contentsWriter, numericalArray, name, isGlobal); });
        }

        private void WriteCharArrayContents(BinaryWriter writer, ICharArray charArray, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetCharArrayFlags(isGlobal));
            WriteDimensions(writer, charArray.Dimensions);
            WriteName(writer, name);
            var array = charArray.String.ToCharArray().Select(c => (ushort)c).ToArray();
            WriteDataElement(writer, DataType.MiUtf16, ConvertToByteArray(array));
        }

        private void WriteCharArray(BinaryWriter writer, ICharArray charArray, string name, bool isGlobal)
        {
            WriteWrappingContents(
                writer,
                charArray,
                fakeWriter => fakeWriter.WriteCharArrayContents(charArray, name),
                contentsWriter => { WriteCharArrayContents(contentsWriter, charArray, name, isGlobal); });
        }

        private void WriteSparseArrayValues<T>(
            BinaryWriter writer, int[] rows, int[] columns, T[] data)
          where T : struct
        {
            WriteDataElement(writer, DataType.MiInt32, ConvertToByteArray(rows));
            WriteDataElement(writer, DataType.MiInt32, ConvertToByteArray(columns));
            if (data is double[])
            {
                WriteDataElement(writer, DataType.MiDouble, ConvertToByteArray(data));
            }
            else if (data is Complex[])
            {
                var complexData = data as Complex[];
                WriteDataElement(
                    writer,
                    DataType.MiDouble,
                    ConvertToByteArray(complexData.Select(c => c.Real).ToArray()));
                WriteDataElement(
                    writer,
                    DataType.MiDouble,
                    ConvertToByteArray(complexData.Select(c => c.Imaginary).ToArray()));
            }
            else if (data is bool[])
            {
                var boolData = data as bool[];
                WriteDataElement(
                    writer,
                    DataType.MiUInt8,
                    boolData.Select(element => element ? (byte)1 : (byte)0).ToArray());
            }
        }

        private (int[] rowIndex, int[] columnIndex, T[] data, uint nonZero) PrepareSparseArrayData<T>(
            ISparseArrayOf<T> array)
            where T : struct, IEquatable<T>
        {
            var dict = array.Data;
            var keys = dict.Keys.ToArray();
            var rowIndexList = new List<int>();
            var valuesList = new List<T>();
            var numberOfColumns = array.Dimensions[1];
            var columnIndex = new int[numberOfColumns + 1];
            columnIndex[0] = 0;
            for (var column = 0; column < numberOfColumns; column++)
            {
                var column1 = column;
                var thisColumn = keys.Where(pair => pair.column == column1 && !dict[pair].Equals(default));
                var thisRow = thisColumn.Select(pair => pair.row).OrderBy(x => x).ToArray();
                rowIndexList.AddRange(thisRow);
                valuesList.AddRange(thisRow.Select(row => dict[(row, column1)]));
                columnIndex[column + 1] = rowIndexList.Count;
            }
            return (rowIndexList.ToArray(), columnIndex, valuesList.ToArray(), (uint)rowIndexList.Count);
        }

        private void WriteSparseArrayContents<T>(
            BinaryWriter writer,
            ISparseArrayOf<T> array,
            string name,
            bool isGlobal)
            where T : struct, IEquatable<T>
        {
            (var rows, var columns, var data, var nonZero) = PrepareSparseArrayData(array);
            WriteSparseArrayFlags(writer, GetSparseArrayFlags(array, isGlobal, nonZero));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteSparseArrayValues(writer, rows, columns, data);
        }

        private void WriteSparseArray<T>(BinaryWriter writer, ISparseArrayOf<T> sparseArray, string name, bool isGlobal)
            where T : unmanaged, IEquatable<T>
        {
            WriteWrappingContents(
                writer,
                sparseArray,
                fakeWriter => fakeWriter.WriteSparseArrayContents(sparseArray, name),
                contentsWriter => { WriteSparseArrayContents(contentsWriter, sparseArray, name, isGlobal); });
        }

        private void WriteFieldNames(BinaryWriter writer, IEnumerable<string> fieldNames)
        {
            var fieldNamesArray = fieldNames.Select(name => Encoding.ASCII.GetBytes(name)).ToArray();
            var maxFieldName = fieldNamesArray.Select(name => name.Length).Max() + 1;
            WriteDataElement(writer, DataType.MiInt32, ConvertToByteArray(new[] { maxFieldName }));
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
            WriteDataElement(writer, DataType.MiInt8, buffer);
        }

        private void WriteStructureArrayValues(BinaryWriter writer, IStructureArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                foreach (var name in array.FieldNames)
                {
                    WriteArray(writer, array[name, i]);
                }
            }
        }

        private void WriteStructureArrayContents(BinaryWriter writer, IStructureArray array, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetArrayFlags(array, isGlobal));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteFieldNames(writer, array.FieldNames);
            WriteStructureArrayValues(writer, array);
        }

        private void WriteStructureArray(
            BinaryWriter writer,
            IStructureArray structureArray,
            string name,
            bool isGlobal)
        {
            WriteWrappingContents(
                writer,
                structureArray,
                fakeWriter => fakeWriter.WriteStructureArrayContents(structureArray, name),
                contentsWriter => { WriteStructureArrayContents(contentsWriter, structureArray, name, isGlobal); });
        }

        private void WriteCellArrayValues(BinaryWriter writer, ICellArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                WriteArray(writer, array[i]);
            }
        }

        private void WriteCellArrayContents(BinaryWriter writer, ICellArray array, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetArrayFlags(array, isGlobal));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteCellArrayValues(writer, array);
        }

        private void WriteCellArray(BinaryWriter writer, ICellArray cellArray, string name, bool isGlobal)
        {
            WriteWrappingContents(
                writer,
                cellArray,
                fakeWriter => fakeWriter.WriteCellArrayContents(cellArray, name),
                contentsWriter => { WriteCellArrayContents(contentsWriter, cellArray, name, isGlobal); });
        }

        private void WriteArray(BinaryWriter writer, IArray array, string variableName = "", bool isGlobal = false)
        {
            switch (array)
            {
                case ICharArray charArray:
                    WriteCharArray(writer, charArray, variableName, isGlobal);
                    break;
                case ISparseArrayOf<double> doubleSparseArray:
                    WriteSparseArray(writer, doubleSparseArray, variableName, isGlobal);
                    break;
                case ISparseArrayOf<Complex> complexSparseArray:
                    WriteSparseArray(writer, complexSparseArray, variableName, isGlobal);
                    break;
                case ISparseArrayOf<bool> boolSparseArray:
                    WriteSparseArray(writer, boolSparseArray, variableName, isGlobal);
                    break;
                case ICellArray cellArray:
                    WriteCellArray(writer, cellArray, variableName, isGlobal);
                    break;
                case IStructureArray structureArray:
                    WriteStructureArray(writer, structureArray, variableName, isGlobal);
                    break;
                default:
                    WriteNumericalArray(writer, array, variableName, isGlobal);
                    break;
            }
        }

        private void WriteVariable(BinaryWriter writer, IVariable variable)
        {
            WriteArray(writer, variable.Value, variable.Name, variable.IsGlobal);
        }

        private void WriteCompressedVariableToSeekableStream(BinaryWriter writer, IVariable variable)
        {
            var position = writer.BaseStream.Position;
            WriteTag(writer, new Tag(DataType.MiCompressed, 0));
            writer.Write((byte)0x78);
            writer.Write((byte)0x9c);
            int compressedLength;
            uint crc;
            var before = writer.BaseStream.Position;
            using (var compressionStream = new DeflateStream(writer.BaseStream, CompressionMode.Compress, leaveOpen: true))
            {
                using var checksumStream = new ChecksumCalculatingStream(compressionStream);
                using var internalWriter = new BinaryWriter(checksumStream, Encoding.UTF8, leaveOpen: true);
                WriteVariable(internalWriter, variable);
                crc = checksumStream.GetCrc();
            }

            var after = writer.BaseStream.Position;
            compressedLength = (int)(after - before) + 6;

            writer.Write(BitConverter.GetBytes(crc).Reverse().ToArray());
            writer.BaseStream.Position = position;
            WriteTag(writer, new Tag(DataType.MiCompressed, compressedLength));
            writer.BaseStream.Seek(0, SeekOrigin.End);
        }

        private void WriteCompressedVariableToUnseekableStream(BinaryWriter writer, IVariable variable)
        {
            using (var compressedStream = new MemoryStream())
            {
                uint crc;
                using (var originalStream = new MemoryStream())
                {
                    using (var internalWriter = new BinaryWriter(originalStream))
                    {
                        WriteVariable(internalWriter, variable);
                        originalStream.Position = 0;
                        crc = CalculateAdler32Checksum(originalStream);
                        originalStream.Position = 0;
                        using (var compressionStream =
                            new DeflateStream(compressedStream, CompressionMode.Compress, leaveOpen: true))
                        {
                            originalStream.CopyTo(compressionStream);
                        }
                    }
                }
                compressedStream.Position = 0;
                WriteTag(writer, new Tag(DataType.MiCompressed, (int)(compressedStream.Length + 6)));
                writer.Write((byte)0x78);
                writer.Write((byte)0x9c);
                compressedStream.CopyTo(writer.BaseStream);
                writer.Write(BitConverter.GetBytes(crc).Reverse().ToArray());
            }
        }
    }
}