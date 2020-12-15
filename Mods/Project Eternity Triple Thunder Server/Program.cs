using System;
using System.Collections.Generic;
using Database;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, OnlineScript> DicOnlineScripts = new Dictionary<string, OnlineScript>();

            MongoDBManager Databse = new MongoDBManager();
            Databse.Init();
            Server OnlineServer = new Server(Databse, DicOnlineScripts);

            DicOnlineScripts.Add(AskGameDataScriptServer.ScriptName, new AskGameDataScriptServer());
            DicOnlineScripts.Add(AskLoginScriptServer.ScriptName, new AskLoginScriptServer(OnlineServer));
            DicOnlineScripts.Add(Core.Online.AskJoinRoomScriptServer.ScriptName, new AskJoinRoomScriptServer(OnlineServer));
            DicOnlineScripts.Add(AskRoomListScriptServer.ScriptName, new AskRoomListScriptServer(OnlineServer));
            DicOnlineScripts.Add(CreateRoomTripleThunderScriptServer.ScriptName, new CreateRoomTripleThunderScriptServer(OnlineServer, TripleThunderClientGroup.Template));
            DicOnlineScripts.Add(SendGameDataScriptServer.ScriptName, new SendGameDataScriptServer(OnlineServer));
            DicOnlineScripts.Add(TransferRoomScriptServer.ScriptName, new TransferRoomScriptServer(OnlineServer, TripleThunderClientGroup.Template));

            IniFile ConnectionInfo = IniFile.ReadFromFile("ConnectionInfo.ini");
            string PublicIP = ConnectionInfo.ReadField("ServerInfo", "PublicIP");
            int PublicPort = int.Parse(ConnectionInfo.ReadField("ServerInfo", "PublicPort"));

            OnlineServer.StartListening(PublicIP, PublicPort);
            Console.ReadKey();
            OnlineServer.StopListening();
        }
    }
}
