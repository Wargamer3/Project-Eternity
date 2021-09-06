using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class CreateRoomScriptServer : OnlineScript
    {
        public const string ScriptName = "Create Room";

        private readonly GameServer Owner;
        private readonly GameClientGroup ClientGroupTemplate;

        private string RoomName;
        private string RoomType;
        private string RoomSubtype;
        private int MaxNumberOfPlayer;
        public GameClientGroup CreatedGroup;

        public CreateRoomScriptServer(GameServer Owner, GameClientGroup ClientGroupTemplate)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ClientGroupTemplate = ClientGroupTemplate;
        }

        public override OnlineScript Copy()
        {
            return new CreateRoomScriptServer(Owner, ClientGroupTemplate);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            IRoomInformations CreatedRoom = Owner.Database.GenerateNewRoom(RoomName, RoomType, RoomSubtype, "", Owner.IP, Owner.Port, MaxNumberOfPlayer);

            CreatedGroup = ClientGroupTemplate.CreateFromTemplate(CreatedRoom);
            CreatedGroup.Room.AddOnlinePlayer(Sender, "Host");

            Owner.DicLocalRoom.Add(CreatedRoom.RoomID, CreatedGroup);
            Owner.DicAllRoom.Add(CreatedRoom.RoomID, CreatedRoom);
            Owner.ListPlayerToRemove.Add(Sender);

            Owner.SharedWriteBuffer.ClearWriteBuffer();
            Owner.SharedWriteBuffer.WriteScript(new RoomListScriptServer(Owner, new List<IRoomInformations>() { CreatedRoom }));

            foreach (IOnlineConnection ActivePlayer in Owner.ListPlayer)
            {
                if (ActivePlayer == Sender)
                {
                    continue;
                }

                ActivePlayer.SendWriteBuffer();
            }

            Sender.Send(new SendRoomIDScriptServer(CreatedRoom.RoomID));
        }

        protected internal override void Read(OnlineReader Sender)
        {
            RoomName = Sender.ReadString();
            RoomType = Sender.ReadString();
            RoomSubtype = Sender.ReadString();
            MaxNumberOfPlayer = Sender.ReadInt32();
        }
    }
}
