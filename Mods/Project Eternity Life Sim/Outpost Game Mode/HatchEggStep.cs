namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class HatchEggStep : LifeSimStep
    {
        private readonly MatchFacts Match;
        private readonly Outpost Outpost;

        public HatchEggStep(PlayerCharacter Owner, MatchFacts Match, Outpost Outpost)
            : base("Hatch Egg", Owner, 0.6)
        {
            this.Match = Match;
            this.Outpost = Outpost;
        }

        protected override void OnFinished()
        {
            if (Match == null || Outpost == null || Match.Eggs <= 0 || Outpost.Side != OutpostSide.Empty)
            {
                return;
            }

            Match.Eggs--;
            Outpost.Side = OutpostSide.Ours;
            Outpost.CreatureAlive = true;
        }

        protected override AutomatedAction Copy()
        {
            return new HatchEggStep(Owner, Match, Outpost);
        }
    }
}
