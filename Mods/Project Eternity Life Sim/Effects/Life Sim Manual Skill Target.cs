using System;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimManualSkillTargetType : ManualSkillTarget
    {
        private LifeSimCharacterParams _Params;
        protected LifeSimCharacterParams Params { get { return _Params; } }

        protected LifeSimManualSkillTargetType(string TargetType, bool MustBeUsedAlone, LifeSimCharacterParams Params)
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
            this._Params = LifeSimCharacterParams.DicParams[ParamsID];
        }
    }
}
