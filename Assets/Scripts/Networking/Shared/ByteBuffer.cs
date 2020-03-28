using Assets.Scripts.Networking.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;


namespace UnityGame.Scripts.Network.Shared
{
    public class ByteBuffer : IDisposable
    {
        int _readPos = 0;

        List<byte> _buffer;
        byte[] _readBuffer;

        public ByteBuffer(byte[] buffer)
        {
            _buffer = new List<byte>(buffer);

            _readBuffer = AsByteArray();
        }

        public ByteBuffer(Opcode opcode)
        {
            _buffer = new List<byte>(4096);
            Write((int)opcode);
        }

        #region Read Data
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                byte _value = _readBuffer[_readPos];
                if (_moveReadPos)
                {
                    _readPos += 1;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                byte[] _value = _buffer.GetRange(_readPos, _length).ToArray();
                if (_moveReadPos)
                {
                    _readPos += _length;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        public short ReadShort(bool _moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                short _value = BitConverter.ToInt16(_readBuffer, _readPos);
                if (_moveReadPos)
                {
                    _readPos += 2;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        internal void Reset()
        {
            _readPos = 0;
        }

        public int ReadInt(bool _moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                int _value = BitConverter.ToInt32(_readBuffer, _readPos);
                if (_moveReadPos)
                {
                    _readPos += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        public long ReadLong(bool _moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                long _value = BitConverter.ToInt64(_readBuffer, _readPos);
                if (_moveReadPos)
                {
                    _readPos += 8;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        public float ReadFloat(bool _moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                float _value = BitConverter.ToSingle(_readBuffer, _readPos);
                if (_moveReadPos)
                {
                    _readPos += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        public bool ReadBool(bool _moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                bool _value = BitConverter.ToBoolean(_readBuffer, _readPos);
                if (_moveReadPos)
                    _readPos += 1;

                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt();
                string _value = Encoding.ASCII.GetString(_readBuffer, _readPos, _length);

                if (_moveReadPos && _value.Length > 0)
                    _readPos += _length;

                return _value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public Vector3 ReadVector3(bool _moveReadPos = true)
        {
            return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }

        public Quaternion ReadQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        #endregion

        #region Writing

        public void Write(byte inc)
        {
            _buffer.Add(inc);
        }

        public void Write(ushort inc)
        {
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(short inc)
        { 
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(int inc)
        {
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(uint inc)
        {
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(long inc)
        {
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(ulong inc)
        {
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(float inc)
        {
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(string inc)
        {
            Write(inc.Length);
            _buffer.AddRange(Encoding.ASCII.GetBytes(inc));
        }

        public void Write(bool inc)
        {
            _buffer.AddRange(BitConverter.GetBytes(inc));
        }

        public void Write(Vector3 inc)
        {
            Write(inc.x);
            Write(inc.y);
            Write(inc.z);
        }

        internal bool Empty()
        {
            return (_buffer is null || _readBuffer is null || _buffer.Count == 0 || _buffer.Count == 0);
        }

        public void Write(Quaternion inc)
        {
            Write(inc.x);
            Write(inc.y);
            Write(inc.z);
            Write(inc.w);
        }

        #endregion

        public byte[] AsByteArray()
        {
            return _buffer.ToArray();
        }

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    _buffer = null;
                    _readBuffer = null;
                    _readPos = 0;
                }

                disposed = true;
            }
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
