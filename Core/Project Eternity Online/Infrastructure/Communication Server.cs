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
        private readonly Dictionary<string, IOnlineConnection> DicPlayerConnectionByName;
        private readonly Dictionary<string, byte[]> DicPlayerInfoByName;
        public readonly List<IOnlineConnection> ListPlayerToRemove;
        public readonly Dictionary<string, CommunicationGroup> DicCommunicationGroup;

        public readonly List<string> ListGroupToRemove;

        public readonly CommunicationGroup GlobalGroup;

        public string IP;
        public int Port;

        public readonly ICommunicationDataManager Database;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        private double DeltaTime = 1.0d / 60.0d;
        private double ElapsedTime = 0d;
        public readonly OnlineWriter SharedWriteBuffer;

        private TcpListener ClientsListener;

        private CancellationTokenSource CancelToken;

        private DateTimeOffset NextRoomUpdateTime;
        public CommunicationServer(ICommunicationDataManager Database, Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            DicPlayerConnectionByName = new Dictionary<string, IOnlineConnection>();
            DicPlayerInfoByName = new Dictionary<string, byte[]>();
            ListPlayerToRemove = new List<IOnlineConnection>();
            DicCommunicationGroup = new Dictionary<string, CommunicationGroup>();
            ListGroupToRemove = new List<string>();
            GlobalGroup = new CommunicationGroup();

            SharedWriteBuffer = new OnlineWriter();

            NextRoomUpdateTime = DateTimeOffset.Now;

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
                WaitForConnections();
                while (ElapsedTime >= DeltaTime)
                {
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
            Parallel.ForEach(GlobalGroup.ListGroupMember, (ActivePlayer, loopState) =>
            {
                if (!ActivePlayer.IsConnected())
                {
                    if (ActivePlayer.HasLeftServer())
                    {
                        ListPlayerToRemove.Add(ActivePlayer);
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
                GlobalGroup.ListGroupMember.Remove(ListPlayerToRemove[0]);
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
                }
            });

            while (ListGroupToRemove.Count > 0)
            {
                DicCommunicationGroup.Remove(ListGroupToRemove[0]);

                ListGroupToRemove.RemoveAt(0);
            }

            if (DateTimeOffset.Now > NextRoomUpdateTime)
            {
                NextRoomUpdateTime = NextRoomUpdateTime.AddSeconds(10);

                SharedWriteBuffer.ClearWriteBuffer();
                SharedWriteBuffer.WriteScript(new PlayerListScriptServer(GetPlayerNames()));

                Parallel.ForEach(GlobalGroup.ListGroupMember, (ActiveMember, loopState) =>
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
            GlobalGroup.AddMember(NewClient);
            NewClient.StartReadingScriptAsync();
        }

        public List<byte[]> GetPlayerNames()
        {
            List<byte[]> ListPlayerName;

            lock (DicPlayerInfoByName)
            {
                ListPlayerName = new List<byte[]>(DicPlayerInfoByName.Values);
            }

            return ListPlayerName;
        }

        public void Identify(IOnlineConnection NewClient, byte[] ClientInfo)
        {
            lock (DicPlayerConnectionByName)
            {
                DicPlayerConnectionByName.Add(NewClient.Name, NewClient);
            }

            lock (DicPlayerInfoByName)
            {
                DicPlayerInfoByName.Add(NewClient.Name, ClientInfo);
            }
        }

        public void CreateCommunicationGroup(string GroupID, IOnlineConnection GroupCreator)
        {
            CommunicationGroup NewGroup = new CommunicationGroup(GroupID, GroupCreator);

            Dictionary<string, OnlineScript> NewDicOnlineScripts = new Dictionary<string, OnlineScript>();
            NewDicOnlineScripts.Add(SendGroupMessageScriptServer.ScriptName, new SendGroupMessageScriptServer(this, NewGroup));
            GroupCreator.AddOrReplaceScripts(NewDicOnlineScripts);
            
            DicCommunicationGroup.Add(GroupID, NewGroup);
            ListPlayerToRemove.Add(GroupCreator);
        }

        public void JoinCommunicationGroup(string GroupID, IOnlineConnection NewMember)
        {
            Dictionary<string, OnlineScript> NewDicOnlineScripts = new Dictionary<string, OnlineScript>();
            NewDicOnlineScripts.Add(SendGroupMessageScriptServer.ScriptName, new SendGroupMessageScriptServer(this, DicCommunicationGroup[GroupID]));
            NewMember.AddOrReplaceScripts(NewDicOnlineScripts);

            DicCommunicationGroup[GroupID].AddMember(NewMember);
            ListPlayerToRemove.Add(NewMember);
        }
    }
}
