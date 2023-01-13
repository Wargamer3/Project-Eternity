using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public unsafe class OnlineConnectionDummy : IOnlineConnection
    {
        public string IP => string.Empty;

        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsGameReady { get; set; }

        public List<OnlineScript> ListAsyncOnlineScript { get; set; }

        public RoleManager Roles { get; set; }

        public OnlineConnectionDummy()
        {
            ListAsyncOnlineScript = new List<OnlineScript>();
            Roles = new RoleManager();
        }

        public void AddOrReplaceScripts(Dictionary<string, OnlineScript> DicNewScript)
        {
        }

        public void Send(OnlineScript ScriptToSend)
        {
        }

        public void SendWriteBuffer()
        {
        }

        public void Close()
        {
        }

        IEnumerable<OnlineScript> IOnlineConnection.ReadScripts()
        {
            return new List<OnlineScript>();
        }

        public void StartReadingScriptAsync()
        {
        }

        private void ReadScriptAsync()
        {
        }

        public void StopReadingScriptAsync()
        {
        }

        public IOnlineConnection ReOpen()
        {
            return this;
        }

        public bool IsConnected()
        {
            return true;
        }

        public bool HasLeftServer()
        {
            return false;
        }

        public override string ToString()
        {
            return IP;
        }
    }
}
