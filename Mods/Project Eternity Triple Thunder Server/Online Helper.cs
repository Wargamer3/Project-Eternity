using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public static class OnlineHelper
    {
        public static Dictionary<string, OnlineScript> GetRoomScriptsServer(RoomInformations NewRoom, Server Owner)
        {
            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();

            DicNewScript.Add(AskChangeCharacterScriptServer.ScriptName, new AskChangeCharacterScriptServer(NewRoom));
            DicNewScript.Add(AskChangePlayerTypeScriptServer.ScriptName, new AskChangePlayerTypeScriptServer(NewRoom));
            DicNewScript.Add(AskChangeTeamScriptServer.ScriptName, new AskChangeTeamScriptServer(NewRoom));
            DicNewScript.Add(AskChangeMapScriptServer.ScriptName, new AskChangeMapScriptServer(NewRoom, Owner));
            DicNewScript.Add(AskChangeRoomSubtypeScriptServer.ScriptName, new AskChangeRoomSubtypeScriptServer(NewRoom));
            DicNewScript.Add(LeaveRoomScriptServer.ScriptName, new LeaveRoomScriptServer(NewRoom, Owner));
            DicNewScript.Add(CreateSFXScriptServer.ScriptName, new CreateSFXScriptServer());
            DicNewScript.Add(CreateVFXScriptServer.ScriptName, new CreateVFXScriptServer());

            return DicNewScript;
        }

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
