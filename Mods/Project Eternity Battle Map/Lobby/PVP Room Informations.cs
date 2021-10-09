using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PVPRoomInformations : RoomInformations
    {
        public int MaxKill;
        public int MaxGameLengthInMinutes;

        public PVPRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, int MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, MaxNumberOfPlayer, 1)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
        }

        public PVPRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListLocalPlayerID, ContentManager Content, byte[] RoomData)
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
                        BattleMapPlayer NewPlayer = new BattleMapPlayer(BR.ReadString(), BR.ReadString(), BR.ReadString(), true, BR.ReadInt32(), BR.ReadBoolean(),
                            Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255));
                        Unit NewUnit = Unit.FromFullName("Normal/Original/Voltaire", Content, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                        Character NewCharacter = new Character("Original/Greg", Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                        NewCharacter.Level = 1;
                        NewUnit.ArrayCharacterActive = new Character[] { NewCharacter };

                        Squad NewSquad = new Squad("Squad", NewUnit);

                        NewPlayer.ListSquadToSpawn.Add(NewSquad);

                        ListRoomPlayer.Add(NewPlayer);
                    }
                }
            }
        }

        public PVPRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort, int MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, Password, OwnerServerIP, OwnerServerPort, 1, MaxNumberOfPlayer, false)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
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
                    for (int P = 0; P < ListRoomPlayer.Count; P++)
                    {
                        BW.Write(ListRoomPlayer[P].ConnectionID);
                        BW.Write(ListRoomPlayer[P].Name);
                        BW.Write(ListRoomPlayer[P].OnlinePlayerType);
                        BW.Write(ListRoomPlayer[P].Team);
                        BW.Write(ListRoomPlayer[P].IsPlayerControlled);
                        BW.Write(ListRoomPlayer[P].Color.R);
                        BW.Write(ListRoomPlayer[P].Color.G);
                        BW.Write(ListRoomPlayer[P].Color.B);
                    }

                    return MS.ToArray();
                }
            }
        }
    }
}
