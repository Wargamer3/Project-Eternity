using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public abstract class AutomaticConquestTargetType : AutomaticSkillTargetType
    {
        private ConquestParams _Params;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly BattleContext LocalContext;
        protected ConquestParams Params { get { return _Params; } }

        public AutomaticConquestTargetType(string TargetType, ConquestParams Params)
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
            this._Params = ConquestParams.DicParams[ParamsID];
        }
    }
}
