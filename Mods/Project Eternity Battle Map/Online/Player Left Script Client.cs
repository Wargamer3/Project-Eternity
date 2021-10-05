using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class PlayerLeftScriptClient : OnlineScript
    {
        public const string ScriptName = "Player Left";

        private readonly RoomInformations Owner;
        private readonly BattleMapOnlineClient Client;
        private readonly GamePreparationScreen MissionSelectScreen;

        private string RoomPlayerID;
        private uint InGamePlayerID;

        public PlayerLeftScriptClient(RoomInformations Owner, BattleMapOnlineClient Client, GamePreparationScreen MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.Client = Client;
            this.MissionSelectScreen = MissionSelectScreen;
        }

        public override OnlineScript Copy()
        {
            return new PlayerLeftScriptClient(Owner, Client, MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            lock (Owner.ListRoomPlayer)
            {
                for (int P = 0; P < Owner.ListRoomPlayer.Count; ++P)
                {
                    if (Owner.ListRoomPlayer[P].ConnectionID == RoomPlayerID)
                    {
                        Owner.ListRoomPlayer.RemoveAt(P);
                    }
                }
            }

            if (Client.BattleMapGame == null)
            {
                MissionSelectScreen.UpdateReadyOrHost();
            }
            else
            {
                /*for (int P = Client.BattleMapGame.ListLocalPlayer.Count - 1; P >= 0; P--)
                {
                    if (Client.BattleMapGame.ListLocalPlayer[P].InGameRobot.ID == InGamePlayerID)
                    {
                        Client.BattleMapGame.ListLocalPlayer.RemoveAt(P);
                        break;
                    }
                }*/
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            RoomPlayerID = Sender.ReadString();
            InGamePlayerID = Sender.ReadUInt32();
        }
    }
}
