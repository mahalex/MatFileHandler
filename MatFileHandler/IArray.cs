// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Numerics;

namespace MatFileHandler
{
    /// <summary>
    /// Parent data accessing interface for all Matlab classes.
    /// </summary>
    public interface IArray
    {
        /// <summary>
        /// Gets a value indicating whether the array is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets dimensions of the array.
        /// </summary>
        int[] Dimensions { get; }

        /// <summary>
        /// Gets the total number of elements in the array.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Tries to convert the array to an array of Double values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Double, or null if the conversion is not possible.</returns>
        double[]? ConvertToDoubleArray();

        /// <summary>
        /// Tries to convert the array to a 2-dimensional array of Double values.
        /// </summary>
        /// <returns>2-dimensional array of values of the array, converted to Double, or null if the conversion is not possible (for example, when the array has more than 2 dimensions).</returns>
        double[,]? ConvertTo2dDoubleArray();

        /// <summary>
        /// Tries to convert the array to a multidimensional array of Double values.
        /// </summary>
        /// <returns>Multidimensional array of values of the array, converted to Double, or null if the conversion is not possible.</returns>
        Array? ConvertToMultidimensionalDoubleArray();

        /// <summary>
        /// Tries to convert the array to an array of Complex values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Complex, or null if the conversion is not possible.</returns>
        Complex[]? ConvertToComplexArray();
    }
}