using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public static class OnlineHelper
    {
        public static Dictionary<string, OnlineScript> GetRoomScriptsServer(RoomInformations NewRoom, GameServer Owner)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            DicNewScript.Add(AskChangePlayerTypeScriptServer.ScriptName, new AskChangePlayerTypeScriptServer(NewRoom));
            DicNewScript.Add(AskChangeTeamScriptServer.ScriptName, new AskChangeTeamScriptServer(NewRoom));
            DicNewScript.Add(AskChangeMapScriptServer.ScriptName, new AskChangeMapScriptServer(NewRoom, Owner));
            DicNewScript.Add(AskChangeRoomSubtypeScriptServer.ScriptName, new AskChangeRoomSubtypeScriptServer(NewRoom));
            DicNewScript.Add(LeaveRoomScriptServer.ScriptName, new LeaveRoomScriptServer(NewRoom, Owner));

            return DicNewScript;
        }

        public static Dictionary<string, OnlineScript> GetBattleMapScriptsServer(BattleMapClientGroup ActiveGroup)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            DicNewScript.Add(MoveCursorScriptServer.ScriptName, new MoveCursorScriptServer(ActiveGroup));
            DicNewScript.Add(OpenMenuScriptServer.ScriptName, new OpenMenuScriptServer(ActiveGroup, ActiveGroup.BattleMapGame.GetOnlineActionPanel()));
            DicNewScript.Add(AskTripleThunderGameDataScriptServer.ScriptName, new AskTripleThunderGameDataScriptServer(ActiveGroup));

            return DicNewScript;
        }
    }
}
