using System;
using System.Diagnostics;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using Database;

namespace ProjectEternity.Communication.Server
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
            DicOnlineScripts.Add(CreateOrJoinCommunicationGroupScriptServer.ScriptName, new CreateOrJoinCommunicationGroupScriptServer(OnlineServer));
            DicOnlineScripts.Add(JoinCommunicationGroupScriptServer.ScriptName, new JoinCommunicationGroupScriptServer(OnlineServer));
            DicOnlineScripts.Add(LeaveCommunicationGroupScriptServer.ScriptName, new LeaveCommunicationGroupScriptServer(OnlineServer));
            DicOnlineScripts.Add(AskClientInfoScriptServer.ScriptName, new AskClientInfoScriptServer(OnlineServer));
            DicOnlineScripts.Add(AddFriendScriptServer.ScriptName, new AddFriendScriptServer(OnlineServer));
            DicOnlineScripts.Add(SendGroupMessageScriptServer.ScriptName, new SendGroupMessageScriptServer(OnlineServer));
            DicOnlineScripts.Add(SendGroupInviteScriptServer.ScriptName, new SendGroupInviteScriptServer(OnlineServer));

            string PublicIP = ConnectionInfo.ReadField("Communication Server Info", "Public IP");
            int PublicPort = int.Parse(ConnectionInfo.ReadField("Communication Server Info", "Public Port"));
            Trace.Listeners.Add(new TextWriterTraceListener("Communication Server Error.log", "myListener"));

            OnlineServer.StartListening(PublicIP, PublicPort);
            Console.ReadKey();
            OnlineServer.StopListening();
        }
    }
}
