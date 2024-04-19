using System.Text;

namespace BinarySerializer
{
    public class BinaryStream : IDisposable
    {
        private bool _disposed;
        private uint _offset;
        private uint _length;
        private uint _currentLength;
        private byte[] _buffer;

        /// <summary>
        /// Resizes the buffer - doubles it
        /// </summary>
        private void ResizeBuffer()
        {
            var buffer = new byte[_length * 2];
            Array.Copy(_buffer, buffer, _length);
            _buffer = buffer;
            _length *= 2;
        }

        private void SetBuffer(byte[] data)
        {
            _length = (uint)data.Length;
            _buffer = data;
            _offset = 0;
            _currentLength = _length;
        }

        private byte[] GetBuffer()
        {
            var buffer = new byte[_currentLength];
            Array.Copy(_buffer, buffer, _currentLength);
            return buffer;
        }

        /// <summary>
        /// Default constructor
        /// Preallocates 1024 bytes
        /// </summary>
        public BinaryStream() => Reset(1024);

        /// <summary>
        /// Constructor - Preallocates Size
        /// </summary>
        /// <param name="length">Initial length of buffer</param>
        public BinaryStream(int length) => Reset(length);

        /// <summary>
        /// Constructor - preload with buffer
        /// </summary>
        /// <param name="data">Buffer to load</param>
        public BinaryStream(byte[] data) => SetBuffer(data);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _buffer =
                [
                ];

            _disposed = true;
        }

        /// <summary>
        /// Empties the buffer and recreates it with the given size
        /// </summary>
        /// <param name="length">Initial length of buffer</param>
        private void Reset(int length)
        {
            _length = (uint)length;
            _buffer = new byte[_length];
            _currentLength = 0;
            _offset = 0;
        }

        /// <summary>
        /// Seeks to a new location within the buffer
        /// Works like the Stream::Seek() method
        /// </summary>
        /// <param name="index">The amount to move</param>
        /// <param name="origin">The reference point to seek from</param>
        /// <returns>The new position</returns>
        public int Seek(int index, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (index > _currentLength)
                        throw new ArgumentOutOfRangeException("Can't seek past the end!");
                    if (index < 0)
                        throw new ArgumentOutOfRangeException("Can't seek past the beginning!");
                    _offset = (uint)index;
                    break;

                case SeekOrigin.Current:
                    if (_offset + index > _currentLength)
                        throw new ArgumentOutOfRangeException("Can't seek past the end!");

                    if (_offset + index < 0)
                        throw new ArgumentOutOfRangeException("Can't seek past the beginning!");
                    _offset += (uint)index;
                    break;

                case SeekOrigin.End:
                    if (_currentLength - index > _currentLength)
                        throw new ArgumentOutOfRangeException("Can't seek past the end!");

                    if (_currentLength - index < 0)
                        throw new ArgumentOutOfRangeException("Can't seek past the beginning!");
                    _offset = (uint)(_currentLength - index);
                    break;
            }
            return (int)_offset;
        }

        /// <summary>
        /// Reads a byte out of the buffer at the current position
        /// Moves the cursor after the value
        /// </summary>
        /// <returns>The byte</returns>
        public virtual byte ReadByte()
        {
            if ((_offset + 1) > (_currentLength + 1))
                throw new Exception("Buffer overflow");
            return _buffer[_offset++];
        }

        /// <summary>
        /// Reads a word (ushort) from the buffer
        /// Moves the cursor past the value
        /// </summary>
        /// <returns>The value</returns>
        public virtual Int16 ReadInt16()
        {
            if ((_offset + 2) > (_currentLength + 1))
                throw new Exception("Buffer overflow");
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    _offset += 2;
                    return *(Int16*)cur;
                }
            }
        }

        /// <summary>
        /// Reads a DWord (uint) from the buffer
        /// Moves the cursor past the value
        /// </summary>
        /// <returns>The value</returns>
        public virtual Int32 ReadInt32()
        {
            if ((_offset + 4) > (_currentLength + 1))
                throw new Exception("Buffer overflow");
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    _offset += 4;
                    return *(Int32*)cur;
                }
            }
        }

        /// <summary>
        /// Reads a Quad Word (ulong) from the buffer
        /// Moves the cursor past the value
        /// </summary>
        /// <returns>The value</returns>
        public virtual Int64 ReadInt64()
        {
            if ((_offset + 8) > (_currentLength + 1))
                throw new Exception("Buffer overflow");
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    _offset += 8;
                    return *(Int64*)cur;
                }
            }
        }

        /// <summary>
        /// Reads a DWord (float) from the buffer
        /// Moves the cursor past the value
        /// </summary>
        /// <returns>The value</returns>
        public virtual float ReadFloat()
        {
            if ((_offset + 4) > (_currentLength + 1))
                throw new Exception("Buffer overflow");
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    _offset += 4;
                    return *(float*)cur;
                }
            }
        }

        /// <summary>
        /// Reads a Quad Word (double) from the buffer
        /// Moves the cursor past the value
        /// </summary>
        /// <returns>The value</returns>
        public virtual double ReadDouble()
        {
            if ((_offset + 8) > (_currentLength + 1))
                throw new Exception("Buffer overflow");
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    _offset += 8;
                    return *(double*)cur;
                }
            }
        }

        /// <summary>
        /// Reads a fixed length string from the buffer
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="length">The length of the string</param>
        /// <returns>The value</returns>
        public virtual string ReadString(int length)
        {
            if ((_offset + length) > (_currentLength + 1))
                throw new Exception("Buffer overflow");

            byte[] buffer = ReadBlock(length);
            return Encoding.GetEncoding("ISO-8859-1")
                           .GetString(buffer);
        }

        /// <summary>
        /// Reads a C-Style string out of the buffer (null-terminated)
        /// Moves the cursor past the value
        /// </summary>
        /// <returns>The value</returns>
        public virtual string ReadCString()
        {
            byte[] buffer =
            [
            ];
            for (uint i = _offset; i < _currentLength; i++)
            {
                if (_buffer[i] == 0)
                {
                    //found terminator
                    buffer = new byte[i - _offset];
                    Array.Copy(_buffer, _offset, buffer, 0, i - _offset);
                    _offset = i + 1;
                    return Encoding.GetEncoding("ISO-8859-1")
                                   .GetString(buffer);
                }
            }
            //hit the end
            buffer = new byte[_currentLength - _offset];
            Array.Copy(_buffer, _offset, buffer, 0, _currentLength - _offset);
            _offset = _currentLength;
            return Encoding.GetEncoding("ISO-8859-1")
                           .GetString(buffer);
        }

        /// <summary>
        /// Reads a raw block of data out of the buffer
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="length">The length of the block</param>
        /// <returns>The value</returns>
        public virtual byte[] ReadBlock(int length)
        {
            var buffer = new byte[length];
            Array.Copy(_buffer, _offset, buffer, 0, length);
            _offset += (uint)length;
            return buffer;
        }

        /// <summary>
        /// Writes a byte into the buffer at the current position
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The byte to write</param>
        public virtual void Write(byte value)
        {
            while (_offset + 1 >= _length)
                ResizeBuffer();
            _buffer[_offset] = value;

            if (_offset + 1 >= _currentLength)
                _currentLength += 1;
            _offset += 1;
        }

        /// <summary>
        /// Writes a ushort into the buffer at the current position
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The short to write</param>
        public virtual void Write(Int16 value)
        {
            while (_offset + 2 >= _length)
                ResizeBuffer();
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    *(Int16*)cur = value;

                    if (_offset + 2 >= _currentLength)
                        _currentLength += (_offset + 2 - _currentLength);

                    _offset += 2;
                }
            }
        }

        /// <summary>
        /// Writes a uint to the buffer at the current position
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The int to write</param>
        public virtual void Write(Int32 value)
        {
            while (_offset + 4 >= _length)
                ResizeBuffer();
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    *(Int32*)cur = value;

                    if (_offset + 4 >= _currentLength)
                        _currentLength += (_offset + 4 - _currentLength);

                    _offset += 4;
                }
            }
        }

        /// <summary>
        /// Writes a ulong into the buffer at the current position
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The long to write</param>
        public virtual void Write(Int64 value)
        {
            while (_offset + 8 >= _length)
                ResizeBuffer();
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    *(Int64*)cur = value;

                    if (_offset + 8 >= _currentLength)
                        _currentLength += (_offset + 8 - _currentLength);

                    _offset += 8;
                }
            }
        }

        /// <summary>
        /// Writes a double into the buffer at the current position
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The long to write</param>
        public virtual void Write(double value)
        {
            while (_offset + 8 >= _length)
                ResizeBuffer();
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    *(double*)cur = value;

                    if (_offset + 8 >= _currentLength)
                        _currentLength += (_offset + 8 - _currentLength);

                    _offset += 8;
                }
            }
        }

        public virtual void Write(float value)
        {
            while (_offset + 4 >= _length)
                ResizeBuffer();
            unsafe
            {
                fixed (byte* current = _buffer)
                {
                    byte* cur = current + _offset;
                    *(float*)cur = value;

                    if (_offset + 4 >= _currentLength)
                        _currentLength += (_offset + 4 - _currentLength);

                    _offset += 4;
                }
            }
        }

        /// <summary>
        /// Writes a fixed-length string into the buffer at the current position
        /// Writes the single-byte equivalent, with no termination indicator
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The string to write</param>
        public virtual void Write(string value)
        {
            byte[] buffer = Encoding.GetEncoding("ISO-8859-1")
                                    .GetBytes(value);
            while ((_offset + buffer.Length) >= _length)
                ResizeBuffer();

            Array.Copy(buffer, 0, _buffer, _offset, buffer.Length);

            if (_offset + value.Length >= _currentLength)
                _currentLength += (uint)(_offset + value.Length - _currentLength);

            _offset += (uint)value.Length;
        }

        /// <summary>
        /// Writes a string into the buffer at the current position
        /// Writes the single-byte equivalent, with a null terminator
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The string to write</param>
        public virtual void WriteCString(string value)
        {
            Write(value);
            Write((byte)0);
        }

        /// <summary>
        /// Writes a block of data into the buffer at the current position
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The data to write</param>
        public virtual void Write(byte[] value)
        {
            while (_offset + value.Length >= _length)
                ResizeBuffer();
            Array.Copy(value, 0, _buffer, _offset, value.Length);

            if (_offset + value.Length >= _currentLength)
                _currentLength += (uint)(_offset + value.Length - _currentLength);

            _offset += (uint)value.Length;
        }

        /// <summary>
        /// Writes a part of a byte array into the buffer at the current position
        /// Moves the cursor past the value
        /// </summary>
        /// <param name="value">The data to write</param>
        /// <param name="length">The number of bytes to write</param>
        public virtual void Write(byte[] value, int length)
        {
            while (_offset + length >= _length)
                ResizeBuffer();
            Array.Copy(value, 0, _buffer, _offset, length);

            if (_offset + length >= _currentLength)
                _currentLength += (uint)(_offset + length - _currentLength);

            _offset += (uint)length;
        }

        /// <summary>
        /// Gets or sets the current offset
        /// This is the cursor - data will be read or written at this position
        /// </summary>
        public uint Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// Gets or sets the buffer
        /// When retrieving, will return a truncated copy of the buffer
        /// When setting, resets the object with the new data
        /// </summary>
        public byte[] Buffer
        {
            get { return GetBuffer(); }
            set { SetBuffer(value); }
        }

        /// <summary>
        /// Gets or sets the current size of the buffer
        /// This is the amount of data stored in the buffer, not the total storage capacity
        /// </summary>
        public uint Length
        {
            get { return _currentLength; }
            set { _currentLength = value; }
        }

        /// <summary>
        /// Appends the byte array to the end of the buffer
        /// This ignores the Offset property
        /// </summary>
        /// <param name="data">The data to append</param>
        public void Append(byte[] data)
        {
            while (_currentLength + data.Length >= _length)
                ResizeBuffer();
            Array.Copy(data, 0, _buffer, _currentLength, data.Length);

            _currentLength += (uint)data.Length;
        }
    }
}