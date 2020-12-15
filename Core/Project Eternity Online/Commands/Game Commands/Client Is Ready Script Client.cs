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
            return new ClientIsReadyScriptClient();
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
