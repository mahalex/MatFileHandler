// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatFileHandler
{
    /// <summary>
    /// Class for building arrays that later can be written to a .mat file.
    /// </summary>
    public class DataBuilder
    {
        /// <summary>
        /// Create a new numerical/logical array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <returns>An array of given element type and dimensions, initialized by zeros.</returns>
        /// <remarks>
        /// Possible values of T:
        ///   Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double,
        ///   ComplexOf&lt;TReal&gt; (where TReal is one of Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single),
        ///   Complex, Boolean.
        /// </remarks>
        public IArrayOf<T> NewArray<T>(params int[] dimensions)
            where T : struct
        {
            return new MatNumericalArrayOf<T>(
                GetStandardFlags<T>(),
                dimensions,
                string.Empty,
                new T[dimensions.NumberOfElements()]);
        }

        /// <summary>
        /// Create a new numerical/logical array and initialize it with the given data.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="data">Initial data.</param>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <returns>An array of given dimensions, initialized by the provided data.</returns>
        /// <remarks>
        /// Possible values of T:
        ///   Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double,
        ///   ComplexOf&lt;TReal&gt; (where TReal is one of Int8, UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single),
        ///   Complex, Boolean.
        /// </remarks>
        public IArrayOf<T> NewArray<T>(T[] data, params int[] dimensions)
            where T : struct
        {
            if (data.Length != dimensions.NumberOfElements())
            {
                throw new ArgumentException("Data size does not match the specified dimensions", "data");
            }
            return new MatNumericalArrayOf<T>(GetStandardFlags<T>(), dimensions, string.Empty, data);
        }

        /// <summary>
        /// Create a new cell array.
        /// </summary>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <returns>A cell array of given dimensions, consisting of empty arrays.</returns>
        public ICellArray NewCellArray(params int[] dimensions)
        {
            var flags = ConstructArrayFlags(ArrayType.MxCell);
            var elements = Enumerable.Repeat(MatArray.Empty() as IArray, dimensions.NumberOfElements()).ToList();
            return new MatCellArray(flags, dimensions, string.Empty, elements);
        }

        /// <summary>
        /// Create a new structure array.
        /// </summary>
        /// <param name="fields">Names of structure fields.</param>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <returns>A structure array of given dimensions with given fields, consisting of empty arrays.</returns>
        public IStructureArray NewStructureArray(IEnumerable<string> fields, params int[] dimensions)
        {
            var flags = ConstructArrayFlags(ArrayType.MxStruct);
            var elements = Enumerable.Repeat(MatArray.Empty() as IArray, dimensions.NumberOfElements()).ToList();
            var dictionary = new Dictionary<string, List<IArray>>();
            foreach (var field in fields)
            {
                dictionary[field] = elements.ToList();
            }
            return new MatStructureArray(flags, dimensions, string.Empty, dictionary);
        }

        /// <summary>
        /// Create a new character array.
        /// </summary>
        /// <param name="contents">A string to initialize the array.</param>
        /// <returns>A 1xn character array with the given string as contents.</returns>
        public ICharArray NewCharArray(string contents)
        {
            return NewCharArray(contents, 1, contents.Length);
        }

        /// <summary>
        /// Create a new character array of specified dimensions.
        /// </summary>
        /// <param name="contents">A string to initialize the array.</param>
        /// <param name="dimensions">The dimensions of the array.</param>
        /// <returns>A character array of given dimensions with the given string as contents.</returns>
        public ICharArray NewCharArray(string contents, params int[] dimensions)
        {
            var flags = ConstructArrayFlags(ArrayType.MxChar);
            var ushortArray = contents.ToCharArray().Select(c => (ushort)c).ToArray();
            return new MatCharArrayOf<ushort>(flags, dimensions, string.Empty, ushortArray, contents);
        }

        /// <summary>
        /// Create a new empty array.
        /// </summary>
        /// <returns>An empty array.</returns>
        public IArray NewEmpty()
        {
            return MatArray.Empty();
        }

        /// <summary>
        /// Create a new sparse array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="dimensions">The dimensions of the array.</param>
        /// <returns>An empty sparse array of given type and given dimensions.</returns>
        public ISparseArrayOf<T> NewSparseArray<T>(params int[] dimensions)
          where T : struct
        {
            return new MatSparseArrayOf<T>(
                GetStandardSparseArrayFlags<T>(),
                dimensions,
                string.Empty,
                new Dictionary<(int, int), T>());
        }

        /// <summary>
        /// Create a new variable.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        /// <param name="isGlobal">Global flag for the variable.</param>
        /// <returns>A new variable with given name and value.</returns>
        public IVariable NewVariable(string name, IArray value, bool isGlobal = false)
        {
            return new MatVariable(value, name, isGlobal);
        }

        /// <summary>
        /// Create a new Matlab file.
        /// </summary>
        /// <param name="variables">Variables in the file.</param>
        /// <returns>A file containing the provided variables.</returns>
        public IMatFile NewFile(IEnumerable<IVariable> variables)
        {
            return new MatFile(variables);
        }

        private ArrayFlags ConstructArrayFlags(ArrayType class_, bool isComplex = false, bool isLogical = false)
        {
            return new ArrayFlags
            {
                Class = class_,
                Variable = (isComplex ? Variable.IsComplex : 0) |
                           (isLogical ? Variable.IsLogical : 0),
            };
        }

        private ArrayFlags GetStandardFlags<T>()
        {
            if (typeof(T) == typeof(sbyte))
            {
                return ConstructArrayFlags(ArrayType.MxInt8);
            }
            if (typeof(T) == typeof(ComplexOf<sbyte>))
            {
                return ConstructArrayFlags(ArrayType.MxInt8, isComplex: true);
            }
            if (typeof(T) == typeof(byte))
            {
                return ConstructArrayFlags(ArrayType.MxUInt8);
            }
            if (typeof(T) == typeof(ComplexOf<byte>))
            {
                return ConstructArrayFlags(ArrayType.MxUInt8, isComplex: true);
            }
            if (typeof(T) == typeof(short))
            {
                return ConstructArrayFlags(ArrayType.MxInt16);
            }
            if (typeof(T) == typeof(ComplexOf<short>))
            {
                return ConstructArrayFlags(ArrayType.MxInt16, isComplex: true);
            }
            if (typeof(T) == typeof(ushort))
            {
                return ConstructArrayFlags(ArrayType.MxUInt16);
            }
            if (typeof(T) == typeof(ComplexOf<ushort>))
            {
                return ConstructArrayFlags(ArrayType.MxUInt16, isComplex: true);
            }
            if (typeof(T) == typeof(int))
            {
                return ConstructArrayFlags(ArrayType.MxInt32);
            }
            if (typeof(T) == typeof(ComplexOf<int>))
            {
                return ConstructArrayFlags(ArrayType.MxInt32, isComplex: true);
            }
            if (typeof(T) == typeof(uint))
            {
                return ConstructArrayFlags(ArrayType.MxUInt32);
            }
            if (typeof(T) == typeof(ComplexOf<uint>))
            {
                return ConstructArrayFlags(ArrayType.MxUInt32, isComplex: true);
            }
            if (typeof(T) == typeof(long))
            {
                return ConstructArrayFlags(ArrayType.MxInt64);
            }
            if (typeof(T) == typeof(ComplexOf<long>))
            {
                return ConstructArrayFlags(ArrayType.MxInt64, isComplex: true);
            }
            if (typeof(T) == typeof(ulong))
            {
                return ConstructArrayFlags(ArrayType.MxUInt64);
            }
            if (typeof(T) == typeof(ComplexOf<ulong>))
            {
                return ConstructArrayFlags(ArrayType.MxUInt64, isComplex: true);
            }
            if (typeof(T) == typeof(float))
            {
                return ConstructArrayFlags(ArrayType.MxSingle);
            }
            if (typeof(T) == typeof(ComplexOf<float>))
            {
                return ConstructArrayFlags(ArrayType.MxSingle, isComplex: true);
            }
            if (typeof(T) == typeof(double))
            {
                return ConstructArrayFlags(ArrayType.MxDouble);
            }
            if (typeof(T) == typeof(Complex))
            {
                return ConstructArrayFlags(ArrayType.MxDouble, isComplex: true);
            }
            if (typeof(T) == typeof(bool))
            {
                return ConstructArrayFlags(ArrayType.MxInt8, isLogical: true);
            }
            return ConstructArrayFlags(ArrayType.MxObject);
        }

        private SparseArrayFlags GetStandardSparseArrayFlags<T>()
        {
            var arrayFlags = GetStandardFlags<T>();
            return new SparseArrayFlags
            {
                ArrayFlags = arrayFlags,
                NzMax = 0,
            };
        }
    }
}