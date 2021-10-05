using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class ChangeRoomExtrasBattleScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Room Extras";

        private readonly PVPRoomInformations Owner;
        private readonly GamePreparationScreen MissionSelectScreen;

        private int MaxKill;
        private int MaxGameLengthInMinutes;

        public ChangeRoomExtrasBattleScriptClient(PVPRoomInformations Owner, GamePreparationScreen MissionSelectScreen)
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
