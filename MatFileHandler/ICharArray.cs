// Copyright 2017 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// Matlab's character array.
    /// </summary>
    public interface ICharArray : IArrayOf<char>
    {
        /// <summary>
        /// Gets the contained string.
        /// </summary>
        string String { get; }
    }
}