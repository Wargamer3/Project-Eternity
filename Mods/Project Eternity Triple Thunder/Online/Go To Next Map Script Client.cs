using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class GoToNextMapScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Go To Next Map";

        private readonly TripleThunderOnlineClient Owner;

        private string NextLevelPath;

        public GoToNextMapScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new GoToNextMapScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.Host.IsGameReady = false;
            Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[0];
            ActiveLayer.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            Owner.TripleThunderGame.PrepareNextLevel(NextLevelPath);
        }

        protected override void Read(OnlineReader Sender)
        {
            NextLevelPath = Sender.ReadString();
        }
    }
}
