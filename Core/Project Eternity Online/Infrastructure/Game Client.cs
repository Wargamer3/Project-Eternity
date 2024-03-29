﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class GameClient
    {
        public IOnlineConnection Host;
        public IOnlineConnection LastTopLevel;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        protected readonly List<DelayedExecutableOnlineScript> ListDelayedOnlineCommand;
        public string ClientVersion;

        public string RoomID;
        public IOnlineGame CurrentGame;
        private CancellationTokenSource CancelToken;

        public bool IsConnected => Host != null && Host.IsConnected();

        public GameClient()
            : this(new Dictionary<string, OnlineScript>())
        {
        }

        public GameClient(Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            RoomID = null;
            CurrentGame = null;
            ListDelayedOnlineCommand = new List<DelayedExecutableOnlineScript>();

            this.DicOnlineScripts = DicOnlineScripts;
        }

        public void ConnectToKnownIP()
        {

        }

        public void Connect(IPAddress HostAddress, int HostPort)
        {
            ChangeHost(HostAddress, HostPort);

            CancelToken = new CancellationTokenSource();

            Task.Run(() => { UpdateLoop(); });
        }

        public void ChangeHost(IPAddress HostAddress, int HostPort)
        {
            IPEndPoint HostEndPoint = new IPEndPoint(HostAddress, HostPort);
            TcpClient UserClient = new TcpClient();
            UserClient.NoDelay = true;
            UserClient.Connect(HostEndPoint);

            // Get a client stream for reading and writing.
            NetworkStream HostStream = UserClient.GetStream();
            ChangeHost(new OnlineConnection(UserClient, HostStream, DicOnlineScripts));
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

                //Was in a room when the server closed.
                if (RoomID != null)
                {
                    NewHost.Send(new TransferRoomScriptClient(RoomID));
                }
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

        public void ConnectToRoom(IRoomInformations RoomToConnect)
        {
            if (RoomToConnect.OwnerServerIP != Host.IP)
            {
                ChangeHost(IPAddress.Parse(RoomToConnect.OwnerServerIP), RoomToConnect.OwnerServerPort);
            }
        }

        public void CreateRoom(string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
        {
            Host.Send(new CreateRoomScriptClient(RoomName, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer));
        }

        public void JoinRoom(string RoomID)
        {
            Host.Send(new AskJoinRoomScriptClient(RoomID));
        }

        public void StartGame()
        {
            Host.Send(new AskStartGameScriptClient());
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
        }

        public void DelayOnlineScript(DelayedExecutableOnlineScript ScriptToDelay)
        {
            lock (ListDelayedOnlineCommand)
            {
                ListDelayedOnlineCommand.Add(ScriptToDelay);
            }
        }
    }
}
