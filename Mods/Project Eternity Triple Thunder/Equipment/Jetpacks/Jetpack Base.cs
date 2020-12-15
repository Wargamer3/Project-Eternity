namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class JetpackBase : UsableEquipment
    {
        public int JetpackFuel;
        public readonly int JetpackFuelMax;
        public static float JetpackTrust = 0.4f;
        public static float MaxJetpackTrust = 3f;

        public JetpackBase(RobotAnimation Owner, int JetpackFuelMax)
            : base(Owner)
        {
            JetpackFuel = JetpackFuelMax;
            this.JetpackFuelMax = JetpackFuelMax;
        }
    }
}
