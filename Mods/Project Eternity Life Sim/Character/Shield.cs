using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Shield
    {
        public string Name;
        public string Description;

        public List<string> ListSpecialEffect;

        public Shield()
        {
            Name = string.Empty;
            Description = string.Empty;

            ListSpecialEffect = new List<string>();
        }

        public Shield(string ShieldPath, ContentManager Content)
        {
            FileStream FS = new FileStream("Content/Life Sim/Shields/" + ShieldPath + ".pes", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            BR.Close();
            FS.Close();
        }
    }
}
