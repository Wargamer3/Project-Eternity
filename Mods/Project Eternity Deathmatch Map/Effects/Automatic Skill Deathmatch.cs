using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class AutomaticDeathmatchTargetType : AutomaticSkillTargetType
    {
        private DeathmatchParams _Params;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly BattleContext LocalContext;
        protected DeathmatchParams Params { get { return _Params; } }

        public AutomaticDeathmatchTargetType(string TargetType, DeathmatchParams Params)
            : base(TargetType)
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
    }
}
