namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ScreenPowerAction : LifeSimStep
    {
        readonly FurnitureItem Box;
        readonly bool Open;

        public ScreenPowerAction(PlayerCharacter Owner, FurnitureItem Box, bool Open)
            : base(Open ? "Open TV" : "Close TV", Owner, 0.3)
        {
            this.Box = Box;
            this.Open = Open;
        }

        protected override void OnFinished()
        {
            if (Box != null)
            {
                Box.Open = Open;
            }
        }

        protected override AutomatedAction Copy()
        {
            return new ScreenPowerAction(Owner, Box, Open);
        }
    }
}
