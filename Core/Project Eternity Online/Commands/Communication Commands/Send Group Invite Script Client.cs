using System;

namespace ProjectEternity.Core.Online
{
    public class SendGroupInviteScriptClient : OnlineScript
    {
        private readonly string GroupID;
        private readonly string GroupName;
        private readonly string ClientToInviteID;

        public SendGroupInviteScriptClient(string GroupID, string GroupName, string ClientToInviteID)
            : base("Send Group Invite")
        {
            this.GroupID = GroupID;
            this.GroupName = GroupName;
            this.ClientToInviteID = ClientToInviteID;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
            WriteBuffer.AppendString(GroupName);
            WriteBuffer.AppendString(ClientToInviteID);
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
