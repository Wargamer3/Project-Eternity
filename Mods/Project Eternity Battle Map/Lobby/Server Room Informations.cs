using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
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

        public override void AddOnlinePlayerServer(IOnlineConnection NewPlayer, string PlayerType)
        {
            ListOnlinePlayer.Add(NewPlayer);
            ListUniqueOnlineConnection.Add(NewPlayer);
            OnlinePlayerBase NewRoomPlayer = new BattleMapPlayer(NewPlayer.ID, NewPlayer.Name, PlayerType, true, 0, true, Color.Blue);
            NewRoomPlayer.OnlineClient = NewPlayer;
            NewRoomPlayer.GameplayType = GameplayTypes.None;
            ListRoomPlayer.Add(NewRoomPlayer);
            CurrentPlayerCount = (byte)ListRoomPlayer.Count;
        }
    }
}
