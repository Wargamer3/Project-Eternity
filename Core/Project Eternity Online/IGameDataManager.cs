using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public interface IGameDataManager
    {
        IRoomInformations GenerateNewRoom(string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int NewOwnerServerPort, byte MinNumberOfPlayer, byte MaxNumberOfPlayer);
        List<IRoomInformations> GetAllRoomUpdatesSinceLastTimeChecked(string ServerVersion);
        void HandleOldData(string ServerIP, int ServerPort);
        void RemoveRoom(string RoomID);
        IRoomInformations TransferRoom(string RoomID, string NewOwnerServerIP);
        void UpdatePlayerCountInRoom(string RoomID, byte CurrentPlayerCount);
        PlayerPOCO LogInPlayer(string Login, string Password, string GameServerIP, int GameServerPort);
        void RemovePlayer(IOnlineConnection PlayerToRemove);
    }
}