using System;

namespace ProjectEternity.Core.Online
{
    public class AskRoomListScriptClient : OnlineScript
    {
        public AskRoomListScriptClient()
            : base("Ask Room List")
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
