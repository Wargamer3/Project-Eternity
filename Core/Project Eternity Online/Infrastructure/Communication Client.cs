﻿using System;
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
        public IOnlineConnection GroupHost;
        public List<IOnlineConnection> ListPrivateMessageGroup;
        private CancellationTokenSource CancelToken;
        private readonly Dictionary<string, OnlineScript> DicOnlineScripts;
        protected readonly List<DelayedExecutableOnlineScript> ListDelayedOnlineCommand;
        public readonly ChatManager Chat;

        public CommunicationClient()
            : this(new Dictionary<string, OnlineScript>())
        {
        }

        public CommunicationClient(Dictionary<string, OnlineScript> DicOnlineScripts)
        {
            ListDelayedOnlineCommand = new List<DelayedExecutableOnlineScript>();
            Chat = new ChatManager();

            this.DicOnlineScripts = DicOnlineScripts;
        }

        public void Connect(IPAddress HostAddress, int HostPort)
        {
            ChangeHost(HostAddress, HostPort);

            CancelToken = new CancellationTokenSource();

            Task.Run(() => { UpdateLoop(); });
        }

        public void ConnectToKnownIP()
        {

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

        public void SendMessage(string Source, string Message)
        {
            if (Source == "Global")
            {
                Host.Send(new SendGlobalMessageScriptClient(Message, ChatManager.MessageColors.White));
            }
            else
            {
                Host.Send(new SendGroupMessageScriptClient(Source, Message, ChatManager.MessageColors.White));
            }
        }

        public void AddFriend(string FriendID)
        {
            Host.Send(new AddFriendScriptClient(FriendID));
        }
    }
}
