// Copyright 2017-2018 Alexander Luzgarev

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

        /// <summary>
        /// Gets the variable with the specified name.
        /// </summary>
        /// <param name="name">The name of the variable to get.</param>
        /// <param name="variable">When this method returns, contains the variable with the specified name, if it is found; otherwise, null.</param>
        /// <returns>True if the file contains a variable with the specified name; otherwise, false.</returns>
        public bool TryGetVariable(string name, out IVariable? variable);
    }
}