// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// A method of writing an IMatFile into a stream that is "unaligned".
    /// </summary>
    public class MatFileWritingToUnalignedMemoryStream : MatFileWritingMethod
    {
        private readonly MatFileWriterOptions? _maybeOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileWritingToUnalignedMemoryStream"/> class.
        /// </summary>
        /// <param name="maybeOptions">Options for the <see cref="MatFileWriter"/>.</param>
        public MatFileWritingToUnalignedMemoryStream(MatFileWriterOptions? maybeOptions)
        {
            _maybeOptions = maybeOptions;
        }

        /// <inheritdoc />
        public override byte[] WriteMatFile(IMatFile matFile)
        {
            using var memoryStream = new MemoryStream();
            memoryStream.Seek(3, SeekOrigin.Begin);
            var matFileWriter = _maybeOptions switch
            {
                { } options => new MatFileWriter(memoryStream, options),
                _ => new MatFileWriter(memoryStream),
            };

            matFileWriter.Write(matFile);
            var fullArray = memoryStream.ToArray();
            var length = fullArray.Length - 3;
            var result = new byte[length];
            Buffer.BlockCopy(fullArray, 3, result, 0, length);
            return result;
        }
    }
}