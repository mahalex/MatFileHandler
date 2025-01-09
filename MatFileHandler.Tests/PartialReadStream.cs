// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// A stream which wraps another stream and only reads one byte at a time.
    /// </summary>
    internal class PartialReadStream : Stream
    {
        private readonly Stream _baseStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialReadStream"/> class.
        /// </summary>
        /// <param name="baseStream">The stream to wrap.</param>
        public PartialReadStream(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        /// <inheritdoc/>
        public override bool CanRead => _baseStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => _baseStream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length => _baseStream.Length;

        /// <inheritdoc/>
        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
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
            return _baseStream.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
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
