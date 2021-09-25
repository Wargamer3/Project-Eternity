using System;

namespace ProjectEternity.Core.Online
{
    public class IdentifyScriptServer : OnlineScript
    {
        public const string ScriptName = "Identify";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string ClientName;

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
            Sender.Name = ClientName;
            OnlineCommunicationServer.Identify(Sender);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            ClientName = Sender.ReadString();
        }
    }
}
