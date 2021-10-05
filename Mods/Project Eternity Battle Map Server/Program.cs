using System;
using System.Diagnostics;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using Database.BattleMap;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using System.Reflection;
using System.IO;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] Files = Directory.GetFiles("Mods", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ass = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                //Get every classes in it.
                Type[] types = ass.GetTypes();
                for (int t = 0; t < types.Length; t++)
                {
                    //Look if the class inherit from Unit somewhere.
                    Type ObjectType = types[t].BaseType;
                    bool InstanceIsBaseObject = ObjectType == typeof(BattleMap);
                    while (ObjectType != null && ObjectType != typeof(BattleMap))
                    {
                        ObjectType = ObjectType.BaseType;
                        if (ObjectType == null)
                            InstanceIsBaseObject = false;
                    }
                    //If this class is from BaseEditor, load it.
                    if (InstanceIsBaseObject)
                    {
                        BattleMap instance = Activator.CreateInstance(types[t]) as BattleMap;
                        BattleMap.DicBattmeMapType.Add(instance.GetMapType(), instance);
                    }
                }
            }

            Dictionary<string, OnlineScript> DicOnlineScripts = new Dictionary<string, OnlineScript>();

            IniFile ConnectionInfo = IniFile.ReadFromFile("Connection Info.ini");
            string ConnectionChain = ConnectionInfo.ReadField("Game Server Info", "Connection Chain");
            string UserInformationChain = ConnectionInfo.ReadField("User Information Info", "Connection Chain");

            GameMongoDBManager Databse = new GameMongoDBManager();
            Databse.Init(ConnectionChain, UserInformationChain);
            GameServer OnlineServer = new GameServer(Databse, DicOnlineScripts);

            DicOnlineScripts.Add(AskLoginScriptServer.ScriptName, new AskLoginScriptServer(OnlineServer));
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
