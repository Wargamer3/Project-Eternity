using System;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEternity.Core.Online;

namespace Database
{
    public class CommunicationMongoDBManager : ICommunicationDataManager
    {
        private DateTime LastTimeChecked;
        private MongoClient DatabaseCommunicationClient;
        private MongoClient DatabaseUserInformationClient;
        private IMongoDatabase DatabaseCommunication;
        private IMongoDatabase DatabaseUserInformation;
        private IMongoCollection<BsonDocument> GlobalCollection;
        private IMongoCollection<BsonDocument> PersonalCollection;
        private IMongoCollection<BsonDocument> PlayersCollection;

        public CommunicationMongoDBManager()
        {
            LastTimeChecked = DateTime.MinValue;
        }

        public void Init(string ConnectionChain, string UserInformationChain)
        {
            //If there's a dependency problem look at the config file and remove the extra stuff that got added automically (ie: Project Eternity Triple Thunder Server.exe.config)
            DatabaseCommunicationClient = new MongoClient(ConnectionChain);
            DatabaseUserInformationClient = new MongoClient(UserInformationChain);

            DatabaseCommunication = DatabaseCommunicationClient.GetDatabase("Communication");
            DatabaseUserInformation = DatabaseUserInformationClient.GetDatabase("UserInformation");

            GlobalCollection = DatabaseCommunication.GetCollection<BsonDocument>("Global");
            PersonalCollection = DatabaseCommunication.GetCollection<BsonDocument>("Personal");
            PlayersCollection = DatabaseUserInformation.GetCollection<BsonDocument>("TripleThunder");
        }
    }
}
