// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MatFileHandler
{
    /// <summary>
    /// Static class for constructing various arrays from raw data elements read from .mat files.
    /// </summary>
    internal static class DataElementConverter
    {
        /// <summary>
        /// Construct a complex sparse array.
        /// </summary>
        /// <param name="flags">Array flags.</param>
        /// <param name="dimensions">Array dimensions.</param>
        /// <param name="name">Array name.</param>
        /// <param name="rowIndex">Row indices.</param>
        /// <param name="columnIndex">Denotes index ranges for each column.</param>
        /// <param name="data">Real parts of the values.</param>
        /// <param name="imaginaryData">Imaginary parts of the values.</param>
        /// <returns>A constructed array.</returns>
        public static MatArray ConvertToMatSparseArrayOfComplex(
            SparseArrayFlags flags,
            int[] dimensions,
            string name,
            int[] rowIndex,
            int[] columnIndex,
            DataElement data,
            DataElement imaginaryData)
        {
            var realParts = DataExtraction.GetDataAsDouble(data);
            var imaginaryParts = DataExtraction.GetDataAsDouble(imaginaryData);
            if (realParts == null)
            {
                throw new HandlerException("Couldn't read sparse array.");
            }
            var dataDictionary =
                ConvertMatlabSparseToDictionary(
                    rowIndex,
                    columnIndex,
                    j => new Complex(realParts[j], imaginaryParts[j]));
            return new MatSparseArrayOf<Complex>(flags, dimensions, name, dataDictionary);
        }

        /// <summary>
        /// Construct a double sparse array or a logical sparse array.
        /// </summary>
        /// <typeparam name="T">Element type (Double or Boolean).</typeparam>
        /// <param name="flags">Array flags.</param>
        /// <param name="dimensions">Array dimensions.</param>
        /// <param name="name">Array name.</param>
        /// <param name="rowIndex">Row indices.</param>
        /// <param name="columnIndex">Denotes index ranges for each column.</param>
        /// <param name="data">The values.</param>
        /// <returns>A constructed array.</returns>
        public static MatArray ConvertToMatSparseArrayOf<T>(
            SparseArrayFlags flags,
            int[] dimensions,
            string name,
            int[] rowIndex,
            int[] columnIndex,
            DataElement data)
            where T : struct
        {
            if (dimensions.Length != 2)
            {
                throw new NotSupportedException("Only 2-dimensional sparse arrays are supported");
            }
            if (data == null)
            {
                throw new ArgumentException("Null data found.", "data");
            }
            var elements =
                ConvertDataToSparseProperType<T>(data, flags.ArrayFlags.Variable.HasFlag(Variable.IsLogical));
            if (elements == null)
            {
                throw new HandlerException("Couldn't read sparse array.");
            }
            var dataDictionary =
                ConvertMatlabSparseToDictionary(rowIndex, columnIndex, j => elements[j]);
            return new MatSparseArrayOf<T>(flags, dimensions, name, dataDictionary);
        }

        /// <summary>
        /// Construct a numerical array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="flags">Array flags.</param>
        /// <param name="dimensions">Array dimensions.</param>
        /// <param name="name">Array name.</param>
        /// <param name="realData">Real parts of the values.</param>
        /// <param name="imaginaryData">Imaginary parts of the values.</param>
        /// <returns>A constructed array.</returns>
        /// <remarks>
        /// Possible values for T: Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double,
        ///   ComplexOf&lt;TReal&gt; (where TReal is one of Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single),
        ///   Complex.
        /// </remarks>
        public static MatArray ConvertToMatNumericalArrayOf<T>(
            ArrayFlags flags,
            int[] dimensions,
            string name,
            DataElement realData,
            DataElement imaginaryData)
            where T : struct
        {
            if (flags.Variable.HasFlag(Variable.IsLogical))
            {
                var data = DataExtraction.GetDataAsUInt8(realData).Select(x => x != 0).ToArray();
                return new MatNumericalArrayOf<bool>(flags, dimensions, name, data);
            }
            switch (flags.Class)
            {
                case ArrayType.MxChar:
                    switch (realData)
                    {
                        case MiNum<byte> dataByte:
                            return ConvertToMatCharArray(flags, dimensions, name, dataByte);
                        case MiNum<ushort> dataUshort:
                            return ConvertToMatCharArray(flags, dimensions, name, dataUshort);
                        default:
                            throw new NotSupportedException("Only utf8, utf16 or ushort char arrays are supported.");
                    }
                case ArrayType.MxDouble:
                case ArrayType.MxSingle:
                case ArrayType.MxInt8:
                case ArrayType.MxUInt8:
                case ArrayType.MxInt16:
                case ArrayType.MxUInt16:
                case ArrayType.MxInt32:
                case ArrayType.MxUInt32:
                case ArrayType.MxInt64:
                case ArrayType.MxUInt64:
                    var dataArray = ConvertDataToProperType<T>(realData, flags.Class);
                    if (flags.Variable.HasFlag(Variable.IsComplex))
                    {
                        var dataArray2 = ConvertDataToProperType<T>(imaginaryData, flags.Class);
                        if (flags.Class == ArrayType.MxDouble)
                        {
                            var complexArray =
                                (dataArray as double[])
                                .Zip(dataArray2 as double[], (x, y) => new Complex(x, y))
                                .ToArray();
                            return new MatNumericalArrayOf<Complex>(flags, dimensions, name, complexArray);
                        }
                        var complexDataArray = dataArray.Zip(dataArray2, (x, y) => new ComplexOf<T>(x, y)).ToArray();
                        return new MatNumericalArrayOf<ComplexOf<T>>(flags, dimensions, name, complexDataArray);
                    }
                    return new MatNumericalArrayOf<T>(flags, dimensions, name, dataArray);
                default:
                    throw new NotSupportedException();
            }
        }

        private static MatCharArrayOf<byte> ConvertToMatCharArray(
            ArrayFlags flags,
            int[] dimensions,
            string name,
            MiNum<byte> dataElement)
        {
            var data = dataElement.Data;
            return new MatCharArrayOf<byte>(flags, dimensions, name, data, Encoding.UTF8.GetString(data));
        }

        private static T[] ConvertDataToProperType<T>(DataElement data, ArrayType arrayType)
        {
            switch (arrayType)
            {
                case ArrayType.MxDouble:
                    return DataExtraction.GetDataAsDouble(data) as T[];
                case ArrayType.MxSingle:
                    return DataExtraction.GetDataAsSingle(data) as T[];
                case ArrayType.MxInt8:
                    return DataExtraction.GetDataAsInt8(data) as T[];
                case ArrayType.MxUInt8:
                    return DataExtraction.GetDataAsUInt8(data) as T[];
                case ArrayType.MxInt16:
                    return DataExtraction.GetDataAsInt16(data) as T[];
                case ArrayType.MxUInt16:
                    return DataExtraction.GetDataAsUInt16(data) as T[];
                case ArrayType.MxInt32:
                    return DataExtraction.GetDataAsInt32(data) as T[];
                case ArrayType.MxUInt32:
                    return DataExtraction.GetDataAsUInt32(data) as T[];
                case ArrayType.MxInt64:
                    return DataExtraction.GetDataAsInt64(data) as T[];
                case ArrayType.MxUInt64:
                    return DataExtraction.GetDataAsUInt64(data) as T[];
                default:
                    throw new NotSupportedException();
            }
        }

        private static T[] ConvertDataToSparseProperType<T>(DataElement data, bool isLogical)
        {
            if (isLogical)
            {
                return DataExtraction.GetDataAsUInt8(data).Select(x => x != 0).ToArray() as T[];
            }
            switch (data)
            {
                case MiNum<double> _:
                    return DataExtraction.GetDataAsDouble(data) as T[];
                default:
                    throw new NotSupportedException();
            }
        }

        private static MatCharArrayOf<ushort> ConvertToMatCharArray(
            ArrayFlags flags,
            int[] dimensions,
            string name,
            MiNum<ushort> dataElement)
        {
            var data = dataElement?.Data;
            return new MatCharArrayOf<ushort>(
                flags,
                dimensions,
                name,
                data,
                new string(data.Select(x => (char)x).ToArray()));
        }

        private static Dictionary<(int, int), T> ConvertMatlabSparseToDictionary<T>(
            int[] rowIndex,
            int[] columnIndex,
            Func<int, T> get)
        {
            var result = new Dictionary<(int, int), T>();
            for (var column = 0; column < columnIndex.Length - 1; column++)
            {
                for (var j = columnIndex[column]; j < columnIndex[column + 1]; j++)
                {
                    var row = rowIndex[j];
                    result[(row, column)] = get(j);
                }
            }
            return result;
        }
    }
}