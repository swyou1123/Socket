﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;   // [][][][][]
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte> ( _buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                _writePos = _readPos = 0;
            }
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
            }
        }

        public bool OnRead(int numOfBytes)  // 성공적 처리후 커서 위치
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;   
        }

        public bool OnWrite(int numOfBytes)  // 클라에서 받은 데이터후 w 옮기기
        {
            if(numOfBytes > FreeSize) 
                return false;

            _writePos += numOfBytes;
            return true;
        }

    }
}
