using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public interface IOnlineConnection
    {
        string ID { get; set; }

        string Name { get; set; }

        string IP { get; }

        bool IsGameReady { get; set; }

        void AddOrReplaceScripts(Dictionary<string, OnlineScript> DicNewScript);

        void Send(OnlineScript ScriptToSend);

        void SendWriteBuffer();

        IEnumerable<OnlineScript> ReadScripts();

        void Close();

        IOnlineConnection ReOpen();

        bool IsConnected();

        bool HasLeftServer();
    }
}
