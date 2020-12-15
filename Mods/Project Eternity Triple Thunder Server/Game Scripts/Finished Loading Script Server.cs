using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class FinishedLoadingScriptServer : OnlineScript
    {
        public const string ScriptName = "Finished Loading";

        private readonly ClientGroup ActiveGroup;

        public FinishedLoadingScriptServer(ClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public override OnlineScript Copy()
        {
            return new FinishedLoadingScriptServer(ActiveGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            Sender.IsGameReady = true;

            foreach (IOnlineConnection ActivePlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                if (!ActivePlayer.IsGameReady)
                    return;
            }

            foreach (IOnlineConnection ActivePlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                ActivePlayer.Send(new ServerIsReadyScriptServer());
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
