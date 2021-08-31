using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public static class OnlineHelper
    {
        public static Dictionary<string, OnlineScript> GetTripleThunderScriptsClient(TripleThunderOnlineClient Owner)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            DicNewScript.Add(SendPlayerUpdateScriptClient.ScriptName, new SendPlayerUpdateScriptClient(Owner));
            DicNewScript.Add(SendPlayerRespawnScriptClient.ScriptName, new SendPlayerRespawnScriptClient(Owner));
            DicNewScript.Add(SendPlayerDamageScriptClient.ScriptName, new SendPlayerDamageScriptClient(Owner));
            DicNewScript.Add(ShootBulletScriptClient.ScriptName, new ShootBulletScriptClient(Owner));
            DicNewScript.Add(ReceiveGameDataScriptClient.ScriptName, new ReceiveGameDataScriptClient(Owner));
            DicNewScript.Add(GoToNextMapScriptClient.ScriptName, new GoToNextMapScriptClient(Owner));
            DicNewScript.Add(GameEndedScriptClient.ScriptName, new GameEndedScriptClient(Owner));
            DicNewScript.Add(CreateSFXScriptClient.ScriptName, new CreateSFXScriptClient(Owner));
            DicNewScript.Add(CreateVFXScriptClient.ScriptName, new CreateVFXScriptClient(Owner));

            return DicNewScript;
        }
    }
}
