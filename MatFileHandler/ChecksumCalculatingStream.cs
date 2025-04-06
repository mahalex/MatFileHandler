// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;

namespace MatFileHandler
{
    /// <summary>
    /// A stream that calculates Adler32 checksum of everything
    /// written to it before passing to another stream.
    /// </summary>
    internal class ChecksumCalculatingStream : Stream
    {
        private const uint BigPrime = 0xFFF1;
        private readonly Stream _stream;
        private uint s1 = 1;
        private uint s2 = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumCalculatingStream"/> class.
        /// </summary>
        /// <param name="stream">Wrapped stream.</param>
        public ChecksumCalculatingStream(Stream stream)
        {
            _stream = stream;
        }

        /// <inheritdoc />
        public override bool CanRead => false;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override long Length => throw new NotImplementedException();

        /// <inheritdoc />
        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            for (var i = offset; i < offset + count; i++)
            {
                s1 = (s1 + buffer[i]) % BigPrime;
                s2 = (s2 + s1) % BigPrime;
            }

            _stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Calculate the checksum of everything written to the stream so far.
        /// </summary>
        /// <returns>Checksum of everything written to the stream so far.</returns>
        public uint GetCrc()
        {
            return (s2 << 16) | s1;
        }
    }
}