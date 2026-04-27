using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimEffect : BaseEffect
    {
        protected LifeSimParams Params;

        protected LifeSimEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        protected LifeSimEffect(string EffectTypeName, bool IsPassive, LifeSimParams Params)
            : base(EffectTypeName, IsPassive)
        {
            this.Params = new LifeSimParams(Params);
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void DoReload(string ParamsID)
        {
            this.Params = LifeSimParams.DicParams[ParamsID];
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }
}
