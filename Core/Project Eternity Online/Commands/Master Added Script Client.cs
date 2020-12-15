using System;

namespace ProjectEternity.Core.Online
{
    public class MasterAddedScriptClient : OnlineScript
    {
        public const string ScriptName = "Master Added";

        private readonly Master Owner;
        private string NewMasterIP;

        public MasterAddedScriptClient(Master Owner)
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
        }

        protected internal override void Read(OnlineReader Sender)
        {
            NewMasterIP = Sender.ReadString();
        }
    }
}
