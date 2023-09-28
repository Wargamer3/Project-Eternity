using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetEffect : BaseEffect
    {
        protected SorcererStreetBattleParams Params;

        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly SorcererStreetBattleContext LocalContext;

        protected SorcererStreetEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        /// <summary>
        /// Used for reflection
        /// </summary>
        /// <param name="EffectTypeName"></param>
        /// <param name="EffectContext"></param>
        protected SorcererStreetEffect(string EffectTypeName, bool IsPassive, SorcererStreetBattleParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = Params;
                LocalContext = new SorcererStreetBattleContext(Params.GlobalContext);
                if (Params.RememberEffects)
                {
                    Params.GlobalContext.ListActivatedEffect.Add(this);
                }
            }
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void DoReload(string ParamsID)
        {
            this.Params = SorcererStreetBattleParams.DicParams[ParamsID];
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }
}
