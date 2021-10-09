using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen.Online
{
    public class DeathmatchOnlineClient : GameClient
    {
        public DeathmatchMap BattleMapGame { get; private set; }

        public DeathmatchOnlineClient(Dictionary<string, OnlineScript> DicOnlineScripts)
            : base(DicOnlineScripts)
        {
        }

        public void SetGame(DeathmatchMap NewGame)
        {
            CurrentGame = NewGame;
            BattleMapGame = NewGame;
        }
    }
}
