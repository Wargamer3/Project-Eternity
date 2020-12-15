using System;
using System.Net.Sockets;

namespace ProjectEternity.Core.Online
{
    public unsafe class OnlineWriter
    {
        private byte[] WriteBuffer;

        public void Send(NetworkStream Destination)
        {
            Destination.Write(WriteBuffer, 0, WriteBuffer.Length);
        }

        public void WriteScript(OnlineScript ScriptToWrite)
        {
            ScriptToWrite.Write(this);
        }

        public void ClearWriteBuffer()
        {
            WriteBuffer = new byte[0];
        }

        public void AppendBoolean(bool NewEntry)
        {
            byte[] ArrayAllData = new byte[WriteBuffer.Length + 1];
            Buffer.BlockCopy(WriteBuffer, 0, ArrayAllData, 0, WriteBuffer.Length);

            ArrayAllData[WriteBuffer.Length] = NewEntry ? (byte)1 : (byte)0;
            WriteBuffer = ArrayAllData;
        }

        public void AppendByte(byte NewEntry)
        {
            byte[] ArrayAllData = new byte[WriteBuffer.Length + 1];
            Buffer.BlockCopy(WriteBuffer, 0, ArrayAllData, 0, WriteBuffer.Length);

            ArrayAllData[WriteBuffer.Length] = NewEntry;
            WriteBuffer = ArrayAllData;
        }

        public void AppendByteArray(byte[] NewEntry)
        {
            AppendInt32(NewEntry.Length);
            byte[] ArrayAllData = new byte[WriteBuffer.Length + NewEntry.Length];

            Buffer.BlockCopy(WriteBuffer, 0, ArrayAllData, 0, WriteBuffer.Length);
            Buffer.BlockCopy(NewEntry, 0, ArrayAllData, WriteBuffer.Length, NewEntry.Length);

            WriteBuffer = ArrayAllData;
        }

        public void AppendInt32(int NewEntry)
        {
            Append4ByteArray((byte*)&NewEntry);
        }

        public void AppendUInt32(uint NewEntry)
        {
            Append4ByteArray((byte*)&NewEntry);
        }

        public void AppendUInt64(ulong NewEntry)
        {
            Append8ByteArray((byte*)&NewEntry);
        }

        public void AppendFloat(float NewEntry)
        {
            Append4ByteArray((byte*)&NewEntry);
        }

        public void AppendString(string NewEntry)
        {
            byte[] TextData;

            if (string.IsNullOrEmpty(NewEntry))
                TextData = new byte[0];
            else
                TextData = System.Text.Encoding.Unicode.GetBytes(NewEntry); //UTF-16 encoding

            AppendInt32(TextData.Length);
            byte[] ArrayAllData = new byte[WriteBuffer.Length + TextData.Length];

            Buffer.BlockCopy(WriteBuffer, 0, ArrayAllData, 0, WriteBuffer.Length);
            Buffer.BlockCopy(TextData, 0, ArrayAllData, WriteBuffer.Length, TextData.Length);

            WriteBuffer = ArrayAllData;
        }

        private void Append4ByteArray(byte* DataPointer)
        {
            byte[] ArrayAllData = new byte[WriteBuffer.Length + 4];
            byte[] ArrayData = new byte[4];

            ArrayData[0] = DataPointer[0];
            ArrayData[1] = DataPointer[1];
            ArrayData[2] = DataPointer[2];
            ArrayData[3] = DataPointer[3];

            if (BitConverter.IsLittleEndian)
                Array.Reverse(ArrayData);

            Buffer.BlockCopy(WriteBuffer, 0, ArrayAllData, 0, WriteBuffer.Length);
            ArrayAllData[WriteBuffer.Length] = ArrayData[0];
            ArrayAllData[WriteBuffer.Length + 1] = ArrayData[1];
            ArrayAllData[WriteBuffer.Length + 2] = ArrayData[2];
            ArrayAllData[WriteBuffer.Length + 3] = ArrayData[3];

            WriteBuffer = ArrayAllData;
        }

        private void Append8ByteArray(byte* DataPointer)
        {
            byte[] ArrayAllData = new byte[WriteBuffer.Length + 8];
            byte[] ArrayData = new byte[8];

            ArrayData[0] = DataPointer[0];
            ArrayData[1] = DataPointer[1];
            ArrayData[2] = DataPointer[2];
            ArrayData[3] = DataPointer[3];
            ArrayData[4] = DataPointer[4];
            ArrayData[5] = DataPointer[5];
            ArrayData[6] = DataPointer[6];
            ArrayData[7] = DataPointer[7];

            if (BitConverter.IsLittleEndian)
                Array.Reverse(ArrayData);

            Buffer.BlockCopy(WriteBuffer, 0, ArrayAllData, 0, WriteBuffer.Length);
            ArrayAllData[WriteBuffer.Length] = ArrayData[0];
            ArrayAllData[WriteBuffer.Length + 1] = ArrayData[1];
            ArrayAllData[WriteBuffer.Length + 2] = ArrayData[2];
            ArrayAllData[WriteBuffer.Length + 3] = ArrayData[3];
            ArrayAllData[WriteBuffer.Length + 4] = ArrayData[4];
            ArrayAllData[WriteBuffer.Length + 5] = ArrayData[5];
            ArrayAllData[WriteBuffer.Length + 6] = ArrayData[6];
            ArrayAllData[WriteBuffer.Length + 7] = ArrayData[7];

            WriteBuffer = ArrayAllData;
        }
    }
}
