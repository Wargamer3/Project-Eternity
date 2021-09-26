using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    /// <summary>
    /// The Server is the only layer that keeps a connection with a Client.
    /// They all share the same database meaning they all share the same rooms and players.
    /// The Server handle the chat and rooms for its Clients and share informations to its sole Server Manager.
    /// </summary>
    public class GameServer
    {
        public readonly List<IOnlineConnection> ListPlayer;
        public readonly List<IOnlineConnection> ListPlayerToRemove;

        public readonly Dictionary<string, GameClientGroup> DicLocalRoom;
        public readonly List<string> ListLocalRoomToRemove;

        public readonly Dictionary<string, GameClientGroup> DicTransferingRoom;
        public readonly List<string> ListTransferingRoomToRemove;

        public readonly Dictionary<string, IRoomInformations> DicAllRoom;

        public string IP;
        public int Port;
        public string ServerVersion;
        public string LatestVersion;//If a Server doesn't have the latest version and there are no Client connected, it can be closed as it won't receive new Clients.

        private DateTimeOffset NextRoomUpdateTime;

        public readonly IGameDataManager Database;
        public readonly OnlineWriter SharedWriteBuffer;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        private double DeltaTime = 1.0d / 60.0d;
        private double ElapsedTime = 0d;

        private TcpListener ClientsListener;

        private CancellationTokenSource CancelToken;

        public GameServer(IGameDataManager Database, Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            ListPlayer = new List<IOnlineConnection>();
            ListPlayerToRemove = new List<IOnlineConnection>();

            DicLocalRoom = new Dictionary<string, GameClientGroup>();
            ListLocalRoomToRemove = new List<string>();

            DicTransferingRoom = new Dictionary<string, GameClientGroup>();
            ListTransferingRoomToRemove = new List<string>();

            DicAllRoom = new Dictionary<string, IRoomInformations>();

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
            Parallel.ForEach(ListPlayer, (ActivePlayer, loopState) =>
            {
                if (!ActivePlayer.IsConnected())
                {
                    if (ActivePlayer.HasLeftServer())
                    {
                        ListPlayerToRemove.Add(ActivePlayer);
                        Database.RemovePlayer(ListPlayerToRemove[0]);
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
                ListPlayer.Remove(ListPlayerToRemove[0]);
                ListPlayerToRemove.RemoveAt(0);
            }

            Parallel.ForEach(DicLocalRoom.Values, (ActiveGroup, loopState) =>
            {
                if (ActiveGroup.IsRunningSlow())
                {
                    //Send Room to another Server
                }

                if (ActiveGroup.CurrentGame != null)
                {
                    ActiveGroup.CurrentGame.Update(DeltaTime);
                }

                for (int P = ActiveGroup.Room.ListOnlinePlayer.Count - 1; P >= 0; --P)
                {
                    IOnlineConnection ActivePlayer = ActiveGroup.Room.ListOnlinePlayer[P];

                    if (!ActivePlayer.IsConnected())
                    {
                        if (ActivePlayer.HasLeftServer())
                        {
                            string PlayerID = ActiveGroup.Room.ListOnlinePlayer[P].ID;
                            ActiveGroup.Room.RemoveOnlinePlayer(P);
                            Database.UpdatePlayerCountInRoom(ActiveGroup.Room.RoomID, ActiveGroup.Room.ListOnlinePlayer.Count);

                            if (ActiveGroup.Room.ListOnlinePlayer.Count == 0)
                            {
                                ListLocalRoomToRemove.Add(ActiveGroup.Room.RoomID);
                            }

                            if (ActiveGroup.CurrentGame == null)
                            {
                                foreach (IOnlineConnection OtherPlayer in ActiveGroup.Room.ListOnlinePlayer)
                                {
                                    OtherPlayer.Send(new PlayerLeftScriptServer(PlayerID, 0));
                                }
                            }
                            else
                            {
                                ActiveGroup.CurrentGame.RemoveOnlinePlayer(PlayerID, ActivePlayer);
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

            while (ListLocalRoomToRemove.Count > 0)
            {
                DicLocalRoom.Remove(ListLocalRoomToRemove[0]);
                DicAllRoom.Remove(ListLocalRoomToRemove[0]);
                Database.RemoveRoom(ListLocalRoomToRemove[0]);
                ListLocalRoomToRemove.RemoveAt(0);
            }

            foreach (GameClientGroup ActiveGroup in DicTransferingRoom.Values)
            {
                foreach (IOnlineConnection ActivePlayer in ActiveGroup.Room.ListOnlinePlayer)
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

                if (ActiveGroup.Room.ListOnlinePlayer.Count == ActiveGroup.Room.CurrentPlayerCount && ActiveGroup.CurrentGame != null)
                {
                    ListTransferingRoomToRemove.Add(ActiveGroup.Room.RoomID);
                }
            }

            while (ListTransferingRoomToRemove.Count > 0)
            {
                DicLocalRoom.Add(ListTransferingRoomToRemove[0], DicTransferingRoom[ListTransferingRoomToRemove[0]]);
                DicTransferingRoom.Remove(ListTransferingRoomToRemove[0]);
                ListTransferingRoomToRemove.RemoveAt(0);
            }

            //TODO: Run task in background
            if (DateTimeOffset.Now > NextRoomUpdateTime)
            {
                NextRoomUpdateTime = NextRoomUpdateTime.AddSeconds(10);

                List<IRoomInformations> ListRoomUpdates = Database.GetAllRoomUpdatesSinceLastTimeChecked(ServerVersion);

                if (ListRoomUpdates != null)
                {
                    foreach (IRoomInformations ActiveRoomUpdate in ListRoomUpdates)
                    {
                        if (ActiveRoomUpdate.IsDead)
                        {
                            DicAllRoom.Remove(ActiveRoomUpdate.RoomID);
                        }
                        else
                        {
                            if (DicAllRoom.ContainsKey(ActiveRoomUpdate.RoomID))
                            {
                                DicAllRoom[ActiveRoomUpdate.RoomID] = ActiveRoomUpdate;
                            }
                            else
                            {
                                DicAllRoom.Add(ActiveRoomUpdate.RoomID, ActiveRoomUpdate);
                            }
                        }
                    }

                    SharedWriteBuffer.ClearWriteBuffer();
                    SharedWriteBuffer.WriteScript(new RoomListScriptServer(this, ListRoomUpdates));
                    Parallel.ForEach(ListPlayer, (ActivePlayer, loopState) =>
                    {
                        ActivePlayer.SendWriteBuffer();
                    });
                }
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

                Database.HandleOldData(IP, Port);

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
            ListPlayer.Add(NewClient);
            NewClient.StartReadingScriptAsync();
            NewClient.Send(new ConnectionSuccessScriptServer());
        }

        public IRoomInformations TransferRoom(string RoomID)
        {
            return Database.TransferRoom(RoomID, IP);
        }
    }
}
