using System.IO;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class SamePenaltyEffect : TraitEffect
    {
        public SamePenaltyEffect()
            : base("Same penalty")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.HoldPenalty = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            return new SamePenaltyEffect();
        }
    }
}
