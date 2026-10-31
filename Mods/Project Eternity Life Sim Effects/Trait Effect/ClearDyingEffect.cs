using System.IO;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ClearDyingEffect : TraitEffect
    {
        public ClearDyingEffect()
            : base("Clear dying")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.ClearDying = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            return new ClearDyingEffect();
        }
    }
}
