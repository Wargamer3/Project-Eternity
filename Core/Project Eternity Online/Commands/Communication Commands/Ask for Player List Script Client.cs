using System;

namespace ProjectEternity.Core.Online
{
    public class AskForPlayersScriptClient : OnlineScript
    {
        public AskForPlayersScriptClient()
            : base("Ask For Player List")
        {
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
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
