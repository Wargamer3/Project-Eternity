using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    class GoToNextMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Go To Next Map";

        private readonly string NextLevelPath;

        public GoToNextMapScriptServer(string NextLevelPath)
            : base(ScriptName)
        {
            this.NextLevelPath = NextLevelPath;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(NextLevelPath);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
