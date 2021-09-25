using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public interface IDataManager
    {
        IRoomInformations GenerateNewRoom(string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int NewOwnerServerPort, int MaxNumberOfPlayer);
        List<IRoomInformations> GetAllRoomUpdatesSinceLastTimeChecked(string ServerVersion);
        void HandleOldData(string ServerIP, int ServerPort);
        void RemoveRoom(string RoomID);
        IRoomInformations TransferRoom(string RoomID, string NewOwnerServerIP);
        void UpdatePlayerCountInRoom(string RoomID, int CurrentPlayerCount);
        PlayerPOCO LogInPlayer(string Login, string Password, string OwnerServerIP, int OwnerServerPort);
        void RemovePlayer(IOnlineConnection onlineConnection);
    }
}