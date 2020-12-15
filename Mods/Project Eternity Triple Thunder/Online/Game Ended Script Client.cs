using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class GameEndedScriptClient : OnlineScript
    {
        public const string ScriptName = "Game Ended";

        private readonly TripleThunderOnlineClient Owner;

        public GameEndedScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new GameEndedScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.TripleThunderGame.PushScreen(new GameEndBattleScreen(Owner.TripleThunderGame));
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
