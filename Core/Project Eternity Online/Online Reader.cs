using System;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    //TODO: Make an OnlineReaderAndArchiver to allow replays
    public unsafe class OnlineReader : ByteReader
    {
        public Dictionary<string, OnlineScript> DicOnlineScripts;
        public readonly TcpClient ActiveClient;
        public readonly NetworkStream ActiveStream;

        public OnlineReader(TcpClient ActiveClient, NetworkStream ActiveStream, Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            this.ActiveClient = ActiveClient;
            this.ActiveStream = ActiveStream;
            this.DicOnlineScripts = DicOnlineScripts;

            ReadBuffer = new byte[0];
            ReadBufferPos = 0;
        }

        internal OnlineScript ReadScript()
        {
            if (ActiveStream.DataAvailable)
                BeginRead();
            else
                return null;

            while (CanRead)
            {
                string ScriptName = ReadString();
                OnlineScript NewScript = DicOnlineScripts[ScriptName].Copy();

                NewScript.Read(this);

                return NewScript;
            }

            return null;
        }

        internal List<OnlineScript> ReadAllScripts()
        {
            List<OnlineScript> ListReadScript = new List<OnlineScript>();

            if (ActiveStream.DataAvailable)
                BeginRead();
            else
                return ListReadScript;

            while (CanRead)
            {
                int OriginalReadBufferPos = ReadBufferPos;
                string ScriptName = null;

                try
                {
                    ScriptName = ReadString();
                    OnlineScript NewScript = DicOnlineScripts[ScriptName].Copy();

                    NewScript.Read(this);

                    ListReadScript.Add(NewScript);
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(ScriptName))
                    {
                        Trace.TraceError(DateTimeOffset.Now + " - Reading [" + ScriptName + "] : " + ex.Message);
                    }
                    else
                    {
                        Trace.TraceError(DateTimeOffset.Now + " - Reading " + ex.Message);
                    }

                    Trace.TraceError(DateTimeOffset.Now + " - Reading " + ex.StackTrace);
                    Trace.Flush();
                    ReadBufferPos = OriginalReadBufferPos;
                    return ListReadScript;
                }
            }

            return ListReadScript;
        }

        internal IEnumerable<OnlineScript> ReadScripts()
        {
            if (ActiveStream.DataAvailable)
                BeginRead();
            else
                yield break;

            while (CanRead)
            {
                int OriginalReadBufferPos = ReadBufferPos;
                string ScriptName = null;

                OnlineScript NewScript;

                try
                {
                    ScriptName = ReadString();
                    OnlineScript ReadScript = DicOnlineScripts[ScriptName].Copy();

                    ReadScript.Read(this);
                    NewScript = ReadScript;
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(ScriptName))
                    {
                        Trace.TraceError(DateTimeOffset.Now + " - Reading [" + ScriptName + "] : " + ex.Message);
                    }
                    else
                    {
                        Trace.TraceError(DateTimeOffset.Now + " - Reading " + ex.Message);
                    }

                    Trace.TraceError(DateTimeOffset.Now + " - Reading " + ex.StackTrace);
                    Trace.Flush();
                    ReadBufferPos = OriginalReadBufferPos;
                    yield break;
                }

                if (NewScript == null)
                    yield break;
                else
                    yield return NewScript;
            }
        }

        public void BeginRead()
        {
            int RemainingReadBuffer = ReadBuffer.Length - ReadBufferPos;

            if (RemainingReadBuffer > 0)
            {
                byte[] ArrayRemainingReadBuffer = new byte[RemainingReadBuffer];
                Buffer.BlockCopy(ReadBuffer, ReadBufferPos, ArrayRemainingReadBuffer, 0, RemainingReadBuffer);
                ReadBuffer = ArrayRemainingReadBuffer;
            }
            else
            {
                ReadBuffer = new byte[0];
            }

            ReadBufferPos = 0;

            if (ActiveStream.CanRead)
            {
                byte[] ArrayNewData = new byte[255];
                while (ActiveStream.DataAvailable)
                {
                    //Fill the data stream.
                    int BytesRead = ActiveStream.Read(ArrayNewData, 0, ArrayNewData.Length);
                    byte[] ArrayAllData = new byte[ReadBuffer.Length + BytesRead];

                    Buffer.BlockCopy(ReadBuffer, 0, ArrayAllData, 0, ReadBuffer.Length);
                    Buffer.BlockCopy(ArrayNewData, 0, ArrayAllData, ReadBuffer.Length, BytesRead);

                    ReadBuffer = ArrayAllData;
                }
            }
        }
    }
}
