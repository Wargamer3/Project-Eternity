using System;

namespace ProjectEternity.Core.Online
{
    public abstract class BaseAskJoinRoomScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Join Room";

        protected readonly GameServer Owner;
        private string RoomID;

        public BaseAskJoinRoomScriptServer(GameServer Owner)
            : this(Owner, null)
        {
        }

        public BaseAskJoinRoomScriptServer(GameServer Owner, string RoomID)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.RoomID = RoomID;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            IRoomInformations RoomToJoin;
            if (Owner.DicAllRoom.TryGetValue(RoomID, out RoomToJoin))
            {
                GameClientGroup LocalRoom = null;
                if (RoomToJoin.OwnerServerIP != Owner.IP)
                {
                    Sender.Send(new JoinRoomRemoteScriptServer(RoomID, RoomToJoin.OwnerServerIP, RoomToJoin.OwnerServerPort));
                }
                else if (Owner.DicLocalRoom.TryGetValue(RoomID, out LocalRoom) && LocalRoom.Room.ListOnlinePlayer.Count < LocalRoom.Room.MaxNumberOfPlayer)
                {
                    Owner.DicLocalRoom[RoomID].Room.AddOnlinePlayer(Sender, "Player");
                    Owner.Database.UpdatePlayerCountInRoom(RoomID, LocalRoom.Room.ListOnlinePlayer.Count);
                    Owner.ListPlayerToRemove.Add(Sender);

                    OnJoinRoomLocal(Sender, RoomID, Owner.DicLocalRoom[RoomID]);
                }
                else
                {
                    //Maybe in transfer, don't allow the client to join
                    if (LocalRoom == null)
                    {
                        Sender.Send(new JoinRoomFailedScriptServer(RoomID, RoomToJoin));
                    }
                    else
                    {
                        Sender.Send(new JoinRoomFailedScriptServer(RoomID, LocalRoom.Room));
                    }
                }
            }
            else
            {
                Sender.Send(new JoinRoomFailedScriptServer(RoomID, RoomToJoin));
            }
        }

        protected abstract void OnJoinRoomLocal(IOnlineConnection Sender, string RoomID, GameClientGroup ActiveGroup);

        protected internal override void Read(OnlineReader Sender)
        {
            RoomID = Sender.ReadString();
        }
    }
}
