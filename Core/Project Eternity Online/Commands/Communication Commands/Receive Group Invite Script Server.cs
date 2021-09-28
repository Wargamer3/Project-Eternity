using System;

namespace ProjectEternity.Core.Online
{
    public class ReceiveGroupInviteScriptServer : OnlineScript
    {
        private readonly string GroupID;
        private readonly string GroupName;

        public ReceiveGroupInviteScriptServer(string GroupID, string GroupName)
            : base("Receive Group Invite")
        {
            this.GroupID = GroupID;
            this.GroupName = GroupName;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
            WriteBuffer.AppendString(GroupName);
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
