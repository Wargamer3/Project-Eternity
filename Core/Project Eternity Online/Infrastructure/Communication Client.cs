using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    class CommunicationClient
    {
        public IOnlineConnection Host;
        public IOnlineConnection GroupHost;
        public List<IOnlineConnection> ListPrivateMessageGroup;

        public void JoinGroup(IOnlineConnection GroupHost)
        {

        }

        public void CreateGroup()
        {

        }

        public void MessageOtherClient()
        {
            //Send message to client
            //Create a groupe to allow direct messaging.
            CreateGroup();
            //Send group invite to other client
        }
    }
}
