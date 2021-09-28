using System;

namespace ProjectEternity.Core.Online
{
    public class AskClientInfoScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Client Info";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string ClientID;

        public AskClientInfoScriptServer(CommunicationServer OnlineCommunicationServer)
            : base(ScriptName)
        {
            this.OnlineCommunicationServer = OnlineCommunicationServer;
        }

        public override OnlineScript Copy()
        {
            return new AskClientInfoScriptServer(OnlineCommunicationServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            byte[] ClientInfo = OnlineCommunicationServer.GetClientInfo(ClientID);
            Sender.Send(new ClientInfoScriptServer(ClientInfo));
        }

        protected internal override void Read(OnlineReader Sender)
        {
            ClientID = Sender.ReadString();
        }
    }
}
