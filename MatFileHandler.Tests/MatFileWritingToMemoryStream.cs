// Copyright 2017-2018 Alexander Luzgarev

using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// A method of writing an IMatFile into a MemoryStream.
    /// </summary>
    public class MatFileWritingToMemoryStream : MatFileWritingMethod
    {
        private readonly MatFileWriterOptions? _maybeOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileWritingToMemoryStream"/> class.
        /// </summary>
        /// <param name="maybeOptions">Options for the <see cref="MatFileWriter"/>.</param>
        public MatFileWritingToMemoryStream(MatFileWriterOptions? maybeOptions)
        {
            _maybeOptions = maybeOptions;
        }

        /// <inheritdoc />
        public override byte[] WriteMatFile(IMatFile matFile)
        {
            using var memoryStream = new MemoryStream();
            var matFileWriter = _maybeOptions switch
            {
                { } options => new MatFileWriter(memoryStream, options),
                _ => new MatFileWriter(memoryStream),
            };

            matFileWriter.Write(matFile);
            return memoryStream.ToArray();
        }
    }
}