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
        /// <summary>
        /// Initializes a new instance of the <see cref="SubsystemData"/> class.
        /// Default constructor: initializes everything to null.
        /// </summary>
        public SubsystemData()
        {
            ClassInformation = null;
            ObjectInformation = null;
            Data = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubsystemData"/> class.
        /// The actual constructor.
        /// </summary>
        /// <param name="classInformation">Information about the classes.</param>
        /// <param name="objectInformation">Information about the objects.</param>
        /// <param name="data">Field values.</param>
        public SubsystemData(ClassInfo[] classInformation, ObjectInfo[] objectInformation, Dictionary<int, IArray> data)
        {
            this.ClassInformation =
                classInformation ?? throw new ArgumentNullException(nameof(classInformation));
            this.ObjectInformation =
                objectInformation ?? throw new ArgumentNullException(nameof(objectInformation));
            this.Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Gets or sets information about all the classes occurring in the file.
        /// </summary>
        public ClassInfo[] ClassInformation { get; set; }

        /// <summary>
        /// Gets or sets the actual data: mapping of "object field" indices to their values.
        /// </summary>
        public IReadOnlyDictionary<int, IArray> Data { get; set; }

        /// <summary>
        /// Gets or sets information about all the objects occurring in the file.
        /// </summary>
        public ObjectInfo[] ObjectInformation { get; set; }

        /// <summary>
        /// Initialize this object from another object.
        /// This ugly hack allows us to read the opaque objects and store references to
        /// the subsystem data in them before parsing the actual subsystem data (which
        /// comes later in the file).
        /// </summary>
        /// <param name="data">Another subsystem data.</param>
        public void Set(SubsystemData data)
        {
            this.ClassInformation = data.ClassInformation;
            this.ObjectInformation = data.ObjectInformation;
            this.Data = data.Data;
        }

        /// <summary>
        /// Stores information about a class.
        /// </summary>
        internal class ClassInfo
        {
            private readonly string[] fieldNames;

            private readonly Dictionary<string, int> fieldToIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="ClassInfo"/> class.
            /// </summary>
            /// <param name="name">Class name.</param>
            /// <param name="fieldNames">Names of the fields.</param>
            public ClassInfo(string name, string[] fieldNames)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                this.fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));
                fieldToIndex = new Dictionary<string, int>();
                for (var i = 0; i < fieldNames.Length; i++)
                {
                    fieldToIndex[fieldNames[i]] = i;
                }
            }

            /// <summary>
            /// Gets names of the fields in the class.
            /// </summary>
            public IReadOnlyCollection<string> FieldNames => fieldNames;

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
    }
}