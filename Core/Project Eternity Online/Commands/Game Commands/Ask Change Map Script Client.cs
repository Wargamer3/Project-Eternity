using System;

namespace ProjectEternity.Core.Online
{
    public class AskChangeMapScriptClient : OnlineScript
    {
        private readonly string CurrentDifficulty;
        private readonly string NewMapName;

        public AskChangeMapScriptClient(string CurrentDifficulty, string NewMapName)
            : base("Ask Change Map")
        {
            this.CurrentDifficulty = CurrentDifficulty;
            this.NewMapName = NewMapName;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(CurrentDifficulty);
            WriteBuffer.AppendString(NewMapName);
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
