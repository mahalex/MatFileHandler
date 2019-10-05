// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Numerics;

namespace MatFileHandler
{
    /// <summary>
    /// Matlab "opaque object" structure.
    /// If this object appears in the "main" section of the .mat file,
    /// it just contains a small data structure pointing to the object's
    /// storage in the "subsystem data" portion of the file.
    /// In this case, an instance of <see cref="OpaqueLink"/> class
    /// will be created.
    /// If this object appears in the "subsystem data" part, it contains
    /// the data of all opaque objects in the file, and that is what we
    /// put into <see cref="RawData"/> property.
    /// </summary>
    internal class Opaque : MatArray, IArray
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Opaque"/> class.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="typeDescription">Type description.</param>
        /// <param name="className">Class name.</param>
        /// <param name="dimensions">Dimensions of the object.</param>
        /// <param name="rawData">Raw object's data.</param>
        public Opaque(string name, string typeDescription, string className, int[] dimensions, DataElement rawData)
            : base(new ArrayFlags(ArrayType.MxOpaque, 0), dimensions, name)
        {
            TypeDescription = typeDescription ?? throw new ArgumentNullException(nameof(typeDescription));
            ClassName = className ?? throw new ArgumentNullException(nameof(className));
            RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
        }

        /// <summary>
        /// Gets class name of the opaque object.
        /// </summary>
        public virtual string ClassName { get; }

        /// <summary>
        /// Gets raw object's data: either links to subsystem data, or actual data.
        /// </summary>
        public DataElement RawData { get; }

        /// <summary>
        /// Gets "type description" of the opaque object.
        /// </summary>
        public string TypeDescription { get; }

        /// <inheritdoc />
        public override Complex[]? ConvertToComplexArray() => null;

        /// <inheritdoc />
        public override double[]? ConvertToDoubleArray() => null;
    }
}