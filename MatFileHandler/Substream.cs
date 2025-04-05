// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;

namespace MatFileHandler
{
    /// <summary>
    /// A stream which reads a finite section of another stream.
    /// </summary>
    internal sealed class Substream : Stream
    {
        private readonly Stream _baseStream;
        private long _bytesRead;

        /// <summary>
        /// Initializes a new instance of the <see cref="Substream"/> class.
        /// </summary>
        /// <param name="baseStream">The <see cref="Stream"/> to wrap.</param>
        /// <param name="length">The number of bytes readable from this <see cref="Substream"/>.</param>
        public Substream(Stream baseStream, long length)
        {
            _baseStream = baseStream;
            Length = length;
        }

        /// <inheritdoc/>
        public override bool CanRead => _baseStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length { get; }

        /// <inheritdoc/>
        public override long Position
        {
            get => _bytesRead;
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Flush() => _baseStream.Flush();

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _baseStream.Read(buffer, offset, (int)Math.Min(count, Length - _bytesRead));

            _bytesRead += bytesRead;

            return bytesRead;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
