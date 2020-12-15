using System;

namespace ProjectEternity.Core.Online
{
    public class AskRoomListScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Room List";

        private readonly Server Owner;

        public AskRoomListScriptServer(Server Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskRoomListScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            ActivePlayer.Send(new RoomListScriptServer(Owner, Owner.DicAllRoom.Values));
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
