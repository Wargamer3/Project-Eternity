using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public interface IRoomInformations
    {
        string RoomID { get; }//Only contains a value for locally create Rooms, if the Room is on another server the ID should be null.
        string RoomName { get; }
        string RoomType { get; }
        string RoomSubtype { get; set; }
        bool IsPlaying { get; set; }
        string Password { get; }
        string MapPath { get; set; }
        int CurrentPlayerCount { get; }
        int MaxNumberOfPlayer { get; }
        string OwnerServerIP { get; }
        int OwnerServerPort { get; }
        bool IsDead { get; }//Used when the DataManager need to tell that a Room is deleted.

        List<IOnlineConnection> ListOnlinePlayer { get; }
        void AddOnlinePlayer(IOnlineConnection NewPlayer, string PlayerType);
        void RemovePlayer(IOnlineConnection OnlinePlayerToRemove);
        void RemoveOnlinePlayer(int Index);
        byte[] GetRoomInfo();
    }
}