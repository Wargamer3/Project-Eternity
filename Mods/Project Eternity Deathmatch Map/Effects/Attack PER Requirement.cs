using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class AttackPERRequirement : BaseSkillRequirement
    {
        public const string OnDeath = "On Death Attack PER";
        public const string OnTileChange = "On Tile Change Attack PER";

        protected readonly AttackPERParams Params;

        public AttackPERRequirement(string EffectTypeName, AttackPERParams Params)
            : base(EffectTypeName)
        {
            this.Params = Params;
        }
    }
}
