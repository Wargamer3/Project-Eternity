using System;

namespace ProjectEternity.Core.Online
{
    public class JoinCommunicationGroupScriptServer : OnlineScript
    {
        public const string ScriptName = "Join Communication Group";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string GroupID;

        public JoinCommunicationGroupScriptServer(CommunicationServer OnlineCommunicationServer)
            : base(ScriptName)
        {
            this.OnlineCommunicationServer = OnlineCommunicationServer;
        }

        public override OnlineScript Copy()
        {
            return new JoinCommunicationGroupScriptServer(OnlineCommunicationServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationServer.JoinCommunicationGroup(GroupID, Sender);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
        }
    }
}
