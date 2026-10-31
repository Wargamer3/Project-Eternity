using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Trait
    {
        public string Name;
        public string Description;
        public List<TraitEffect> ListEffect;

        public Trait()
        {
            Name = string.Empty;
            Description = string.Empty;
            ListEffect = new List<TraitEffect>();
        }

        public Trait(string FilePath)
        {
            FileStream FS = new FileStream("Content/Life Sim/Traits/" + FilePath + ".pet", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            Read(BR);
            BR.Close();
            FS.Close();
        }

        public void Read(BinaryReader BR)
        {
            Name = BR.ReadString();
            Description = BR.ReadString();
            ListEffect = new List<TraitEffect>();
            int ListEffectCount;
            try
            {
                ListEffectCount = BR.ReadByte();
            }
            catch (EndOfStreamException)
            {
                return;
            }


            for (int i = 0; i < ListEffectCount; i++)
            {
                string TraitEffectType = BR.ReadString();
                TraitEffect Prototype;
                if (!TraitEffect.DicEffect.TryGetValue(TraitEffectType, out Prototype))
                {
                    throw new InvalidDataException("Missing trait effect " + TraitEffectType);
                }

                ListEffect.Add(Prototype.LoadCopy(BR));
            }
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write(Name ?? string.Empty);
            BW.Write(Description ?? string.Empty);
            int Count = ListEffect == null ? 0 : ListEffect.Count;
            BW.Write((byte)Count);
            for (int i = 0; i < Count; i++)
            {
                ListEffect[i].Write(BW);
            }
        }
    }
}
