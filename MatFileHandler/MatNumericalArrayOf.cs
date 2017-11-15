// Copyright 2017 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatFileHandler
{
    /// <summary>
    /// A numerical array.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    internal class MatNumericalArrayOf<T> : MatArray, IArrayOf<T>
      where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatNumericalArrayOf{T}"/> class.
        /// </summary>
        /// <param name="flags">Array parameters.</param>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <param name="name">Array name.</param>
        /// <param name="data">Array contents.</param>
        public MatNumericalArrayOf(ArrayFlags flags, int[] dimensions, string name, T[] data)
            : base(flags, dimensions, name)
        {
            Data = data;
        }

        /// <inheritdoc />
        public T[] Data { get; }

        /// <inheritdoc />
        public T this[params int[] list]
        {
            get => Data[Dimensions.DimFlatten(list)];
            set => Data[Dimensions.DimFlatten(list)] = value;
        }

        /// <summary>
        /// Tries to convert the array to an array of Double values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Double, or null if the conversion is not possible.</returns>
        public override double[] ConvertToDoubleArray()
        {
            return Data as double[] ?? Data.Select(x => Convert.ToDouble(x)).ToArray();
        }

        /// <summary>
        /// Tries to convert the array to an array of Complex values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Complex, or null if the conversion is not possible.</returns>
        public override Complex[] ConvertToComplexArray()
        {
            if (Data is Complex[])
            {
                return Data as Complex[];
            }
            if (Data is ComplexOf<sbyte>[])
            {
                return ConvertToComplex(Data as ComplexOf<sbyte>[]);
            }
            if (Data is ComplexOf<byte>[])
            {
                return ConvertToComplex(Data as ComplexOf<byte>[]);
            }
            if (Data is ComplexOf<short>[])
            {
                return ConvertToComplex(Data as ComplexOf<short>[]);
            }
            if (Data is ComplexOf<ushort>[])
            {
                return ConvertToComplex(Data as ComplexOf<ushort>[]);
            }
            if (Data is ComplexOf<int>[])
            {
                return ConvertToComplex(Data as ComplexOf<int>[]);
            }
            if (Data is ComplexOf<uint>[])
            {
                return ConvertToComplex(Data as ComplexOf<uint>[]);
            }
            if (Data is ComplexOf<long>[])
            {
                return ConvertToComplex(Data as ComplexOf<long>[]);
            }
            if (Data is ComplexOf<ulong>[])
            {
                return ConvertToComplex(Data as ComplexOf<ulong>[]);
            }
            return ConvertToDoubleArray().Select(x => new Complex(x, 0.0)).ToArray();
        }

        private static Complex[] ConvertToComplex<TS>(IEnumerable<ComplexOf<TS>> array)
          where TS : struct
        {
            return array.Select(x => new Complex(Convert.ToDouble(x.Real), Convert.ToDouble(x.Imaginary))).ToArray();
        }
    }
}