using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchEffect : BaseEffect
    {
        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        private DeathmatchParams _Params;
        protected DeathmatchParams Params { get { return _Params; } }
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly BattleContext LocalContext;

        public DeathmatchEffect(string EffectTypeName, bool IsPassive)
           : base(EffectTypeName, IsPassive)
        {
            _Params = null;
        }

        public DeathmatchEffect(string EffectTypeName, bool IsPassive, DeathmatchParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this._Params = Params;
                LocalContext = new BattleContext(_Params.GlobalContext);
            }
        }

        protected override void DoReload(string ParamsID)
        {
            this._Params = DeathmatchParams.DicParams[ParamsID];
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

        public override bool CanActivate()
        {
            return true;
        }
    }
}
