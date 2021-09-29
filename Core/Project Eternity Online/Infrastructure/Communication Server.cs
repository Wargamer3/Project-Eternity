using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    /// <summary>
    /// Handle communication between local clients or send communication higher to allow cross server communication.
    /// To avoid excessive cross server communication the clients in the communication servers should match the game servers.
    /// When a client sends a message it check if the receiver is on the server, if it's not it send it to its Server Manager that will send it to the proper Server
    /// </summary>
    public class CommunicationServer
    {
        public readonly IOnlineConnection Host;
        private readonly List<IOnlineConnection> ListPlayerConnection;
        private readonly Dictionary<string, byte[]> DicPlayerInfoByName;
        internal readonly Dictionary<string, IOnlineConnection> DicPlayerByID;

        public readonly List<KeyValuePair<IOnlineConnection, string>> ListPlayerToRemove;

        public readonly Dictionary<string, CommunicationGroup> DicCommunicationGroup;

        public readonly List<string> ListGroupToRemove;

        public string IP;
        public int Port;

        public readonly ICommunicationDataManager Database;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        private double DeltaTime = 1.0d / 60.0d;
        private double ElapsedTime = 0d;

        public readonly OnlineWriter SharedWriteBuffer;

        private TcpListener ClientsListener;

        private CancellationTokenSource CancelToken;

        private DateTimeOffset NextPlayerUpdateTime;

        public CommunicationServer(ICommunicationDataManager Database, Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            ListPlayerConnection = new List<IOnlineConnection>();
            DicPlayerInfoByName = new Dictionary<string, byte[]>();
            DicPlayerByID = new Dictionary<string, IOnlineConnection>();
            ListPlayerToRemove = new List<KeyValuePair<IOnlineConnection, string>>();
            DicCommunicationGroup = new Dictionary<string, CommunicationGroup>();
            ListGroupToRemove = new List<string>();
            DicCommunicationGroup.Add("Global", new CommunicationGroup(true));

            SharedWriteBuffer = new OnlineWriter();

            NextPlayerUpdateTime = DateTimeOffset.Now;

            this.DicOnlineScripts = DicOnlineScripts;

            this.Database = Database;
        }

        private void WaitForConnections()
        {
            if (ClientsListener.Pending())
            {
                TcpClient ConnectingClient = ClientsListener.AcceptTcpClient();
                ConnectingClient.NoDelay = true;

                NetworkStream ClientStream = ConnectingClient.GetStream();

                IOnlineConnection NewConnection = new OnlineConnection(ConnectingClient, ClientStream, new Dictionary<string, OnlineScript>(DicOnlineScripts), SharedWriteBuffer);
                OnClientConnected(NewConnection);
            }
        }

        private void Update()
        {
            while (!CancelToken.IsCancellationRequested)
            {
                Stopwatch gameTime = Stopwatch.StartNew();
                while (ElapsedTime >= DeltaTime)
                {
                    WaitForConnections();
                    UpdatePlayers();
                    ElapsedTime -= DeltaTime;
                }
                gameTime.Stop();
                ElapsedTime += gameTime.Elapsed.TotalSeconds;
            }

            ClientsListener.Stop();
            ClientsListener = null;
        }

        public void UpdatePlayers()
        {
            Parallel.ForEach(ListPlayerConnection, (ActivePlayer, loopState) =>
            {
                if (!ActivePlayer.IsConnected())
                {
                    if (ActivePlayer.HasLeftServer())
                    {
                        ListPlayerToRemove.Add(new KeyValuePair<IOnlineConnection, string>(ActivePlayer, null));
                        Database.RemovePlayer(ActivePlayer);
                    }
                }
                else
                {
                    lock (ActivePlayer.ListAsyncOnlineScript)
                    {
                        foreach (OnlineScript ActiveScript in ActivePlayer.ListAsyncOnlineScript)
                        {
                            ActiveScript.Execute(ActivePlayer);
                        }

                        ActivePlayer.ListAsyncOnlineScript.Clear();
                    }
                }
            });

            while (ListPlayerToRemove.Count > 0)
            {
                DicCommunicationGroup[ListPlayerToRemove[0].Value].ListGroupMember.Remove(ListPlayerToRemove[0].Key);
                DicPlayerInfoByName.Remove(ListPlayerToRemove[0].Key.ID);
                DicPlayerByID.Remove(ListPlayerToRemove[0].Key.ID);
                ListPlayerToRemove.RemoveAt(0);
            }

            Parallel.ForEach(DicCommunicationGroup.Values, (ActiveGroup, loopState) =>
            {
                if (ActiveGroup.IsRunningSlow())
                {
                    //Send Room to another Server
                }

                for (int P = ActiveGroup.ListGroupMember.Count - 1; P >= 0; --P)
                {
                    IOnlineConnection ActivePlayer = ActiveGroup.ListGroupMember[P];

                    if (!ActivePlayer.IsConnected())
                    {
                        if (ActivePlayer.HasLeftServer())
                        {
                            string PlayerID = ActiveGroup.ListGroupMember[P].ID;
                            ActiveGroup.RemoveOnlinePlayer(P);

                            if (ActiveGroup.ListGroupMember.Count == 0)
                            {
                                ListGroupToRemove.Add(ActiveGroup.GroupID);
                            }

                            ActivePlayer.StopReadingScriptAsync();
                        }
                    }
                }
            });

            while (ListGroupToRemove.Count > 0)
            {
                DicCommunicationGroup.Remove(ListGroupToRemove[0]);

                ListGroupToRemove.RemoveAt(0);
            }

            if (DateTimeOffset.Now > NextPlayerUpdateTime)
            {
                NextPlayerUpdateTime = NextPlayerUpdateTime.AddSeconds(10);

                SharedWriteBuffer.ClearWriteBuffer();
                SharedWriteBuffer.WriteScript(new PlayerListScriptServer(GetPlayerNames()));

                Parallel.ForEach(DicCommunicationGroup["Global"].ListGroupMember, (ActiveMember, loopState) =>
                {
                    ActiveMember.SendWriteBuffer();
                });
            }
        }

        public void StartListening(string PublicIP, int ClientsPort)
        {
            try
            {
                CancelToken = new CancellationTokenSource();

                IPEndPoint ListeningEndPointClient = new IPEndPoint(IPAddress.Any, ClientsPort);
                ClientsListener = new TcpListener(ListeningEndPointClient);
                ClientsListener.Start();

                this.IP = PublicIP;
                this.Port = ClientsPort;

                Task.Run(() => { Update(); });
            }
            catch (Exception)
            {
                ClientsListener = null;
            }
        }

        public void StopListening()
        {
            CancelToken.Cancel();
        }

        public void OnClientConnected(IOnlineConnection NewClient)
        {
            lock (ListPlayerConnection)
            {
                ListPlayerConnection.Add(NewClient);
            }

            DicCommunicationGroup["Global"].AddMember(NewClient);
            NewClient.StartReadingScriptAsync();
        }

        public void SendGlobalMessage(string Message, ChatManager.MessageColors MessageColor)
        {
            foreach (IOnlineConnection ActiveOnlinePlayer in DicPlayerByID.Values)
            {
                ActiveOnlinePlayer.Send(new ReceiveGlobalMessageScriptServer(Message, MessageColor));
            }
        }

        public Dictionary<string, byte[]> GetPlayerNames()
        {
            Dictionary<string, byte[]> ListPlayerName;

            lock (DicPlayerInfoByName)
            {
                ListPlayerName = new Dictionary<string, byte[]>(DicPlayerInfoByName);
            }

            return ListPlayerName;
        }

        public void Identify(IOnlineConnection NewClient, byte[] ClientInfo)
        {
            lock (DicPlayerInfoByName)
            {
                DicPlayerInfoByName.Add(NewClient.ID, ClientInfo);
            }

            lock (DicPlayerByID)
            {
                DicPlayerByID.Add(NewClient.ID, NewClient);
            }

            Database.UpdatePlayerCommunicationIP(NewClient.ID, IP, Port);

            NewClient.Send(new FriendListScriptServer(Database.GetFriendList(NewClient.ID)));
        }

        public void CreateOrJoinCommunicationGroup(string GroupID, bool SaveLogs, IOnlineConnection GroupCreator)
        {
            lock (DicCommunicationGroup)
            {
                if (!DicCommunicationGroup.ContainsKey(GroupID))
                {
                    CommunicationGroup NewGroup = new CommunicationGroup(GroupID, SaveLogs, GroupCreator);
                    DicCommunicationGroup.Add(GroupID, NewGroup);
                }
                else
                {
                    DicCommunicationGroup[GroupID].AddMember(GroupCreator);
                }
            }

            if (SaveLogs)
            {
                Dictionary<string, ChatManager.MessageColors> OldMessages = Database.GetGroupMessages(GroupID);
                if (OldMessages.Count > 0)
                {
                    GroupCreator.Send(new MessageListGroupScriptServer(GroupID, OldMessages));
                }
            }
        }

        public void JoinCommunicationGroup(string GroupID, IOnlineConnection NewMember)
        {
            DicCommunicationGroup[GroupID].AddMember(NewMember);
        }

        public void LeaveCommunicationGroup(string GroupID, IOnlineConnection NewMember)
        {
            ListPlayerToRemove.Add(new KeyValuePair<IOnlineConnection, string>(NewMember, GroupID));
        }

        public byte[] GetClientInfo(string ClientID)
        {
            return Database.GetClientInfo(ClientID);
        }

        public void AddFriend(IOnlineConnection Sender, string FriendID)
        {
            Database.AddFriend(Sender, FriendID);
        }
    }
}
