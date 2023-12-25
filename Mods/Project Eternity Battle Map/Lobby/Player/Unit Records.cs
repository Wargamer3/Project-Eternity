using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnitRecords
    {
        public Dictionary<string, uint> DicCharacterIDByNumberOfKills;
        public Dictionary<string, uint> DicCharacterIDByTotalDamageGiven;
        public Dictionary<string, uint> DicCharacterIDByNumberOfDeaths;
        public Dictionary<string, uint> DicCharacterIDByNumberOfUses;
        public Dictionary<string, uint> DicCharacterIDByTurnsOnField;
        public Dictionary<string, uint> DicCharacterIDByNumberOfTilesTraveled;

        public Dictionary<string, uint> DicUnitIDByNumberOfKills;
        public Dictionary<string, uint> DicUnitIDByTotalDamageGiven;
        public Dictionary<string, uint> DicUnitIDByNumberOfDeaths;
        public Dictionary<string, uint> DicUnitIDByNumberOfUses;
        public Dictionary<string, uint> DicUnitIDByTurnsOnField;
        public Dictionary<string, uint> DicUnitIDByNumberOfTilesTraveled;

        public UnitRecords()
        {
            DicCharacterIDByNumberOfKills = new Dictionary<string, uint>();
            DicCharacterIDByTotalDamageGiven = new Dictionary<string, uint>();
            DicCharacterIDByNumberOfDeaths = new Dictionary<string, uint>();
            DicCharacterIDByNumberOfUses = new Dictionary<string, uint>();
            DicCharacterIDByTurnsOnField = new Dictionary<string, uint>();
            DicCharacterIDByNumberOfTilesTraveled = new Dictionary<string, uint>();

            DicUnitIDByNumberOfKills = new Dictionary<string, uint>();
            DicUnitIDByTotalDamageGiven = new Dictionary<string, uint>();
            DicUnitIDByNumberOfDeaths = new Dictionary<string, uint>();
            DicUnitIDByNumberOfUses = new Dictionary<string, uint>();
            DicUnitIDByTurnsOnField = new Dictionary<string, uint>();
            DicUnitIDByNumberOfTilesTraveled = new Dictionary<string, uint>();
        }

        public UnitRecords(BinaryReader BR)
        {
            #region Characters

            int DicCharacterIDByNumberOfKillsCount = BR.ReadInt32();
            DicCharacterIDByNumberOfKills = new Dictionary<string, uint>(DicCharacterIDByNumberOfKillsCount);
            for (int i = 0; i < DicCharacterIDByNumberOfKillsCount; ++i)
            {
                DicCharacterIDByNumberOfKills.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByTotalDamageGivenCount = BR.ReadInt32();
            DicCharacterIDByTotalDamageGiven = new Dictionary<string, uint>(DicCharacterIDByTotalDamageGivenCount);
            for (int i = 0; i < DicCharacterIDByTotalDamageGivenCount; ++i)
            {
                DicCharacterIDByTotalDamageGiven.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByNumberOfDeathsCount = BR.ReadInt32();
            DicCharacterIDByNumberOfDeaths = new Dictionary<string, uint>(DicCharacterIDByNumberOfDeathsCount);
            for (int i = 0; i < DicCharacterIDByNumberOfDeathsCount; ++i)
            {
                DicCharacterIDByNumberOfDeaths.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByNumberOfUsesCount = BR.ReadInt32();
            DicCharacterIDByNumberOfUses = new Dictionary<string, uint>(DicCharacterIDByNumberOfUsesCount);
            for (int i = 0; i < DicCharacterIDByNumberOfUsesCount; ++i)
            {
                DicCharacterIDByNumberOfUses.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByTurnsOnFieldCount = BR.ReadInt32();
            DicCharacterIDByTurnsOnField = new Dictionary<string, uint>(DicCharacterIDByTurnsOnFieldCount);
            for (int i = 0; i < DicCharacterIDByTurnsOnFieldCount; ++i)
            {
                DicCharacterIDByTurnsOnField.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByNumberOfTilesTraveledCount = BR.ReadInt32();
            DicCharacterIDByNumberOfTilesTraveled = new Dictionary<string, uint>(DicCharacterIDByNumberOfTilesTraveledCount);
            for (int i = 0; i < DicCharacterIDByNumberOfTilesTraveledCount; ++i)
            {
                DicCharacterIDByNumberOfTilesTraveled.Add(BR.ReadString(), BR.ReadUInt32());
            }

            #endregion

            #region Units

            int DicUnitIDByNumberOfKillsCount = BR.ReadInt32();
            DicUnitIDByNumberOfKills = new Dictionary<string, uint>(DicUnitIDByNumberOfKillsCount);
            for (int i = 0; i < DicUnitIDByNumberOfKillsCount; ++i)
            {
                DicUnitIDByNumberOfKills.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByTotalDamageGivenCount = BR.ReadInt32();
            DicUnitIDByTotalDamageGiven = new Dictionary<string, uint>(DicUnitIDByTotalDamageGivenCount);
            for (int i = 0; i < DicUnitIDByTotalDamageGivenCount; ++i)
            {
                DicUnitIDByTotalDamageGiven.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByNumberOfDeathsCount = BR.ReadInt32();
            DicUnitIDByNumberOfDeaths = new Dictionary<string, uint>(DicUnitIDByNumberOfDeathsCount);
            for (int i = 0; i < DicUnitIDByNumberOfDeathsCount; ++i)
            {
                DicUnitIDByNumberOfDeaths.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByNumberOfUsesCount = BR.ReadInt32();
            DicUnitIDByNumberOfUses = new Dictionary<string, uint>(DicUnitIDByNumberOfUsesCount);
            for (int i = 0; i < DicUnitIDByNumberOfUsesCount; ++i)
            {
                DicUnitIDByNumberOfUses.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByTurnsOnFieldCount = BR.ReadInt32();
            DicUnitIDByTurnsOnField = new Dictionary<string, uint>(DicUnitIDByTurnsOnFieldCount);
            for (int i = 0; i < DicUnitIDByTurnsOnFieldCount; ++i)
            {
                DicUnitIDByTurnsOnField.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitNameByNumberOfTilesTraveledCount = BR.ReadInt32();
            DicUnitIDByNumberOfTilesTraveled = new Dictionary<string, uint>(DicUnitNameByNumberOfTilesTraveledCount);
            for (int i = 0; i < DicUnitNameByNumberOfTilesTraveledCount; ++i)
            {
                DicUnitIDByNumberOfTilesTraveled.Add(BR.ReadString(), BR.ReadUInt32());
            }

            #endregion
        }

        public UnitRecords(ByteReader BR)
        {
            #region Characters

            int DicCharacterIDByNumberOfKillsCount = BR.ReadInt32();
            DicCharacterIDByNumberOfKills = new Dictionary<string, uint>(DicCharacterIDByNumberOfKillsCount);
            for (int i = 0; i < DicCharacterIDByNumberOfKillsCount; ++i)
            {
                DicCharacterIDByNumberOfKills.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByTotalDamageGivenCount = BR.ReadInt32();
            DicCharacterIDByTotalDamageGiven = new Dictionary<string, uint>(DicCharacterIDByTotalDamageGivenCount);
            for (int i = 0; i < DicCharacterIDByTotalDamageGivenCount; ++i)
            {
                DicCharacterIDByTotalDamageGiven.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByNumberOfDeathsCount = BR.ReadInt32();
            DicCharacterIDByNumberOfDeaths = new Dictionary<string, uint>(DicCharacterIDByNumberOfDeathsCount);
            for (int i = 0; i < DicCharacterIDByNumberOfDeathsCount; ++i)
            {
                DicCharacterIDByNumberOfDeaths.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByNumberOfUsesCount = BR.ReadInt32();
            DicCharacterIDByNumberOfUses = new Dictionary<string, uint>(DicCharacterIDByNumberOfUsesCount);
            for (int i = 0; i < DicCharacterIDByNumberOfUsesCount; ++i)
            {
                DicCharacterIDByNumberOfUses.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByTurnsOnFieldCount = BR.ReadInt32();
            DicCharacterIDByTurnsOnField = new Dictionary<string, uint>(DicCharacterIDByTurnsOnFieldCount);
            for (int i = 0; i < DicCharacterIDByTurnsOnFieldCount; ++i)
            {
                DicCharacterIDByTurnsOnField.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicCharacterIDByNumberOfTilesTraveledCount = BR.ReadInt32();
            DicCharacterIDByNumberOfTilesTraveled = new Dictionary<string, uint>(DicCharacterIDByNumberOfTilesTraveledCount);
            for (int i = 0; i < DicCharacterIDByNumberOfTilesTraveledCount; ++i)
            {
                DicCharacterIDByNumberOfTilesTraveled.Add(BR.ReadString(), BR.ReadUInt32());
            }

            #endregion

            #region Units

            int DicUnitIDByNumberOfKillsCount = BR.ReadInt32();
            DicUnitIDByNumberOfKills = new Dictionary<string, uint>(DicUnitIDByNumberOfKillsCount);
            for (int i = 0; i < DicUnitIDByNumberOfKillsCount; ++i)
            {
                DicUnitIDByNumberOfKills.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByTotalDamageGivenCount = BR.ReadInt32();
            DicUnitIDByTotalDamageGiven = new Dictionary<string, uint>(DicUnitIDByTotalDamageGivenCount);
            for (int i = 0; i < DicUnitIDByTotalDamageGivenCount; ++i)
            {
                DicUnitIDByTotalDamageGiven.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByNumberOfDeathsCount = BR.ReadInt32();
            DicUnitIDByNumberOfDeaths = new Dictionary<string, uint>(DicUnitIDByNumberOfDeathsCount);
            for (int i = 0; i < DicUnitIDByNumberOfDeathsCount; ++i)
            {
                DicUnitIDByNumberOfDeaths.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByNumberOfUsesCount = BR.ReadInt32();
            DicUnitIDByNumberOfUses = new Dictionary<string, uint>(DicUnitIDByNumberOfUsesCount);
            for (int i = 0; i < DicUnitIDByNumberOfUsesCount; ++i)
            {
                DicUnitIDByNumberOfUses.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitIDByTurnsOnFieldCount = BR.ReadInt32();
            DicUnitIDByTurnsOnField = new Dictionary<string, uint>(DicUnitIDByTurnsOnFieldCount);
            for (int i = 0; i < DicUnitIDByTurnsOnFieldCount; ++i)
            {
                DicUnitIDByTurnsOnField.Add(BR.ReadString(), BR.ReadUInt32());
            }

            int DicUnitNameByNumberOfTilesTraveledCount = BR.ReadInt32();
            DicUnitIDByNumberOfTilesTraveled = new Dictionary<string, uint>(DicUnitNameByNumberOfTilesTraveledCount);
            for (int i = 0; i < DicUnitNameByNumberOfTilesTraveledCount; ++i)
            {
                DicUnitIDByNumberOfTilesTraveled.Add(BR.ReadString(), BR.ReadUInt32());
            }

            #endregion
        }

        public UnitRecords(UnitRecords Clone)
        {
            DicCharacterIDByNumberOfKills = new Dictionary<string, uint>(Clone.DicCharacterIDByNumberOfKills);
            DicCharacterIDByTotalDamageGiven = new Dictionary<string, uint>(Clone.DicCharacterIDByTotalDamageGiven);
            DicCharacterIDByNumberOfDeaths = new Dictionary<string, uint>(Clone.DicCharacterIDByNumberOfDeaths);
            DicCharacterIDByNumberOfUses = new Dictionary<string, uint>(Clone.DicCharacterIDByNumberOfUses);
            DicCharacterIDByTurnsOnField = new Dictionary<string, uint>(Clone.DicCharacterIDByTurnsOnField);
            DicCharacterIDByNumberOfTilesTraveled = new Dictionary<string, uint>(Clone.DicCharacterIDByNumberOfTilesTraveled);

            DicUnitIDByNumberOfKills = new Dictionary<string, uint>(Clone.DicUnitIDByNumberOfKills);
            DicUnitIDByTotalDamageGiven = new Dictionary<string, uint>(Clone.DicUnitIDByTotalDamageGiven);
            DicUnitIDByNumberOfDeaths = new Dictionary<string, uint>(Clone.DicUnitIDByNumberOfDeaths);
            DicUnitIDByNumberOfUses = new Dictionary<string, uint>(Clone.DicUnitIDByNumberOfUses);
            DicUnitIDByTurnsOnField = new Dictionary<string, uint>(Clone.DicUnitIDByTurnsOnField);
            DicUnitIDByNumberOfTilesTraveled = new Dictionary<string, uint>(Clone.DicUnitIDByNumberOfTilesTraveled);
        }

        public void Save(BinaryWriter BW)
        {
            #region Characters

            BW.Write(DicCharacterIDByNumberOfKills.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicCharacterIDByNumberOfKills)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            BW.Write(DicCharacterIDByTotalDamageGiven.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicCharacterIDByTotalDamageGiven)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            BW.Write(DicCharacterIDByNumberOfDeaths.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicCharacterIDByNumberOfDeaths)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            BW.Write(DicCharacterIDByNumberOfUses.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicCharacterIDByNumberOfUses)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }
            BW.Write(DicCharacterIDByTurnsOnField.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicCharacterIDByTurnsOnField)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            BW.Write(DicCharacterIDByNumberOfTilesTraveled.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicCharacterIDByNumberOfTilesTraveled)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            #endregion

            #region Units

            BW.Write(DicUnitIDByNumberOfKills.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitIDByNumberOfKills)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            BW.Write(DicUnitIDByTotalDamageGiven.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitIDByTotalDamageGiven)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            BW.Write(DicUnitIDByNumberOfDeaths.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitIDByNumberOfDeaths)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            BW.Write(DicUnitIDByNumberOfUses.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitIDByNumberOfUses)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }
            BW.Write(DicUnitIDByTurnsOnField.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitIDByTurnsOnField)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }
            BW.Write(DicUnitIDByNumberOfTilesTraveled.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitIDByNumberOfTilesTraveled)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }

            #endregion
        }

        public void RegisterCharacter(string ID)
        {
            DicCharacterIDByNumberOfKills.Add(ID, 0);
            DicCharacterIDByTotalDamageGiven.Add(ID, 0);
            DicCharacterIDByNumberOfDeaths.Add(ID, 0);
            DicCharacterIDByNumberOfUses.Add(ID, 0);
            DicCharacterIDByTurnsOnField.Add(ID, 0);
            DicCharacterIDByNumberOfTilesTraveled.Add(ID, 0);
        }

        public void RegisterUnit(string ID)
        {
            DicUnitIDByNumberOfKills.Add(ID, 0);
            DicUnitIDByTotalDamageGiven.Add(ID, 0);
            DicUnitIDByNumberOfDeaths.Add(ID, 0);
            DicUnitIDByNumberOfUses.Add(ID, 0);
            DicUnitIDByTurnsOnField.Add(ID, 0);
            DicUnitIDByNumberOfTilesTraveled.Add(ID, 0);
        }

        internal void AddCharacterKill(string ID)
        {
            if (!DicCharacterIDByNumberOfKills.ContainsKey(ID))
            {
                RegisterCharacter(ID);
            }

            DicCharacterIDByNumberOfKills[ID] += 1;
        }

        internal void AddCharacterDeath(string ID)
        {
            if (!DicCharacterIDByNumberOfKills.ContainsKey(ID))
            {
                RegisterCharacter(ID);
            }

            DicCharacterIDByNumberOfDeaths[ID] += 1;
        }

        internal void AddUnitKill(string ID)
        {
            if (!DicUnitIDByNumberOfKills.ContainsKey(ID))
            {
                RegisterUnit(ID);
            }

            DicUnitIDByNumberOfKills[ID] += 1;
        }

        internal void AddUnitDamageGiven(string ID, uint Damage)
        {
            if (!DicUnitIDByNumberOfKills.ContainsKey(ID))
            {
                RegisterUnit(ID);
            }

            DicUnitIDByTotalDamageGiven[ID] += Damage;
        }

        internal void AddUnitDeath(string ID)
        {
            if (!DicUnitIDByNumberOfDeaths.ContainsKey(ID))
            {
                RegisterUnit(ID);
            }

            DicUnitIDByNumberOfDeaths[ID] += 1;
        }
    }
}
