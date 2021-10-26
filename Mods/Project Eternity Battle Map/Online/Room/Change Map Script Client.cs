using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class ChangeMapScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Map";

        private readonly RoomInformations Owner;
        private readonly GamePreparationScreen MissionSelectScreen;

        private string MapName;
        private string MapType;
        private string MissionPath;

        public ChangeMapScriptClient(RoomInformations Owner, GamePreparationScreen MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.MissionSelectScreen = MissionSelectScreen;
        }

        public override OnlineScript Copy()
        {
            return new ChangeMapScriptClient(Owner, MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            MissionSelectScreen.UpdateSelectedMap(MapName, MapType, MissionPath);
        }

        protected override void Read(OnlineReader Sender)
        {
            MapName = Sender.ReadString();
            MapType = Sender.ReadString();
            MissionPath = Sender.ReadString();
        }
    }
}
