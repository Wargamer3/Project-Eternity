using System;

namespace ProjectEternity.Core.Online
{
    public class MasterListScriptClient : OnlineScript
    {
        public const string ScriptName = "Master List";

        private readonly Master Owner;
        private string NewMasterIP;

        public MasterListScriptClient(Master Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new MasterListScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
