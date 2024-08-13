using System;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public abstract class ConquestManualSkillActivation : ManualSkillTarget
    {
        private ConquestParams _Params;
        protected ConquestParams Params { get { return _Params; } }

        protected ConquestManualSkillActivation(string TargetType, bool MustBeUsedAlone, ConquestParams Params)
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
            this._Params = ConquestParams.DicParams[ParamsID];
        }
    }
}
