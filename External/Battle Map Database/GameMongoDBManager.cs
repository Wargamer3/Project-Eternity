using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace Database.BattleMap
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

            return new BattleMapRoomInformations(document.GetValue("_id").AsObjectId.ToString(), RoomName, RoomType, RoomSubtype, Password, OwnerServerIP, OwnerServerPort, MinNumberOfPlayer, MaxNumberOfPlayer);
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
                    { "LastConnection", DateTime.UtcNow },
                    { "NumberOfFailedConnection", 0 },
                    { "Inventory",
                        new BsonDocument
                        {
                            { "Money", 0 },
                            { "OwnedSquads",
                                new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        { "RelativePath", "Normal/Multiplayer/Voltaire" },
                                        { "PilotFullName", "Multiplayer/Greg" },
                                    },
                                    new BsonDocument
                                    {
                                        { "RelativePath", "Normal/Multiplayer/Mazinger Z" },
                                        { "PilotFullName", "Multiplayer/Kouji" },
                                    },
                                }
                            },
                            { "OwnedCharacters",
                                new BsonArray
                                {
                                    "Multiplayer/Greg",
                                    "Multiplayer/Kouji",
                                }
                            },
                            { "OwnedMissions",
                                new BsonArray
                                {
                                }
                            },
                            { "SquadLoadouts",
                                new BsonArray
                                {
                                    new BsonArray
                                    {
                                        new BsonDocument
                                        {
                                            { "RelativePath", "Normal/Multiplayer/Voltaire" },
                                            { "PilotFullName", "Multiplayer/Greg" },
                                        },
                                        new BsonDocument
                                        {
                                            { "RelativePath", "Normal/Multiplayer/Mazinger Z" },
                                            { "PilotFullName", "Multiplayer/Kouji" },
                                        },
                                    }
                                }
                            }
                        }
                    },
                    { "Unlocks",
                        new BsonDocument
                        {
                            { "Characters",
                                new BsonArray
                                {
                                }
                            },
                            { "Units",
                                new BsonArray
                                {
                                }
                            },
                            { "Missions",
                                new BsonArray
                                {
                                }
                            },
                        }
                    },
                    { "Records",
                        new BsonDocument
                        {
                            { "TotalSecondsPlayed", 0d},

                            { "TotalKills", 0},
                            { "TotalTurnPlayed", 0},
                            { "TotalTilesTraveled", 0},

                            { "CurrentMoney", 0},
                            { "TotalMoney", 0},

                            { "CurrentCoins", 0},
                            { "TotalCoins", 0},

                            { "UnitRecords",
                                new BsonDocument
                                {
                                    { "CharacterIDByNumberOfKills",
                                        new BsonArray
                                        {
                                        }
                                    },
                                    { "CharacterIDByNumberOfUses",
                                        new BsonArray
                                        {
                                        }
                                    },
                                    { "CharacterIDByTurnsOnField",
                                        new BsonArray
                                        {
                                        }
                                    },
                                    { "CharacterIDByNumberOfTilesTraveled",
                                        new BsonArray
                                        {
                                        }
                                    },
                                    { "UnitIDByNumberOfKills",
                                        new BsonArray
                                        {
                                        }
                                    },
                                    { "UnitIDByNumberOfUses",
                                        new BsonArray
                                        {
                                        }
                                    },
                                    { "UnitIDByTurnsOnField",
                                        new BsonArray
                                        {
                                        }
                                    },
                                    { "UnitIDByNumberOfTilesTraveled",
                                        new BsonArray
                                        {
                                        }
                                    },
                                }
                            },
                            {"BattleRecords",
                                new BsonDocument
                                {
                                    { "NumberOfGamesPlayed", 0},
                                    { "NumberOfGamesWon", 0},
                                    { "NumberOfGamesLost", 0},
                                    { "NumberOfKills", 0},
                                    { "NumberOfUnitsLost", 0},

                                    { "TotalDamageGiven", 0},
                                    { "TotalDamageReceived", 0},
                                    { "TotalDamageRecovered", 0},
                                }
                            },
                            {"BonusRecords",
                                new BsonDocument
                                {
                                    { "NumberOfBonusObtainedByName",
                                        new BsonArray
                                        {
                                        }
                                    },
                                }
                            },
                            { "MultiplayerInformation",
                                new BsonDocument
                                {
                                    { "CampaignLevelInformation",
                                        new BsonArray
                                        {
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
            FoundPlayer.ID = FoundPlayerDocument.GetValue("_id").AsObjectId.ToString();
            string Name = FoundPlayerDocument.GetValue("Name").AsString;
            FoundPlayer.Name = Name;

            ByteWriter BW = new ByteWriter();
            GetUnlockesBytes(BW, FoundPlayerDocument);
            GetRecordsBytes(BW, FoundPlayerDocument);
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

            BsonArray OwnedSquadsArray = Inventory.GetValue("OwnedSquads").AsBsonArray;
            BW.AppendInt32(OwnedSquadsArray.Count);

            foreach (BsonValue ActiveSquad in OwnedSquadsArray)
            {
                BsonDocument ActiveUnitDocument = ActiveSquad.AsBsonDocument;
                BW.AppendString(ActiveUnitDocument.GetValue("RelativePath").AsString);
                BW.AppendString(ActiveUnitDocument.GetValue("PilotFullName").AsString);
            }

            BsonArray OwnedCharactersArray = Inventory.GetValue("OwnedCharacters").AsBsonArray;
            BW.AppendInt32(OwnedCharactersArray.Count);

            foreach (BsonValue ActiveCharacter in OwnedCharactersArray)
            {
                BW.AppendString(ActiveCharacter.AsString);
            }

            BsonArray OwnedMissionsArray = Inventory.GetValue("OwnedMissions").AsBsonArray;
            BW.AppendInt32(OwnedMissionsArray.Count);

            foreach (BsonValue ActiveMission in OwnedMissionsArray)
            {
                BW.AppendString(ActiveMission.AsString);
            }

            BsonArray SquadLoadoutsArray = Inventory.GetValue("SquadLoadouts").AsBsonArray;
            BW.AppendInt32(SquadLoadoutsArray.Count);

            foreach (BsonValue ActiveLoadout in SquadLoadoutsArray)
            {
                BsonArray ActiveSquadDocument = ActiveLoadout.AsBsonArray;
                BW.AppendInt32(ActiveSquadDocument.Count);

                foreach (BsonValue ActiveSquad in ActiveSquadDocument)
                {
                    BsonDocument ActiveUnitDocument = ActiveSquad.AsBsonDocument;
                    BW.AppendString(ActiveUnitDocument.GetValue("RelativePath").AsString);
                    BW.AppendString(ActiveUnitDocument.GetValue("PilotFullName").AsString);
                }
            }
        }

        private void GetUnlockesBytes(ByteWriter BW, BsonDocument FoundPlayerDocument)
        {
            BsonDocument Inventory = FoundPlayerDocument.GetValue("Unlocks").AsBsonDocument;

            BsonArray UnlockedCharactersArray = Inventory.GetValue("Characters").AsBsonArray;
            BsonArray UnlockedUnitsArray = Inventory.GetValue("Units").AsBsonArray;
            BsonArray UnlockedMissionsArray = Inventory.GetValue("Missions").AsBsonArray;

            BW.AppendInt32(UnlockedCharactersArray.Count);
            BW.AppendInt32(UnlockedUnitsArray.Count);
            BW.AppendInt32(UnlockedMissionsArray.Count);

            foreach (BsonValue ActiveCharacter in UnlockedCharactersArray)
            {
                BW.AppendString(ActiveCharacter.AsString);
            }

            foreach (BsonValue ActiveUnit in UnlockedUnitsArray)
            {
                BW.AppendString(ActiveUnit.AsString);
            }

            foreach (BsonValue ActiveMission in UnlockedMissionsArray)
            {
                BW.AppendString(ActiveMission.AsString);
            }
        }

        private void GetRecordsBytes(ByteWriter BW, BsonDocument FoundPlayerDocument)
        {
            BsonDocument Records = FoundPlayerDocument.GetValue("Records").AsBsonDocument;

            BW.AppendDouble(Records.GetValue("TotalSecondsPlayed").AsDouble);

            BW.AppendUInt32((uint)Records.GetValue("TotalKills").AsInt32);
            BW.AppendUInt32((uint)Records.GetValue("TotalTurnPlayed").AsInt32);
            BW.AppendUInt32((uint)Records.GetValue("TotalTilesTraveled").AsInt32);

            BW.AppendUInt32((uint)Records.GetValue("CurrentMoney").AsInt32);
            BW.AppendUInt32((uint)Records.GetValue("TotalMoney").AsInt32);

            BW.AppendUInt32((uint)Records.GetValue("CurrentCoins").AsInt32);
            BW.AppendUInt32((uint)Records.GetValue("TotalCoins").AsInt32);

            GetUnitRecordsBytes(BW, Records);
            GetBattleRecordsBytes(BW, Records);
            GetBonusRecordsBytes(BW, Records);

            GetCampaignLevelInformationBytes(BW, Records);
        }

        private void GetUnitRecordsBytes(ByteWriter BW, BsonDocument Records)
        {
            BsonDocument UnitRecords = Records.GetValue("UnitRecords").AsBsonDocument;

            BsonArray CharacterIDByNumberOfKillsArray = UnitRecords.GetValue("CharacterIDByNumberOfKills").AsBsonArray;
            BW.AppendInt32(CharacterIDByNumberOfKillsArray.Count);
            foreach (BsonDocument ActiveMission in CharacterIDByNumberOfKillsArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByNumberOfUsesArray = UnitRecords.GetValue("CharacterIDByNumberOfUses").AsBsonArray;
            BW.AppendInt32(CharacterIDByNumberOfUsesArray.Count);
            foreach (BsonDocument ActiveMission in CharacterIDByNumberOfUsesArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByTurnsOnFieldArray = UnitRecords.GetValue("CharacterIDByTurnsOnField").AsBsonArray;
            BW.AppendInt32(CharacterIDByTurnsOnFieldArray.Count);
            foreach (BsonDocument ActiveMission in CharacterIDByTurnsOnFieldArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByNumberOfTilesTraveledArray = UnitRecords.GetValue("CharacterIDByNumberOfTilesTraveled").AsBsonArray;
            BW.AppendInt32(CharacterIDByNumberOfTilesTraveledArray.Count);
            foreach (BsonDocument ActiveMission in CharacterIDByNumberOfTilesTraveledArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByNumberOfKillsArray = UnitRecords.GetValue("UnitIDByNumberOfKills").AsBsonArray;
            BW.AppendInt32(UnitIDByNumberOfKillsArray.Count);
            foreach (BsonDocument ActiveMission in UnitIDByNumberOfKillsArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByNumberOfUsesArray = UnitRecords.GetValue("UnitIDByNumberOfUses").AsBsonArray;
            BW.AppendInt32(UnitIDByNumberOfUsesArray.Count);
            foreach (BsonDocument ActiveMission in UnitIDByNumberOfUsesArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByTurnsOnFieldArray = UnitRecords.GetValue("UnitIDByTurnsOnField").AsBsonArray;
            BW.AppendInt32(UnitIDByTurnsOnFieldArray.Count);
            foreach (BsonDocument ActiveMission in UnitIDByTurnsOnFieldArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByNumberOfTilesTraveledArray = UnitRecords.GetValue("UnitIDByNumberOfTilesTraveled").AsBsonArray;
            BW.AppendInt32(UnitIDByNumberOfTilesTraveledArray.Count);
            foreach (BsonDocument ActiveMission in UnitIDByNumberOfTilesTraveledArray)
            {
                BW.AppendString(ActiveMission.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveMission.GetValue("Kills").AsInt32);
            }
        }

        private void GetBattleRecordsBytes(ByteWriter BW, BsonDocument Records)
        {
            BsonDocument UnitRecords = Records.GetValue("BattleRecords").AsBsonDocument;

            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfGamesPlayed").AsInt32);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfGamesWon").AsInt32);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfGamesLost").AsInt32);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfKills").AsInt32);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfUnitsLost").AsInt32);

            BW.AppendUInt32((uint)UnitRecords.GetValue("TotalDamageGiven").AsInt32);
            BW.AppendUInt32((uint)UnitRecords.GetValue("TotalDamageReceived").AsInt32);
            BW.AppendUInt32((uint)UnitRecords.GetValue("TotalDamageRecovered").AsInt32);
        }

        private void GetBonusRecordsBytes(ByteWriter BW, BsonDocument Records)
        {
            BsonDocument BonusRecords = Records.GetValue("BonusRecords").AsBsonDocument;
            BsonArray NumberOfBonusObtainedByNameArray = BonusRecords.GetValue("NumberOfBonusObtainedByName").AsBsonArray;

            BW.AppendInt32(NumberOfBonusObtainedByNameArray.Count);
            foreach (BsonDocument NumberOfBonusObtainedByName in NumberOfBonusObtainedByNameArray)
            {
                BW.AppendString(NumberOfBonusObtainedByName.GetValue("Name").AsString);
                BW.AppendUInt32((uint)NumberOfBonusObtainedByName.GetValue("Count").AsInt32);
            }
        }

        private void GetCampaignLevelInformationBytes(ByteWriter BW, BsonDocument Records)
        {
            BsonDocument MultiplayerInformation = Records.GetValue("MultiplayerInformation").AsBsonDocument;
            BsonArray CampaignLevelInformation = MultiplayerInformation.GetValue("CampaignLevelInformation").AsBsonArray;

            BW.AppendInt32(CampaignLevelInformation.Count);
            foreach (BsonDocument NumberOfBonusObtainedByName in CampaignLevelInformation)
            {
                BW.AppendString(NumberOfBonusObtainedByName.GetValue("Name").AsString);
                BW.AppendInt64(NumberOfBonusObtainedByName.GetValue("FirstCompletionDate").AsBsonDateTime.ToUniversalTime().Ticks);
                BW.AppendUInt32((uint)NumberOfBonusObtainedByName.GetValue("MaxScore").AsInt32);
            }
        }
    }
}
