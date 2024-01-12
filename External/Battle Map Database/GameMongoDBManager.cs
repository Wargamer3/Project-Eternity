using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
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

        List<Tuple<string, string>> ListDefaultUnit;

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

            ListDefaultUnit = new List<Tuple<string, string>>();
            IniFile IniDefaultUnits = IniFile.ReadFromFile("Content/Battle Lobby Default Units.ini");
            foreach (string ActiveKey in IniDefaultUnits.ReadAllKeys())
            {
                string UnitPath = IniDefaultUnits.ReadField(ActiveKey, "Path");
                string PilotPath = IniDefaultUnits.ReadField(ActiveKey, "Pilot");
                ListDefaultUnit.Add(new Tuple<string, string>(UnitPath, PilotPath));
            }
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
                BsonDocument NewPlayer = InitNewPlayer(Login, Password);

                PlayersCollection.InsertOne(NewPlayer);
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

        private BsonDocument InitNewPlayer(string Login, string Password)
        {
            BsonDocument NewPlayer = new BsonDocument
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
                        { "OwnedUnits",
                            new BsonArray
                            {
                            }
                        },
                        { "OwnedCharacters",
                            new BsonArray
                            {
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
                        { "TotalSecondsPlayed", 0d },

                        { "TotalKills", 0L },
                        { "TotalTurnPlayed", 0L },
                        { "TotalTilesTraveled", 0L },

                        { "CurrentMoney", 0L },
                        { "TotalMoney", 0L },

                        { "CurrentCoins", 0L },
                        { "TotalCoins", 0L },

                        { "UnitRecords",
                            new BsonDocument
                            {
                                { "CharacterIDByNumberOfKills",
                                    new BsonArray
                                    {
                                    }
                                },
                                { "CharacterIDByTotalDamageGiven",
                                    new BsonArray
                                    {
                                    }
                                },
                                { "CharacterIDByNumberOfDeaths",
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
                                { "UnitIDByTotalDamageGiven",
                                    new BsonArray
                                    {
                                    }
                                },
                                { "UnitIDByNumberOfDeaths",
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
                                { "NumberOfGamesPlayed", 0L },
                                { "NumberOfGamesWon", 0L },
                                { "NumberOfGamesLost", 0L },
                                { "NumberOfKills", 0L },
                                { "NumberOfUnitsLost", 0L },

                                { "TotalDamageGiven", 0L },
                                { "TotalDamageReceived", 0L },
                                { "TotalDamageRecovered", 0L },
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
                        { "MapRecords",
                            new BsonDocument
                            {
                                { "CampaignLevelsInformation",
                                    new BsonArray
                                    {
                                    }
                                },
                                { "MapsInformation",
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

            BsonDocument Inventory = NewPlayer.GetValue("Inventory").AsBsonDocument;
            BsonArray OwnedUnits = Inventory.GetValue("OwnedUnits").AsBsonArray;
            BsonArray OwnedCharacters = Inventory.GetValue("OwnedCharacters").AsBsonArray;
            BsonArray SquadLoadouts = Inventory.GetValue("SquadLoadouts").AsBsonArray;

            foreach (Tuple<string, string> ActiveUnitAndPilot in ListDefaultUnit)
            {
                BsonDocument NewSquad = new BsonDocument();
                NewSquad.Add("RelativePath", ActiveUnitAndPilot.Item1);
                OwnedUnits.Add(NewSquad);

                if (!string.IsNullOrEmpty(ActiveUnitAndPilot.Item2))
                {
                    OwnedCharacters.Add(ActiveUnitAndPilot.Item2);
                }
            }

            BsonDocument NewSquadLoadout = new BsonDocument();
            NewSquadLoadout.Add("RelativePath", ListDefaultUnit[0].Item1);
            NewSquadLoadout.Add("PilotFullName", ListDefaultUnit[0].Item2);

            BsonArray temp = new BsonArray();
            temp.Add(NewSquadLoadout);

            BsonDocument ActiveSquadDocument = new BsonDocument();

            ActiveSquadDocument.Add("Name", "Default");
            ActiveSquadDocument.Add("Squads", temp);
            SquadLoadouts.Add(ActiveSquadDocument);

            return NewPlayer;
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

        public PlayerPOCO GetPlayerInventory(string ID)
        {
            FilterDefinition<BsonDocument> LastTimeCheckedFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ID));
            BsonDocument FoundPlayerDocument = PlayersCollection.Find(LastTimeCheckedFilter).FirstOrDefault();

            PlayerPOCO FoundPlayer = new PlayerPOCO();
            FoundPlayer.ID = FoundPlayerDocument.GetValue("_id").AsObjectId.ToString();
            string Name = FoundPlayerDocument.GetValue("Name").AsString;
            FoundPlayer.Name = Name;

            ByteWriter BW = new ByteWriter();
            BW.AppendInt32(FoundPlayerDocument.GetValue("Level").AsInt32);
            BW.AppendInt32(FoundPlayerDocument.GetValue("EXP").AsInt32);
            GetUnlocksBytes(BW, FoundPlayerDocument);
            GetRecordsBytes(BW, FoundPlayerDocument);
            GetInventoryBytes(BW, FoundPlayerDocument);
            FoundPlayer.Info = BW.GetBytes();
            BW.ClearWriteBuffer();

            return FoundPlayer;
        }

        private void GetInventoryBytes(ByteWriter BW, BsonDocument FoundPlayerDocument)
        {
            BsonDocument Inventory = FoundPlayerDocument.GetValue("Inventory").AsBsonDocument;

            BsonArray OwnedUnitsArray = Inventory.GetValue("OwnedUnits").AsBsonArray;
            BW.AppendInt32(OwnedUnitsArray.Count);

            foreach (BsonValue ActiveSquad in OwnedUnitsArray)
            {
                BsonDocument ActiveUnitDocument = ActiveSquad.AsBsonDocument;
                BW.AppendString(ActiveUnitDocument.GetValue("RelativePath").AsString);
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
                BsonDocument ActiveSquadDocument = ActiveLoadout.AsBsonDocument;

                BW.AppendString(ActiveSquadDocument.GetValue("Name").AsString);

                BsonArray SquadArray = ActiveSquadDocument.GetValue("Squads").AsBsonArray;
                BW.AppendInt32(SquadArray.Count);

                foreach (BsonValue ActiveSquad in SquadArray)
                {
                    BsonDocument ActiveUnitDocument = ActiveSquad.AsBsonDocument;
                    BW.AppendString(ActiveUnitDocument.GetValue("RelativePath").AsString);
                    BW.AppendString(ActiveUnitDocument.GetValue("PilotFullName").AsString);
                }
            }
        }

        private void GetUnlocksBytes(ByteWriter BW, BsonDocument FoundPlayerDocument)
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

            BW.AppendUInt32((uint)Records.GetValue("TotalKills").AsInt64);
            BW.AppendUInt32((uint)Records.GetValue("TotalTurnPlayed").AsInt64);
            BW.AppendUInt32((uint)Records.GetValue("TotalTilesTraveled").AsInt64);

            BW.AppendUInt32((uint)Records.GetValue("CurrentMoney").AsInt64);
            BW.AppendUInt32((uint)Records.GetValue("TotalMoney").AsInt64);

            BW.AppendUInt32((uint)Records.GetValue("CurrentCoins").AsInt64);
            BW.AppendUInt32((uint)Records.GetValue("TotalCoins").AsInt64);

            GetUnitRecordsBytes(BW, Records);
            GetBattleRecordsBytes(BW, Records);
            GetBonusRecordsBytes(BW, Records);

            GetCampaignLevelInformationBytes(BW, Records);
            GetMapsInformation(BW, Records);
        }

        private void GetUnitRecordsBytes(ByteWriter BW, BsonDocument Records)
        {
            BsonDocument UnitRecords = Records.GetValue("UnitRecords").AsBsonDocument;

            BsonArray CharacterIDByNumberOfKillsArray = UnitRecords.GetValue("CharacterIDByNumberOfKills").AsBsonArray;
            BW.AppendInt32(CharacterIDByNumberOfKillsArray.Count);
            foreach (BsonDocument ActiveCharacter in CharacterIDByNumberOfKillsArray)
            {
                BW.AppendString(ActiveCharacter.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveCharacter.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByTotalDamageGivenArray = UnitRecords.GetValue("CharacterIDByTotalDamageGiven").AsBsonArray;
            BW.AppendInt32(CharacterIDByTotalDamageGivenArray.Count);
            foreach (BsonDocument ActiveCharacter in CharacterIDByTotalDamageGivenArray)
            {
                BW.AppendString(ActiveCharacter.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveCharacter.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByNumberOfDeathsArray = UnitRecords.GetValue("CharacterIDByNumberOfDeaths").AsBsonArray;
            BW.AppendInt32(CharacterIDByNumberOfDeathsArray.Count);
            foreach (BsonDocument ActiveCharacter in CharacterIDByNumberOfDeathsArray)
            {
                BW.AppendString(ActiveCharacter.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveCharacter.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByNumberOfUsesArray = UnitRecords.GetValue("CharacterIDByNumberOfUses").AsBsonArray;
            BW.AppendInt32(CharacterIDByNumberOfUsesArray.Count);
            foreach (BsonDocument ActiveCharacter in CharacterIDByNumberOfUsesArray)
            {
                BW.AppendString(ActiveCharacter.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveCharacter.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByTurnsOnFieldArray = UnitRecords.GetValue("CharacterIDByTurnsOnField").AsBsonArray;
            BW.AppendInt32(CharacterIDByTurnsOnFieldArray.Count);
            foreach (BsonDocument ActiveCharacter in CharacterIDByTurnsOnFieldArray)
            {
                BW.AppendString(ActiveCharacter.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveCharacter.GetValue("Kills").AsInt32);
            }

            BsonArray CharacterIDByNumberOfTilesTraveledArray = UnitRecords.GetValue("CharacterIDByNumberOfTilesTraveled").AsBsonArray;
            BW.AppendInt32(CharacterIDByNumberOfTilesTraveledArray.Count);
            foreach (BsonDocument ActiveCharacter in CharacterIDByNumberOfTilesTraveledArray)
            {
                BW.AppendString(ActiveCharacter.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveCharacter.GetValue("Kills").AsInt32);
            }


            BsonArray UnitIDByNumberOfKillsArray = UnitRecords.GetValue("UnitIDByNumberOfKills").AsBsonArray;
            BW.AppendInt32(UnitIDByNumberOfKillsArray.Count);
            foreach (BsonDocument ActiveUnit in UnitIDByNumberOfKillsArray)
            {
                BW.AppendString(ActiveUnit.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveUnit.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByTotalDamageGivenArray = UnitRecords.GetValue("UnitIDByTotalDamageGiven").AsBsonArray;
            BW.AppendInt32(UnitIDByTotalDamageGivenArray.Count);
            foreach (BsonDocument ActiveUnit in UnitIDByTotalDamageGivenArray)
            {
                BW.AppendString(ActiveUnit.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveUnit.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByNumberOfDeathsArray = UnitRecords.GetValue("UnitIDByNumberOfDeaths").AsBsonArray;
            BW.AppendInt32(UnitIDByNumberOfDeathsArray.Count);
            foreach (BsonDocument ActiveUnit in UnitIDByNumberOfDeathsArray)
            {
                BW.AppendString(ActiveUnit.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveUnit.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByNumberOfUsesArray = UnitRecords.GetValue("UnitIDByNumberOfUses").AsBsonArray;
            BW.AppendInt32(UnitIDByNumberOfUsesArray.Count);
            foreach (BsonDocument ActiveUnit in UnitIDByNumberOfUsesArray)
            {
                BW.AppendString(ActiveUnit.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveUnit.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByTurnsOnFieldArray = UnitRecords.GetValue("UnitIDByTurnsOnField").AsBsonArray;
            BW.AppendInt32(UnitIDByTurnsOnFieldArray.Count);
            foreach (BsonDocument ActiveUnit in UnitIDByTurnsOnFieldArray)
            {
                BW.AppendString(ActiveUnit.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveUnit.GetValue("Kills").AsInt32);
            }

            BsonArray UnitIDByNumberOfTilesTraveledArray = UnitRecords.GetValue("UnitIDByNumberOfTilesTraveled").AsBsonArray;
            BW.AppendInt32(UnitIDByNumberOfTilesTraveledArray.Count);
            foreach (BsonDocument ActiveUnit in UnitIDByNumberOfTilesTraveledArray)
            {
                BW.AppendString(ActiveUnit.GetValue("Path").AsString);
                BW.AppendUInt32((uint)ActiveUnit.GetValue("Kills").AsInt32);
            }
        }

        private void GetBattleRecordsBytes(ByteWriter BW, BsonDocument Records)
        {
            BsonDocument UnitRecords = Records.GetValue("BattleRecords").AsBsonDocument;

            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfGamesPlayed").AsInt64);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfGamesWon").AsInt64);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfGamesLost").AsInt64);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfKills").AsInt64);
            BW.AppendUInt32((uint)UnitRecords.GetValue("NumberOfUnitsLost").AsInt64);

            BW.AppendUInt32((uint)UnitRecords.GetValue("TotalDamageGiven").AsInt64);
            BW.AppendUInt32((uint)UnitRecords.GetValue("TotalDamageReceived").AsInt64);
            BW.AppendUInt32((uint)UnitRecords.GetValue("TotalDamageRecovered").AsInt64);
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
            BsonDocument MultiplayerInformation = Records.GetValue("MapRecords").AsBsonDocument;
            BsonArray CampaignLevelsInformation = MultiplayerInformation.GetValue("CampaignLevelsInformation").AsBsonArray;

            BW.AppendInt32(CampaignLevelsInformation.Count);
            foreach (BsonDocument CampaignLevelInformation in CampaignLevelsInformation)
            {
                BW.AppendString(CampaignLevelInformation.GetValue("Name").AsString);
                BW.AppendInt64(CampaignLevelInformation.GetValue("FirstCompletionDate").AsBsonDateTime.ToUniversalTime().Ticks);
                BW.AppendUInt32((uint)CampaignLevelInformation.GetValue("MaxScore").AsInt32);
            }
        }

        private void GetMapsInformation(ByteWriter BW, BsonDocument Records)
        {
            BsonDocument MultiplayerInformation = Records.GetValue("MapRecords").AsBsonDocument;
            BsonArray MapsInformation = MultiplayerInformation.GetValue("MapsInformation").AsBsonArray;

            BW.AppendInt32(MapsInformation.Count);
            foreach (BsonDocument MapInformation in MapsInformation)
            {
                BW.AppendString(MapInformation.GetValue("Name").AsString);
                BW.AppendUInt32((uint)MapInformation.GetValue("TotalSecondsPlayed").AsInt64);
                BW.AppendInt32(MapInformation.GetValue("TotalTimesPlayed").AsInt32);
                BW.AppendInt32(MapInformation.GetValue("TotalVictories").AsInt32);
                BW.AppendInt32(MapInformation.GetValue("TotalUnitsKilled").AsInt32);
                BW.AppendUInt32((uint)MapInformation.GetValue("TotalDamageGiven").AsInt64);
            }
        }

        public void SavePlayerInventory(string ID, object PlayerToSave)
        {
            BattleMapPlayer Player = (BattleMapPlayer)PlayerToSave;
            BsonDocument PlayerDocument = new BsonDocument
            {
                { "Name", Player.Name },
                { "Level", Player.Level },
                { "EXP", Player.EXP },
                { "Ranking", 1 },
                { "License", 1 },
                { "Guild", "" },
                { "Inventory",
                    new BsonDocument
                    {
                        { "Money", 0 },
                        { "OwnedUnits",
                            new BsonArray
                            {
                            }
                        },
                        { "OwnedCharacters",
                            new BsonArray
                            {
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
                        { "TotalSecondsPlayed", Player.Records.TotalSecondsPlayed },

                        { "TotalKills", Player.Records.TotalKills },
                        { "TotalTurnPlayed", Player.Records.TotalTurnPlayed},
                        { "TotalTilesTraveled", Player.Records.TotalTilesTraveled},

                        { "CurrentMoney", Player.Records.CurrentMoney},
                        { "TotalMoney", Player.Records.TotalMoney},

                        { "CurrentCoins", Player.Records.CurrentCoins},
                        { "TotalCoins", Player.Records.TotalCoins},

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
                                { "NumberOfGamesPlayed", Player.Records.PlayerBattleRecords.NumberOfGamesPlayed },
                                { "NumberOfGamesWon", Player.Records.PlayerBattleRecords.NumberOfGamesWon },
                                { "NumberOfGamesLost", Player.Records.PlayerBattleRecords.NumberOfGamesLost },
                                { "NumberOfKills", Player.Records.PlayerBattleRecords.NumberOfKills },
                                { "NumberOfUnitsLost", Player.Records.PlayerBattleRecords.NumberOfUnitsLost },

                                { "TotalDamageGiven", Player.Records.PlayerBattleRecords.TotalDamageGiven },
                                { "TotalDamageReceived", Player.Records.PlayerBattleRecords.TotalDamageReceived },
                                { "TotalDamageRecovered", Player.Records.PlayerBattleRecords.TotalDamageRecovered },
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
                        { "MapRecords",
                            new BsonDocument
                            {
                                { "CampaignLevelsInformation",
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

            BsonDocument Inventory = PlayerDocument.GetValue("Inventory").AsBsonDocument;
            BsonArray OwnedUnits = Inventory.GetValue("OwnedUnits").AsBsonArray;
            BsonArray OwnedCharacters = Inventory.GetValue("OwnedCharacters").AsBsonArray;
            BsonArray SquadLoadouts = Inventory.GetValue("SquadLoadouts").AsBsonArray;

            foreach (Tuple<string, string> ActiveUnitAndPilot in ListDefaultUnit)
            {
                BsonDocument NewSquad = new BsonDocument();
                NewSquad.Add("RelativePath", ActiveUnitAndPilot.Item1);
                OwnedUnits.Add(NewSquad);

                if (!string.IsNullOrEmpty(ActiveUnitAndPilot.Item2))
                {
                    OwnedCharacters.Add(ActiveUnitAndPilot.Item2);
                }
            }

            BsonDocument NewSquadLoadout = new BsonDocument();
            NewSquadLoadout.Add("RelativePath", ListDefaultUnit[0].Item1);
            NewSquadLoadout.Add("PilotFullName", ListDefaultUnit[0].Item2);

            BsonArray temp = new BsonArray();
            temp.Add(NewSquadLoadout);
            SquadLoadouts.Add(temp);

            BsonDocument Unlocks = PlayerDocument.GetValue("Unlocks").AsBsonDocument;
            BsonDocument Records = PlayerDocument.GetValue("Records").AsBsonDocument;
            BsonArray Friends = PlayerDocument.GetValue("Friends").AsBsonArray;

            SetInventoryInformation(PlayerDocument, Player);
            SetUnlocksInformation(PlayerDocument, Player);
            SetRecordsInformation(PlayerDocument, Player);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ID));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("Name", Player.Name)
                .Set("Level", Player.Level)
                .Set("EXP", 0)
                .Set("Ranking", 1)
                .Set("License", 1)
                .Set("Guild", "")
                .Set("Inventory", Inventory)
                .Set("Unlocks", Unlocks)
                .Set("Records", Records)
                .Set("Friends", Friends);
            PlayersCollection.UpdateOneAsync(filter, update);
        }

        private void SetInventoryInformation(BsonDocument FoundPlayerDocument, BattleMapPlayer Player)
        {
            BsonDocument Inventory = FoundPlayerDocument.GetValue("Inventory").AsBsonDocument;

            BsonArray OwnedUnitsArray = Inventory.GetValue("OwnedUnits").AsBsonArray;
            
            foreach (UnitInfo ActiveSquad in Player.Inventory.DicOwnedUnit.Values)
            {
                OwnedUnitsArray.Add(ActiveSquad.Leader.RelativePath);
                OwnedUnitsArray.Add(ActiveSquad.QuantityOwned);
            }

            BsonArray OwnedCharactersArray = Inventory.GetValue("OwnedCharacters").AsBsonArray;

            foreach (CharacterInfo ActiveCharacter in Player.Inventory.DicOwnedCharacter.Values)
            {
                OwnedCharactersArray.Add(ActiveCharacter.Pilot.FullName);
                OwnedCharactersArray.Add(ActiveCharacter.QuantityOwned);
            }

            BsonArray OwnedMissionsArray = Inventory.GetValue("OwnedMissions").AsBsonArray;
            foreach (MissionInfo ActiveMission in Player.Inventory.DicOwnedMission.Values)
            {
                OwnedMissionsArray.Add(ActiveMission.MapPath);
                OwnedMissionsArray.Add(ActiveMission.QuantityOwned);
            }

            BsonArray SquadLoadoutsArray = Inventory.GetValue("SquadLoadouts").AsBsonArray;
            foreach (PlayerLoadout ActiveLoadout in Player.Inventory.ListSquadLoadout)
            {
                BsonArray LoadoutArray = new BsonArray();
                foreach (Squad ActiveSquad in ActiveLoadout.ListSpawnSquad)
                {
                    BsonDocument NewSquadLoadout = new BsonDocument();
                    NewSquadLoadout.Add("RelativePath", ActiveSquad.CurrentLeader.RelativePath);
                    NewSquadLoadout.Add("PilotFullName", ActiveSquad.CurrentLeader.Pilot.FullName);
                    LoadoutArray.Add(NewSquadLoadout);
                }

                BsonDocument ActiveSquadDocument = new BsonDocument();

                ActiveSquadDocument.Add("Name", ActiveLoadout.Name);
                ActiveSquadDocument.Add("Squads", LoadoutArray);
                SquadLoadoutsArray.Add(ActiveSquadDocument);
            }
        }

        private void SetUnlocksInformation(BsonDocument FoundPlayerDocument, BattleMapPlayer Player)
        {
            BsonDocument Inventory = FoundPlayerDocument.GetValue("Unlocks").AsBsonDocument;

            BsonArray UnlockedCharactersArray = Inventory.GetValue("Characters").AsBsonArray;
            BsonArray UnlockedUnitsArray = Inventory.GetValue("Units").AsBsonArray;
            BsonArray UnlockedMissionsArray = Inventory.GetValue("Missions").AsBsonArray;

            foreach (UnlockableCharacter ActiveCharacter in Player.UnlockInventory.ListUnlockedCharacter)
            {
                UnlockedCharactersArray.Add(ActiveCharacter.Path);
            }

            foreach (UnlockableUnit ActiveUnit in Player.UnlockInventory.RootUnitContainer.ListUnlockedUnit)
            {
                UnlockedUnitsArray.Add(ActiveUnit.Path);
            }

            foreach (UnlockableMission ActiveMission in Player.UnlockInventory.ListUnlockedMission)
            {
                UnlockedMissionsArray.Add(ActiveMission.Path);
            }
        }

        private void SetRecordsInformation( BsonDocument FoundPlayerDocument, BattleMapPlayer Player)
        {
            BsonDocument Records = FoundPlayerDocument.GetValue("Records").AsBsonDocument;

            SetUnitRecordsInformation(Records, Player);
            SetBonusRecordsInformation(Records, Player);

            SetCampaignLevelInformationInformation(Records, Player);
            SetMapsInformation(Records, Player);
        }

        private void SetUnitRecordsInformation(BsonDocument Records, BattleMapPlayer Player)
        {
            BsonDocument UnitRecords = Records.GetValue("UnitRecords").AsBsonDocument;

            BsonArray CharacterIDByNumberOfKillsArray = UnitRecords.GetValue("CharacterIDByNumberOfKills").AsBsonArray;

            foreach (KeyValuePair<string, uint> CharacterIDAndKills in Player.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", CharacterIDAndKills.Key);
                CharacterIDAndKillsDocuement.Add("Kills", CharacterIDAndKills.Value);

                CharacterIDByNumberOfKillsArray.Add(CharacterIDAndKillsDocuement);
            }

            BsonArray CharacterIDByNumberOfDeathArray = UnitRecords.GetValue("CharacterIDByNumberOfKills").AsBsonArray;

            foreach (KeyValuePair<string, uint> CharacterIDAndDeaths in Player.Records.PlayerUnitRecords.DicCharacterIDByNumberOfDeaths)
            {
                BsonDocument CharacterIDAndDeathsDocuement = new BsonDocument();
                CharacterIDAndDeathsDocuement.Add("Path", CharacterIDAndDeaths.Key);
                CharacterIDAndDeathsDocuement.Add("Kills", CharacterIDAndDeaths.Value);

                CharacterIDByNumberOfDeathArray.Add(CharacterIDAndDeathsDocuement);
            }

            BsonArray CharacterIDByNumberOfUsesArray = UnitRecords.GetValue("CharacterIDByNumberOfUses").AsBsonArray;

            foreach (KeyValuePair<string, uint> CharacterIDAndUses in Player.Records.PlayerUnitRecords.DicCharacterIDByNumberOfUses)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", CharacterIDAndUses.Key);
                CharacterIDAndKillsDocuement.Add("Uses", CharacterIDAndUses.Value);

                CharacterIDByNumberOfUsesArray.Add(CharacterIDAndKillsDocuement);
            }

            BsonArray CharacterIDByTurnsOnFieldArray = UnitRecords.GetValue("CharacterIDByTurnsOnField").AsBsonArray;

            foreach (KeyValuePair<string, uint> CharacterIDAndTurns in Player.Records.PlayerUnitRecords.DicCharacterIDByTurnsOnField)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", CharacterIDAndTurns.Key);
                CharacterIDAndKillsDocuement.Add("Turns", CharacterIDAndTurns.Value);

                CharacterIDByTurnsOnFieldArray.Add(CharacterIDAndKillsDocuement);
            }

            BsonArray CharacterIDByNumberOfTilesTraveledArray = UnitRecords.GetValue("CharacterIDByNumberOfTilesTraveled").AsBsonArray;

            foreach (KeyValuePair<string, uint> CharacterIDAndTurns in Player.Records.PlayerUnitRecords.DicCharacterIDByNumberOfTilesTraveled)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", CharacterIDAndTurns.Key);
                CharacterIDAndKillsDocuement.Add("Tiles", CharacterIDAndTurns.Value);

                CharacterIDByNumberOfTilesTraveledArray.Add(CharacterIDAndKillsDocuement);
            }

            BsonArray UnitIDByNumberOfKillsArray = UnitRecords.GetValue("UnitIDByNumberOfKills").AsBsonArray;

            foreach (KeyValuePair<string, uint> UnitIDAndKills in Player.Records.PlayerUnitRecords.DicUnitIDByNumberOfKills)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", UnitIDAndKills.Key);
                CharacterIDAndKillsDocuement.Add("Kills", UnitIDAndKills.Value);

                UnitIDByNumberOfKillsArray.Add(CharacterIDAndKillsDocuement);
            }

            BsonArray UnitIDByNumberOfDeathsArray = UnitRecords.GetValue("UnitIDByNumberOfKills").AsBsonArray;

            foreach (KeyValuePair<string, uint> UnitIDAndDeaths in Player.Records.PlayerUnitRecords.DicUnitIDByNumberOfDeaths)
            {
                BsonDocument CharacterIDAndDeathsDocuement = new BsonDocument();
                CharacterIDAndDeathsDocuement.Add("Path", UnitIDAndDeaths.Key);
                CharacterIDAndDeathsDocuement.Add("Kills", UnitIDAndDeaths.Value);

                UnitIDByNumberOfDeathsArray.Add(CharacterIDAndDeathsDocuement);
            }

            BsonArray UnitIDByNumberOfUsesArray = UnitRecords.GetValue("UnitIDByNumberOfUses").AsBsonArray;

            foreach (KeyValuePair<string, uint> UnitIDAndUses in Player.Records.PlayerUnitRecords.DicCharacterIDByNumberOfTilesTraveled)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", UnitIDAndUses.Key);
                CharacterIDAndKillsDocuement.Add("Uses", UnitIDAndUses.Value);

                UnitIDByNumberOfUsesArray.Add(CharacterIDAndKillsDocuement);
            }

            BsonArray UnitIDByTurnsOnFieldArray = UnitRecords.GetValue("UnitIDByTurnsOnField").AsBsonArray;

            foreach (KeyValuePair<string, uint> UnitIDAndTurns in Player.Records.PlayerUnitRecords.DicCharacterIDByNumberOfTilesTraveled)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", UnitIDAndTurns.Key);
                CharacterIDAndKillsDocuement.Add("Turns", UnitIDAndTurns.Value);

                UnitIDByTurnsOnFieldArray.Add(CharacterIDAndKillsDocuement);
            }

            BsonArray UnitIDByNumberOfTilesTraveledArray = UnitRecords.GetValue("UnitIDByNumberOfTilesTraveled").AsBsonArray;

            foreach (KeyValuePair<string, uint> UnitIDAndTiles in Player.Records.PlayerUnitRecords.DicCharacterIDByNumberOfTilesTraveled)
            {
                BsonDocument CharacterIDAndKillsDocuement = new BsonDocument();
                CharacterIDAndKillsDocuement.Add("Path", UnitIDAndTiles.Key);
                CharacterIDAndKillsDocuement.Add("Tiles", UnitIDAndTiles.Value);

                UnitIDByNumberOfTilesTraveledArray.Add(CharacterIDAndKillsDocuement);
            }
        }

        private void SetBonusRecordsInformation(BsonDocument Records, BattleMapPlayer Player)
        {
            BsonDocument BonusRecords = Records.GetValue("BonusRecords").AsBsonDocument;
            BsonArray NumberOfBonusObtainedByNameArray = BonusRecords.GetValue("NumberOfBonusObtainedByName").AsBsonArray;

            foreach (KeyValuePair<string, uint> NumberOfBonusObtainedByName in Player.Records.PlayerBonusRecords.DicNumberOfBonusObtainedByName)
            {
                BsonDocument NumberOfBonusObtainedByNameDocument = new BsonDocument();
                NumberOfBonusObtainedByNameDocument.Add("Name", NumberOfBonusObtainedByName.Key);
                NumberOfBonusObtainedByNameDocument.Add("Count", NumberOfBonusObtainedByName.Value);

                NumberOfBonusObtainedByNameArray.Add(NumberOfBonusObtainedByNameDocument);
            }
        }

        private void SetCampaignLevelInformationInformation(BsonDocument Records, BattleMapPlayer Player)
        {
            BsonDocument MultiplayerInformation = Records.GetValue("MapRecords").AsBsonDocument;
            BsonArray CampaignLevelInformation = MultiplayerInformation.GetValue("CampaignLevelsInformation").AsBsonArray;

            foreach (KeyValuePair<string, CampaignRecord> LevelInformation in Player.Records.DicCampaignLevelInformation)
            {
                BsonDocument NumberOfBonusObtainedByNameDocument = new BsonDocument();
                NumberOfBonusObtainedByNameDocument.Add("Name", LevelInformation.Value.Name);
                NumberOfBonusObtainedByNameDocument.Add("FirstCompletionDate", LevelInformation.Value.FirstCompletionDate.UtcDateTime);
                NumberOfBonusObtainedByNameDocument.Add("MaxScore", LevelInformation.Value.MaxScore);

                CampaignLevelInformation.Add(NumberOfBonusObtainedByNameDocument);
            }
        }

        private void SetMapsInformation(BsonDocument Records, BattleMapPlayer Player)
        {
            BsonDocument MultiplayerInformation = Records.GetValue("MapRecords").AsBsonDocument;
            BsonArray CampaignLevelInformation = MultiplayerInformation.GetValue("MapsInformation").AsBsonArray;

            foreach (MapRecord LevelInformation in Player.Records.DicMapRecord.Values)
            {
                BsonDocument NumberOfBonusObtainedByNameDocument = new BsonDocument();
                NumberOfBonusObtainedByNameDocument.Add("Name", LevelInformation.Name);
                NumberOfBonusObtainedByNameDocument.Add("TotalSecondsPlayed", LevelInformation.TotalSecondsPlayed);
                NumberOfBonusObtainedByNameDocument.Add("TotalTimesPlayed", LevelInformation.TotalTimesPlayed);
                NumberOfBonusObtainedByNameDocument.Add("TotalVictories", LevelInformation.TotalVictories);
                NumberOfBonusObtainedByNameDocument.Add("TotalUnitsKilled", LevelInformation.TotalUnitsKilled);
                NumberOfBonusObtainedByNameDocument.Add("TotalDamageGiven", LevelInformation.TotalDamageGiven);

                CampaignLevelInformation.Add(NumberOfBonusObtainedByNameDocument);
            }
        }
    }
}
