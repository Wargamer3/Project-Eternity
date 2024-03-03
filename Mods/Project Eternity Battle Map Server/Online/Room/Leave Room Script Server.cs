using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class LeaveRoomScriptServer : OnlineScript
    {
        public const string ScriptName = "Leave Room";

        private readonly IRoomInformations Owner;
        private readonly GameServer OnlineServer;

        public LeaveRoomScriptServer(IRoomInformations Owner, GameServer OnlineServer)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new LeaveRoomScriptServer(Owner, OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            OnlineServer.ListPlayer.Add(Sender);
            Owner.RemovePlayer(Sender);
            OnlineServer.Database.UpdatePlayerCountInRoom(Owner.RoomID, (byte)Owner.ListOnlinePlayer.Count);

            if (Owner.ListOnlinePlayer.Count == 0)
            {
                OnlineServer.ListLocalRoomToRemove.Add(Owner.RoomID);
            }

            //Send created Player to all Players.
            foreach (IOnlineConnection ActivePlayerInRoom in Owner.ListUniqueOnlineConnection)
            {
                ActivePlayerInRoom.Send(new PlayerLeftScriptServer(Sender.ID, 0));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
