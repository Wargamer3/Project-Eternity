using System;

namespace ProjectEternity.Core.Online
{
    public class JoinRoomFailedScriptServer : OnlineScript
    {
        private readonly string RoomID;
        private readonly IRoomInformations RoomToJoin;

        public JoinRoomFailedScriptServer(string RoomID, IRoomInformations RoomToJoin)
            : base("Join Room Failed")
        {
            this.RoomID = RoomID;
            this.RoomToJoin = RoomToJoin;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomID);
            WriteBuffer.AppendBoolean(RoomToJoin != null);
            if (RoomToJoin != null)
            {
                WriteBuffer.AppendInt32(RoomToJoin.CurrentPlayerCount);
                WriteBuffer.AppendInt32(RoomToJoin.MaxNumberOfPlayer);
            }
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
