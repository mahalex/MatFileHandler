// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// A stream which wraps another stream and only reads one byte at a time.
    /// </summary>
    internal class PartialUnseekableReadStream : Stream
    {
        private readonly Stream _baseStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialUnseekableReadStream"/> class.
        /// </summary>
        /// <param name="baseStream">The stream to wrap.</param>
        public PartialUnseekableReadStream(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        /// <inheritdoc/>
        public override bool CanRead => _baseStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length => _baseStream.Length;

        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            _baseStream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, Math.Min(1, count));
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _baseStream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
