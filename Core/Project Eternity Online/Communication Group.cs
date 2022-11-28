using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    /// <summary>
    /// Group with direct connection to each client to avoid cross server communication.
    /// </summary>
    public class CommunicationGroup
    {
        public readonly string GroupID;
        public readonly List<IOnlineConnection> ListGroupMember;
        public readonly bool SaveLogs;

        internal CommunicationGroup(bool SaveLogs)
        {
            this.SaveLogs = SaveLogs;

            ListGroupMember = new List<IOnlineConnection>();
        }

        internal CommunicationGroup(string GroupID, bool SaveLogs, IOnlineConnection GroupCreator)
            : this(SaveLogs)
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

        internal void RemoveOnlinePlayer(IOnlineConnection MemberToRemove)
        {
            ListGroupMember.Remove(MemberToRemove);
        }

        internal bool IsRunningSlow()
        {
            return false;
        }
    }
}
