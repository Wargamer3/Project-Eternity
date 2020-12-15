using System;

namespace ProjectEternity.Core.Online
{
    public class TransferRoomScriptServer : OnlineScript
    {
        public const string ScriptName = "Transfer Room";

        private readonly Server Owner;
        private readonly ClientGroup ClientGroupTemplate;

        string RoomID;

        public TransferRoomScriptServer(Server Owner, ClientGroup ClientGroupTemplate)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ClientGroupTemplate = ClientGroupTemplate;
        }

        public TransferRoomScriptServer(Server Owner, ClientGroup ClientGroupTemplate, string RoomID)
            : this(Owner, ClientGroupTemplate)
        {
            this.RoomID = RoomID;
        }

        public override OnlineScript Copy()
        {
            return new TransferRoomScriptServer(Owner, ClientGroupTemplate, RoomID);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            ClientGroup ActiveTransferingRoom;

            if (Owner.DicTransferingRoom.TryGetValue(RoomID, out ActiveTransferingRoom) && ActiveTransferingRoom.Room.ListOnlinePlayer.Count < ActiveTransferingRoom.Room.CurrentPlayerCount)
            {
                ActiveTransferingRoom.Room.AddOnlinePlayer(ActivePlayer, "Host");
            }
            else if (Owner.DicAllRoom.ContainsKey(RoomID))
            {
                ClientGroup NewRoom = ClientGroupTemplate.CreateFromTemplate(Owner.TransferRoom(RoomID));
                Owner.DicTransferingRoom.Add(RoomID, NewRoom);
                NewRoom.Room.AddOnlinePlayer(ActivePlayer, "Host");
                ActivePlayer.Send(new AskGameDataScriptServer());
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            RoomID = Sender.ReadString();
        }
    }
}
