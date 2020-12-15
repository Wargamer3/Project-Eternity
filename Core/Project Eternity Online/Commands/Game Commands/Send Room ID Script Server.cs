using System;

namespace ProjectEternity.Core.Online
{
    public class SendRoomIDScriptServer : OnlineScript
    {
        private readonly string RoomID;

        public SendRoomIDScriptServer(string RoomID)
            : base("Send Room ID")
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

        protected internal override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
