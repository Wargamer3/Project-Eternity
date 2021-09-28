using System;

namespace ProjectEternity.Core.Online
{
    public class ReceiveRemoteGroupInviteScriptServer : OnlineScript
    {
        private readonly string GroupID;
        private readonly string GroupName;
        private readonly string ClientID;
        private readonly string ClientName;
        private readonly string ClientToInviteID;
        private readonly string CommunicationServerIP;
        private readonly int CommunicationServerPort;

        public ReceiveRemoteGroupInviteScriptServer(string GroupID, string GroupName, string ClientID, string ClientName, string ClientToInviteID, string CommunicationServerIP, int CommunicationServerPort)
            : base("Receive Remote Group Invite")
        {
            this.GroupID = GroupID;
            this.GroupName = GroupName;
            this.ClientID = ClientID;
            this.ClientName = ClientName;
            this.ClientToInviteID = ClientToInviteID;
            this.CommunicationServerIP = CommunicationServerIP;
            this.CommunicationServerPort = CommunicationServerPort;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
            WriteBuffer.AppendString(GroupName);
            WriteBuffer.AppendString(ClientID);
            WriteBuffer.AppendString(ClientName);
            WriteBuffer.AppendString(ClientToInviteID);
            WriteBuffer.AppendString(CommunicationServerIP);
            WriteBuffer.AppendInt32(CommunicationServerPort);
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
