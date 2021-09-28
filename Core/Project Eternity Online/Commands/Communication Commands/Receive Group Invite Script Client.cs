using System;

namespace ProjectEternity.Core.Online
{
    public class ReceiveGroupInviteScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Receive Group Invite";

        private readonly CommunicationClient OnlineCommunicationClient;

        private string GroupID;
        private string GroupName;

        public ReceiveGroupInviteScriptClient(CommunicationClient OnlineCommunicationClient)
            : base(ScriptName)
        {
            this.OnlineCommunicationClient = OnlineCommunicationClient;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGroupInviteScriptClient(OnlineCommunicationClient);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationClient.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            OnlineCommunicationClient.Chat.OpenTab(GroupID, GroupName);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
            GroupName = Sender.ReadString();
        }
    }
}
