﻿using System;
using System.IO;
using System.Collections.Generic;

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
        public PVPRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListLocalPlayerID, byte[] RoomData)
            : base(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapName, ListLocalPlayerID)
        {
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
                        BW.Write(ListRoomPlayer[P].PlayerType);
                        BW.Write(ListRoomPlayer[P].Team);
                    }

                    return MS.ToArray();
                }
            }
        }
    }
}
