using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PlayerRecords
    {
        public double TotalSecondsPlayed;

        public uint TotalKills;
        public uint TotalTurnPlayed;
        public uint TotalTilesTraveled;

        private uint _CurrentMoney;
        public uint CurrentMoney { get { return _CurrentMoney; }
            set { if (value > _CurrentMoney) TotalMoney += value -_CurrentMoney; _CurrentMoney = value; } }
        public uint TotalMoney;

        private uint _CurrentCoins;
        public uint CurrentCoins { get { return _CurrentCoins; } set { if (value > _CurrentCoins) TotalCoins += _CurrentCoins - value; _CurrentCoins = value; } }
        public uint TotalCoins;

        public UnitRecords PlayerUnitRecords;
        public BattleRecords PlayerBattleRecords;
        public BonusRecords PlayerBonusRecords;

        public Dictionary<string, CampaignRecord> DicCampaignLevelInformation;
        public Dictionary<string, MapRecord> DicMapRecord;

        public PlayerRecords()
        {
            TotalSecondsPlayed = 0;

            TotalKills = 0;
            TotalTurnPlayed = 0;
            TotalTilesTraveled = 0;

            _CurrentMoney = 0;
            TotalMoney = 0;

            _CurrentCoins = 0;
            TotalCoins = 0;

            PlayerUnitRecords = new UnitRecords();
            PlayerBattleRecords = new BattleRecords();
            PlayerBonusRecords = new BonusRecords();

            DicCampaignLevelInformation = new Dictionary<string, CampaignRecord>();
            DicMapRecord = new Dictionary<string, MapRecord>();
        }

        public PlayerRecords(PlayerRecords Clone)
        {
            TotalSecondsPlayed = Clone.TotalSecondsPlayed;

            TotalKills = Clone.TotalKills;
            TotalTurnPlayed = Clone.TotalTurnPlayed;
            TotalTilesTraveled = Clone.TotalTilesTraveled;

            _CurrentMoney = Clone._CurrentMoney;
            TotalMoney = Clone.TotalMoney;

            _CurrentCoins = Clone._CurrentCoins;
            TotalCoins = Clone.TotalMoney;

            PlayerUnitRecords = new UnitRecords(Clone.PlayerUnitRecords);
            PlayerBattleRecords = new BattleRecords(Clone.PlayerBattleRecords);
            PlayerBonusRecords = new BonusRecords(Clone.PlayerBonusRecords);

            DicCampaignLevelInformation = new Dictionary<string, CampaignRecord>(Clone.DicCampaignLevelInformation.Count);
            foreach (KeyValuePair<string, CampaignRecord> ActiveCloneRecord in Clone.DicCampaignLevelInformation)
            {
                DicCampaignLevelInformation.Add(ActiveCloneRecord.Key, new CampaignRecord(ActiveCloneRecord.Value));
            }

            DicMapRecord = new Dictionary<string, MapRecord>(Clone.DicMapRecord.Count);
            foreach (MapRecord ActiveCloneRecord in Clone.DicMapRecord.Values)
            {
                DicMapRecord.Add(ActiveCloneRecord.Name, new MapRecord(ActiveCloneRecord));
            }
        }

        public void Load(BinaryReader BR)
        {
            TotalSecondsPlayed = BR.ReadDouble();

            TotalKills = BR.ReadUInt32();
            TotalTurnPlayed = BR.ReadUInt32();
            TotalTilesTraveled = BR.ReadUInt32();

            _CurrentMoney = BR.ReadUInt32();
            TotalMoney = BR.ReadUInt32();

            _CurrentCoins = BR.ReadUInt32();
            TotalCoins = BR.ReadUInt32();

            PlayerUnitRecords = new UnitRecords(BR);
            PlayerBattleRecords = new BattleRecords(BR);
            PlayerBonusRecords = new BonusRecords(BR);

            int ListCampaignLevelInformationCount = BR.ReadInt32();
            DicCampaignLevelInformation = new Dictionary<string, CampaignRecord>(ListCampaignLevelInformationCount);
            for (int i = 0; i < ListCampaignLevelInformationCount; ++i)
            {
                CampaignRecord LoadedCampaignRecord = new CampaignRecord(BR);
                DicCampaignLevelInformation.Add(LoadedCampaignRecord.Name, LoadedCampaignRecord);
            }

            int ListMapRecordCount = BR.ReadInt32();
            DicMapRecord = new Dictionary<string, MapRecord>(ListMapRecordCount);
            for (int i = 0; i < ListMapRecordCount; ++i)
            {
                MapRecord LoadMapRecord = new MapRecord(BR);
                DicMapRecord.Add(LoadMapRecord.Name, LoadMapRecord);
            }
        }

        internal void Load(ByteReader BR)
        {
            TotalSecondsPlayed = BR.ReadDouble();

            TotalKills = BR.ReadUInt32();
            TotalTurnPlayed = BR.ReadUInt32();
            TotalTilesTraveled = BR.ReadUInt32();

            _CurrentMoney = BR.ReadUInt32();
            TotalMoney = BR.ReadUInt32();

            _CurrentCoins = BR.ReadUInt32();
            TotalCoins = BR.ReadUInt32();

            PlayerUnitRecords = new UnitRecords(BR);
            PlayerBattleRecords = new BattleRecords(BR);
            PlayerBonusRecords = new BonusRecords(BR);

            int ListCampaignLevelInformationCount = BR.ReadInt32();
            DicCampaignLevelInformation = new Dictionary<string, CampaignRecord>(ListCampaignLevelInformationCount);
            for (int i = 0; i < ListCampaignLevelInformationCount; ++i)
            {
                CampaignRecord LoadedCampaignRecord = new CampaignRecord(BR);
                DicCampaignLevelInformation.Add(LoadedCampaignRecord.Name, LoadedCampaignRecord);
            }

            int ListMapRecordCount = BR.ReadInt32();
            DicMapRecord = new Dictionary<string, MapRecord>(ListMapRecordCount);
            for (int i = 0; i < ListMapRecordCount; ++i)
            {
                MapRecord LoadMapRecord = new MapRecord(BR);
                DicMapRecord.Add(LoadMapRecord.Name, LoadMapRecord);
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(TotalSecondsPlayed);

            BW.Write(TotalKills);
            BW.Write(TotalTurnPlayed);
            BW.Write(TotalTilesTraveled);

            BW.Write(_CurrentMoney);
            BW.Write(TotalMoney);

            BW.Write(_CurrentCoins);
            BW.Write(TotalCoins);

            PlayerUnitRecords.Save(BW);
            PlayerBattleRecords.Save(BW);
            PlayerBonusRecords.Save(BW);

            BW.Write(DicCampaignLevelInformation.Count);
            foreach (KeyValuePair<string, CampaignRecord> ActiveLevel in DicCampaignLevelInformation)
            {
                ActiveLevel.Value.Save(BW);
            }

            BW.Write(DicMapRecord.Count);
            foreach (MapRecord ActiveMapRecord in DicMapRecord.Values)
            {
                ActiveMapRecord.Save(BW);
            }
        }

        public void AddCharacterKill(string ID)
        {
            PlayerUnitRecords.AddCharacterKill(ID);
        }

        public void AddCharacterDeath(string ID)
        {
            PlayerUnitRecords.AddCharacterDeath(ID);
        }

        public void AddUnitDamageGiven(string MapName, string ID, uint Damage)
        {
            PlayerUnitRecords.AddUnitDamageGiven(ID, Damage);

            PlayerBattleRecords.TotalDamageGiven += Damage;

            MapRecord FoundMapRecord = GetMapRecord(MapName);

            FoundMapRecord.TotalDamageGiven += Damage;
        }

        public void AddUnitDamageReceived(uint Damage)
        {
            PlayerBattleRecords.TotalDamageReceived += Damage;
        }

        public void AddUnitKill(string MapName, string ID)
        {
            TotalKills++;
            PlayerUnitRecords.AddUnitKill(ID);

            MapRecord FoundMapRecord = GetMapRecord(MapName);

            FoundMapRecord.TotalUnitsKilled++;
        }

        public void AddUnitDeath(string ID)
        {
            PlayerUnitRecords.AddUnitDeath(ID);
        }

        private MapRecord GetMapRecord(string MapName)
        {
            MapRecord FoundMapRecord;

            if (!DicMapRecord.TryGetValue(MapName, out FoundMapRecord))
            {
                FoundMapRecord = new MapRecord(MapName);
                DicMapRecord.Add(MapName, FoundMapRecord);
            }

            return FoundMapRecord;
        }
    }
}
