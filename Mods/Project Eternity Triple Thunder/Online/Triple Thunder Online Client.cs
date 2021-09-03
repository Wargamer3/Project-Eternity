using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public interface DelayedExecutableOnlineScript
    {
        void ExecuteOnMainThread();
    }

    public class TripleThunderOnlineClient : Client
    {
        public FightingZone TripleThunderGame { get; private set; }
        private readonly List<DelayedExecutableOnlineScript> ListDelayedOnlineCommand;

        public TripleThunderOnlineClient(Dictionary<string, OnlineScript> DicOnlineScripts)
            : base(DicOnlineScripts)
        {
            ListDelayedOnlineCommand = new List<DelayedExecutableOnlineScript>();
        }

        public void SetGame(FightingZone NewGame)
        {
            CurrentGame = NewGame;
            TripleThunderGame = NewGame;
        }

        public void ExecuteDelayedScripts()
        {
            lock (ListDelayedOnlineCommand)
            {
                foreach (DelayedExecutableOnlineScript ActiveCommand in ListDelayedOnlineCommand)
                {
                    ActiveCommand.ExecuteOnMainThread();
                }

                ListDelayedOnlineCommand.Clear();
            }
        }

        public void DelayOnlineScript(DelayedExecutableOnlineScript ScriptToDelay)
        {
            lock (ListDelayedOnlineCommand)
            {
                ListDelayedOnlineCommand.Add(ScriptToDelay);
            }
        }
    }
}
