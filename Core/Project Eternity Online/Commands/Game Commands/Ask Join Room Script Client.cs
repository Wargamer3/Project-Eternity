using System;

namespace ProjectEternity.Core.Online
{
    public class AskJoinRoomScriptClient : OnlineScript
    {
        private readonly string RoomID;

        public AskJoinRoomScriptClient(string RoomID)
            : base("Ask Join Room")
        {
            this.RoomID = RoomID;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomID);
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
