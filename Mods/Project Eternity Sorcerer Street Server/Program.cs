using System;
using System.Diagnostics;
using System.Collections.Generic;
using Database.SorcererStreet;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Project Eternity Sorcerer Street Server";

            BattleMap instance = new SorcererStreetScreen.SorcererStreetMap();
            BattleMap.DicBattmeMapType.Add(instance.GetMapType(), instance);

            PlayerManager.DicUnitType = Unit.LoadAllUnits();
            PlayerManager.DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            PlayerManager.DicEffect = BaseEffect.LoadAllEffects();
            PlayerManager.DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            PlayerManager.DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();

            Dictionary<string, OnlineScript> DicOnlineScripts = new Dictionary<string, OnlineScript>();

            IniFile ConnectionInfo = IniFile.ReadFromFile("Connection Info.ini");
            string ConnectionChain = ConnectionInfo.ReadField("Game Server Info", "Connection Chain");
            string UserInformationChain = ConnectionInfo.ReadField("User Information Info", "Connection Chain");

            GameMongoDBManager Databse = new GameMongoDBManager();
            Databse.Init(ConnectionChain, UserInformationChain);
            GameServer OnlineServer = new GameServer(Databse, DicOnlineScripts);

            DicOnlineScripts.Add(AskLoginScriptServer.ScriptName, new AskLoginScriptServer(OnlineServer));
            DicOnlineScripts.Add(AskPlayerInventoryScriptServer.ScriptName, new AskPlayerInventoryScriptServer(OnlineServer));
            DicOnlineScripts.Add(BaseAskJoinRoomScriptServer.ScriptName, new AskJoinRoomScriptServer(OnlineServer));
            DicOnlineScripts.Add(AskRoomListScriptServer.ScriptName, new AskRoomListScriptServer(OnlineServer));
            DicOnlineScripts.Add(BaseCreateRoomScriptServer.ScriptName, new CreateRoomScriptServer(OnlineServer, BattleMapClientGroup.Template));
            DicOnlineScripts.Add(TransferRoomScriptServer.ScriptName, new TransferRoomScriptServer(OnlineServer, BattleMapClientGroup.Template));

            string PublicIP = ConnectionInfo.ReadField("Game Server Info", "Public IP");
            int PublicPort = int.Parse(ConnectionInfo.ReadField("Game Server Info", "Public Port"));
            Trace.Listeners.Add(new TextWriterTraceListener("Game Server Error.log", "myListener"));

            OnlineServer.StartListening(PublicIP, PublicPort);
            Console.ReadKey();
            OnlineServer.StopListening();
        }
    }
}
