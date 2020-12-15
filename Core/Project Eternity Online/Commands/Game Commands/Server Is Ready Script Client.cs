using System;

namespace ProjectEternity.Core.Online
{
    public class ServerIsReadyScriptClient : OnlineScript
    {
        public const string ScriptName = "Server Is Ready";

        public ServerIsReadyScriptClient()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return new ServerIsReadyScriptClient();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
            Host.IsGameReady = true;
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
