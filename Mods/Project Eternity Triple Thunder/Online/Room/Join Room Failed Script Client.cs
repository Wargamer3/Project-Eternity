using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class JoinRoomFailedScriptClient : OnlineScript
    {
        public const string ScriptName = "Join Room Failed";

        private readonly TripleThunderOnlineClient Owner;
        private readonly Lobby ScreenOwner;

        private string RoomID;
        private bool RoomExist;
        private byte CurrentPlayerCount;
        private byte MinNumberOfPlayer;
        private byte MaxNumberOfPlayer;

        public JoinRoomFailedScriptClient(TripleThunderOnlineClient Owner, Lobby ScreenOwner)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ScreenOwner = ScreenOwner;
        }

        public override OnlineScript Copy()
        {
            return new JoinRoomFailedScriptClient(Owner, ScreenOwner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            if (RoomExist)
            {
                ScreenOwner.DicAllRoom[RoomID].CurrentPlayerCount = CurrentPlayerCount;
                ScreenOwner.DicAllRoom[RoomID].MinNumberOfPlayer = MinNumberOfPlayer;
                ScreenOwner.DicAllRoom[RoomID].MaxNumberOfPlayer = MaxNumberOfPlayer;
            }
            else
            {
                lock (ScreenOwner.DicAllRoom)
                {
                    if (ScreenOwner.DicAllRoom.ContainsKey(RoomID))
                    {
                        ScreenOwner.DicAllRoom.Remove(RoomID);
                    }
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            RoomID = Sender.ReadString();
            RoomExist = Sender.ReadBoolean();

            if (RoomExist)
            {
                CurrentPlayerCount = Sender.ReadByte();
                MinNumberOfPlayer = Sender.ReadByte();
                MaxNumberOfPlayer = Sender.ReadByte();
            }
        }
    }
}
