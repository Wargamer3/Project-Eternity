using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ChangePlayerTypeScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Player Type";

        private readonly RoomInformations Owner;
        private readonly IMissionSelect MissionSelectScreen;

        private string PlayerID;
        private string PlayerType;

        public ChangePlayerTypeScriptClient(RoomInformations Owner, IMissionSelect MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.MissionSelectScreen = MissionSelectScreen;
        }

        public override OnlineScript Copy()
        {
            return new ChangePlayerTypeScriptClient(Owner, MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            foreach (Player ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == PlayerID)
                {
                    ActivePlayer.PlayerType = PlayerType;
                }
            }

            MissionSelectScreen.UpdateReadyOrHost();
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            PlayerType = Sender.ReadString();
        }
    }
}
