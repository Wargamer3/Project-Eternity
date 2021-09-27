using System;

namespace ProjectEternity.Core.Online
{
    public class IdentifyScriptClient : OnlineScript
    {
        public const string ScriptName = "Identify";

        private readonly string ClientName;
        private readonly byte[] ClientInfo;

        public IdentifyScriptClient(string ClientName, byte[] ClientInfo)
            : base(ScriptName)
        {
            this.ClientName = ClientName;
            this.ClientInfo = ClientInfo;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ClientName);
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
