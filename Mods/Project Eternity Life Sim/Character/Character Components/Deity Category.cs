using System;
using System.IO;
using System.Text;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class DeityCategory
    {
        public string Name;
        public string Description;

        public DeityCategory(string DeityPath)
        {
            FileStream FS = new FileStream("Content/Life Sim/Deity Categories/" + DeityPath + ".pedc", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            BR.Close();
            FS.Close();
        }
    }
}
