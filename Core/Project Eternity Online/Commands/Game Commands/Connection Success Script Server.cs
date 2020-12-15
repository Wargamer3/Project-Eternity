using System;

namespace ProjectEternity.Core.Online
{
    public class ConnectionSuccessScriptServer : OnlineScript
    {
        public const string ScriptName = "Connection Success";

        public ConnectionSuccessScriptServer()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
