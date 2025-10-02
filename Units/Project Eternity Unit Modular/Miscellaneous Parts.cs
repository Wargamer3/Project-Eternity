using System.Text;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Units.Modular
{
    public enum PartTypes { HeadAntena = 0, HeadEars = 1, HeadEyes = 2, HeadCPU = 3, TorsoCore = 4, TorsoRadiator = 5, Shell = 11, Strength = 12 };

    public class Part : ShopItem
    {
        //Unit boosts.
        public int BaseHP, BaseEN, BaseArmor, BaseMobility;

        public float BaseMovement;
        //Pilot boosts.
        public int MEL, RNG, DEF, SKL, EVA, HIT;

        public PartTypes PartType;

        public Part(Part CopyPart)
        {
        }

        public Part(string RelativePath, PartTypes PartType)
            : base(RelativePath)
        {
            string FullPath = RelativePath;
            switch (PartType)
            {
                case PartTypes.HeadAntena:
                    FullPath = "Head/Antena/" + RelativePath;
                    break;
                case PartTypes.HeadEars:
                    FullPath = "Head/Ears/" + RelativePath;
                    break;
                case PartTypes.HeadEyes:
                    FullPath = "Head/Eyes/" + RelativePath;
                    break;
                case PartTypes.HeadCPU:
                    FullPath = "Head/CPU/" + RelativePath;
                    break;
                case PartTypes.TorsoCore:
                    FullPath = "Torso/Core/" + RelativePath;
                    break;
                case PartTypes.TorsoRadiator:
                    FullPath = "Torso/Radiator/" + RelativePath;
                    break;
            }

            FileStream FS = new FileStream("Content/Deathmatch/Units/Modular/" + FullPath + ".pep", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            this.PartType = PartType;
            this.Description = BR.ReadString();
            this.Price = BR.ReadInt32();

            this.BaseHP = BR.ReadInt32();
            this.BaseEN = BR.ReadInt32();
            this.BaseArmor = BR.ReadInt32();
            this.BaseMobility = BR.ReadInt32();
            this.BaseMovement = BR.ReadSingle();

            this.MEL = BR.ReadInt32();
            this.RNG = BR.ReadInt32();
            this.DEF = BR.ReadInt32();
            this.SKL = BR.ReadInt32();
            this.EVA = BR.ReadInt32();
            this.HIT = BR.ReadInt32();

            FS.Close();
            BR.Close();
        }
    }
}
