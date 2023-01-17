using System;
using System.Collections.Generic;
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
        private string MapPath;
        private string GameMode;
        private byte MinNumberOfPlayer;
        private byte MaxNumberOfPlayer;
        private List<string> ListMandatoryMutator;

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
            MissionSelectScreen.UpdateSelectedMap(MapName, MapType, MapPath, GameMode, MinNumberOfPlayer, MaxNumberOfPlayer, ListMandatoryMutator);
        }

        protected override void Read(OnlineReader Sender)
        {
            MapName = Sender.ReadString();
            MapType = Sender.ReadString();
            MapPath = Sender.ReadString();
            GameMode = Sender.ReadString();
            MinNumberOfPlayer = Sender.ReadByte();
            MaxNumberOfPlayer = Sender.ReadByte();

            int ListMandatoryMutatorCount = Sender.ReadInt32();
            ListMandatoryMutator = new List<string>(ListMandatoryMutatorCount);
            for (int M = 0; M < ListMandatoryMutatorCount; M++)
            {
                ListMandatoryMutator.Add(Sender.ReadString());
            }
        }
    }
}
