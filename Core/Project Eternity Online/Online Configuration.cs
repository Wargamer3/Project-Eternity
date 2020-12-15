using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public unsafe class OnlineConfiguration
    {
        public Dictionary<string, OnlineScript> DicOnlineScripts = new Dictionary<string, OnlineScript>();

        private List<IOnlineConnection> ListPlayerClient;
        private IOnlineConnection Host;
        private TcpListener HostListener;
        private bool CanListen;
        private readonly OnlineWriter WriteBuffer;

        public bool IsHost { get { return Host == null; } }

        public OnlineConfiguration()
        {
            ListPlayerClient = new List<IOnlineConnection>();
            Host = null;
        }

        public void StartListening()
        {
            try
            {
                IPEndPoint ListeningEndPoint = new IPEndPoint(IPAddress.Any, 2312);
                HostListener = new TcpListener(ListeningEndPoint);
                HostListener.Start();
                CanListen = true;
            }
            catch (Exception)
            {
                CanListen = false;
            }
        }

        public void StopListening()
        {
            CanListen = false;

            try
            {
                if (HostListener != null)
                {
                    HostListener.Stop();
                    HostListener = null;
                }
            }
            catch (Exception)
            {
            }
        }

        public void CheckForNewClient()
        {
            if (!CanListen)
            {
                return;
            }

            try
            {
                if (HostListener.Pending())
                {
                    //Accept the pending client connection and return a TcpClient object initialized for communication.
                    TcpClient PlayerClient = HostListener.AcceptTcpClient();
                    PlayerClient.NoDelay = true;

                    // Get a stream object for reading and writing
                    NetworkStream PlayerStream = PlayerClient.GetStream();

                    OnlineConnection NewPlayer = new OnlineConnection(PlayerClient, PlayerStream, DicOnlineScripts, WriteBuffer);
                    ListPlayerClient.Add(NewPlayer);
                }
            }
            catch (SocketException)
            {
            }
        }

        public bool JoinHost(IPAddress HostAddress)
        {
            StopListening();

            for (int P = 0; P < ListPlayerClient.Count; P++)
            {
                // Shutdown and end connection
                ListPlayerClient[P].Close();
            }

            ListPlayerClient.Clear();

            IPEndPoint HostEndPoint = new IPEndPoint(HostAddress, 2312);
            TcpClient UserClient = new TcpClient();
            UserClient.NoDelay = true;
            UserClient.Connect(HostEndPoint);

            // Get a client stream for reading and writing.
            NetworkStream HostStream = UserClient.GetStream();
            Host = new OnlineConnection(UserClient, HostStream, null);

            return UserClient.Connected;
        }
        
        public void Update()
        {
            CheckForNewClient();

            for (int C = ListPlayerClient.Count - 1; C >= 0 ; --C)
            {
                IOnlineConnection Sender = ListPlayerClient[C];

                foreach (OnlineScript ActiveScript in Sender.ReadScripts())
                {
                    ActiveScript.Execute(Sender);

                    SendToClients(ActiveScript, Sender);
                }
            }
        }

        private void SendToClients(OnlineScript ScriptToSend, IOnlineConnection Sender)
        {
            WriteBuffer.ClearWriteBuffer();
            WriteBuffer.WriteScript(ScriptToSend);

            foreach (IOnlineConnection OnlinePlayer in ListPlayerClient)
            {
                if (OnlinePlayer == Sender)
                    continue;

                OnlinePlayer.SendWriteBuffer();
            }
        }

        private void Send(OnlineScript ScriptToSend)
        {
            if (IsHost && ListPlayerClient.Count > 0)
            {
                SendToClients(ScriptToSend, null);
            }
            else if (Host != null)
            {
                Host.Send(ScriptToSend);
            }
        }

        public void ExecuteAndSend(OnlineScript ScriptToSend)
        {
            ScriptToSend.Execute(null);
            Send(ScriptToSend);
        }
    }
}
