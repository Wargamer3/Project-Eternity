using System;

namespace ProjectEternity.Core.Online
{
    public class MasterAddedScriptServer : OnlineScript
    {
        private readonly string NewMasterIP;

        public MasterAddedScriptServer(string NewMasterIP)
            : base("Master Added")
        {
            this.NewMasterIP = NewMasterIP;
        }

        public override OnlineScript Copy()
        {
            return new MasterAddedScriptServer(NewMasterIP);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(NewMasterIP);
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
