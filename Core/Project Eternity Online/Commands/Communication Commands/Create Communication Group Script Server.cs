using System;

namespace ProjectEternity.Core.Online
{
    public class CreateCommunicationGroupScriptServer : OnlineScript
    {
        public const string ScriptName = "Create Communication Group";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string GroupID;

        public CreateCommunicationGroupScriptServer(CommunicationServer OnlineCommunicationServer)
            : base(ScriptName)
        {
            this.OnlineCommunicationServer = OnlineCommunicationServer;
        }

        public override OnlineScript Copy()
        {
            return new CreateCommunicationGroupScriptServer(OnlineCommunicationServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationServer.CreateCommunicationGroup(GroupID, Sender);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
        }
    }
}
