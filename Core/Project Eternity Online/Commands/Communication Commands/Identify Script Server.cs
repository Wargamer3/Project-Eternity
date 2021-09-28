using System;

namespace ProjectEternity.Core.Online
{
    public class IdentifyScriptServer : OnlineScript
    {
        public const string ScriptName = "Identify";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string ClientID;
        private string ClientName;
        private bool Spectator;
        private byte[] ClientInfo;

        public IdentifyScriptServer(CommunicationServer OnlineCommunicationServer)
            : base(ScriptName)
        {
            this.OnlineCommunicationServer = OnlineCommunicationServer;
        }

        public override OnlineScript Copy()
        {
            return new IdentifyScriptServer(OnlineCommunicationServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            Sender.ID = ClientID;
            Sender.Name = ClientName;

            if (!Spectator)
            {
                OnlineCommunicationServer.Identify(Sender, ClientInfo);
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            ClientID = Sender.ReadString();
            ClientName = Sender.ReadString();
            Spectator = Sender.ReadBoolean();
            ClientInfo = Sender.ReadByteArray();
        }
    }
}
