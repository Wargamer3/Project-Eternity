using System;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimManualSkillTargetType : ManualSkillTarget
    {
        private LifeSimParams _Params;
        protected LifeSimParams Params { get { return _Params; } }

        protected LifeSimManualSkillTargetType(string TargetType, bool MustBeUsedAlone, LifeSimParams Params)
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
            this._Params = LifeSimParams.DicParams[ParamsID];
        }
    }
}
