using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace Database.TripleThunder
{
    public class GameMongoDBManager : IGameDataManager
    {
        private DateTime LastTimeChecked;
        private MongoClient DatabaseTripleThunderClient;
        private MongoClient DatabaseUserInformationClient;
        private IMongoDatabase DatabaseTripleThunder;
        private IMongoDatabase DatabaseUserInformation;
        private IMongoCollection<BsonDocument> RoomsCollection;
        private IMongoCollection<BsonDocument> PlayersCollection;

        public GameMongoDBManager()
        {
            LastTimeChecked = DateTime.MinValue;
        }

        public void Init(string ConnectionChain, string UserInformationChain)
        {
            //If there's a dependency problem look at the config file and remove the extra stuff that got added automically (ie: Project Eternity Triple Thunder Server.exe.config)
            DatabaseTripleThunderClient = new MongoClient(ConnectionChain);
            DatabaseUserInformationClient = new MongoClient(UserInformationChain);
            DatabaseTripleThunder = DatabaseTripleThunderClient.GetDatabase("TripleThunder");
            DatabaseUserInformation = DatabaseUserInformationClient.GetDatabase("UserInformation");
            RoomsCollection = DatabaseTripleThunder.GetCollection<BsonDocument>("Rooms");
            PlayersCollection = DatabaseUserInformation.GetCollection<BsonDocument>("TripleThunder");
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
                byte CurrentPlayerCount = (byte)ActiveDocument.GetValue("CurrentPlayerCount").AsInt32;
                byte MinNumberOfPlayer = (byte)ActiveDocument.GetValue("MaxNumberOfPlayer").AsInt32;
                byte MaxNumberOfPlayer = (byte)ActiveDocument.GetValue("MaxNumberOfPlayer").AsInt32;
                bool IsDead = ActiveDocument.GetValue("IsDead").AsBoolean;

                IRoomInformations NewRoom = new ServerRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, IsPlaying, Password,
                    OwnerServerIP, OwnerServerPort, CurrentPlayerCount, MinNumberOfPlayer, MaxNumberOfPlayer, IsDead);
                ListFoundRoom.Add(NewRoom);
            }

            LastTimeChecked = CurrentTime;
            return ListFoundRoom;
        }

        public void HandleOldData(string OwnerServerIP, int OwnerServerPort)
        {
            FilterDefinition<BsonDocument> RoomFilter = Builders<BsonDocument>.Filter.Eq("OwnerServerIP", OwnerServerIP) & Builders<BsonDocument>.Filter.Eq("OwnerServerPort", OwnerServerPort);

            RoomsCollection.DeleteManyAsync(RoomFilter);

            FilterDefinition<BsonDocument> PlayerFilter = Builders<BsonDocument>.Filter.Eq("GameServerIP", OwnerServerIP) & Builders<BsonDocument>.Filter.Eq("GameServerPort", OwnerServerPort);
            UpdateDefinition<BsonDocument> PlayerUpdate = Builders<BsonDocument>.Update.Set("GameServerIP", "").Set("GameServerPort", 0);
            PlayersCollection.UpdateManyAsync(PlayerFilter, PlayerUpdate);
        }

        public IRoomInformations GenerateNewRoom(string RoomName, string RoomType, string RoomSubtype, string Password, string OwnerServerIP, int OwnerServerPort, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
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
                { "MinNumberOfPlayer", MinNumberOfPlayer },
                { "MaxNumberOfPlayer", MaxNumberOfPlayer },
                { "IsDead", false },
            };

            RoomsCollection.InsertOne(document);

            if (RoomType == RoomInformations.RoomTypeMission)
            {
                return new MissionRoomInformations(document.GetValue("_id").AsObjectId.ToString(), RoomName, RoomType, RoomSubtype, Password, OwnerServerIP, OwnerServerPort, MinNumberOfPlayer, MaxNumberOfPlayer);
            }
            else if (RoomType == RoomInformations.RoomTypeBattle)
            {
                return new BattleRoomInformations(document.GetValue("_id").AsObjectId.ToString(), RoomName, RoomType, RoomSubtype, Password, OwnerServerIP, OwnerServerPort, MinNumberOfPlayer, MaxNumberOfPlayer);
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

        public void UpdatePlayerCountInRoom(string RoomID, byte CurrentPlayerCount)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(RoomID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("CurrentPlayerCount", CurrentPlayerCount).Set("LastUpdate", DateTime.Now);
            RoomsCollection.UpdateOneAsync(filter, update);
        }

        public void RemovePlayer(IOnlineConnection PlayerToRemove)
        {
            UpdatePlayerIsLoggedIn(PlayerToRemove.ID, "", 0);
        }

        public PlayerPOCO LogInPlayer(string Login, string Password, string GameServerIP, int GameServerPort)
        {
            FilterDefinition<BsonDocument> LastTimeCheckedFilter = Builders<BsonDocument>.Filter.Eq("Login", Login) & Builders<BsonDocument>.Filter.Eq("Password", Password);
            BsonDocument FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();

            if (FoundPlayerDocument == null && PlayersCollection.Find(Builders<BsonDocument>.Filter.Eq("Login", Login)).FirstOrDefault() == null)
            {
                BsonDocument document = new BsonDocument
                {
                    { "Login", Login },
                    { "Name", Login },
                    { "Level", 1 },
                    { "Ranking", 1 },
                    { "License", 1 },
                    { "Guild", "" },
                    { "CharacterType", "Jack" },
                    { "GameServerIP", "" },
                    { "GameServerPort", 0 },
                    { "CommunicationServerIP", "" },
                    { "CommunicationServerPort", 0 },
                    { "Password", Password },
                    { "NumberOfFailedConnection", 0 },
                    { "Equipment",
                        new BsonArray
                        {
                            new BsonDocument { { "Money", 0 }, { "EXP", 0 } }
                        }
                    },
                    { "Friends",
                        new BsonArray
                        {
                        }
                    }
                };

                PlayersCollection.InsertOne(document);
                FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();
            }

            bool LoggedIn = !string.IsNullOrEmpty(FoundPlayerDocument.GetValue("GameServerIP").AsString);

            if (LoggedIn)
            {
                return LogInPlayer(Login + "1", Password, GameServerIP, GameServerPort);
            }
            else
            {
                PlayerPOCO FoundPlayer = new PlayerPOCO();
                FoundPlayer.ID = FoundPlayerDocument.GetValue("_id").AsObjectId.ToString();
                string Name = FoundPlayerDocument.GetValue("Name").AsString;
                FoundPlayer.Name = Name;

                ByteWriter BW = new ByteWriter();
                BW.AppendString(Name);
                BW.AppendInt32(FoundPlayerDocument.GetValue("Level").AsInt32);
                FoundPlayer.Info = BW.GetBytes();
                BW.ClearWriteBuffer();

                UpdatePlayerIsLoggedIn(FoundPlayer.ID, GameServerIP, GameServerPort);

                return FoundPlayer;
            }
        }

        private void UpdatePlayerIsLoggedIn(string ID, string GameServerIP, int GameServerPort)
        {
            if (ID == null)
            {
                return;
            }

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("GameServerIP", GameServerIP).Set("GameServerPort", GameServerPort);
            PlayersCollection.UpdateOneAsync(filter, update);
        }
    }
}
