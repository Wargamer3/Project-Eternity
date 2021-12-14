using System;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ServerRoomInformations : RoomInformations
    {
        public ServerRoomInformations(string RoomID, bool IsDead)
             : base(RoomID, IsDead)
        {
        }

        public ServerRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, bool IsPlaying, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, byte CurrentPlayerCount)
            : base(RoomID, RoomName, RoomType, RoomSubtype, IsPlaying, MinNumberOfPlayer, MaxNumberOfPlayer, CurrentPlayerCount)
        {
        }

        public ServerRoomInformations(string RoomID, string RoomName, string RoomType, string RoomSubtype, bool IsPlaying, string Password, string OwnerServerIP, int OwnerServerPort,
            byte CurrentPlayerCount, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, bool IsDead)
            : base(RoomID, RoomName, RoomType, RoomSubtype, IsPlaying, Password, OwnerServerIP, OwnerServerPort, CurrentPlayerCount, MinNumberOfPlayer, MaxNumberOfPlayer, IsDead)
        {
        }
    }
}
