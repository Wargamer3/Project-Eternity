﻿using System;

namespace ProjectEternity.Core.Online
{
    public unsafe class ByteReader
    {
        protected byte[] ReadBuffer;
        protected int ReadBufferPos;

        public bool CanRead { get { return ReadBufferPos < ReadBuffer.Length; } }

        public ByteReader()
        {
            ReadBuffer = new byte[0];
            ReadBufferPos = 0;
        }

        public ByteReader(byte[] ReadBuffer)
        {
            this.ReadBuffer = ReadBuffer;
            ReadBufferPos = 0;
        }

        public void Clear()
        {
            ReadBuffer = new byte[0];
            ReadBufferPos = 0;
        }

        public bool ReadBoolean()
        {
            return ReadBuffer[ReadBufferPos++] == 1 ? true : false;
        }

        public byte ReadByte()
        {
            return ReadBuffer[ReadBufferPos++];
        }

        public byte[] ReadByteArray()
        {
            int ArrayLength = ReadInt32();
            byte[] ArrayData = new byte[ArrayLength];

            Buffer.BlockCopy(ReadBuffer, ReadBufferPos, ArrayData, 0, ArrayLength);

            ReadBufferPos += ArrayLength;

            return ArrayData;
        }

        public int ReadInt32()
        {
            byte[] ArrayData = Read4Bytes();

            fixed (byte* DataPointer = &ArrayData[0])
            {
                int* Int32Pointer = (int*)DataPointer;
                return *Int32Pointer;
            }
        }

        public uint ReadUInt32()
        {
            byte[] ArrayData = Read4Bytes();

            fixed (byte* DataPointer = &ArrayData[0])
            {
                uint* UInt32Pointer = (uint*)DataPointer;
                return *UInt32Pointer;
            }
        }

        public ulong ReadUInt64()
        {
            byte[] ArrayData = Read8Bytes();

            fixed (byte* DataPointer = &ArrayData[0])
            {
                ulong* ULongPointer = (ulong*)DataPointer;
                return *ULongPointer;
            }
        }

        public float ReadFloat()
        {
            byte[] ArrayData = Read4Bytes();

            fixed (byte* DataPointer = &ArrayData[0])
            {
                float* FloatPointer = (float*)DataPointer;
                return *FloatPointer;
            }
        }

        private byte[] Read4Bytes()
        {
            byte[] ArrayData = new byte[4];
            ArrayData[0] = ReadBuffer[ReadBufferPos];
            ArrayData[1] = ReadBuffer[ReadBufferPos + 1];
            ArrayData[2] = ReadBuffer[ReadBufferPos + 2];
            ArrayData[3] = ReadBuffer[ReadBufferPos + 3];

            if (BitConverter.IsLittleEndian)
                Array.Reverse(ArrayData);

            ReadBufferPos += 4;

            return ArrayData;
        }

        private byte[] Read8Bytes()
        {
            byte[] ArrayData = new byte[8];
            ArrayData[0] = ReadBuffer[ReadBufferPos];
            ArrayData[1] = ReadBuffer[ReadBufferPos + 1];
            ArrayData[2] = ReadBuffer[ReadBufferPos + 2];
            ArrayData[3] = ReadBuffer[ReadBufferPos + 3];
            ArrayData[4] = ReadBuffer[ReadBufferPos + 4];
            ArrayData[5] = ReadBuffer[ReadBufferPos + 5];
            ArrayData[6] = ReadBuffer[ReadBufferPos + 6];
            ArrayData[7] = ReadBuffer[ReadBufferPos + 7];

            if (BitConverter.IsLittleEndian)
                Array.Reverse(ArrayData);

            ReadBufferPos += 8;

            return ArrayData;
        }

        public string ReadString()
        {
            int TextLength = ReadInt32();
            string Text = System.Text.Encoding.Unicode.GetString(ReadBuffer, ReadBufferPos, TextLength); //UTF-16 encoding
            ReadBufferPos += TextLength;

            return Text;
        }
    }
}
