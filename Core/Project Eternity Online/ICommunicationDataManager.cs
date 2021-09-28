using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public interface ICommunicationDataManager
    {
        byte[] GetClientInfo(string ClientName);
        void AddFriend(IOnlineConnection Sender, string ClientName);
        List<PlayerPOCO> GetFriendList(string ClientID);
    }
}