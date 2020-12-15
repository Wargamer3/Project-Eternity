using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.Core.Online
{
    public class AskGameDataScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Game Data";

        public AskGameDataScriptServer()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return null;
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
