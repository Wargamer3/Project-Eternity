using System;

namespace ProjectEternity.Core.Online
{
    public class TransferRoomScriptClient : OnlineScript
    {
        public TransferRoomScriptClient(string RoomID)
            : base("Transfer Room")
        {
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
