using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ChangeCharacterScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Character";

        private readonly RoomInformations Owner;
        private readonly IMissionSelect MissionSelectScreen;

        private string PlayerID;
        private string CharacterType;

        public ChangeCharacterScriptClient(RoomInformations Owner, IMissionSelect MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.MissionSelectScreen = MissionSelectScreen;
        }

        public override OnlineScript Copy()
        {
            return new ChangeCharacterScriptClient(Owner, MissionSelectScreen);
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
                    ActivePlayer.Equipment.CharacterType = CharacterType;
                    MissionSelectScreen.UpdateCharacter(ActivePlayer);
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            CharacterType = Sender.ReadString();
        }
    }
}
