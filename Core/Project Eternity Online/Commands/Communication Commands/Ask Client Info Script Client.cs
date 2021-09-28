using System;

namespace ProjectEternity.Core.Online
{
    public class AskClientInfoScriptClient : OnlineScript
    {
        private readonly string ClientName;

        public AskClientInfoScriptClient(string ClientName)
            : base("Ask Client Info")
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

        protected internal override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
