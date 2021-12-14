using System.Collections.Generic;
using System.IO;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class MissionRoomInformations : RoomInformations
    {
        public MissionRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, MinNumberOfPlayer, MaxNumberOfPlayer, 1)
        {
        }

        public MissionRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string CurrentDifficulty, string MapName, List<string> ListLocalPlayerID, byte[] RoomData)
            : base(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapName, ListLocalPlayerID)
        {
            using (MemoryStream MS = new MemoryStream(RoomData))
            {
                using (BinaryReader BR = new BinaryReader(MS))
                {
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

        public MissionRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort,
            byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(RoomID, RoomName, RoomType, RoomSubtype, false, Password, OwnerServerIP, OwnerServerPort, 1, MinNumberOfPlayer, MaxNumberOfPlayer, false)
        {

        }
    }
}
