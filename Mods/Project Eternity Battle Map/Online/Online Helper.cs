using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public static class OnlineHelper
    {
        public static Dictionary<string, OnlineScript> GetBattleMapScriptsClient(BattleMapOnlineClient Owner)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            DicNewScript.Add(ReceiveGameDataScriptClient.ScriptName, new ReceiveGameDataScriptClient(Owner));
            //DicNewScript.Add(GameEndedScriptClient.ScriptName, new GameEndedScriptClient(Owner));

            return DicNewScript;
        }
    }
}
