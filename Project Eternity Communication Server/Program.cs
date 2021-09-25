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

            IniFile ConnectionInfo = IniFile.ReadFromFile("ConnectionInfo.ini");
            string ConnectionChain = ConnectionInfo.ReadField("CommunicationServerInfo", "ConnectionChain");

            MongoDBManager Databse = new MongoDBManager();
            Databse.Init(ConnectionChain);
            Core.Online.CommunicationServer OnlineServer = new Core.Online.CommunicationServer(Databse, DicOnlineScripts);


            string PublicIP = ConnectionInfo.ReadField("CommunicationServerInfo", "PublicIP");
            int PublicPort = int.Parse(ConnectionInfo.ReadField("CommunicationServerInfo", "PublicPort"));
            Trace.Listeners.Add(new TextWriterTraceListener("ServerError.log", "myListener"));

            OnlineServer.StartListening(PublicIP, PublicPort);
            Console.ReadKey();
            OnlineServer.StopListening();
        }
    }
}
