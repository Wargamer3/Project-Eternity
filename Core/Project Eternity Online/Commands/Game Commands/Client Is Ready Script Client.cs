using System;

namespace ProjectEternity.Core.Online
{
    public class ClientIsReadyScriptClient : OnlineScript
    {
        public const string ScriptName = "Client Is Ready";

        public ClientIsReadyScriptClient()
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
