// Copyright 2017 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// An interface for accessing the contents of .mat files.
    /// </summary>
    public interface IMatFile
    {
        /// <summary>
        /// Gets a list of variables present in the file.
        /// </summary>
        /// <remarks>
        /// Variables are in the order in which they appear in the .mat file.
        /// </remarks>
        IVariable[] Variables { get; }

        /// <summary>
        /// Lookup variable by name.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        IVariable this[string variableName] { get; set; }
    }
}