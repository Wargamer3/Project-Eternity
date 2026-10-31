namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class MovementClips
    {
        public const int Idle = 0;
        public const int Walk = 1;

        public static int IndexFor(string ClipName, int ClipCount)
        {
            if (ClipCount <= 0)
            {
                return -1;
            }

            if (ClipName == "Walking" && ClipCount > Walk)
            {
                return Walk;
            }

            return Idle;
        }
    }
}
