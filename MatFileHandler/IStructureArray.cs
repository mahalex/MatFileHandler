// Copyright 2017-2018 Alexander Luzgarev

using System.Collections.Generic;

namespace MatFileHandler
{
    /// <summary>
    /// Matlab's structure array.
    /// </summary>
    public interface IStructureArray : IArrayOf<IReadOnlyDictionary<string, IArray>>
    {
        /// <summary>
        /// Gets a list of all fields in the structure.
        /// </summary>
        IEnumerable<string> FieldNames { get; }

        /// <summary>
        /// Get value of a given structure's field.
        /// </summary>
        /// <param name="field">Field name.</param>
        /// <param name="list">Index of the element in the structure array.</param>
        IArray this[string field, params int[] list] { get; set; }
    }
}