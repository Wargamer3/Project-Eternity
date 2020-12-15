using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    /// <summary>
    /// The Master only exist to direct Clients to a Server Manager on their first connection or when a Client lose connection to its Server Manager.
    /// They all share the same list of Server Managers so if a Master creates a Server Manager it will communicate with all others.
    /// If a new Master is needed a user will need to add it through an already running Master so that it can be shared with the other Masters.
    /// It will also check to see if a Server Manager is not responding to not direct Client to a dead end.
    /// It can also store Server Managers as Idle which is never used but can be changed to an active state should a Server Manager die and need to be replaced.
    /// Can probably be replaced by a DNS with load balancing.
    /// </summary>
    public class Master
    {
        public struct ServerManagerInfo
        {
            public string IP;
            public string Version;
            public IOnlineConnection ServerManagerConnection;

            public ServerManagerInfo(IOnlineConnection ServerManagerConnection)
            {
                this.ServerManagerConnection = ServerManagerConnection;
                IP = ServerManagerConnection.IP;
                Version = null;
            }

            public ServerManagerInfo(string IP)
            {
                this.IP = IP;
                Version = null;
                ServerManagerConnection = null;
            }
        }

        //TODO: Replace with rest API call instead of keeping a connection alive
        public readonly List<IOnlineConnection> ListOtherMaster;
        public readonly List<ServerManagerInfo> ListServerManager;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        private TcpListener ClientsListener;
        private TcpListener MastersListener;

        private CancellationTokenSource CancelToken;
        private OnlineWriter SharedWriteBuffer;

        private string LatestServerVersion;

        public Master()
        {
            ListOtherMaster = new List<IOnlineConnection>();
            ListServerManager = new List<ServerManagerInfo>();

            SharedWriteBuffer = new OnlineWriter();
            SharedWriteBuffer.ClearWriteBuffer();

            DicOnlineScripts = new Dictionary<string, OnlineScript>();

            DicOnlineScripts.Add(MasterListScriptClient.ScriptName, new MasterListScriptClient(this));
            DicOnlineScripts.Add(MasterAddedScriptClient.ScriptName, new MasterAddedScriptClient(this));
            DicOnlineScripts.Add(ServerManagerAddedScriptClient.ScriptName, new ServerManagerAddedScriptClient(this));
        }

        public IOnlineConnection CreateNewConnection(TcpClient ActiveClient, NetworkStream ActiveStream)
        {
            return new OnlineConnection(ActiveClient, ActiveStream, DicOnlineScripts, SharedWriteBuffer);
        }

        public void ConnectToExistingMaster(IOnlineConnection NewMaster)
        {
            SharedWriteBuffer.ClearWriteBuffer();
            SharedWriteBuffer.WriteScript(new MasterAddedScriptServer(NewMaster.IP));

            foreach (IOnlineConnection ActiveMaster in ListOtherMaster)
            {
                ActiveMaster.SendWriteBuffer();
            }

            NewMaster.Send(new MasterListScriptServer(ListOtherMaster));
            ListOtherMaster.Add(NewMaster);
        }

        public void ConnectToExistingServerManager(IOnlineConnection NewServerManager)
        {
            SharedWriteBuffer.ClearWriteBuffer();
            SharedWriteBuffer.WriteScript(new ServerManagerAddedScriptServer(NewServerManager.IP));

            foreach (IOnlineConnection ActiveMaster in ListOtherMaster)
            {
                ActiveMaster.SendWriteBuffer();
            }

            //Don't send idle masters
            NewServerManager.Send(new MasterListScriptServer(ListOtherMaster));
            ListServerManager.Add(new ServerManagerInfo(NewServerManager));
        }

        public void UpdateServerVersion(string NewServerVersion)
        {
            LatestServerVersion = NewServerVersion;

            RemoveOutOfDateServerManagersAfterVersionChange();
        }

        /// <summary>
        /// A master will only send clients to Server Managers with the most up to date version, as such it doesn't need to remember
        /// Server Managers that are out of date.
        /// </summary>
        private void RemoveOutOfDateServerManagersAfterVersionChange()
        {
            for (int S = ListServerManager.Count - 1; S >= 0; --S)
            {
                if (ListServerManager[S].Version != LatestServerVersion)
                {
                    ListServerManager.RemoveAt(S);
                }
            }
        }

        private void WaitForConnections()
        {
            while (!CancelToken.IsCancellationRequested)
            {
                if (ClientsListener.Pending())
                {
                    TcpClient ConnectingClient = ClientsListener.AcceptTcpClient();
                    ConnectingClient.NoDelay = true;

                    NetworkStream ClientStream = ConnectingClient.GetStream();

                    IOnlineConnection NewConnection = CreateNewConnection(ConnectingClient, ClientStream);
                    OnClientConnected(NewConnection);
                }

                if (MastersListener.Pending() && ListOtherMaster.Count == 0)
                {
                    TcpClient ConnectingMaster = MastersListener.AcceptTcpClient();
                    ConnectingMaster.NoDelay = true;

                    NetworkStream MasterStream = ConnectingMaster.GetStream();

                    IOnlineConnection NewConnection = CreateNewConnection(ConnectingMaster, MasterStream);
                    OnMasterConnected(NewConnection);
                }
            }

            ClientsListener.Stop();
            ClientsListener = null;

            MastersListener.Stop();
            MastersListener = null;
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

                Task.Run(() => { WaitForConnections(); });
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
            ListOtherMaster.Add(NewMaster);
        }
    }
}
