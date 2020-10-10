// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MatFileHandler
{
    /// <summary>
    /// Structure array.
    /// </summary>
    internal class MatStructureArray : MatArray, IStructureArray
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatStructureArray"/> class.
        /// </summary>
        /// <param name="flags">Array properties.</param>
        /// <param name="dimensions">Dimensions of the array.</param>
        /// <param name="name">Array name.</param>
        /// <param name="fields">Array contents.</param>
        public MatStructureArray(
            ArrayFlags flags,
            int[] dimensions,
            string name,
            Dictionary<string, List<IArray>> fields)
            : base(flags, dimensions, name)
        {
            Fields = fields;
        }

        /// <inheritdoc />
        public IEnumerable<string> FieldNames => Fields.Keys;

        /// <summary>
        /// Gets null: not implemented.
        /// </summary>
        public IReadOnlyDictionary<string, IArray>[] Data
        {
            get
            {
                var result = new IReadOnlyDictionary<string, IArray>[Count];
                for (var i = 0; i < Count; i++)
                {
                    result[i] = FieldNames.ToDictionary(name => name, name => Fields[name][i]);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a dictionary that maps field names to lists of values.
        /// </summary>
        internal Dictionary<string, List<IArray>> Fields { get; }

        /// <inheritdoc />
        public IArray this[string field, params int[] list]
        {
            get => Fields[field][Dimensions.DimFlatten(list)];
            set => Fields[field][Dimensions.DimFlatten(list)] = value;
        }

        /// <inheritdoc />
        IReadOnlyDictionary<string, IArray> IArrayOf<IReadOnlyDictionary<string, IArray>>.this[params int[] list]
        {
            get => ExtractStructure(Dimensions.DimFlatten(list));
            set => throw new NotSupportedException(
                "Cannot set structure elements via this[params int[]] indexer. Use this[string, int[]] instead.");
        }

        private IReadOnlyDictionary<string, IArray> ExtractStructure(int i)
        {
            return new MatStructureArrayElement(this, i);
        }

        /// <summary>
        /// Provides access to an element of a structure array by fields.
        /// </summary>
        internal class MatStructureArrayElement : IReadOnlyDictionary<string, IArray>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MatStructureArrayElement"/> class.
            /// </summary>
            /// <param name="parent">Parent structure array.</param>
            /// <param name="index">Index in the structure array.</param>
            internal MatStructureArrayElement(MatStructureArray parent, int index)
            {
                Parent = parent;
                Index = index;
            }

            /// <summary>
            /// Gets the number of fields.
            /// </summary>
            public int Count => Parent.Fields.Count;

            /// <summary>
            /// Gets a list of all fields.
            /// </summary>
            public IEnumerable<string> Keys => Parent.Fields.Keys;

            /// <summary>
            /// Gets a list of all values.
            /// </summary>
            public IEnumerable<IArray> Values => Parent.Fields.Values.Select(array => array[Index]);

            private MatStructureArray Parent { get; }

            private int Index { get; }

            /// <summary>
            /// Gets the value of a given field.
            /// </summary>
            /// <param name="key">Field name.</param>
            /// <returns>The corresponding value.</returns>
            public IArray this[string key] => Parent.Fields[key][Index];

            /// <summary>
            /// Enumerates fieldstructure/value pairs of the dictionary.
            /// </summary>
            /// <returns>All field/value pairs in the structure.</returns>
            public IEnumerator<KeyValuePair<string, IArray>> GetEnumerator()
            {
                foreach (var field in Parent.Fields)
                {
                    yield return new KeyValuePair<string, IArray>(field.Key, field.Value[Index]);
                }
            }

            /// <summary>
            /// Enumerates field/value pairs of the structure.
            /// </summary>
            /// <returns>All field/value pairs in the structure.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Checks if the structure has a given field.
            /// </summary>
            /// <param name="key">Field name.</param>
            /// <returns>True iff the structure has a given field.</returns>
            public bool ContainsKey(string key) => Parent.Fields.ContainsKey(key);

            /// <summary>
            /// Tries to get the value of a given field.
            /// </summary>
            /// <param name="key">Field name.</param>
            /// <param name="value">Value (or null if the field is not present).</param>
            /// <returns>Success status of the query.</returns>
#pragma warning disable CS8767
            public bool TryGetValue(string key, out IArray? value)
#pragma warning restore CS8767
            {
                var success = Parent.Fields.TryGetValue(key, out var array);
                if (!success)
                {
                    value = default;
                    return false;
                }
                value = array[Index];
                return true;
            }
        }
    }
}
