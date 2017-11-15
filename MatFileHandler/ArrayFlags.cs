// Copyright 2017 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// Type of a Matlab array.
    /// </summary>
    internal enum ArrayType
    {
        /// <summary>
        /// Cell array.
        /// </summary>
        MxCell = 1,

        /// <summary>
        /// Structure array.
        /// </summary>
        MxStruct = 2,

        /// <summary>
        /// Matlab object.
        /// </summary>
        MxObject = 3,

        /// <summary>
        /// Character array.
        /// </summary>
        MxChar = 4,

        /// <summary>
        /// Sparse array.
        /// </summary>
        MxSparse = 5,

        /// <summary>
        /// Double array.
        /// </summary>
        MxDouble = 6,

        /// <summary>
        /// Single array.
        /// </summary>
        MxSingle = 7,

        /// <summary>
        /// Int8 array.
        /// </summary>
        MxInt8 = 8,

        /// <summary>
        /// UInt8 array.
        /// </summary>
        MxUInt8 = 9,

        /// <summary>
        /// Int16 array.
        /// </summary>
        MxInt16 = 10,

        /// <summary>
        /// UInt16 array.
        /// </summary>
        MxUInt16 = 11,

        /// <summary>
        /// Int32 array.
        /// </summary>
        MxInt32 = 12,

        /// <summary>
        /// UInt32 array.
        /// </summary>
        MxUInt32 = 13,

        /// <summary>
        /// Int64 array.
        /// </summary>
        MxInt64 = 14,

        /// <summary>
        /// UInt64 array.
        /// </summary>
        MxUInt64 = 15,

        /// <summary>
        /// Undocumented object (?) array type.
        /// </summary>
        MxNewObject = 17,
    }

    /// <summary>
    /// Variable flags.
    /// </summary>
    [Flags]
    internal enum Variable
    {
        /// <summary>
        /// Indicates a logical array.
        /// </summary>
        IsLogical = 2,

        /// <summary>
        /// Indicates a global variable.
        /// </summary>
        IsGlobal = 4,

        /// <summary>
        /// Indicates a complex array.
        /// </summary>
        IsComplex = 8,
    }

    /// <summary>
    /// Array properties.
    /// </summary>
    internal struct ArrayFlags
    {
        /// <summary>
        /// Array type.
        /// </summary>
        public ArrayType Class;

        /// <summary>
        /// Variable flags.
        /// </summary>
        public Variable Variable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayFlags"/> struct.
        /// </summary>
        /// <param name="class_">Array type.</param>
        /// <param name="variable">Variable flags.</param>
        public ArrayFlags(ArrayType class_, Variable variable)
        {
            Class = class_;
            Variable = variable;
        }
    }

    /// <summary>
    /// Sparse array properties.
    /// </summary>
    internal struct SparseArrayFlags
    {
        /// <summary>
        /// Usual array properties.
        /// </summary>
        public ArrayFlags ArrayFlags;

        /// <summary>
        /// Maximal number of non-zero elements.
        /// </summary>
        public uint NzMax;
    }
}