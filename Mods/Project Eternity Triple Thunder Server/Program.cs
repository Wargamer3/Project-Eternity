using System;
using System.Diagnostics;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;
using Database;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, OnlineScript> DicOnlineScripts = new Dictionary<string, OnlineScript>();

            IniFile ConnectionInfo = IniFile.ReadFromFile("ConnectionInfo.ini");
            string ConnectionChain = ConnectionInfo.ReadField("GameServerInfo", "ConnectionChain");

            MongoDBManager Databse = new MongoDBManager();
            Databse.Init(ConnectionChain);
            GameServer OnlineServer = new GameServer(Databse, DicOnlineScripts);

            DicOnlineScripts.Add(AskGameDataScriptServer.ScriptName, new AskGameDataScriptServer());
            DicOnlineScripts.Add(AskLoginScriptServer.ScriptName, new AskLoginScriptServer(OnlineServer));
            DicOnlineScripts.Add(Core.Online.AskJoinRoomScriptServer.ScriptName, new AskJoinRoomScriptServer(OnlineServer));
            DicOnlineScripts.Add(AskRoomListScriptServer.ScriptName, new AskRoomListScriptServer(OnlineServer));
            DicOnlineScripts.Add(CreateRoomTripleThunderScriptServer.ScriptName, new CreateRoomTripleThunderScriptServer(OnlineServer, TripleThunderClientGroup.Template));
            DicOnlineScripts.Add(SendGameDataScriptServer.ScriptName, new SendGameDataScriptServer(OnlineServer));
            DicOnlineScripts.Add(TransferRoomScriptServer.ScriptName, new TransferRoomScriptServer(OnlineServer, TripleThunderClientGroup.Template));

            string PublicIP = ConnectionInfo.ReadField("GameServerInfo", "PublicIP");
            int PublicPort = int.Parse(ConnectionInfo.ReadField("GameServerInfo", "PublicPort"));
            Trace.Listeners.Add(new TextWriterTraceListener("ServerError.log", "myListener"));

            OnlineServer.StartListening(PublicIP, PublicPort);
            Console.ReadKey();
            OnlineServer.StopListening();
        }
    }
}
