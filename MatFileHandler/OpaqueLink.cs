// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MatFileHandler
{
    /// <summary>
    /// Implementation of Matlab's "opaque objects" via links to subsystem data.
    /// </summary>
    internal class OpaqueLink : Opaque, IMatObject
    {
        private readonly SubsystemData subsystemData;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpaqueLink"/> class.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="typeDescription">Description of object's class.</param>
        /// <param name="className">Name of the object's class.</param>
        /// <param name="dimensions">Dimensions of the object.</param>
        /// <param name="data">Raw data containing links to object's storage.</param>
        /// <param name="indexToObjectId">Links to object's storage.</param>
        /// <param name="classIndex">Index of object's class.</param>
        /// <param name="subsystemData">Reference to global subsystem data.</param>
        public OpaqueLink(
            string name,
            string typeDescription,
            string className,
            int[] dimensions,
            DataElement data,
            int[] indexToObjectId,
            int classIndex,
            SubsystemData subsystemData)
            : base(name, typeDescription, className, dimensions, data)
        {
            IndexToObjectId = indexToObjectId ?? throw new ArgumentNullException(nameof(indexToObjectId));
            ClassIndex = classIndex;
            this.subsystemData = subsystemData ?? throw new ArgumentNullException(nameof(subsystemData));
        }

        /// <summary>
        /// Gets index of this object's class in subsystem data class list.
        /// </summary>
        public int ClassIndex { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IArray>[] Data => null;

        /// <inheritdoc />
        public IEnumerable<string> FieldNames => FieldNamesArray;

        /// <summary>
        /// Gets links to the fields stored in subsystem data.
        /// </summary>
        public int[] IndexToObjectId { get; }

        /// <summary>
        /// Gets name of this object's class.
        /// </summary>
        public override string ClassName => subsystemData.ClassInformation[ClassIndex].Name;

        private string[] FieldNamesArray => subsystemData.ClassInformation[ClassIndex].FieldNames.ToArray();

        /// <inheritdoc />
        public IArray this[string field, params int[] list]
        {
            get
            {
                if (TryGetValue(field, out var result, list))
                {
                    return result;
                }

                throw new IndexOutOfRangeException();
            }
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IArray> this[params int[] list]
        {
            get => ExtractObject(Dimensions.DimFlatten(list));
            set => throw new NotImplementedException();
        }

        private IReadOnlyDictionary<string, IArray> ExtractObject(int i)
        {
            return new OpaqueObjectArrayElement(this, i);
        }

        private bool TryGetValue(string field, out IArray output, params int[] list)
        {
            var index = Dimensions.DimFlatten(list);
            var maybeFieldIndex = subsystemData.ClassInformation[ClassIndex].FindField(field);
            if (!(maybeFieldIndex is int fieldIndex))
            {
                output = default(IArray);
                return false;
            }

            if (index >= IndexToObjectId.Length)
            {
                output = default(IArray);
                return false;
            }

            var objectId = IndexToObjectId[index];
            var objectInfo = subsystemData.ObjectInformation[objectId];
            var fieldId = objectInfo.FieldLinks[fieldIndex];
            output = subsystemData.Data[fieldId];
            return true;
        }

        /// <summary>
        /// Provides access to a single object in object array.
        /// </summary>
        internal class OpaqueObjectArrayElement : IReadOnlyDictionary<string, IArray>
        {
            private readonly int index;
            private readonly OpaqueLink parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="OpaqueObjectArrayElement"/> class.
            /// </summary>
            /// <param name="parent">Parent object array.</param>
            /// <param name="index">Index of the object in the array.</param>
            public OpaqueObjectArrayElement(OpaqueLink parent, int index)
            {
                this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
                this.index = index;
            }

            /// <inheritdoc />
            public int Count => parent.FieldNamesArray.Length;

            /// <inheritdoc />
            public IEnumerable<string> Keys => parent.FieldNames;

            /// <inheritdoc />
            public IEnumerable<IArray> Values => parent.FieldNames.Select(fieldName => parent[fieldName, index]);

            /// <inheritdoc />
            public IArray this[string key] => parent[key, index];

            /// <inheritdoc />
            public bool ContainsKey(string key)
            {
                return parent.FieldNames.Contains(key);
            }

            /// <inheritdoc />
            public IEnumerator<KeyValuePair<string, IArray>> GetEnumerator()
            {
                foreach (var field in parent.FieldNamesArray)
                {
                    yield return new KeyValuePair<string, IArray>(field, parent[field, index]);
                }
            }

            /// <inheritdoc />
            public bool TryGetValue(string key, out IArray value)
            {
                return parent.TryGetValue(key, out value, index);
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}