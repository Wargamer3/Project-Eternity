using System;

namespace ProjectEternity.Core.Online
{
    public class ServerManagerAddedScriptServer : OnlineScript
    {
        private readonly string NewServerManagerIP;

        public ServerManagerAddedScriptServer(string NewServerManagerIP)
            : base("Server Manager Added")
        {
            this.NewServerManagerIP = NewServerManagerIP;
        }

        public override OnlineScript Copy()
        {
            return new MasterAddedScriptServer(NewServerManagerIP);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(NewServerManagerIP);
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
