using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchAttackPEREffect : AttackPEREffect
    {
        protected readonly DeathmatchParams Params;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly AttackPERContext LocalContext;

        public DeathmatchAttackPEREffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        /// <summary>
        /// Used for reflection
        /// </summary>
        /// <param name="EffectTypeName"></param>
        /// <param name="EffectContext"></param>
        public DeathmatchAttackPEREffect(string EffectTypeName, bool IsPassive, DeathmatchParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = Params;
                LocalContext = new AttackPERContext(Params.GlobalAttackContext);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }
}
