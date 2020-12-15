using System;

namespace ProjectEternity.Core.Online
{
    public class ChangeRoomSubtypeScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Room Subtype";

        private readonly string RoomSubtype;

        public ChangeRoomSubtypeScriptServer(string RoomSubtype)
            : base(ScriptName)
        {
            this.RoomSubtype = RoomSubtype;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomSubtype);
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
