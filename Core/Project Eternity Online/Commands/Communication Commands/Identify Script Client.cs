using System;

namespace ProjectEternity.Core.Online
{
    public class IdentifyScriptClient : OnlineScript
    {
        public const string ScriptName = "Identify";

        private readonly string ClientName;

        public IdentifyScriptClient(string ClientName)
            : base(ScriptName)
        {
            this.ClientName = ClientName;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ClientName);
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
