using System;

namespace ProjectEternity.Core.Online
{
    public class AskStartGameScriptClient : OnlineScript
    {
        public AskStartGameScriptClient()
            : base("Ask Start Game")
        {
        }

        public override OnlineScript Copy()
        {
            return null;
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
