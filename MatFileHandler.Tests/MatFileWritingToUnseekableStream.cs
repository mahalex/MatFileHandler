// Copyright 2017-2018 Alexander Luzgarev

using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// A method of writing an IMatFile into a stream that is not seekable.
    /// </summary>
    public class MatFileWritingToUnseekableStream : MatFileWritingMethod
    {
        private readonly MatFileWriterOptions? _maybeOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileWritingToUnseekableStream"/> class.
        /// </summary>
        /// <param name="maybeOptions">Options for the <see cref="MatFileWriter"/>.</param>
        public MatFileWritingToUnseekableStream(MatFileWriterOptions? maybeOptions)
        {
            _maybeOptions = maybeOptions;
        }

        /// <inheritdoc />
        public override byte[] WriteMatFile(IMatFile matFile)
        {
            using var memoryStream = new MemoryStream();
            using var unseekableStream = new UnseekableWriteStream(memoryStream);
            var matFileWriter = _maybeOptions switch
            {
                { } options => new MatFileWriter(unseekableStream, options),
                _ => new MatFileWriter(unseekableStream),
            };

            matFileWriter.Write(matFile);
            return memoryStream.ToArray();
        }
    }
}