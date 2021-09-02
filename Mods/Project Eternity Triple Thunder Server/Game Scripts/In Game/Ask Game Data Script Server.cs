using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class AskTripleThunderGameDataScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Game Data";

        private readonly TripleThunderClientGroup ActiveGroup;

        public AskTripleThunderGameDataScriptServer(TripleThunderClientGroup ActiveGroup)
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
