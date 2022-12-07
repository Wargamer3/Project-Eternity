using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetRoomInformations : RoomInformations
    {
        public int MaxKill;
        public int MaxGameLengthInMinutes;

        public SorcererStreetRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, MinNumberOfPlayer, MaxNumberOfPlayer, 1)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
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

                        NewPlayer.Inventory.CharacterModelPath = BR.ReadString();

                        NewPlayer.LoadGamePieceModel();

                        CardBook NewActiveBook = new CardBook(BR.ReadString());
                        NewActiveBook.BookModel = BR.ReadString();
                        NewPlayer.Inventory.ActiveBook = NewActiveBook;

                        int ListCardCount = BR.ReadInt32();
                        for (int C = 0; C < ListCardCount; ++C)
                        {
                            Card LoadedCard = Card.LoadCard(BR.ReadString(), GameScreen.ContentFallback, SorcererStreetBattleParams.DicParams[""].DicRequirement, SorcererStreetBattleParams.DicParams[""].DicEffect, SorcererStreetBattleParams.DicParams[""].DicAutomaticSkillTarget);
                            LoadedCard.QuantityOwned = BR.ReadInt32();
                            NewActiveBook.AddCard(LoadedCard);
                        }
                        ListRoomPlayer.Add(NewPlayer);
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

        public override void AddOnlinePlayer(IOnlineConnection NewPlayer, string PlayerType)
        {
            ListOnlinePlayer.Add(NewPlayer);
            ListUniqueOnlineConnection.Add(NewPlayer);
            OnlinePlayerBase NewRoomPlayer = new Player(NewPlayer.ID, NewPlayer.Name, PlayerType, true, 0, true, Color.Blue, new List<Card>());
            NewRoomPlayer.OnlineClient = NewPlayer;
            NewRoomPlayer.GameplayType = GameplayTypes.None;
            ListRoomPlayer.Add(NewRoomPlayer);
            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
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
                        BW.Write(ActivePlayer.Team);
                        BW.Write(ActivePlayer.IsPlayerControlled);
                        BW.Write(ActivePlayer.Color.R);
                        BW.Write(ActivePlayer.Color.G);
                        BW.Write(ActivePlayer.Color.B);

                        BW.Write(ActivePlayer.Inventory.CharacterModelPath);

                        BW.Write(ActivePlayer.Inventory.ActiveBook.BookName);
                        BW.Write(ActivePlayer.Inventory.ActiveBook.BookModel);

                        BW.Write(ActivePlayer.Inventory.ActiveBook.ListCard.Count);
                        foreach (Card ActiveCard in ActivePlayer.Inventory.ActiveBook.ListCard)
                        {
                            BW.Write(ActiveCard.CardType + "/" + ActiveCard.Path);
                            BW.Write(ActiveCard.QuantityOwned);
                        }
                    }

                    return MS.ToArray();
                }
            }
        }
    }
}
