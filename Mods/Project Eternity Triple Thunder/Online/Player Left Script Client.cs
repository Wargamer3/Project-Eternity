using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class PlayerLeftScriptClient : OnlineScript
    {
        public const string ScriptName = "Player Left";

        private readonly RoomInformations Owner;
        private readonly TripleThunderOnlineClient Client;
        private readonly IMissionSelect MissionSelectScreen;

        private string RoomPlayerID;
        private uint InGamePlayerID;

        public PlayerLeftScriptClient(RoomInformations Owner, TripleThunderOnlineClient Client, IMissionSelect MissionSelectScreen)
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

            if (Client.TripleThunderGame == null)
            {
                MissionSelectScreen.UpdateReadyOrHost();
            }
            else
            {
                for (int P = Client.TripleThunderGame.ListLocalPlayer.Count - 1; P >= 0; P--)
                {
                    if (Client.TripleThunderGame.ListLocalPlayer[P].InGameRobot.ID == InGamePlayerID)
                    {
                        Client.TripleThunderGame.ListLocalPlayer.RemoveAt(P);
                        break;
                    }
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            RoomPlayerID = Sender.ReadString();
            InGamePlayerID = Sender.ReadUInt32();
        }
    }
}
