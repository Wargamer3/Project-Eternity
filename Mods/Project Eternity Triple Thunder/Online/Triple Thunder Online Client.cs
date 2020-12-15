﻿using System;
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

        public TripleThunderOnlineClient(Dictionary<string, OnlineScript> DicOnlineScripts)
            : base(DicOnlineScripts)
        {
        }

        public void SetGame(FightingZone NewGame)
        {
            CurrentGame = NewGame;
            TripleThunderGame = NewGame;
        }
    }
}
