using System;

namespace ProjectEternity.Core.Online
{
    public class ClientInfoScriptServer : OnlineScript
    {
        public const string ScriptName = "Client Info";

        private readonly byte[] ClientInfo;

        public ClientInfoScriptServer(byte[] ClientInfo)
            : base(ScriptName)
        {
            this.ClientInfo = ClientInfo;
        }

        public override OnlineScript Copy()
        {
            return new ClientInfoScriptServer(ClientInfo);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendByteArray(ClientInfo);
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
