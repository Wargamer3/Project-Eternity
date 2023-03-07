using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class CheckNewUnlocksScriptClient : OnlineScript
    {
        public const string ScriptName = "Check New Unlocks";

        private readonly string ID;

        public CheckNewUnlocksScriptClient(string ID)
            : base(ScriptName)
        {
            this.ID = ID;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ID);
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
