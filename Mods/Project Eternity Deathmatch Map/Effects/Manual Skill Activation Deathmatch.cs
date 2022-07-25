using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public abstract class ManualSkillActivationDeathmatch : ManualSkillTarget
    {
        private DeathmatchParams _Params;
        protected DeathmatchParams Params { get { return _Params; } }

        protected ManualSkillActivationDeathmatch(string TargetType, bool MustBeUsedAlone, DeathmatchParams Params)
            : base(TargetType, MustBeUsedAlone)
        {
            if (Params != null)
            {
                this._Params = Params;
                //Don't copy into local as local is never used.
            }
        }

        protected override void DoReload(string ParamsID)
        {
            this._Params = DeathmatchParams.DicParams[ParamsID];
        }
    }
}
