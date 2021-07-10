// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Numerics;

namespace MatFileHandler
{
    /// <summary>
    /// Base class for various Matlab arrays.
    /// </summary>
    internal class MatArray : DataElement, IArray
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatArray"/> class.
        /// </summary>
        /// <param name="flags">Array properties.</param>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <param name="name">Array name.</param>
        protected MatArray(
            ArrayFlags flags,
            int[] dimensions,
            string name)
        {
            Flags = flags;
            Dimensions = dimensions;
            Name = name;
        }

        /// <inheritdoc />
        public int[] Dimensions { get; }

        /// <summary>
        /// Gets the array name.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public int Count => Dimensions.NumberOfElements();

        /// <inheritdoc />
        public bool IsEmpty => Dimensions.Length == 0;

        /// <summary>
        /// Gets properties of the array.
        /// </summary>
        internal ArrayFlags Flags { get; }

        /// <summary>
        /// Returns a new empty array.
        /// </summary>
        /// <returns>Empty array.</returns>
        public static MatArray Empty()
        {
            return new MatArray(new ArrayFlags { Class = ArrayType.MxCell, Variable = 0 }, new int[] { }, string.Empty);
        }

        /// <inheritdoc />
        public virtual double[]? ConvertToDoubleArray()
        {
            return null;
        }

        /// <inheritdoc />
        public virtual double[,]? ConvertTo2dDoubleArray()
        {
            return ConvertToMultidimensionalDoubleArray() as double[,];
        }

        /// <inheritdoc />
        public virtual Array? ConvertToMultidimensionalDoubleArray()
        {
            return null;
        }

        /// <inheritdoc />
        public virtual Complex[]? ConvertToComplexArray()
        {
            return null;
        }
    }
}