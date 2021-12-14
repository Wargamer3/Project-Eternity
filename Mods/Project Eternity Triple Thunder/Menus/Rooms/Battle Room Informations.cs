using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class BattleRoomInformations : RoomInformations
    {
        public int MaxKill;
        public int MaxGameLengthInMinutes;

        public BattleRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, MinNumberOfPlayer, MaxNumberOfPlayer, 1)
        {
            MaxKill = 20;
            MaxGameLengthInMinutes = 10;
        }

        public BattleRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListLocalPlayerID, byte[] RoomData)
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
                        Player NewPlayer = new Player(BR.ReadString(), BR.ReadString(), BR.ReadString(), BR.ReadInt32());
                        NewPlayer.Equipment.CharacterType = BR.ReadString();
                        ListRoomPlayer.Add(NewPlayer);
                    }
                }
            }
        }

        public BattleRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, Password, OwnerServerIP, OwnerServerPort, 1, MinNumberOfPlayer, MaxNumberOfPlayer, false)
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
                        BW.Write(ListRoomPlayer[P].Equipment.CharacterType);
                    }

                    return MS.ToArray();
                }
            }
        }
    }
}
