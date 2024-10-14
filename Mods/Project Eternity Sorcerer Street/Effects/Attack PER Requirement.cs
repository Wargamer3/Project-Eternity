using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class AttackPERRequirement : BaseSkillRequirement
    {
        public const string OnDeath = "On Death Attack PER";
        public const string OnTileChange = "On Tile Change Attack PER";
        public const string OnDistanceTravelled = "Distance Travelled Attack PER";

        private AttackPERParams _Params;
        protected AttackPERParams Params { get { return _Params; } }

        public AttackPERRequirement(string EffectTypeName, AttackPERParams Params)
            : base(EffectTypeName)
        {
            this._Params = Params;
        }

        protected override void DoReload(string ParamsID)
        {
            this._Params = SorcererStreetBattleParams.DicParams[ParamsID].AttackParams;
        }
    }
}
