// Copyright 2017-2018 Alexander Luzgarev

using System;
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
            return Data switch
            {
                sbyte[] sbyteData => DataExtraction.SbyteToDouble(sbyteData),
                byte[] byteData => DataExtraction.ByteToDouble(byteData),
                short[] shortData => DataExtraction.ShortToDouble(shortData),
                ushort[] ushortData => DataExtraction.UshortToDouble(ushortData),
                int[] intData => DataExtraction.IntToDouble(intData),
                uint[] uintData => DataExtraction.UintToDouble(uintData),
                long[] longData => DataExtraction.LongToDouble(longData),
                ulong[] ulongData => DataExtraction.UlongToDouble(ulongData),
                float[] floatData => DataExtraction.FloatToDouble(floatData),
                double[] doubleData => doubleData,
                _ => throw new HandlerException("Cannot convert data to double array.")
            };
        }

        /// <inheritdoc />
        public override Array? ConvertToMultidimensionalDoubleArray()
        {
            var doubleData = ConvertToDoubleArray();
            var rearrangedData = Dimensions.UnflattenArray(doubleData);
            return rearrangedData;
        }

        /// <summary>
        /// Tries to convert the array to an array of Complex values.
        /// </summary>
        /// <returns>Array of values of the array, converted to Complex, or null if the conversion is not possible.</returns>
        public override Complex[]? ConvertToComplexArray()
        {
            return Data switch
            {
                Complex[] data => data,
                ComplexOf<sbyte>[] ofs => ConvertToComplex(ofs),
                ComplexOf<byte>[] ofs => ConvertToComplex(ofs),
                ComplexOf<short>[] ofs => ConvertToComplex(ofs),
                ComplexOf<ushort>[] ofs => ConvertToComplex(ofs),
                ComplexOf<int>[] ofs => ConvertToComplex(ofs),
                ComplexOf<uint>[] ofs => ConvertToComplex(ofs),
                ComplexOf<long>[] ofs => ConvertToComplex(ofs),
                ComplexOf<ulong>[] ofs => ConvertToComplex(ofs),
                ComplexOf<float>[] ofs => ConvertToComplex(ofs),
                _ => ConvertToComplex(ConvertToDoubleArray())
            };
        }

        private static Complex[] ConvertToComplex(ComplexOf<sbyte>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<byte>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<short>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<ushort>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<int>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<uint>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<long>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<ulong>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(ComplexOf<float>[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(Convert.ToDouble(array[i].Real), Convert.ToDouble(array[i].Imaginary));
            }

            return result;
        }

        private static Complex[] ConvertToComplex(double[] array)
        {
            var result = new Complex[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new Complex(array[i], 0.0);
            }

            return result;
        }
    }
}