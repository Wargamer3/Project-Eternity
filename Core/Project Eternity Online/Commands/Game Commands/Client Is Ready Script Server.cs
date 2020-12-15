using System;

namespace ProjectEternity.Core.Online
{
    public class ClientIsReadyScriptServer : OnlineScript
    {
        public const string ScriptName = "Client Is Ready";

        public ClientIsReadyScriptServer()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return new ClientIsReadyScriptServer();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            ActivePlayer.IsGameReady = true;
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
