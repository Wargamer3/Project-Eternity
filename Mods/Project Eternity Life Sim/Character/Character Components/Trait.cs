using System;
using System.IO;
using System.Text;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Trait
    {
        public string Name;
        public string Description;

        public Trait()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        public Trait(string FilePath)
        {
            FileStream FS = new FileStream("Content/Life Sim/Traits/" + FilePath + ".pet", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            BR.Close();
            FS.Close();
        }
    }
}
