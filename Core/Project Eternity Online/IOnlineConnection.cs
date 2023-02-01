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

        object ExtraInformation { get; set; }

        RoleManager Roles { get; set; }

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

    public class RoleManager
    {
        public const string Host = "Host";
        public const string Ready = "Ready";

        public readonly List<string> ListActiveRole;
        public bool IsRoomHost;
        public bool IsRoomReady;
        public bool HasControl;//Identify if the player is allowed to send commands or have control in general
        public bool IsGlobalAdmin;

        public RoleManager()
        {
            ListActiveRole = new List<string>();
        }

        public void Reset()
        {
            ListActiveRole.Clear();
            IsRoomHost = false;
            IsRoomReady = false;
        }

        public void AddRole(string NewRole)
        {
            ListActiveRole.Add(NewRole);

            if (NewRole == Host)
            {
                IsRoomHost = true;
            }
            else if (NewRole == Ready)
            {
                IsRoomReady = true;
            }
        }

        public void RemoveRole(string RoleToRemove)
        {
            ListActiveRole.Remove(RoleToRemove);

            if (RoleToRemove == Host)
            {
                IsRoomHost = false;
            }
            else if (RoleToRemove == Ready)
            {
                IsRoomReady = false;
            }
        }
    }
}
