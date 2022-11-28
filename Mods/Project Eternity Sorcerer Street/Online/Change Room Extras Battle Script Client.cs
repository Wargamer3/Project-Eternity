using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Online
{
    public class ChangeRoomExtrasBattleScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Room Extras";

        private readonly SorcererStreetRoomInformations Owner;
        private readonly GamePreparationScreen MissionSelectScreen;

        private int MaxKill;
        private int MaxGameLengthInMinutes;

        public ChangeRoomExtrasBattleScriptClient(SorcererStreetRoomInformations Owner, GamePreparationScreen MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.MissionSelectScreen = MissionSelectScreen;
        }

        public override OnlineScript Copy()
        {
            return new ChangeRoomExtrasBattleScriptClient(Owner, MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.MaxKill = MaxKill;
            Owner.MaxGameLengthInMinutes = MaxGameLengthInMinutes;
        }

        protected override void Read(OnlineReader Sender)
        {
            MaxKill = Sender.ReadInt32();
            MaxGameLengthInMinutes = Sender.ReadInt32();
        }
    }
}
