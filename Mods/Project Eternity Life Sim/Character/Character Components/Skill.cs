using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class CharacterSkill
    {
        public string Name;
        public string Description;
        public int Level;

        public List<Unlockable> ListAction;

        public CharacterSkill()
        {
            Name = string.Empty;
            Description = string.Empty;

            ListAction = new List<Unlockable>();
        }

        public CharacterSkill(string FilePath)
        {
            FileStream FS = new FileStream("Content/Life Sim/Skills/" + FilePath + ".pes", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            byte ListActionCount = BR.ReadByte();
            ListAction = new List<Unlockable>(ListActionCount);
            for (int A = 0; A < ListActionCount; ++A)
            {
                ListAction.Add(new Unlockable(BR));
            }

            BR.Close();
            FS.Close();
        }
    }
}
