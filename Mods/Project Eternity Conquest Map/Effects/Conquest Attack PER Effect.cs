using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public abstract class ConquestAttackPEREffect : AttackPEREffect
    {
        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        private ConquestParams _Params;
        protected ConquestParams Params { get { return _Params; } }
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly AttackPERContext LocalContext;

        public ConquestAttackPEREffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            _Params = null;
        }

        public ConquestAttackPEREffect(string EffectTypeName, bool IsPassive, ConquestParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this._Params = Params;
                LocalContext = new AttackPERContext(Params.GlobalAttackContext);
            }
        }

        protected override void DoReload(string ParamsID)
        {
            this._Params = ConquestParams.DicParams[ParamsID];
        }

        protected override void Load(BinaryReader BR)
        {
            if (LifetimeType == Core.Effects.SkillEffect.LifetimeTypePermanent)
                LifetimeTypeValue = -1;
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
