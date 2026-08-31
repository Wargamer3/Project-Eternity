using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Language
    {
        public string Name;
        public string Description;

        public List<string> ListSpecialEffect;

        public Language()
        {
            Name = string.Empty;
            Description = string.Empty;

            ListSpecialEffect = new List<string>();
        }

        public Language(string FilePath)
        {
            FileStream FS = new FileStream("Content/Life Sim/Languages/" + FilePath + ".pel", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            BR.Close();
            FS.Close();
        }
    }
}
