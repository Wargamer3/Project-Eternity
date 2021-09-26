using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    /// <summary>
    /// Group with direct connection to each client to avoid cross server communication.
    /// </summary>
    public class CommunicationGroup
    {
        public string GroupID;
        public readonly List<IOnlineConnection> ListGroupMember;

        internal CommunicationGroup()
        {
            ListGroupMember = new List<IOnlineConnection>();
        }

        internal CommunicationGroup(string GroupID, IOnlineConnection GroupCreator)
            : this()
        {
            this.GroupID = GroupID;
            AddMember(GroupCreator);
        }

        internal void AddMember(IOnlineConnection NewMember)
        {
            ListGroupMember.Add(NewMember);
        }

        internal void RemoveOnlinePlayer(int MemberIndex)
        {
            ListGroupMember.RemoveAt(MemberIndex);
        }

        internal bool IsRunningSlow()
        {
            return false;
        }
    }
}
