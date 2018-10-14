// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// Describes compression usage strategy for writing files.
    /// </summary>
    public enum CompressionUsage
    {
        /// <summary>
        /// Never use compression.
        /// </summary>
        Never,

        /// <summary>
        /// Always use compression.
        /// </summary>
        Always,
    }
}
