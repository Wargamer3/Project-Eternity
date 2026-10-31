namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class KillCreatureStep : LifeSimStep
    {
        private readonly Outpost Outpost;

        public KillCreatureStep(PlayerCharacter Owner, Outpost Outpost)
            : base("Kill Creature", Owner, 0.6)
        {
            this.Outpost = Outpost;
        }

        protected override void OnFinished()
        {
            if (Outpost == null || !Outpost.CreatureAlive)
            {
                return;
            }

            Outpost.CreatureAlive = false;
            Outpost.Side = OutpostSide.Empty;
        }

        protected override AutomatedAction Copy()
        {
            return new KillCreatureStep(Owner, Outpost);
        }
    }
}
