using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace Database
{
    public class MongoDBManager : IDataManager
    {
        private DateTime LastTimeChecked;
        private MongoClient DatabaseClient;
        private IMongoDatabase Database;
        private IMongoCollection<BsonDocument> RoomsCollection;
        private IMongoCollection<BsonDocument> PlayersCollection;

        public MongoDBManager()
        {
            LastTimeChecked = DateTime.MinValue;
        }

        public void Init()
        {
            IniFile ConnectionInfo = IniFile.ReadFromFile("ConnectionInfo.ini");
            string ConnectionChain = ConnectionInfo.ReadField("ServerInfo", "ConnectionChain");

            //If there's a dependency problem look at the config file (ie: Project Eternity Triple Thunder Server.exe.config)
            DatabaseClient = new MongoClient(ConnectionChain);
            Database = DatabaseClient.GetDatabase("TripleThunder");
            RoomsCollection = Database.GetCollection<BsonDocument>("Rooms");
            PlayersCollection = Database.GetCollection<BsonDocument>("Players");
        }

        public List<IRoomInformations> GetAllRoomUpdatesSinceLastTimeChecked(string ServerVersion)
        {
            DateTime CurrentTime = DateTime.Now;
            FilterDefinition<BsonDocument> LastTimeCheckedFilter = Builders<BsonDocument>.Filter.Gte("LastUpdate", LastTimeChecked);
            List<BsonDocument> ListFoundRoomDocument = RoomsCollection.Find(LastTimeCheckedFilter).ToList();
            List<IRoomInformations> ListFoundRoom = new List<IRoomInformations>();

            foreach (BsonDocument ActiveDocument in ListFoundRoomDocument)
            {
                string RoomID = ActiveDocument.GetValue("_id").AsObjectId.ToString();
                string RoomName = ActiveDocument.GetValue("RoomName").AsString;
                string RoomType = ActiveDocument.GetValue("RoomType").AsString;
                string RoomSubtype = ActiveDocument.GetValue("RoomSubtype").AsString;
                bool IsPlaying = ActiveDocument.GetValue("IsPlaying").AsBoolean;
                string Password = ActiveDocument.GetValue("Password").AsString;
                string OwnerServerIP = ActiveDocument.GetValue("OwnerServerIP").AsString;
                int OwnerServerPort = ActiveDocument.GetValue("OwnerServerPort").AsInt32;
                int CurrentPlayerCount = ActiveDocument.GetValue("CurrentPlayerCount").AsInt32;
                int MaxNumberOfPlayer = ActiveDocument.GetValue("MaxNumberOfPlayer").AsInt32;
                bool IsDead = ActiveDocument.GetValue("IsDead").AsBoolean;

                IRoomInformations NewRoom = new ServerRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, IsPlaying, Password, OwnerServerIP, OwnerServerPort, CurrentPlayerCount, MaxNumberOfPlayer, IsDead);
                ListFoundRoom.Add(NewRoom);
            }

            LastTimeChecked = CurrentTime;
            return ListFoundRoom;
        }

        public void HandleOldData(string OwnerServerIP, int OwnerServerPort)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("OwnerServerIP", OwnerServerIP) & Builders<BsonDocument>.Filter.Eq("OwnerServerPort", OwnerServerPort);

            RoomsCollection.DeleteManyAsync(filter);

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("OwnerServerIP", "").Set("OwnerServerPort", 0);
            PlayersCollection.UpdateManyAsync(filter, update);
        }

        public IRoomInformations GenerateNewRoom(string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort, int MaxNumberOfPlayer)
        {
            BsonDocument document = new BsonDocument
            {
                { "LastUpdate", DateTime.Now },
                { "RoomName", RoomName },
                { "RoomType", RoomType },
                { "RoomSubtype", RoomSubtype },
                { "IsPlaying", false },
                { "Password", Password },
                { "OwnerServerIP", OwnerServerIP },
                { "OwnerServerPort", OwnerServerPort },
                { "CurrentPlayerCount", 1 },
                { "MaxNumberOfPlayer", MaxNumberOfPlayer },
                { "IsDead", false },
            };

            RoomsCollection.InsertOne(document);

            if (RoomType == RoomInformations.RoomTypeMission)
            {
                return new MissionRoomInformations(document.GetValue("_id").AsObjectId.ToString(), RoomName, RoomType, RoomSubtype, Password, OwnerServerIP, OwnerServerPort, MaxNumberOfPlayer);
            }
            else if (RoomType == RoomInformations.RoomTypeBattle)
            {
                return new BattleRoomInformations(document.GetValue("_id").AsObjectId.ToString(), RoomName, RoomType, RoomSubtype, Password, OwnerServerIP, OwnerServerPort, MaxNumberOfPlayer);
            }

            return null;
        }

        public void RemoveRoom(string RoomID)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(RoomID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("IsDead", true).Set("LastUpdate", DateTime.Now);
            RoomsCollection.UpdateOneAsync(filter, update);
        }

        public IRoomInformations TransferRoom(string RoomID, string OwnerServerIP)
        {
            return null;
        }

        public void UpdatePlayerCountInRoom(string RoomID, int CurrentPlayerCount)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(RoomID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("CurrentPlayerCount", CurrentPlayerCount).Set("LastUpdate", DateTime.Now);
            RoomsCollection.UpdateOneAsync(filter, update);
        }

        public void RemovePlayer(IOnlineConnection OnlineConnection)
        {
            UpdatePlayerIsLoggedIn(OnlineConnection.ID, "", 0);
        }

        public PlayerPOCO LogInPlayer(string Login, string Password, string OwnerServerIP, int OwnerServerPort)
        {
            FilterDefinition<BsonDocument> LastTimeCheckedFilter = Builders<BsonDocument>.Filter.Eq("Login", Login) & Builders<BsonDocument>.Filter.Eq("Password", Password);
            BsonDocument FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();

            if (FoundPlayerDocument == null && PlayersCollection.Find(Builders<BsonDocument>.Filter.Eq("Login", Login)).FirstOrDefault() == null)
            {
                BsonDocument document = new BsonDocument
                {
                    { "Login", Login },
                    { "Name", Login },
                    { "CharacterType", "Jack" },
                    { "OwnerServerIP", "" },
                    { "OwnerServerPort", 0 },
                    { "Password", Password },
                    { "NumberOfFailedConnection", 0 },
                    { "Equipment",
                        new BsonArray
                        {
                            new BsonDocument { { "Money", 0 }, { "EXP", 0 } } }
                        }
                };

                PlayersCollection.InsertOne(document);
                FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();
            }

            bool LoggedIn = !string.IsNullOrEmpty(FoundPlayerDocument.GetValue("OwnerServerIP").AsString);

            if (LoggedIn)
            {
                return LogInPlayer(Login + "1", Password, OwnerServerIP, OwnerServerPort);
            }
            else
            {
                PlayerPOCO FoundPlayer = new PlayerPOCO();
                FoundPlayer.ID = FoundPlayerDocument.GetValue("_id").AsObjectId.ToString();
                FoundPlayer.Name = FoundPlayerDocument.GetValue("Name").AsString;
                UpdatePlayerIsLoggedIn(FoundPlayer.ID, OwnerServerIP, OwnerServerPort);

                return FoundPlayer;
            }
        }

        private void UpdatePlayerIsLoggedIn(string ID, string OwnerServerIP, int OwnerServerPort)
        {
            if (ID == null)
            {
                return;
            }

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("OwnerServerIP", OwnerServerIP).Set("OwnerServerPort", OwnerServerPort);
            PlayersCollection.UpdateOneAsync(filter, update);
        }
    }
}
