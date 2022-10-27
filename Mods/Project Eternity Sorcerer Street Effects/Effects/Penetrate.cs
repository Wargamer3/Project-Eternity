using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class PenetrateEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Penetrate";

        public ElementalAffinity[] ArrayAffinity;

        public PenetrateEffect()
            : base(Name, false)
        {
            ArrayAffinity = new ElementalAffinity[0];
        }

        public PenetrateEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            int ArrayAffinityLength = BR.ReadByte();
            ArrayAffinity = new ElementalAffinity[ArrayAffinityLength];
            for (int A = 0; A < ArrayAffinityLength; ++A)
            {
                ArrayAffinity[A] = (ElementalAffinity)BR.ReadByte();
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)ArrayAffinity.Length);
            for (int A = 0; A < ArrayAffinity.Length; ++A)
            {
                BW.Write((byte)ArrayAffinity[A]);
            }
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            PenetrateEffect NewEffect = new PenetrateEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ElementalAffinity[] Lands
        {
            get
            {
                return ArrayAffinity;
            }
            set
            {
                ArrayAffinity = value;
            }
        }

        #endregion
    }
}
