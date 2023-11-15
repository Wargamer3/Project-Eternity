using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ManualSkillActivationSorcererStreet : ManualSkillTarget
    {
        public static readonly string PlayerTargetType = "Sorcerer Street Player";
        private SorcererStreetBattleParams _Params;
        protected SorcererStreetBattleParams Params { get { return _Params; } }

        protected ManualSkillActivationSorcererStreet(string TargetType, bool MustBeUsedAlone, SorcererStreetBattleParams Params)
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
            this._Params = SorcererStreetBattleParams.DicParams[ParamsID];
        }
    }
}
