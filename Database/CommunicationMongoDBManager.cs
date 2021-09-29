using System;
using System.Collections.Generic;
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

        public void UpdatePlayerCommunicationIP(string ClientID, string CommunicationServerIP, int CommunicationServerPort)
        {
            if (ClientID == null)
            {
                return;
            }

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ClientID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("CommunicationServerIP", CommunicationServerIP).Set("CommunicationServerPort", CommunicationServerPort);
            PlayersCollection.UpdateOneAsync(filter, update);
        }

        public void RemovePlayer(IOnlineConnection PlayerToRemove)
        {
            UpdatePlayerCommunicationIP(PlayerToRemove.ID, "", 0);
        }

        public void GetPlayerCommunicationIP(string ClientID, out string CommunicationServerIP, out int CommunicationServerPort)
        {
            FilterDefinition<BsonDocument> LastTimeCheckedFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ClientID));
            BsonDocument FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();

            CommunicationServerIP = FoundPlayerDocument.GetValue("CommunicationServerIP").AsString;
            CommunicationServerPort = FoundPlayerDocument.GetValue("CommunicationServerPort").AsInt32;
        }

        public byte[] GetClientInfo(string ClientID)
        {
            FilterDefinition<BsonDocument> LastTimeCheckedFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ClientID));
            BsonDocument FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();

            ByteWriter BW = new ByteWriter();

            BW.AppendByte((byte)FoundPlayerDocument.GetValue("Ranking").AsInt32);
            BW.AppendByte((byte)FoundPlayerDocument.GetValue("License").AsInt32);
            BW.AppendString(FoundPlayerDocument.GetValue("Guild").AsString);

            byte[] ClientInfo = BW.GetBytes();
            BW.ClearWriteBuffer();

            return ClientInfo;
        }

        public void AddFriend(IOnlineConnection Sender, string ClientID)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(Sender.ID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.AddToSet("Friends", new ObjectId(ClientID));
            PlayersCollection.UpdateOneAsync(filter, update);
        }

        public List<PlayerPOCO> GetFriendList(string ClientID)
        {
            List<PlayerPOCO> ListFriend = new List<PlayerPOCO>();

            FilterDefinition<BsonDocument> IDFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ClientID));

            BsonDocument result = PlayersCollection.Aggregate().Match(IDFilter).Lookup("TripleThunder", "Friends", "_id", "FriendsInfo").FirstOrDefault();

            BsonArray Friends = result.GetValue("FriendsInfo").AsBsonArray;
            foreach (BsonDocument ActiveFriend in Friends)
            {
                PlayerPOCO FoundPlayer = new PlayerPOCO();
                FoundPlayer.ID = ActiveFriend.GetValue("_id").AsObjectId.ToString();
                string Name = ActiveFriend.GetValue("Name").AsString;
                FoundPlayer.Name = Name;

                ByteWriter BW = new ByteWriter();
                BW.AppendString(Name);
                BW.AppendInt32(ActiveFriend.GetValue("Level").AsInt32);
                FoundPlayer.Info = BW.GetBytes();
                BW.ClearWriteBuffer();

                ListFriend.Add(FoundPlayer);
            }

            return ListFriend;
        }

        public void SaveGroupMessage(DateTime UtcNow, string GroupID, string Message, byte MessageColor)
        {
            BsonDocument NewEntry = new BsonDocument
                {
                    { "Date", UtcNow },
                    { "GroupID", GroupID },
                    { "Message", Message },
                    { "MessageColor", MessageColor },
                };

            PersonalCollection.InsertOneAsync(NewEntry);
        }

        public Dictionary<string, ChatManager.MessageColors> GetGroupMessages(string GroupID)
        {
            FilterDefinition<BsonDocument> SourceFilter = Builders<BsonDocument>.Filter.Eq("GroupID", GroupID);
            List<BsonDocument> ListMessages = PersonalCollection.Find(SourceFilter).ToList();

            Dictionary<string, ChatManager.MessageColors> DicChatHistory = new Dictionary<string, ChatManager.MessageColors>();

            foreach (BsonDocument ActiveMessage in ListMessages)
            {
                DicChatHistory.Add(ActiveMessage.GetValue("Message").AsString, (ChatManager.MessageColors)ActiveMessage.GetValue("MessageColor").AsInt32);
            }

            return DicChatHistory;
        }
    }
}
