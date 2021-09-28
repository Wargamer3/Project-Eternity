using System;

namespace ProjectEternity.Core.Online
{
    public class IdentifyScriptClient : OnlineScript
    {
        public const string ScriptName = "Identify";

        private readonly string ClientID;
        private readonly string ClientName;
        private readonly bool Spectator;
        private readonly byte[] ClientInfo;

        public IdentifyScriptClient(string ClientID, string ClientName, bool Spectator, byte[] ClientInfo)
            : base(ScriptName)
        {
            this.ClientID = ClientID;
            this.ClientName = ClientName;
            this.Spectator = Spectator;
            this.ClientInfo = ClientInfo;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ClientID);
            WriteBuffer.AppendString(ClientName);
            WriteBuffer.AppendBoolean(Spectator);
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
