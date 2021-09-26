﻿using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class AskForPlayersScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask For Player List";

        private readonly CommunicationServer OnlineServer;

        public AskForPlayersScriptServer(CommunicationServer OnlineServer)
            : base(ScriptName)
        {
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new AskForPlayersScriptServer(OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            List<string> ListPlayerName = OnlineServer.GetPlayerNames();
            foreach (IOnlineConnection ActiveOnlinePlayer in OnlineServer.GlobalGroup.ListGroupMember)
            {
                ActiveOnlinePlayer.Send(new PlayerListScriptServer(ListPlayerName));
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
