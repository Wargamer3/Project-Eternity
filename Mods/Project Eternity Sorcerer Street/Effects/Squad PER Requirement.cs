﻿using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SquadPERRequirement : BaseSkillRequirement
    {
        public static string OnShoot = "On Shoot Squad PER";

        private SquadPERParams _Params;
        protected SquadPERParams Params { get { return _Params; } }

        public SquadPERRequirement(string EffectTypeName, SquadPERParams Params)
            : base(EffectTypeName)
        {
            this._Params = Params;
        }

        protected override void DoReload(string ParamsID)
        {
            this._Params = SorcererStreetBattleParams.DicParams[ParamsID].SquadParams;
        }
    }
}
