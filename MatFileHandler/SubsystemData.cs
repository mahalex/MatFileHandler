// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;

namespace MatFileHandler
{
    /// <summary>
    /// "Subsystem data" of the .mat file.
    /// Subsystem data stores the actual contents
    /// of all the "opaque objects" in the file.
    /// </summary>
    internal class SubsystemData
    {
        private RealSubsystemData? _realData;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubsystemData"/> class.
        /// Default constructor: initializes everything to null.
        /// </summary>
        public SubsystemData()
        {
            _realData = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubsystemData"/> class.
        /// The actual constructor.
        /// </summary>
        /// <param name="classInformation">Information about the classes.</param>
        /// <param name="objectInformation">Information about the objects.</param>
        /// <param name="fieldNames">Field names.</param>
        /// <param name="data">Field values.</param>
        public SubsystemData(
            Dictionary<int, ClassInfo> classInformation,
            Dictionary<int, ObjectInfo> objectInformation,
            string[] fieldNames,
            Dictionary<int, IArray> data)
        {
            _realData = new RealSubsystemData(
                classInformation,
                objectInformation,
                fieldNames,
                data);
        }

        /// <summary>
        /// Gets information about all the classes occurring in the file.
        /// </summary>
        public Dictionary<int, ClassInfo> ClassInformation =>
            _realData?.ClassInformation ?? throw new HandlerException("Subsystem data missing.");

        /// <summary>
        /// Gets the actual data: mapping of "object field" indices to their values.
        /// </summary>
        public IReadOnlyDictionary<int, IArray> Data =>
            _realData?.Data ?? throw new HandlerException("Subsystem data missing.");

        /// <summary>
        /// Gets information about all the objects occurring in the file.
        /// </summary>
        public Dictionary<int, ObjectInfo> ObjectInformation =>
            _realData?.ObjectInformation ?? throw new HandlerException("Subsystem data missing.");

        /// <summary>
        /// Gets field names.
        /// </summary>
        public string[] FieldNames =>
            _realData?.FieldNames ?? throw new HandlerException("Subsystem data missing.");

        /// <summary>
        /// Initialize this object from another object.
        /// This ugly hack allows us to read the opaque objects and store references to
        /// the subsystem data in them before parsing the actual subsystem data (which
        /// comes later in the file).
        /// </summary>
        /// <param name="data">Another subsystem data.</param>
        public void Set(SubsystemData data)
        {
            _realData = new RealSubsystemData(
                data.ClassInformation,
                data.ObjectInformation,
                data.FieldNames,
                data.Data);
        }

        /// <summary>
        /// Stores information about a class.
        /// </summary>
        internal class ClassInfo
        {
            private readonly Dictionary<string, int> fieldToIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="ClassInfo"/> class.
            /// </summary>
            /// <param name="name">Class name.</param>
            /// <param name="fieldToIndex">A dictionary mapping field names to field ids.</param>
            public ClassInfo(string name, Dictionary<string, int> fieldToIndex)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                this.fieldToIndex = fieldToIndex ?? throw new ArgumentNullException(nameof(fieldToIndex));
            }

            /// <summary>
            /// Gets names of the fields in the class.
            /// </summary>
            public IReadOnlyCollection<string> FieldNames => fieldToIndex.Keys;

            /// <summary>
            /// Gets name of the class.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Find a field index given its name.
            /// </summary>
            /// <param name="fieldName">Field name.</param>
            /// <returns>Field index.</returns>
            public int? FindField(string fieldName)
            {
                if (fieldToIndex.TryGetValue(fieldName, out var index))
                {
                    return index;
                }

                return null;
            }
        }

        /// <summary>
        /// Stores information about an object.
        /// </summary>
        internal class ObjectInfo
        {
            private readonly Dictionary<int, int> fieldLinks;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectInfo"/> class.
            /// </summary>
            /// <param name="fieldLinks">A dictionary mapping the field indices to "field values" indices.</param>
            public ObjectInfo(Dictionary<int, int> fieldLinks)
            {
                this.fieldLinks = fieldLinks ?? throw new ArgumentNullException(nameof(fieldLinks));
            }

            /// <summary>
            /// Gets mapping between the field indices and "field values" indices.
            /// </summary>
            public IReadOnlyDictionary<int, int> FieldLinks => fieldLinks;
        }

        private class RealSubsystemData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RealSubsystemData"/> class.
            /// </summary>
            /// <param name="classInformation">Class information.</param>
            /// <param name="objectInformation">Object information.</param>
            /// <param name="fieldNames">Field names.</param>
            /// <param name="data">Data.</param>
            public RealSubsystemData(
                Dictionary<int, ClassInfo> classInformation,
                Dictionary<int, ObjectInfo> objectInformation,
                string[] fieldNames,
                IReadOnlyDictionary<int, IArray> data)
            {
                ClassInformation = classInformation ?? throw new ArgumentNullException(nameof(classInformation));
                ObjectInformation = objectInformation ?? throw new ArgumentNullException(nameof(objectInformation));
                FieldNames = fieldNames;
                Data = data ?? throw new ArgumentNullException(nameof(data));
            }

            /// <summary>
            /// Gets information about all the classes occurring in the file.
            /// </summary>
            public Dictionary<int, ClassInfo> ClassInformation { get; }

            /// <summary>
            /// Gets the actual data: mapping of "object field" indices to their values.
            /// </summary>
            public IReadOnlyDictionary<int, IArray> Data { get; }

            /// <summary>
            /// Gets information about all the objects occurring in the file.
            /// </summary>
            public Dictionary<int, ObjectInfo> ObjectInformation { get; }

            /// <summary>
            /// Gets field names.
            /// </summary>
            public string[] FieldNames { get; }
        }
    }
}