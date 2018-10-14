// Copyright 2017-2018 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// Helpers for working with Matlab data types.
    /// </summary>
    internal static class DataTypeExtensions
    {
        /// <summary>
        /// Get data type size in bytes.
        /// </summary>
        /// <param name="type">A data type.</param>
        /// <returns>Size in bytes.</returns>
        public static int Size(this DataType type)
        {
            switch (type)
            {
                case DataType.MiInt8:
                    return 1;
                case DataType.MiUInt8:
                    return 1;
                case DataType.MiInt16:
                    return 2;
                case DataType.MiUInt16:
                    return 2;
                case DataType.MiInt32:
                    return 4;
                case DataType.MiUInt32:
                    return 4;
                case DataType.MiSingle:
                    return 4;
                case DataType.MiDouble:
                    return 8;
                case DataType.MiInt64:
                    return 8;
                case DataType.MiUInt64:
                    return 8;
                case DataType.MiMatrix:
                    return 0;
                case DataType.MiCompressed:
                    return 0;
                case DataType.MiUtf8:
                    return 1;
                case DataType.MiUtf16:
                    return 2;
                case DataType.MiUtf32:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}