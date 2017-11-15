// Copyright 2017 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// An interface for accessing the variable contents.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the value of the variable.
        /// </summary>
        IArray Value { get; }

        /// <summary>
        /// Gets a value indicating whether the variable is global.
        /// </summary>
        bool IsGlobal { get; }
    }
}