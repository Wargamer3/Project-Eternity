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
        private readonly Dictionary<string, IOnlineConnection> DicPlayerByName;
        public readonly List<IOnlineConnection> ListPlayerToRemove;
        public readonly List<CommunicationGroup> ListGroup;

        public readonly CommunicationGroup GlobalGroup;

        public string IP;
        public int Port;

        public readonly IDataManager Database;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        private double DeltaTime = 1.0d / 60.0d;
        private double ElapsedTime = 0d;
        public readonly OnlineWriter SharedWriteBuffer;

        private TcpListener ClientsListener;

        private CancellationTokenSource CancelToken;

        public CommunicationServer(IDataManager Database, Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            DicPlayerByName = new Dictionary<string, IOnlineConnection>();
            ListPlayerToRemove = new List<IOnlineConnection>();
            ListGroup = new List<CommunicationGroup>();
            GlobalGroup = new CommunicationGroup();

            SharedWriteBuffer = new OnlineWriter();

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
                GlobalGroup.ListGroupMember.Remove(ListPlayerToRemove[0]);
                ListPlayerToRemove.RemoveAt(0);
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
            GlobalGroup.AddMember(NewClient);
            NewClient.StartReadingScriptAsync();
        }

        public List<string> GetPlayerNames()
        {
            List<string> ListPlayerName;

            lock (DicPlayerByName)
            {
                ListPlayerName = new List<string>(DicPlayerByName.Keys);
            }

            return ListPlayerName;
        }

        internal void Identify(IOnlineConnection NewClient)
        {
            lock (DicPlayerByName)
            {
                DicPlayerByName.Add(NewClient.Name, NewClient);
            }
        }
    }
}
