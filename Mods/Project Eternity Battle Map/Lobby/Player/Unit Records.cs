using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnitRecords
    {
        public Dictionary<string, uint> DicCharacterIDByNumberOfKills;
        public Dictionary<string, uint> DicCharacterIDByNumberOfDeath;
        public Dictionary<string, uint> DicCharacterIDByNumberOfUses;
        public Dictionary<string, uint> DicCharacterIDByTurnsOnField;
        public Dictionary<string, uint> DicCharacterIDByNumberOfTilesTraveled;

        public Dictionary<string, uint> DicUnitIDByNumberOfKills;
        public Dictionary<string, uint> DicUnitIDByNumberOfDeath;
        public Dictionary<string, uint> DicUnitIDByNumberOfUses;
        public Dictionary<string, uint> DicUnitIDByTurnsOnField;
        public Dictionary<string, uint> DicUnitNameByNumberOfTilesTraveled;

        public UnitRecords()
        {
            DicCharacterIDByNumberOfKills = new Dictionary<string, uint>();
            DicCharacterIDByNumberOfUses = new Dictionary<string, uint>();
            DicCharacterIDByTurnsOnField = new Dictionary<string, uint>();
            DicCharacterIDByNumberOfTilesTraveled = new Dictionary<string, uint>();

            DicUnitIDByNumberOfKills = new Dictionary<string, uint>();
            DicUnitIDByNumberOfUses = new Dictionary<string, uint>();
            DicUnitIDByTurnsOnField = new Dictionary<string, uint>();
            DicUnitNameByNumberOfTilesTraveled = new Dictionary<string, uint>();
        }

        public UnitRecords(BinaryReader BR)
        {
            int DicCharacterIDByNumberOfKillsCount = BR.ReadInt32();
            DicCharacterIDByNumberOfKills = new Dictionary<string, uint>(DicCharacterIDByNumberOfKillsCount);
            for (int i = 0; i < DicCharacterIDByNumberOfKillsCount; ++i)
            {
                DicCharacterIDByNumberOfKills.Add(BR.ReadString(), BR.ReadUInt32());
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

            int DicUnitIDByNumberOfKillsCount = BR.ReadInt32();
            DicUnitIDByNumberOfKills = new Dictionary<string, uint>(DicUnitIDByNumberOfKillsCount);
            for (int i = 0; i < DicUnitIDByNumberOfKillsCount; ++i)
            {
                DicUnitIDByNumberOfKills.Add(BR.ReadString(), BR.ReadUInt32());
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
            DicUnitNameByNumberOfTilesTraveled = new Dictionary<string, uint>(DicUnitNameByNumberOfTilesTraveledCount);
            for (int i = 0; i < DicUnitNameByNumberOfTilesTraveledCount; ++i)
            {
                DicUnitNameByNumberOfTilesTraveled.Add(BR.ReadString(), BR.ReadUInt32());
            }
        }

        public UnitRecords(UnitRecords Clone)
        {
            DicCharacterIDByNumberOfKills = new Dictionary<string, uint>(Clone.DicCharacterIDByNumberOfKills);
            DicCharacterIDByNumberOfUses = new Dictionary<string, uint>(Clone.DicCharacterIDByNumberOfUses);
            DicCharacterIDByTurnsOnField = new Dictionary<string, uint>(Clone.DicCharacterIDByTurnsOnField);
            DicCharacterIDByNumberOfTilesTraveled = new Dictionary<string, uint>(Clone.DicCharacterIDByNumberOfTilesTraveled);

            DicUnitIDByNumberOfKills = new Dictionary<string, uint>(Clone.DicUnitIDByNumberOfKills);
            DicUnitIDByNumberOfUses = new Dictionary<string, uint>(Clone.DicUnitIDByNumberOfUses);
            DicUnitIDByTurnsOnField = new Dictionary<string, uint>(Clone.DicUnitIDByTurnsOnField);
            DicUnitNameByNumberOfTilesTraveled = new Dictionary<string, uint>(Clone.DicUnitNameByNumberOfTilesTraveled);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(DicCharacterIDByNumberOfKills.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicCharacterIDByNumberOfKills)
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

            BW.Write(DicUnitIDByNumberOfKills.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitIDByNumberOfKills)
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
            BW.Write(DicUnitNameByNumberOfTilesTraveled.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicUnitNameByNumberOfTilesTraveled)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }
        }

        public void RegisterCharacter(string ID)
        {
            DicCharacterIDByNumberOfKills.Add(ID, 0);
        }

        public void RegisterUnit(string ID)
        {
            DicUnitIDByNumberOfKills.Add(ID, 0);
        }

        public void AddCharacterKill(string ID)
        {
            if (!DicCharacterIDByNumberOfKills.ContainsKey(ID))
            {
                DicCharacterIDByNumberOfKills.Add(ID, 1);
            }
            else
            {
                DicCharacterIDByNumberOfKills[ID] += 1;
            }
        }

        public void AddUnitKill(string ID)
        {
            if (!DicUnitIDByNumberOfKills.ContainsKey(ID))
            {
                DicUnitIDByNumberOfKills.Add(ID, 1);
            }
            else
            {
                DicUnitIDByNumberOfKills[ID] += 1;
            }
        }
    }
}
