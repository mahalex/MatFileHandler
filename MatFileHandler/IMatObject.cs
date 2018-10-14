// Copyright 2017-2018 Alexander Luzgarev

using System.Collections.Generic;

namespace MatFileHandler
{
    /// <summary>
    /// An interface to access Matlab objects (more precisely, "object arrays").
    /// This is very similar to the <see cref="IStructureArray"/> interface:
    /// an object holds fields that you can access, and the name of its class.
    /// Additionally, you can treat is as an array of dictionaries mapping
    /// field names to contents of fields.
    /// </summary>
    public interface IMatObject : IArrayOf<IReadOnlyDictionary<string, IArray>>
    {
        /// <summary>
        /// Gets the name of object's class.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// Gets the names of object's fields.
        /// </summary>
        IEnumerable<string> FieldNames { get; }

        /// <summary>
        /// Access a given field of a given object in the array.
        /// </summary>
        /// <param name="field">Field name.</param>
        /// <param name="list">Index of the object to access.</param>
        /// <returns>The value of the field in the selected object.</returns>
        IArray this[string field, params int[] list] { get; set; }
    }
}
