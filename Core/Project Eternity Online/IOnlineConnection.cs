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

        List<OnlineScript> ListAsyncOnlineScript { get; }

        void AddOrReplaceScripts(Dictionary<string, OnlineScript> DicNewScript);

        void Send(OnlineScript ScriptToSend);

        void SendWriteBuffer();

        IEnumerable<OnlineScript> ReadScripts();

        void StartReadingScriptAsync();

        void StopReadingScriptAsync();

        void Close();

        IOnlineConnection ReOpen();

        bool IsConnected();

        bool HasLeftServer();
    }
}
