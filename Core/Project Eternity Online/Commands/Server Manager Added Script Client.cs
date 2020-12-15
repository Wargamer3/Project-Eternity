using System;

namespace ProjectEternity.Core.Online
{
    public class ServerManagerAddedScriptClient : OnlineScript
    {
        public const string ScriptName = "Server Manager Added";

        private readonly Master Owner;
        private string NewServerManagerIP;

        public ServerManagerAddedScriptClient(Master Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new MasterAddedScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
            Owner.ListServerManager.Add(new Master.ServerManagerInfo(NewServerManagerIP));
        }

        protected internal override void Read(OnlineReader Sender)
        {
            NewServerManagerIP = Sender.ReadString();
        }
    }
}
