using System.IO;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class SacrificeEffect : TraitEffect
    {
        public SacrificeEffect()
            : base("Sacrifice")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.Sacrifice = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            return new SacrificeEffect();
        }
    }
}
