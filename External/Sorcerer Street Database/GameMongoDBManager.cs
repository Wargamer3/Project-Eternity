using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace Database.SorcererStreet
{
    public class GameMongoDBManager : IGameDataManager
    {
        private DateTime LastTimeChecked;
        private MongoClient DatabaseBattleMapClient;
        private MongoClient DatabaseUserInformationClient;
        private IMongoDatabase DatabaseBattleMap;
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
            DatabaseBattleMapClient = new MongoClient(ConnectionChain);
            DatabaseUserInformationClient = new MongoClient(UserInformationChain);
            DatabaseBattleMap = DatabaseBattleMapClient.GetDatabase("BattleMap");
            DatabaseUserInformation = DatabaseUserInformationClient.GetDatabase("UserInformation");
            RoomsCollection = DatabaseBattleMap.GetCollection<BsonDocument>("Rooms");
            PlayersCollection = DatabaseUserInformation.GetCollection<BsonDocument>("BattleMap");
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
                byte MinNumberOfPlayer = (byte)ActiveDocument.GetValue("MinNumberOfPlayer").AsInt32;
                byte MaxNumberOfPlayer = (byte)ActiveDocument.GetValue("MaxNumberOfPlayer").AsInt32;
                bool IsDead = ActiveDocument.GetValue("IsDead").AsBoolean;

                IRoomInformations NewRoom = new BattleMapRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, IsPlaying, Password,
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

            return new SorcererStreetRoomInformations(document.GetValue("_id").AsObjectId.ToString(), RoomName, RoomType, RoomSubtype, Password, OwnerServerIP, OwnerServerPort, MinNumberOfPlayer, MaxNumberOfPlayer);
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
                    { "EXP", 0 },
                    { "Ranking", 1 },
                    { "License", 1 },
                    { "Guild", "" },
                    { "GameServerIP", "" },
                    { "GameServerPort", 0 },
                    { "CommunicationServerIP", "" },
                    { "CommunicationServerPort", 0 },
                    { "Password", Password },
                    { "NumberOfFailedConnection", 0 },
                    { "Inventory",
                        new BsonDocument
                        {
                            { "Money", 0 },
                            { "CharacterModelPath", "" },
                            { "GlobalBook",
                                new BsonDocument
                                {
                                    { "BookName", "GlobalBook" },
                                    { "BookModel", "" },
                                    { "LastModification", (long)0 },
                                    { "Wins", 0 },
                                    { "Matches", 0},
                                    { "Cards",
                                        new BsonArray
                                        {
                                            new BsonDocument { { "CardType", "Creature" }, { "Path", "Fire/Ares" } , { "QuantityOwned", 10 } },
                                            new BsonDocument { { "CardType", "Creature" }, { "Path", "Earth/Angostura" } , { "QuantityOwned", 10 } },
                                            new BsonDocument { { "CardType", "Item" }, { "Path", "Abyssal Tome" } , { "QuantityOwned", 1 } },
                                        }
                                    }
                                }
                            },
                            { "ActiveBookName", "Default" },
                            { "Books",
                                new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        { "BookName", "Default" },
                                        { "BookModel", "" },
                                        { "LastModification", (long)0 },
                                        { "Wins", 0 },
                                        { "Matches", 0},
                                        { "Cards",
                                            new BsonArray
                                            {
                                                new BsonDocument { { "CardType", "Creature" }, { "Path", "Fire/Ares" } , { "QuantityOwned", 10 } },
                                                new BsonDocument { { "CardType", "Creature" }, { "Path", "Earth/Angostura" } , { "QuantityOwned", 10 } },
                                                new BsonDocument { { "CardType", "Item" }, { "Path", "Abyssal Tome" } , { "QuantityOwned", 1 } },
                                            }
                                        }
                                    },
                                    new BsonDocument
                                    {
                                        { "BookName", "Default 2" },
                                        { "BookModel", "" },
                                        { "LastModification", (long)0 },
                                        { "Wins", 0 },
                                        { "Matches", 0},
                                        { "Cards",
                                            new BsonArray
                                            {
                                                new BsonDocument { { "CardType", "Creature" }, { "Path", "Fire/Ares" } , { "QuantityOwned", 10 } },
                                                new BsonDocument { { "CardType", "Creature" }, { "Path", "Earth/Angostura" } , { "QuantityOwned", 10 } },
                                                new BsonDocument { { "CardType", "Item" }, { "Path", "Abyssal Tome" } , { "QuantityOwned", 1 } },
                                            }
                                        }
                                    },
                                }
                            }
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

        public PlayerPOCO GetPlayerInventory(string ID)
        {
            FilterDefinition<BsonDocument> LastTimeCheckedFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ID));
            BsonDocument FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();

            PlayerPOCO FoundPlayer = new PlayerPOCO();
            FoundPlayer.ID = ID;
            string Name = FoundPlayerDocument.GetValue("Name").AsString;
            FoundPlayer.Name = Name;

            ByteWriter BW = new ByteWriter();
            GetInventoryBytes(BW, FoundPlayerDocument);
            FoundPlayer.Info = BW.GetBytes();
            BW.ClearWriteBuffer();

            return FoundPlayer;
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

        private void GetInventoryBytes(ByteWriter BW, BsonDocument FoundPlayerDocument)
        {
            BsonDocument Inventory = FoundPlayerDocument.GetValue("Inventory").AsBsonDocument;
            BW.AppendString(Inventory.GetValue("CharacterModelPath").AsString);

            ProcessBook(BW, Inventory.GetValue("GlobalBook").AsBsonDocument);

            BW.AppendString(Inventory.GetValue("ActiveBookName").AsString);

            BsonArray BooksArray = Inventory.GetValue("Books").AsBsonArray;
            BW.AppendInt32(BooksArray.Count);
            foreach (BsonValue ActiveBook in BooksArray)
            {
                ProcessBook(BW, ActiveBook.AsBsonDocument);
            }
        }

        private void ProcessBook(ByteWriter BW, BsonDocument Book)
        {
            BW.AppendString(Book.GetValue("BookName").AsString);
            BW.AppendString(Book.GetValue("BookModel").AsString);
            BW.AppendInt64(Book.GetValue("LastModification").AsInt64);
            BW.AppendInt32(Book.GetValue("Wins").AsInt32);
            BW.AppendInt32(Book.GetValue("Matches").AsInt32);

            BsonArray CardsArray = Book.GetValue("Cards").AsBsonArray;
            BW.AppendInt32(CardsArray.Count);
            foreach (BsonValue ActiveCard in CardsArray)
            {
                BsonDocument ActiveCardDocument = ActiveCard.AsBsonDocument;
                BW.AppendString(ActiveCardDocument.GetValue("CardType").AsString);
                BW.AppendString(ActiveCardDocument.GetValue("Path").AsString);
                BW.AppendInt32(ActiveCardDocument.GetValue("QuantityOwned").AsInt32);
            }
        }

        public void SavePlayerInventory(string ID, object PlayerToSave)
        {
            throw new NotImplementedException();
        }
    }
}
