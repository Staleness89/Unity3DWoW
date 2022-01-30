using System.IO;

    /// <summary>
    ///     View a part of a stream. All offsets and lengths will be relative to this part.
    ///     If '=' means 4 bytes and [ is when the substream was created : ====[====(You're here)========>
    ///     Position in inner stream : 32. Position in substream : 16.
    ///     Length of inner stream : 64. Length of substream : 48.
    /// </summary>
    public class Substream : Stream
    {
        private readonly long _beginning;
        private readonly Stream _innerStream;

        public Stream GetInnerStream()
        {
            return _innerStream;
        }

        public Substream(Stream inner)
        {
            _innerStream = inner;
            _beginning = inner.Position;
        }

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length - _beginning;

        public override long Position
        {
            get { return _innerStream.Position - _beginning; }

            set { _innerStream.Position = value + _beginning; }
        }

        public override void Flush() => _innerStream.Flush();

        public override long Seek(long offset, SeekOrigin origin)
            => _innerStream.Seek(offset + _beginning, origin) - _beginning;

        public override void SetLength(long value) => _innerStream.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);

        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);
    }
