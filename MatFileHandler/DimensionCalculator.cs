// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Linq;

namespace MatFileHandler
{
    /// <summary>
    /// Extension method related to dimension calculations.
    /// </summary>
    public static class DimensionCalculator
    {
        /// <summary>
        /// Convert a sequence of indices into a single index (according to Matlab rules).
        /// </summary>
        /// <param name="dimensions">Dimensions of an array.</param>
        /// <param name="indices">A sequence of indices.</param>
        /// <returns>Index of the corresponding element in the array.</returns>
        public static int DimFlatten(this int[] dimensions, int[] indices)
        {
            var product = 1;
            var result = 0;
            for (var i = 0; i < indices.Length; i++)
            {
                result += product * indices[i];
                product *= dimensions[i];
            }
            return result;
        }

        /// <summary>
        /// Calculate the number of elements in an array given its dimensions.
        /// </summary>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <returns>Total number of elements in an array.</returns>
        public static int NumberOfElements(this int[] dimensions)
        {
            return dimensions.Aggregate(1, (x, y) => x * y);
        }

        /// <summary>
        /// Rearrange data from a flat (column-major) array into a multi-dimensional array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="dimensions">Target array dimensions.</param>
        /// <param name="data">Flat (column-major) data to rearrange.</param>
        /// <returns>An array of specified dimensions containing data from the original flat array, layed out according to column-major order.</returns>
        public static Array UnflattenArray<T>(this int[] dimensions, T[] data)
        {
            var result = Array.CreateInstance(typeof(T), dimensions);
            var n = dimensions.NumberOfElements();
            var indices = new int[dimensions.Length];
            for (var i = 0; i < n; i++)
            {
                result.SetValue(data[i], indices);
                IncrementMultiIndex(dimensions, indices);
            }

            return result;
        }

        private static void IncrementMultiIndex(int[] dimensions, int[] indices)
        {
            var currentPosition = 0;
            while (true)
            {
                if (currentPosition >= indices.Length)
                {
                    break;
                }
                indices[currentPosition]++;
                if (indices[currentPosition] >= dimensions[currentPosition])
                {
                    indices[currentPosition] = 0;
                    currentPosition++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}