using System;

namespace ProjectEternity.Core.Online
{
    public class SendGroupInviteScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Group Invite";

        private readonly CommunicationServer OnlineServer;

        private string GroupID;
        private string GroupName;
        private string ClientToInviteID;

        public SendGroupInviteScriptServer(CommunicationServer OnlineServer)
            : base(ScriptName)
        {
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new SendGroupInviteScriptServer(OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            IOnlineConnection ClientToInvite;
            if (OnlineServer.DicPlayerByID.TryGetValue(ClientToInviteID, out ClientToInvite))
            {
                if (!OnlineServer.DicCommunicationGroup.ContainsKey(GroupID))
                {
                    OnlineServer.CreateOrJoinCommunicationGroup(GroupID, true, Sender);
                }

                OnlineServer.JoinCommunicationGroup(GroupID, ClientToInvite);

                ClientToInvite.Send(new ReceiveGroupInviteScriptServer(GroupID, GroupName));
            }
            else
            {
                //cross server
                string CommunicationServerIP;
                int CommunicationServerPort;
                OnlineServer.Database.GetPlayerCommunicationIP(ClientToInviteID, out CommunicationServerIP, out CommunicationServerPort);

                if (OnlineServer.IP != CommunicationServerIP || OnlineServer.Port != CommunicationServerPort)
                {
                    Sender.Send(new ReceiveRemoteGroupInviteScriptServer(GroupID, GroupName, Sender.ID, Sender.Name, ClientToInviteID, CommunicationServerIP, CommunicationServerPort));
                }
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
            GroupName = Sender.ReadString();
            ClientToInviteID = Sender.ReadString();
        }
    }
}
