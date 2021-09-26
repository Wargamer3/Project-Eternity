using System;

namespace ProjectEternity.Core.Online
{
    public class CreateCommunicationGroupScriptClient : OnlineScript
    {
        private readonly string GroupID;

        public CreateCommunicationGroupScriptClient(string GroupID)
            : base("Create Communication Group")
        {
            this.GroupID = GroupID;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
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
