using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class CommunicationClient
    {
        public IOnlineConnection Host;
        public IOnlineConnection LastTopLevel;
        public Dictionary<string, CommunicationClient> DicCrossServerCommunicationByGroupID;
        private CancellationTokenSource CancelToken;
        internal readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        protected readonly List<DelayedExecutableOnlineScript> ListDelayedOnlineCommand;
        public readonly ChatManager Chat;

        public bool IsConnected => Host != null && Host.IsConnected();

        public CommunicationClient()
            : this(new Dictionary<string, OnlineScript>())
        {
        }

        public CommunicationClient(Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            DicCrossServerCommunicationByGroupID = new Dictionary<string, CommunicationClient>();
            ListDelayedOnlineCommand = new List<DelayedExecutableOnlineScript>();
            Chat = new ChatManager();

            CancelToken = new CancellationTokenSource();

            this.DicOnlineScripts = DicOnlineScripts;
        }

        public void Connect(IPAddress HostAddress, int HostPort)
        {
            ChangeHost(HostAddress, HostPort);

            if (Host != null)
            {
                Task.Run(() => { UpdateLoop(); });
            }
        }

        public void ConnectToKnownIP()
        {

        }

        public void ChangeHost(IPAddress HostAddress, int HostPort)
        {
            try
            {
                IPEndPoint HostEndPoint = new IPEndPoint(HostAddress, HostPort);
                TcpClient UserClient = new TcpClient();
                UserClient.NoDelay = true;
                UserClient.Connect(HostEndPoint);

                // Get a client stream for reading and writing.
                NetworkStream HostStream = UserClient.GetStream();
                ChangeHost(new OnlineConnection(UserClient, HostStream, DicOnlineScripts));
            }
            catch (Exception ex)
            {

            }
        }

        public void ChangeHost(IOnlineConnection NewHost)
        {
            if (LastTopLevel != NewHost)
            {
                LastTopLevel = Host;
                if (Host != null)
                {
                    Host.Close();
                }

                Host = NewHost;
            }
        }

        public void UpdateLoop()
        {
            while (!CancelToken.IsCancellationRequested)
            {
                Update();
            }

            Host.Close();
            Host = null;
        }

        public void Update()
        {
            foreach (OnlineScript ActiveScript in Host.ReadScripts())
            {
                ActiveScript.Execute(Host);
            }

            foreach (CommunicationClient ActiveGroup in DicCrossServerCommunicationByGroupID.Values)
            {
                ActiveGroup.Update();
            }

            if (!Host.IsConnected())
            {
                OnConnectionLost();
            }
        }

        public void OnConnectionLost()
        {
            if (LastTopLevel != null)
            {
                Host = LastTopLevel;
                LastTopLevel = null;
                Host.ReOpen();
            }
            else
            {
                ConnectToKnownIP();
            }
        }

        public void ExecuteDelayedScripts()
        {
            lock (ListDelayedOnlineCommand)
            {
                foreach (DelayedExecutableOnlineScript ActiveCommand in ListDelayedOnlineCommand)
                {
                    ActiveCommand.ExecuteOnMainThread();
                }

                ListDelayedOnlineCommand.Clear();
            }

            foreach (CommunicationClient ActiveGroup in DicCrossServerCommunicationByGroupID.Values)
            {
                ActiveGroup.ExecuteDelayedScripts();
            }
        }

        public void DelayOnlineScript(DelayedExecutableOnlineScript ScriptToDelay)
        {
            lock (ListDelayedOnlineCommand)
            {
                ListDelayedOnlineCommand.Add(ScriptToDelay);
            }
        }

        public void SendMessage(string GroupID, ChatManager.ChatMessage MessageToSend)
        {
            if (GroupID == "Global")
            {
                Host.Send(new SendGlobalMessageScriptClient(MessageToSend));
            }
            else if (DicCrossServerCommunicationByGroupID.ContainsKey(GroupID))
            {
                DicCrossServerCommunicationByGroupID[GroupID].Host.Send(new SendGroupMessageScriptClient(GroupID, MessageToSend));
            }
            else
            {
                Host.Send(new SendGroupMessageScriptClient(GroupID, MessageToSend));
            }
        }

        public void AddFriend(string FriendID)
        {
            Host.Send(new AddFriendScriptClient(FriendID));
        }
    }
}
