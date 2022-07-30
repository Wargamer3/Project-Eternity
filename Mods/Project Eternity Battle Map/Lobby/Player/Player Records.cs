using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PlayerRecords
    {
        public double TotalSecondsPlayed;

        public uint TotalKills;
        public uint TotalTurnPlayed;
        public uint TotalTilesTraveled;

        private uint _CurrentMoney;
        public uint CurrentMoney { get { return _CurrentMoney; } set { if (value > _CurrentMoney) TotalMoney += _CurrentMoney - value; _CurrentMoney = value; } }
        public uint TotalMoney;

        private uint _CurrentCoins;
        public uint CurrentCoins { get { return _CurrentCoins; } set { if (value > _CurrentCoins) TotalCoins += _CurrentCoins - value; _CurrentCoins = value; } }
        public uint TotalCoins;

        public UnitRecords PlayerUnitRecords;
        public BattleRecords PlayerBattleRecords;
        public BonusRecords PlayerBonusRecords;

        public List<MultiplayerRecord> ListCampaignLevelInformation;

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

            ListCampaignLevelInformation = new List<MultiplayerRecord>();
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

            ListCampaignLevelInformation = new List<MultiplayerRecord>(Clone.ListCampaignLevelInformation.Count);
            foreach (MultiplayerRecord ActiveCloneRecord in Clone.ListCampaignLevelInformation)
            {
                ListCampaignLevelInformation.Add(new MultiplayerRecord(ActiveCloneRecord));
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
            ListCampaignLevelInformation = new List<MultiplayerRecord>(ListCampaignLevelInformationCount);
            for (int i = 0; i < ListCampaignLevelInformationCount; ++i)
            {
                ListCampaignLevelInformation.Add(new MultiplayerRecord(BR));
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

            BW.Write(ListCampaignLevelInformation.Count);
            for (int i = 0; i < ListCampaignLevelInformation.Count; ++i)
            {
                ListCampaignLevelInformation[i].Save(BW);
            }
        }
    }
}
