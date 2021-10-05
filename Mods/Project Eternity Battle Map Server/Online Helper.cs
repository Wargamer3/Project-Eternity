using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public static class OnlineHelper
    {
        public static Dictionary<string, OnlineScript> GetRoomScriptsServer(PVPRoomInformations NewRoom, GameServer Owner)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            DicNewScript.Add(AskChangeCharacterScriptServer.ScriptName, new AskChangeCharacterScriptServer(NewRoom));
            DicNewScript.Add(AskChangePlayerTypeScriptServer.ScriptName, new AskChangePlayerTypeScriptServer(NewRoom));
            DicNewScript.Add(AskChangeTeamScriptServer.ScriptName, new AskChangeTeamScriptServer(NewRoom));
            DicNewScript.Add(AskChangeMapScriptServer.ScriptName, new AskChangeMapScriptServer(NewRoom, Owner));
            DicNewScript.Add(AskChangeRoomSubtypeScriptServer.ScriptName, new AskChangeRoomSubtypeScriptServer(NewRoom));
            DicNewScript.Add(LeaveRoomScriptServer.ScriptName, new LeaveRoomScriptServer(NewRoom, Owner));

            return DicNewScript;
        }

        public static Dictionary<string, OnlineScript> GetBattleMapScriptsServer(GameClientGroup ActiveGroup, OnlinePlayer ActivePlayer)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            //DicNewScript.Add(FinishedLoadingScriptServer.ScriptName, new FinishedLoadingScriptServer(ActiveGroup));

            return DicNewScript;
        }
    }
}
