using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchAttackPEREffect : AttackPEREffect
    {
        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        private DeathmatchParams _Params;
        protected DeathmatchParams Params { get { return _Params; } }
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly AttackPERContext LocalContext;

        public DeathmatchAttackPEREffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            _Params = null;
        }

        public DeathmatchAttackPEREffect(string EffectTypeName, bool IsPassive, DeathmatchParams Params)
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
            this._Params = DeathmatchParams.DicParams[ParamsID];
        }

        protected override void Load(BinaryReader BR)
        {
            if (Lifetime[0].LifetimeType == Core.Effects.SkillEffect.LifetimeTypePermanent)
                Lifetime[0].LifetimeTypeValue = -1;
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
