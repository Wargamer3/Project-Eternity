using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class AutomaticDeathmatchTargetType : AutomaticSkillTargetType
    {
        private DeathmatchParams _Params;
        protected DeathmatchParams Params { get { return _Params; } }

        public AutomaticDeathmatchTargetType(string TargetType, DeathmatchParams Params)
            : base(TargetType)
        {
            this._Params = Params;
        }

        protected override void DoReload(string ParamsID)
        {
            this._Params = DeathmatchParams.DicParams[ParamsID];
        }
    }
}
