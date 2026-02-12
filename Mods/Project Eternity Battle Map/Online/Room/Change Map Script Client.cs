using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class ChangeMapScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Map";

        private readonly RoomInformations Owner;
        private readonly GamePreparationScreen MissionSelectScreen;

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
        }

        protected override void Read(OnlineReader Sender)
        {
            MissionSelectScreen.ReadSelectedMap(Sender);
        }
    }
}
