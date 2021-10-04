using System;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ServerRoomInformations : RoomInformations
    {
        public ServerRoomInformations(string RoomID, bool IsDead)
             : base(RoomID, IsDead)
        {
        }

        public ServerRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, bool IsPlaying, int MaxPlayer, int CurrentClientCount)
            : base(RoomID, RoomName, RoomType, RoomSubtype, IsPlaying, MaxPlayer, CurrentClientCount)
        {
        }

        public ServerRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, bool IsPlaying, string Password, string OwnerServerIP, int OwnerServerPort, int CurrentPlayerCount, int MaxNumberOfPlayer, bool IsDead)
            : base(RoomID, RoomName, RoomType, RoomSubtype, IsPlaying, Password, OwnerServerIP, OwnerServerPort, CurrentPlayerCount, MaxNumberOfPlayer, IsDead)
        {
        }
    }
}
