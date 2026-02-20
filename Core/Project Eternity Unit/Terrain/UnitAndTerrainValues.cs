using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public class UnitAndTerrainValues
    {
        public static UnitAndTerrainValues Default = new UnitAndTerrainValues();

        public List<string> ListUnitType = new List<string>();
        public List<TerrainType> ListTerrainType = new List<TerrainType>();

        public void Load()
        {
            if (!File.Exists("Content/Unit and Terrain Types.bin"))
            {
                Save();
            }

            FileStream FS = new FileStream("Content/Unit and Terrain Types.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            int ListUnitTypeCount = BR.ReadInt32();
            for (int T = 0; T < ListUnitTypeCount; ++T)
            {
                ListUnitType.Add(BR.ReadString());
            }

            int ListTerrainTypeCount = BR.ReadInt32();
            for (int T = 0; T < ListTerrainTypeCount; ++T)
            {
                ListTerrainType.Add(new TerrainType(BR, this));
            }

            FS.Close();
            BR.Close();
        }

        public void Save()
        {
            FileStream FS = new FileStream("Content/Unit and Terrain Types.bin", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(ListUnitType.Count);
            for (int T = 0; T < ListUnitType.Count; ++T)
            {
                BW.Write(ListUnitType[T]);
            }

            BW.Write(ListTerrainType.Count);
            for (int T = 0; T < ListTerrainType.Count; ++T)
            {
                ListTerrainType[T].Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        public bool CanMove(UnitMapComponent Unit, UnitStats Stats, byte TerrainTypeIndex)
        {
            return ListTerrainType[TerrainTypeIndex].CanMove(Unit, Stats);
        }

        public float GetMVCost(UnitMapComponent Unit, UnitStats Stats, byte TerrainTypeIndex, byte TerrainTypeIndexNext)
        {
            if (TerrainTypeIndex != TerrainTypeIndexNext)
            {
                return ListTerrainType[TerrainTypeIndexNext].GetEntryCost(Unit, Stats);
            }

            return ListTerrainType[TerrainTypeIndexNext].GetMovementCost(Unit, Stats);
        }

        public float GetENCost(UnitMapComponent Unit, UnitStats Stats, byte TerrainTypeIndex)
        {
            return ListTerrainType[TerrainTypeIndex].GetENCost(Unit, Stats);
        }

        public int GetENUsedPerTurnCost(UnitMapComponent Unit, UnitStats Stats, byte TerrainTypeIndex)
        {
            return ListTerrainType[TerrainTypeIndex].GetENUsedPerTurnCost(Unit, Stats);
        }
    }
}
