using ProjectEternity.Core.Online;
using System;
using System.Collections.Generic;

namespace ProjectEternity.Communication.Server
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

        protected override void Execute(IOnlineConnection Sender)
        {
            List<byte[]> ListPlayerName = OnlineServer.GetPlayerNames();
            foreach (IOnlineConnection ActiveOnlinePlayer in OnlineServer.GlobalGroup.ListGroupMember)
            {
                ActiveOnlinePlayer.Send(new PlayerListScriptServer(ListPlayerName));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
