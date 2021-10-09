using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskTripleThunderGameDataScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Game Data";

        private readonly GameClientGroup ActiveGroup;

        public AskTripleThunderGameDataScriptServer(GameClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public override OnlineScript Copy()
        {
            return new AskTripleThunderGameDataScriptServer(ActiveGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection ActivePlayer)
        {
            ActivePlayer.IsGameReady = true;
            ActivePlayer.Send(new ReceiveGameDataScriptServer(ActiveGroup, ActivePlayer));
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
