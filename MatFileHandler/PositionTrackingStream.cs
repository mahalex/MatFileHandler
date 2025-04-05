// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;

namespace MatFileHandler;

/// <summary>
/// A stream which wraps another stream and tracks the number of bytes read
/// for the purpose of adjusting for padding.
/// </summary>
internal sealed class PositionTrackingStream : Stream
{
    private readonly Stream _baseStream;
    private long _position;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionTrackingStream"/> class.
    /// </summary>
    /// <param name="baseStream">The stream to wrap.</param>
    public PositionTrackingStream(Stream baseStream)
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
        get => _position;
        set => throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override void Flush() => _baseStream.Flush();

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = _baseStream.Read(buffer, offset, count);

        _position += bytesRead;

        return bytesRead;
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void SetLength(long value) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

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
