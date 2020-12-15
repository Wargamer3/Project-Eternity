using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    /// <summary>
    /// The Server Manager only exist to direct Clients to a healthy Server on their first connection or when a Client lose connection to its Server.
    /// They all have thir own list of Server and they can create or delete one whenever needed.
    /// Unlike the Master, the Server Manager handle logic as it monitor Servers to make sure they are not overloaded and will transfer their workload if needed.
    /// </summary>
    public class ServerManager
    {
        private readonly List<IOnlineConnection> ListServer;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        private IOnlineConnection Master;
        public string ServerVersion;
        public string LatestVersion;//If a Server Manager doesn't have the latest version and there are no Server connected, it can be closed as it won't receive new Clients.
        private TcpListener ClientsListener;
        private TcpListener MastersListener;

        private CancellationTokenSource CancelToken;
        private readonly OnlineWriter SharedWriteBuffer;

        public ServerManager()
        {
            ListServer = new List<IOnlineConnection>();

            SharedWriteBuffer = new OnlineWriter();
            SharedWriteBuffer.ClearWriteBuffer();

            DicOnlineScripts = new Dictionary<string, OnlineScript>();
            //DicOnlineScripts.Add(MasterListScriptClient.ScriptName, new MasterListScriptClient(this));
        }

        public ServerManager(Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            ListServer = new List<IOnlineConnection>();

            SharedWriteBuffer = new OnlineWriter();
            SharedWriteBuffer.ClearWriteBuffer();

            this.DicOnlineScripts = DicOnlineScripts;
        }

        public IOnlineConnection CreateNewConnection(TcpClient ActiveClient, NetworkStream ActiveStream)
        {
            return new OnlineConnection(ActiveClient, ActiveStream, DicOnlineScripts, SharedWriteBuffer);
        }

        public void ConnectToExistingServer(IOnlineConnection NewServer)
        {
            ListServer.Add(NewServer);
        }

        private void Update()
        {
            while (!CancelToken.IsCancellationRequested)
            {
                WaitForConnections();

                if (Master != null)
                {
                    foreach (OnlineScript ActiveScript in Master.ReadScripts())
                    {
                        ActiveScript.Execute(Master);
                    }
                }
            }

            ClientsListener.Stop();
            ClientsListener = null;

            MastersListener.Stop();
            MastersListener = null;
        }

        private void WaitForConnections()
        {
            if (ClientsListener.Pending())
            {
                TcpClient ConnectingClient = ClientsListener.AcceptTcpClient();
                ConnectingClient.NoDelay = true;

                NetworkStream ClientStream = ConnectingClient.GetStream();

                IOnlineConnection NewConnection = new OnlineConnection(ConnectingClient, ClientStream, DicOnlineScripts);
                OnClientConnected(NewConnection);
            }

            if (MastersListener.Pending())
            {
                TcpClient ConnectingMaster = MastersListener.AcceptTcpClient();
                ConnectingMaster.NoDelay = true;

                NetworkStream MasterStream = ConnectingMaster.GetStream();

                IOnlineConnection NewConnection = new OnlineConnection(ConnectingMaster, MasterStream, DicOnlineScripts);
                OnMasterConnected(NewConnection);
            }
        }

        public void StartListening(int ClientsPort, int MastersPort)
        {
            try
            {
                CancelToken = new CancellationTokenSource();

                IPEndPoint ListeningEndPointClient = new IPEndPoint(IPAddress.Any, ClientsPort);
                ClientsListener = new TcpListener(ListeningEndPointClient);
                ClientsListener.Start();

                IPEndPoint ListeningEndPointMaster = new IPEndPoint(IPAddress.Any, MastersPort);
                MastersListener = new TcpListener(ListeningEndPointMaster);
                MastersListener.Start();

                Task.Run(() => { Update(); });
            }
            catch (Exception)
            {
                ClientsListener = null;
                MastersListener = null;
            }
        }

        public void StopListening()
        {
            CancelToken.Cancel();
        }

        public void OnClientConnected(IOnlineConnection NewClient)
        {
            NewClient.Send(new RedirectScriptServer());
            NewClient.Close();
        }

        public void OnMasterConnected(IOnlineConnection NewMaster)
        {
            if (Master != null)
            {
                Master.Close();
            }

            Master = NewMaster;
        }
    }
}
