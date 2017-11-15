// Copyright 2017 Alexander Luzgarev

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
        /// Gets the number of elements in the array.
        /// </summary>
        int NumberOfElements { get; }

        /// <summary>
        /// Tries to convert the array to an array of Double values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Double, or null if the conversion is not possible.</returns>
        double[] ConvertToDoubleArray();

        /// <summary>
        /// Tries to convert the array to an array of Complex values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Complex, or null if the conversion is not possible.</returns>
        Complex[] ConvertToComplexArray();
    }
}