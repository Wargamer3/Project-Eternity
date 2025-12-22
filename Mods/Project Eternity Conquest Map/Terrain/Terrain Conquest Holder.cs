using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestTerrainHolder
    {
        public List<string> ListMoveType;
        public List<ConquestTerrainTypeAttributes> ListConquestTerrainType;

        public ConquestTerrainHolder()
        {
            ListMoveType = new List<string>();
            ListConquestTerrainType = new List<ConquestTerrainTypeAttributes>();
        }

        public void LoadData()
        {
            FileStream FS = new FileStream("Content/Conquest Terrains And Movements.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);

            int ListMoveTypeCount = BR.ReadInt32();
            ListMoveType = new List<string>(ListMoveTypeCount);
            for (int i = 0; i < ListMoveTypeCount; ++i)
            {
                ListMoveType.Add(BR.ReadString());
            }

            int ListConquestTerrainTypeCount = BR.ReadInt32();
            ListConquestTerrainType = new List<ConquestTerrainTypeAttributes>(ListConquestTerrainTypeCount);
            for (int i = 0; i < ListConquestTerrainTypeCount; ++i)
            {
                ListConquestTerrainType.Add(new ConquestTerrainTypeAttributes(BR));
            }

            BR.Close();
            FS.Close();
        }
    }
}
