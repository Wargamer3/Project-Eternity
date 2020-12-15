using System;

namespace ProjectEternity.Core.Online
{
    public class RedirectScriptServer : OnlineScript
    {
        private readonly string ServerIP;
        private readonly int ServerPort;
        private readonly string CommunicationServerIP;
        private readonly int CommunicationServerPort;

        public RedirectScriptServer()
            : base("Redirect")
        {
        }

        public RedirectScriptServer(string ServerIP, int ServerPort, string CommunicationServerIP, int CommunicationServerPort)
            : base("Redirect")
        {
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.CommunicationServerIP = CommunicationServerIP;
            this.CommunicationServerPort = CommunicationServerPort;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ServerIP);
            WriteBuffer.AppendInt32(ServerPort);
            WriteBuffer.AppendString(CommunicationServerIP);
            WriteBuffer.AppendInt32(CommunicationServerPort);
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
