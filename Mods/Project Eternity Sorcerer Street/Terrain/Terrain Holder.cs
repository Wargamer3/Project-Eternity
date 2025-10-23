using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetTerrainHolder
    {
        public List<string> ListTerrainType;

        public SorcererStreetTerrainHolder()
        {
            ListTerrainType = new List<string>();
        }

        public void LoadData()
        {
            FileStream FS = new FileStream("Content/Sorcerer Street Terrains.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);

            int ListMoveTypeCount = BR.ReadInt32();
            ListTerrainType = new List<string>(ListMoveTypeCount);
            for (int i = 0; i < ListMoveTypeCount; ++i)
            {
                ListTerrainType.Add(BR.ReadString());
            }

            BR.Close();
            FS.Close();
        }
    }
}
