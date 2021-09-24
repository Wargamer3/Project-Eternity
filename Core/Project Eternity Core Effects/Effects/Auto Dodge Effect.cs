﻿using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class AutoDodgeEffect : SkillEffect
    {
        public static string Name = "Auto Dodge Effect";

        public AutoDodgeEffect()
            : base(Name, true)
        {
        }

        public AutoDodgeEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        public override bool CanActivate()
        {
            return !Params.GlobalContext.EffectTargetUnit.Boosts.AutoDodgeModifier;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.AutoDodgeModifier = true;

            return string.Empty;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.AutoDodgeModifier = true;
        }

        protected override BaseEffect DoCopy()
        {
            AutoDodgeEffect NewEffect = new AutoDodgeEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
