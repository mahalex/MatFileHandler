// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatFileHandler
{
    /// <summary>
    /// Sparse array.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <remarks>Possible values of T: Double, Complex, Boolean.</remarks>
    internal class MatSparseArrayOf<T> : MatArray, ISparseArrayOf<T>
      where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatSparseArrayOf{T}"/> class.
        /// </summary>
        /// <param name="flags">Array properties.</param>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <param name="name">Array name.</param>
        /// <param name="data">Array contents.</param>
        public MatSparseArrayOf(
            SparseArrayFlags flags,
            int[] dimensions,
            string name,
            Dictionary<(int row, int column), T> data)
            : base(flags.ArrayFlags, dimensions, name)
        {
            DataDictionary = data;
        }

        /// <inheritdoc />
        T[] IArrayOf<T>.Data =>
            Enumerable.Range(0, Dimensions[0] * Dimensions[1])
                .Select(i => this[i])
                .ToArray();

        /// <inheritdoc />
        public IReadOnlyDictionary<(int row, int column), T> Data => DataDictionary;

        private Dictionary<(int row, int column), T> DataDictionary { get; }

        /// <inheritdoc />
        public T this[params int[] list]
        {
            get
            {
                var rowAndColumn = GetRowAndColumn(list);
                return DataDictionary.ContainsKey(rowAndColumn) ? DataDictionary[rowAndColumn] : default(T);
            }
            set => DataDictionary[GetRowAndColumn(list)] = value;
        }

        /// <summary>
        /// Tries to convert the array to an array of Double values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Double, or null if the conversion is not possible.</returns>
        public override double[] ConvertToDoubleArray()
        {
            var data = ((IArrayOf<T>)this).Data;
            return data as double[] ?? data.Select(x => Convert.ToDouble(x)).ToArray();
        }

        /// <inheritdoc />
        public override Array? ConvertToMultidimensionalDoubleArray()
        {
            if (Dimensions.Length != 2)
            {
                return null;
            }

            var result = new double[Dimensions[0], Dimensions[1]];
            foreach (var pair in Data)
            {
                result[pair.Key.row, pair.Key.column] = Convert.ToDouble(pair.Value);
            }

            return result;
        }

        /// <summary>
        /// Tries to convert the array to an array of Complex values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Complex, or null if the conversion is not possible.</returns>
        public override Complex[] ConvertToComplexArray()
        {
            var data = ((IArrayOf<T>)this).Data;
            return data as Complex[] ?? ConvertToDoubleArray().Select(x => new Complex(x, 0.0)).ToArray();
        }

        private (int row, int column) GetRowAndColumn(int[] indices)
        {
            switch (indices.Length)
            {
                case 1:
                    return (indices[0] % Dimensions[0], indices[0] / Dimensions[0]);
                case 2:
                    return (indices[0], indices[1]);
                default:
                    throw new NotSupportedException("Invalid index for sparse array.");
            }
        }
    }
}