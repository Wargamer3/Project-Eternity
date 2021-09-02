using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ChangeRoomExtrasMissionScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Room Extras";

        private readonly MissionRoomInformations Owner;
        private readonly IMissionSelect MissionSelectScreen;

        public ChangeRoomExtrasMissionScriptClient(MissionRoomInformations Owner, IMissionSelect MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.MissionSelectScreen = MissionSelectScreen;
        }

        public override OnlineScript Copy()
        {
            return new ChangeRoomExtrasMissionScriptClient(Owner, MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
