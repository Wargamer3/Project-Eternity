using System;
using System.Net;

namespace ProjectEternity.Core.Online
{
    public class ReceiveRemoteGroupInviteScriptClient : OnlineScript
    {
        public const string ScriptName = "Receive Remote Group Invite";

        private readonly CommunicationClient OnlineCommunicationClient;

        private string GroupID;
        private string GroupName;
        private string ClientID;
        private string ClientName;
        private string ClientToInviteID;
        private string CommunicationServerIP;
        private int CommunicationServerPort;

        public ReceiveRemoteGroupInviteScriptClient(CommunicationClient OnlineCommunicationClient)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveRemoteGroupInviteScriptClient(OnlineCommunicationClient);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            CommunicationClient CrossServerClient = new CommunicationClient(OnlineCommunicationClient.DicOnlineScripts);

            CrossServerClient.ChangeHost(IPAddress.Parse(CommunicationServerIP), CommunicationServerPort);

            OnlineCommunicationClient.DicCrossServerCommunicationByGroupID.Add(GroupID, CrossServerClient);
            CrossServerClient.Host.Send(new IdentifyScriptClient(ClientID, ClientName, true, new byte[0]));
            CrossServerClient.Host.Send(new SendGroupInviteScriptClient(GroupID, GroupName, ClientToInviteID));
            OnlineCommunicationClient.Chat.AddMessage(GroupID, "Connected to Server: " + CommunicationServerIP + ":" + CommunicationServerPort, ChatManager.MessageColors.White);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
            GroupName = Sender.ReadString();
            ClientID = Sender.ReadString();
            ClientName = Sender.ReadString();
            ClientToInviteID = Sender.ReadString();
            CommunicationServerIP = Sender.ReadString();
            CommunicationServerPort = Sender.ReadInt32();
        }
    }
}
