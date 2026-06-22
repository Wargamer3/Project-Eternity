using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimEffect : BaseEffect
    {
        protected LifeSimCharacterParams Params;

        protected LifeSimEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        protected LifeSimEffect(string EffectTypeName, bool IsPassive, LifeSimCharacterParams Params)
            : base(EffectTypeName, IsPassive)
        {
            this.Params = new LifeSimCharacterParams(Params);
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void DoReload(string ParamsID)
        {
            this.Params = LifeSimCharacterParams.DicParams[ParamsID];
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }
}
