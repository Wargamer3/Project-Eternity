namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class ArmorBase : UsableEquipment
    {
        public readonly int DamageReductionPercent;

        public ArmorBase(RobotAnimation Owner, int DamageReductionPercent)
            : base(Owner)
        {
            this.DamageReductionPercent = DamageReductionPercent;
        }
    }
}
