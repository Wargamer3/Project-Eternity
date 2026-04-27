using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class LifeSimTerrainHolder
    {
        public List<LifeSimTerrainType> ListTerrainType;

        public LifeSimTerrainHolder()
        {
            ListTerrainType = new List<LifeSimTerrainType>();
        }

        public void LoadData()
        {
            if (!File.Exists("Content/Life Sim/Life Sim Terrains.bin"))
            {
                FileStream FSC = new FileStream("Content/Life Sim/Life Sim Terrains.bin", FileMode.Create);
                BinaryWriter BW = new BinaryWriter(FSC, Encoding.Unicode);

                ListTerrainType.Add(new LifeSimTerrainType("Land"));

                Save(BW);

                BW.Flush();
                BW.Close();
                FSC.Close();
            }

            FileStream FS = new FileStream("Content/Life Sim/Life Sim Terrains.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);

            int ListMoveTypeCount = BR.ReadInt32();
            ListTerrainType = new List<LifeSimTerrainType>(ListMoveTypeCount);
            for (int i = 0; i < ListMoveTypeCount; ++i)
            {
                ListTerrainType.Add(new LifeSimTerrainType(BR));
            }

            BR.Close();
            FS.Close();
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListTerrainType.Count);
            foreach (LifeSimTerrainType ActiveMoveType in ListTerrainType)
            {
                ActiveMoveType.Save(BW);
            }
        }

        public string[] GetTerrainTypes()
        {
            string[] ArrayTerrainType = new string[ListTerrainType.Count];
            for (int T = 0; T < ListTerrainType.Count; ++T)
            {
                ArrayTerrainType[T] = ListTerrainType[T].TerrainType;
            }

            return ArrayTerrainType;
        }
    }

    public class LifeSimTerrainType
    {
        public string TerrainType;
        public List<string> ListPassiveSkill;

        public LifeSimTerrainType(string TerrainType)
        {
            this.TerrainType = TerrainType;
            ListPassiveSkill = new List<string>();
        }

        public LifeSimTerrainType(BinaryReader BR)
        {
            TerrainType = BR.ReadString();
            int ListPassiveSkillCount = BR.ReadByte();
            ListPassiveSkill = new List<string>(ListPassiveSkillCount);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(TerrainType);
            BW.Write((byte)ListPassiveSkill.Count);
        }
    }
}
