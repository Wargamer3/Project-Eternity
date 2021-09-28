using System;

namespace ProjectEternity.Core.Online
{
    public class LeaveCommunicationGroupScriptServer : OnlineScript
    {
        public const string ScriptName = "Leave Communication Group";

        private readonly CommunicationServer OnlineCommunicationServer;

        private string GroupID;

        public LeaveCommunicationGroupScriptServer(CommunicationServer OnlineCommunicationServer)
            : base(ScriptName)
        {
            this.OnlineCommunicationServer = OnlineCommunicationServer;
        }

        public override OnlineScript Copy()
        {
            return new LeaveCommunicationGroupScriptServer(OnlineCommunicationServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            OnlineCommunicationServer.LeaveCommunicationGroup(GroupID, Sender);
        }

        protected internal override void Read(OnlineReader Sender)
        {
            GroupID = Sender.ReadString();
        }
    }
}
