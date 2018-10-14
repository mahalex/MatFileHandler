// Copyright 2017-2018 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// Raw variable read from the file.
    /// This gives a way to deal with "subsystem data" which looks like
    /// a variable and can only be detected by comparing its offset with
    /// the value stored in the file's header.
    /// </summary>
    internal class RawVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawVariable"/> class.
        /// </summary>
        /// <param name="offset">Offset of the variable in the source file.</param>
        /// <param name="dataElement">Data element parsed from the file.</param>
        internal RawVariable(long offset, DataElement dataElement)
        {
            Offset = offset;
            DataElement = dataElement ?? throw new ArgumentNullException(nameof(dataElement));
        }

        /// <summary>
        /// Gets data element with the variable's contents.
        /// </summary>
        public DataElement DataElement { get; }

        /// <summary>
        /// Gets offset of the variable in the .mat file.
        /// </summary>
        public long Offset { get; }
    }
}