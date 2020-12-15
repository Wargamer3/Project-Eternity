using System;

namespace ProjectEternity.Core.Online
{
    public class PlayerLeftScriptServer : OnlineScript
    {
        public const string ScriptName = "Player Left";

        private readonly string RoomPlayerID;
        private readonly uint InGamePlayerID;

        public PlayerLeftScriptServer(string RoomPlayerID, uint InGamePlayerID)
            : base(ScriptName)
        {
            this.RoomPlayerID = RoomPlayerID;
            this.InGamePlayerID = InGamePlayerID;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomPlayerID);
            WriteBuffer.AppendUInt32(InGamePlayerID);
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
