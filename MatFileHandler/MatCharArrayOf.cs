// Copyright 2017 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// Character array.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <remarks>
    /// Possible values of T: UInt8 (for UTF-8 encoded arrays), UInt16 (for UTF-16 encoded arrays).
    /// </remarks>
    internal class MatCharArrayOf<T> : MatNumericalArrayOf<T>, ICharArray
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatCharArrayOf{T}"/> class.
        /// </summary>
        /// <param name="flags">Array parameters.</param>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <param name="name">Array name.</param>
        /// <param name="rawData">Raw data (UTF-8 or UTF-16).</param>
        /// <param name="stringData">Contents as a string.</param>
        internal MatCharArrayOf(ArrayFlags flags, int[] dimensions, string name, T[] rawData, string stringData)
            : base(flags, dimensions, name, rawData)
        {
            StringData = stringData;
        }

        /// <summary>
        /// Gets the contents of the array as a string.
        /// </summary>
        public string String => StringData;

        /// <summary>
        /// Gets the contents of the array as a char array.
        /// </summary>
        char[] IArrayOf<char>.Data => StringData.ToCharArray();

        private string StringData { get; set; }

        /// <summary>
        /// Provides access to the characters of the string contents.
        /// </summary>
        /// <param name="indices">Indices of an element.</param>
        /// <returns>Value of the element.</returns>
        char IArrayOf<char>.this[params int[] indices]
        {
            get => StringData[Dimensions.DimFlatten(indices)];
            set
            {
                var chars = StringData.ToCharArray();
                chars[Dimensions.DimFlatten(indices)] = value;
                StringData = chars.ToString();
            }
        }
    }
}