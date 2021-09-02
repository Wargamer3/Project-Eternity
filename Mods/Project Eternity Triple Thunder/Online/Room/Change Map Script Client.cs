﻿using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ChangeMapScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Map";

        private readonly RoomInformations Owner;
        private readonly IMissionSelect MissionSelectScreen;

        private string CurrentDifficulty;
        private string MissionPath;

        public ChangeMapScriptClient(RoomInformations Owner, IMissionSelect MissionSelectScreen)
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
            MissionSelectScreen.UpdateSelectedMap(CurrentDifficulty, MissionPath);
        }

        protected override void Read(OnlineReader Sender)
        {
            CurrentDifficulty = Sender.ReadString();
            MissionPath = Sender.ReadString();
        }
    }
}
