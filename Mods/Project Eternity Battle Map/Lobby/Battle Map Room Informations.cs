using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapRoomInformations : RoomInformations
    {
        public int MaxKill;
        public int MaxGameLengthInMinutes;

        public BattleMapRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, MinNumberOfPlayer, MaxNumberOfPlayer, 1)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
        }

        public BattleMapRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListJoiningLocalPlayer, ContentManager Content, byte[] RoomData)
            : base(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapName, ListJoiningLocalPlayer)
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
                        BattleMapPlayer NewPlayer = new BattleMapPlayer(BR.ReadString(), BR.ReadString(), BR.ReadString(), true, BR.ReadInt32(), BR.ReadBoolean(),
                            Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255));
                        Unit NewUnit = Unit.FromFullName("Normal/Original/Voltaire", Content, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                        Character NewCharacter = new Character("Original/Greg", Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                        NewCharacter.Level = 1;
                        NewUnit.ArrayCharacterActive = new Character[] { NewCharacter };

                        Squad NewSquad = new Squad("Squad", NewUnit);

                        NewPlayer.Inventory.ActiveLoadout.ListSpawnSquad.Add(NewSquad);

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

        public BattleMapRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort,
            byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, Password, OwnerServerIP, OwnerServerPort, 1, MinNumberOfPlayer, MaxNumberOfPlayer, false)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
        }

        public override void AddOnlinePlayerServer(IOnlineConnection NewPlayer, string PlayerType)
        {
            OnlinePlayerBase NewRoomPlayer = new BattleMapPlayer(NewPlayer.ID, NewPlayer.Name, PlayerType, true, 0, true, Color.Blue);
            NewRoomPlayer.OnlineClient = NewPlayer;
            NewRoomPlayer.GameplayType = GameplayTypes.None;
            ListRoomPlayer.Add(NewRoomPlayer);
            ListOnlinePlayer.Add(NewPlayer);
            ListUniqueOnlineConnection.Add(NewPlayer);
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
                    foreach (OnlinePlayerBase ActivePlayer in ListRoomPlayer)
                    {
                        BW.Write(ActivePlayer.ConnectionID);
                        BW.Write(ActivePlayer.Name);
                        BW.Write(ActivePlayer.OnlinePlayerType);
                        BW.Write(ActivePlayer.Team);
                        BW.Write(ActivePlayer.IsPlayerControlled);
                        BW.Write(ActivePlayer.Color.R);
                        BW.Write(ActivePlayer.Color.G);
                        BW.Write(ActivePlayer.Color.B);

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
