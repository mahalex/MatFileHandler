// Copyright 2017 Alexander Luzgarev

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
    }
}