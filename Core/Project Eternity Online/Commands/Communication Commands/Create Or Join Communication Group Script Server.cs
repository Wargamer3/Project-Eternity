using System;

namespace ProjectEternity.Core.Online
{
    public class CreateOrJoinCommunicationGroupScriptServer : OnlineScript
    {
        public const string ScriptName = "Create Communication Group";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string GroupID;
        private bool SaveLogs;

        public CreateOrJoinCommunicationGroupScriptServer(CommunicationServer OnlineCommunicationServer)
            : base(ScriptName)
        {
            this.OnlineCommunicationServer = OnlineCommunicationServer;
        }

        public override OnlineScript Copy()
        {
            return new CreateOrJoinCommunicationGroupScriptServer(OnlineCommunicationServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationServer.CreateOrJoinCommunicationGroup(GroupID, SaveLogs, Sender);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
            SaveLogs = Sender.ReadBoolean();
        }
    }
}
