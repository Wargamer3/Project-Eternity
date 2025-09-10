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

        private string MapName;
        private string MapType;
        private string MapPath;
        private string GameMode;
        private Point MapSize;
        private string MapDescription;
        private byte MinNumberOfPlayer;
        private byte MaxNumberOfPlayer;
        private byte MaxSquadPerPlayer;
        private GameModeInfo GameInfo;
        private List<string> ListMandatoryMutator;
        private List<Color> ListMapTeam;

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
            MissionSelectScreen.UpdateSelectedMap(MapName, MapType, MapPath, GameMode, MapSize, MapDescription, MinNumberOfPlayer, MaxNumberOfPlayer, MaxSquadPerPlayer, GameInfo, ListMandatoryMutator, ListMapTeam);
        }

        protected override void Read(OnlineReader Sender)
        {
            MapName = Sender.ReadString();
            MapType = Sender.ReadString();
            MapPath = Sender.ReadString();
            GameMode = Sender.ReadString();
            MapSize = new Point(Sender.ReadByte(), Sender.ReadByte());
            MapDescription = Sender.ReadString();
            MinNumberOfPlayer = Sender.ReadByte();
            MaxNumberOfPlayer = Sender.ReadByte();
            MaxSquadPerPlayer = Sender.ReadByte();

            string GameInfoName = Sender.ReadString();
            GameInfo = BattleMap.DicBattmeMapType[MapType].GetAvailableGameModes()[GameInfoName].Copy();
            GameInfo.Read(Sender);

            int ListMandatoryMutatorCount = Sender.ReadInt32();
            ListMandatoryMutator = new List<string>(ListMandatoryMutatorCount);
            for (int M = 0; M < ListMandatoryMutatorCount; M++)
            {
                ListMandatoryMutator.Add(Sender.ReadString());
            }

            int ListMapTeamCount = Sender.ReadInt32();
            ListMapTeam = new List<Color>(ListMapTeamCount);
            for (int M = 0; M < ListMapTeamCount; M++)
            {
                ListMapTeam.Add(Color.FromNonPremultiplied(Sender.ReadByte(), Sender.ReadByte(), Sender.ReadByte(), 255));
            }
        }
    }
}
