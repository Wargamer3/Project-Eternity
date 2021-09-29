using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public interface ICommunicationDataManager
    {
        void UpdatePlayerCommunicationIP(string ClientID, string CommunicationServerIP, int CommunicationServerPort);
        void RemovePlayer(IOnlineConnection PlayerToRemove);
        void GetPlayerCommunicationIP(string ClientID, out string CommunicationServerIP, out int CommunicationServerPort);
        byte[] GetClientInfo(string ClientName);
        void AddFriend(IOnlineConnection Sender, string ClientName);
        List<PlayerPOCO> GetFriendList(string ClientID);
        void SaveGroupMessage(DateTime UtcNow, string GroupID, string Message, byte MessageColor);
        Dictionary<string, ChatManager.MessageColors> GetGroupMessages(string GroupID);
    }
}