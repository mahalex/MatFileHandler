// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// Type of the data attached to the tag.
    /// </summary>
    internal enum DataType
    {
        /// <summary>
        /// An array of Int8 elements.
        /// </summary>
        MiInt8 = 1,

        /// <summary>
        /// An array of UInt8 elements.
        /// </summary>
        MiUInt8 = 2,

        /// <summary>
        /// An array of Int16 elements.
        /// </summary>
        MiInt16 = 3,

        /// <summary>
        /// An array of UInt16 elements.
        /// </summary>
        MiUInt16 = 4,

        /// <summary>
        /// An array of Int32 elements.
        /// </summary>
        MiInt32 = 5,

        /// <summary>
        /// An array of UInt32 elements.
        /// </summary>
        MiUInt32 = 6,

        /// <summary>
        /// An array of Single elements.
        /// </summary>
        MiSingle = 7,

        /// <summary>
        /// An array of Double elements.
        /// </summary>
        MiDouble = 9,

        /// <summary>
        /// An array of Int64 elements.
        /// </summary>
        MiInt64 = 12,

        /// <summary>
        /// An array of UInt64 elements.
        /// </summary>
        MiUInt64 = 13,

        /// <summary>
        /// A matrix.
        /// </summary>
        MiMatrix = 14,

        /// <summary>
        /// A compressed data element.
        /// </summary>
        MiCompressed = 15,

        /// <summary>
        /// An array of UTF-8 elements.
        /// </summary>
        MiUtf8 = 16,

        /// <summary>
        /// An array of UTF-16 elements.
        /// </summary>
        MiUtf16 = 17,

        /// <summary>
        /// An array of UTF-32 elements.
        /// </summary>
        MiUtf32 = 18,
    }

    /// <summary>
    /// Data element tag.
    /// </summary>
    internal class Tag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        /// <param name="type">Data type.</param>
        /// <param name="length">Data length (number of elements).</param>
        public Tag(DataType type, int length)
        {
            Type = type;
            Length = length;
        }

        /// <summary>
        /// Gets the type of the attached data.
        /// </summary>
        public DataType Type { get; }

        /// <summary>
        /// Gets data length (number of elements).
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets size of a data element in bytes.
        /// </summary>
        public int ElementSize => Type.Size();
    }
}