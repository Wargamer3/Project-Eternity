﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;
using Database.TripleThunder;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    class Program
    {
        static void Main(string[] args)
        {
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
            DicOnlineScripts.Add(BaseCreateRoomScriptServer.ScriptName, new CreateRoomTripleThunderScriptServer(OnlineServer, TripleThunderClientGroup.Template));
            DicOnlineScripts.Add(SendGameDataScriptServer.ScriptName, new SendGameDataScriptServer(OnlineServer));
            DicOnlineScripts.Add(TransferRoomScriptServer.ScriptName, new TransferRoomScriptServer(OnlineServer, TripleThunderClientGroup.Template));

            string PublicIP = ConnectionInfo.ReadField("Game Server Info", "Public IP");
            int PublicPort = int.Parse(ConnectionInfo.ReadField("Game Server Info", "Public Port"));
            Trace.Listeners.Add(new TextWriterTraceListener("Game Server Error.log", "myListener"));

            OnlineServer.StartListening(PublicIP, PublicPort);
            Console.ReadKey();
            OnlineServer.StopListening();
        }
    }
}
