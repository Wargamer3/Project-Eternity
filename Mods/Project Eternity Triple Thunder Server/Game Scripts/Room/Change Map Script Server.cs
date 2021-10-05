using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class ChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Map";

        private readonly string CurrentDifficulty;
        private readonly string MissionPath;

        public ChangeMapScriptServer(string CurrentDifficulty, string MissionPath)
            : base(ScriptName)
        {
            this.CurrentDifficulty = CurrentDifficulty;
            this.MissionPath = MissionPath;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(CurrentDifficulty);
            WriteBuffer.AppendString(MissionPath);
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
