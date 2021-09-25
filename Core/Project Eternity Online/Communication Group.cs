using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    /// <summary>
    /// Group with direct connection to each client to avoid cross server communication.
    /// </summary>
    public class CommunicationGroup
    {
        public readonly List<IOnlineConnection> ListGroupMember;

        public CommunicationGroup()
        {
            ListGroupMember = new List<IOnlineConnection>();
        }

        public void AddMember(IOnlineConnection NewMember)
        {
            ListGroupMember.Add(NewMember);
        }
    }
}
