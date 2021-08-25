using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ChangeRoomSubtypeScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Room Subtype";

        private readonly IMissionSelect MissionSelectScreen;

        private string RoomSubtype;

        public ChangeRoomSubtypeScriptClient(IMissionSelect MissionSelectScreen)
            : base(ScriptName)
        {
            this.MissionSelectScreen = MissionSelectScreen;
        }

        public override OnlineScript Copy()
        {
            return new ChangeRoomSubtypeScriptClient(MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            MissionSelectScreen.UpdateRoomSubtype(RoomSubtype);
        }

        protected override void Read(OnlineReader Sender)
        {
            RoomSubtype = Sender.ReadString();
        }
    }
}
