using System;
using System.Diagnostics;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using Database;

namespace ProjectEternity.CommunicationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, OnlineScript> DicOnlineScripts = new Dictionary<string, OnlineScript>();

            IniFile ConnectionInfo = IniFile.ReadFromFile("Connection Info.ini");
            string ConnectionChain = ConnectionInfo.ReadField("Communication Server Info", "Connection Chain");
            string UserInformationChain = ConnectionInfo.ReadField("User Information Info", "Connection Chain");

            CommunicationMongoDBManager Databse = new CommunicationMongoDBManager();
            Databse.Init(ConnectionChain, UserInformationChain);
            CommunicationServer OnlineServer = new CommunicationServer(Databse, DicOnlineScripts);

            DicOnlineScripts.Add(SendGlobalMessageScriptServer.ScriptName, new SendGlobalMessageScriptServer(OnlineServer));
            DicOnlineScripts.Add(IdentifyScriptClient.ScriptName, new IdentifyScriptServer(OnlineServer));
            DicOnlineScripts.Add(AskForPlayersScriptServer.ScriptName, new AskForPlayersScriptServer(OnlineServer));
            DicOnlineScripts.Add(CreateCommunicationGroupScriptServer.ScriptName, new CreateCommunicationGroupScriptServer(OnlineServer));
            DicOnlineScripts.Add(JoinCommunicationGroupScriptServer.ScriptName, new JoinCommunicationGroupScriptServer(OnlineServer));

            string PublicIP = ConnectionInfo.ReadField("Communication Server Info", "Public IP");
            int PublicPort = int.Parse(ConnectionInfo.ReadField("Communication Server Info", "Public Port"));
            Trace.Listeners.Add(new TextWriterTraceListener("Communication Server Error.log", "myListener"));

            OnlineServer.StartListening(PublicIP, PublicPort);
            Console.ReadKey();
            OnlineServer.StopListening();
        }
    }
}
