using System;

namespace ProjectEternity.Core.Online
{
    public class CreateRoomScriptClient : OnlineScript
    {
        private readonly string RoomName;
        private readonly string RoomType;
        private readonly string RoomSubtype;
        private readonly byte MinNumberOfPlayer;
        private readonly byte MaxNumberOfPlayer;

        public CreateRoomScriptClient(string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base("Create Room")
        {
            this.RoomName = RoomName;
            this.RoomType = RoomType;
            this.RoomSubtype = RoomSubtype;
            this.MinNumberOfPlayer = MinNumberOfPlayer;
            this.MaxNumberOfPlayer = MaxNumberOfPlayer;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomName);
            WriteBuffer.AppendString(RoomType);
            WriteBuffer.AppendString(RoomSubtype);
            WriteBuffer.AppendByte(MinNumberOfPlayer);
            WriteBuffer.AppendByte(MaxNumberOfPlayer);
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
