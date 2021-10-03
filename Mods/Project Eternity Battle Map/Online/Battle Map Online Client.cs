using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class BattleMapOnlineClient : GameClient
    {
        public BattleMap BattleMapGame { get; private set; }

        public BattleMapOnlineClient(Dictionary<string, OnlineScript> DicOnlineScripts)
            : base(DicOnlineScripts)
        {
        }

        public void SetGame(BattleMap NewGame)
        {
            CurrentGame = NewGame;
            BattleMapGame = NewGame;
        }
    }
}
