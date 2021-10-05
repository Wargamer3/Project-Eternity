using System;

namespace ProjectEternity.Core.Online
{
    public class AskGameDataFromClientScriptServer : OnlineScript
    {
        public AskGameDataFromClientScriptServer()
            : base("Ask Game Data From Client")
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
