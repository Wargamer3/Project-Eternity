using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public static class OnlineHelper
    {
        public static Dictionary<string, OnlineScript> GetTripleThunderScriptsServer(ClientGroup ActiveGroup, Player ActivePlayer)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            DicNewScript.Add(FinishedLoadingScriptServer.ScriptName, new FinishedLoadingScriptServer(ActiveGroup));
            DicNewScript.Add(SendPlayerUpdateScriptServer.ScriptName, new SendPlayerUpdateScriptServer((TripleThunderClientGroup)ActiveGroup, ActivePlayer));
            DicNewScript.Add(ShootBulletScriptServer.ScriptName, new ShootBulletScriptServer((TripleThunderClientGroup)ActiveGroup, ActivePlayer));
            DicNewScript.Add(AskTripleThunderGameDataScriptServer.ScriptName, new AskTripleThunderGameDataScriptServer((TripleThunderClientGroup)ActiveGroup));

            return DicNewScript;
        }
    }
}
