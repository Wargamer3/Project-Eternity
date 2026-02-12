using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Online;
using static ProjectEternity.GameScreens.BattleMapScreen.GameOptionsSelectMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetMapInfo : MapInfo
    {
        public string Forts;
        public int MagicAtStart;
        public int MagicGainPerLap;
        public int TowerMagicGain;
        public int MagicGoal;
        public int LowestDieRoll;
        public int HighestDieRoll;
        public int NumberOfArenas;

        public SorcererStreetMapInfo()
            : base()
        {
        }

        protected SorcererStreetMapInfo(string MapName, string MapModName, string MapPath)
            : base(MapName, MapModName, MapPath)
        {
        }

        public override void ReadInfo(BinaryReader BR, string GameMode)
        {
            base.ReadInfo(BR, GameMode);

            int TileSizeX = BR.ReadInt32();
            int TileSizeY = BR.ReadInt32();

            string CameraType = BR.ReadString();

            int Camera2DPositionStartX = BR.ReadInt32();
            int Camera2DPositionStartY = BR.ReadInt32();

            int ListBackgroundsPathCount = BR.ReadInt32();
            for (int B = 0; B < ListBackgroundsPathCount; B++)
            {
                BR.ReadString();
            }
            int ListForegroundsPathCount = BR.ReadInt32();
            for (int F = 0; F < ListForegroundsPathCount; F++)
            {
                BR.ReadString();
            }

            MagicAtStart = BR.ReadInt32();
            MagicGainPerLap = BR.ReadInt32();
            TowerMagicGain = BR.ReadInt32();
            MagicGoal = BR.ReadInt32();
            LowestDieRoll = BR.ReadByte();
            HighestDieRoll = BR.ReadByte();
        }

        public override MapInfo Clone(string MapName, string MapModName, string MapPath)
        {
            return new SorcererStreetMapInfo(MapName, MapModName, MapPath);
        }
    }

    public class SorcererStreetRoomInformations : RoomInformations
    {
        public string Forts;
        public int MagicAtStart;
        public int MagicGainPerLap;
        public int TowerMagicGain;
        public int MagicGoal;
        public int LowestDieRoll;
        public int HighestDieRoll;
        public int NumberOfArenas;

        public int MaxKill;
        public int MaxGameLengthInMinutes;

        public SorcererStreetRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, MinNumberOfPlayer, MaxNumberOfPlayer, 1)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
            Forts = string.Empty;
        }

        public SorcererStreetRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListLocalPlayerID, ContentManager Content, byte[] RoomData)
            : base(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapName, ListLocalPlayerID)
        {
            using (MemoryStream MS = new MemoryStream(RoomData))
            {
                using (BinaryReader BR = new BinaryReader(MS))
                {
                    MaxKill = BR.ReadInt32();
                    MaxGameLengthInMinutes = BR.ReadInt32();

                    int NumberOfPlayers = BR.ReadInt32();
                    for (int P = 0; P < NumberOfPlayers; ++P)
                    {
                        Player NewPlayer = new Player(BR.ReadString(), BR.ReadString(), BR.ReadString(), true, BR.ReadInt32(), BR.ReadBoolean(),
                            Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255), new List<Card>());

                        NewPlayer.Inventory.Character = new PlayerCharacterInfo(new PlayerCharacter(BR.ReadString(), GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget));

                        CardBook NewActiveBook = new CardBook(BR.ReadString());
                        NewActiveBook.BookModel = BR.ReadString();
                        NewPlayer.Inventory.ActiveBook = NewActiveBook;

                        int ListCardCount = BR.ReadInt32();
                        for (int C = 0; C < ListCardCount; ++C)
                        {
                            Card LoadedCard = Card.LoadCard(BR.ReadString(), GameScreen.ContentFallback, SorcererStreetBattleParams.DicParams[""].DicRequirement, SorcererStreetBattleParams.DicParams[""].DicEffect, SorcererStreetBattleParams.DicParams[""].DicAutomaticSkillTarget, SorcererStreetBattleParams.DicParams[""].DicManualSkillTarget);
                            NewActiveBook.AddCard(new CardInfo(LoadedCard, BR.ReadByte()));
                        }
                        int ListRoleCount = BR.ReadInt32();
                        for (int R = 0; R < ListRoleCount; ++R)
                        {
                            NewPlayer.OnlineClient.Roles.AddRole(BR.ReadString());
                        }

                        ListRoomPlayer.Add(NewPlayer);
                        ListOnlinePlayer.Add(NewPlayer.OnlineClient);
                    }
                }
            }
        }

        public SorcererStreetRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort,
            byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, Password, OwnerServerIP, OwnerServerPort, 1, MinNumberOfPlayer, MaxNumberOfPlayer, false)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
        }

        public override void ReadFromMapInfo(MapInfo SelectedMapInfo)
        {
            base.ReadFromMapInfo(SelectedMapInfo);

            SorcererStreetMapInfo SelectedSorcererStreetMapInfo = (SorcererStreetMapInfo)SelectedMapInfo;

            Forts = SelectedSorcererStreetMapInfo.Forts;
            MagicAtStart = SelectedSorcererStreetMapInfo.MagicAtStart;
            MagicGainPerLap = SelectedSorcererStreetMapInfo.MagicGainPerLap;
            TowerMagicGain = SelectedSorcererStreetMapInfo.TowerMagicGain;
            MagicGoal = SelectedSorcererStreetMapInfo.MagicGoal;
            LowestDieRoll = SelectedSorcererStreetMapInfo.LowestDieRoll;
            HighestDieRoll = SelectedSorcererStreetMapInfo.HighestDieRoll;
            NumberOfArenas = SelectedSorcererStreetMapInfo.NumberOfArenas;
        }

        public override void WriteSelectedMap(OnlineWriter WriteBuffer)
        {
            base.WriteSelectedMap(WriteBuffer);

            WriteBuffer.AppendString(Forts);
        }

        public override void ReadSelectedMap(OnlineReader Sender)
        {
            base.ReadSelectedMap(Sender);

            Forts = Sender.ReadString();
        }

        public override byte[] GetRoomInfo()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                using (BinaryWriter BW = new BinaryWriter(MS))
                {
                    BW.Write(MaxKill);
                    BW.Write(MaxGameLengthInMinutes);

                    BW.Write(ListRoomPlayer.Count);
                    foreach (Player ActivePlayer in ListRoomPlayer)
                    {
                        BW.Write(ActivePlayer.ConnectionID);
                        BW.Write(ActivePlayer.Name);
                        BW.Write(ActivePlayer.OnlinePlayerType);
                        BW.Write(ActivePlayer.TeamIndex);
                        BW.Write(ActivePlayer.IsPlayerControlled);
                        BW.Write(ActivePlayer.Color.R);
                        BW.Write(ActivePlayer.Color.G);
                        BW.Write(ActivePlayer.Color.B);

                        BW.Write(ActivePlayer.Inventory.Character.Character.CharacterPath);

                        BW.Write(ActivePlayer.Inventory.ActiveBook.BookName);
                        BW.Write(ActivePlayer.Inventory.ActiveBook.BookModel);

                        BW.Write(ActivePlayer.Inventory.ActiveBook.ListCard.Count);
                        foreach (CardInfo ActiveCard in ActivePlayer.Inventory.ActiveBook.ListCard)
                        {
                            BW.Write(ActiveCard.Card.CardType + "/" + ActiveCard.Card.Path);
                            BW.Write(ActiveCard.QuantityOwned);
                        }

                        BW.Write(ActivePlayer.OnlineClient.Roles.ListActiveRole.Count);
                        for (int R = 0; R < ActivePlayer.OnlineClient.Roles.ListActiveRole.Count; ++R)
                        {
                            BW.Write(ActivePlayer.OnlineClient.Roles.ListActiveRole[R]);
                        }
                    }

                    return MS.ToArray();
                }
            }
        }
    }
}
