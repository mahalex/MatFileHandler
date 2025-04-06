// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler.Tests
{
    /// <summary>
    /// A method of writing IMatFile into a byte buffer.
    /// </summary>
    public abstract class MatFileWritingMethod
    {
        /// <summary>
        /// Write an IMatFile into a byte buffer.
        /// </summary>
        /// <param name="matFile"></param>
        /// <returns></returns>
        public abstract byte[] WriteMatFile(IMatFile matFile);
    }
}