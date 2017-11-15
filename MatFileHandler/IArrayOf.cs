// Copyright 2017 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// An interface providing access to array's contents.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <remarks>
    /// Possible values of T
    /// * for numerical arrays:
    ///   Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double,
    ///   ComplexOf&lt;TReal&gt; (where TReal is one of Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single),
    ///   Complex;
    /// * for sparse arrays:
    ///   Double, Complex, Boolean;
    /// * for character arrays:
    ///   UInt8, UInt16, Char;
    /// * for logical arrays:
    ///   Boolean;
    /// * for cell arrays:
    ///   IArray;
    /// * for structure arrays:
    ///   IReadOnlyDictionary&lt;string, IArray&gt;;
    /// </remarks>
    public interface IArrayOf<T> : IArray
    {
        /// <summary>
        /// Gets all data as an array.
        /// </summary>
        T[] Data { get; }

        /// <summary>
        /// Get an element by index.
        /// </summary>
        /// <param name="list">Index of the element.</param>
        T this[params int[] list] { get; set; }
    }
}