using System;

namespace ProjectEternity.Core.Online
{
    public class AskChangeRoomSubtypeScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Room Subtype";

        private readonly IRoomInformations Owner;

        private string RoomSubtype;

        public AskChangeRoomSubtypeScriptServer(IRoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeRoomSubtypeScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            Owner.RoomSubtype = RoomSubtype;
            //TODO: Update database

            foreach (IOnlineConnection ActiveOnlinePlayer in Owner.ListUniqueOnlineConnection)
            {
                ActiveOnlinePlayer.Send(new ChangeRoomSubtypeScriptServer(RoomSubtype));
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            RoomSubtype = Sender.ReadString();
        }
    }
}
