// Copyright 2017-2018 Alexander Luzgarev

using System.Collections.Generic;

namespace MatFileHandler
{
    /// <summary>
    /// Matlab's sparse array.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <remarks>Possible values of T: Double, Complex, Boolean.</remarks>
    public interface ISparseArrayOf<T> : IArrayOf<T>
      where T : struct
    {
        /// <summary>
        /// Gets a dictionary mapping indices to values.
        /// </summary>
        new IReadOnlyDictionary<(int, int), T> Data { get; }
    }
}