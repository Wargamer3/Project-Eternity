using System;
using System.Net.Sockets;

namespace ProjectEternity.Core.Online
{
    public unsafe class OnlineWriter : ByteWriter
    {
        public void Send(NetworkStream Destination)
        {
            Destination.Write(WriteBuffer, 0, WriteBuffer.Length);
        }

        public void WriteScript(OnlineScript ScriptToWrite)
        {
            ScriptToWrite.Write(this);
        }
    }
}
