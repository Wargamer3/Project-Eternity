namespace ProjectEternity.GameScreens
{
    public static class PictureOptions
    {
        public static int Shadows = 0;
        public static bool Shade;
        public static bool Shafts;
        public static int Grass = 0;
        public static int Trees = 0;
        public static bool Lamps;
        public static int Distance = 0;
        public static bool FastTime;
        public static bool Msaa;

        public static int ShadowCascades
        {
            get { return Shadows <= 0 ? 0 : Shadows == 1 ? 1 : 3; }
        }

        public static float Reach
        {
            get { return Distance <= 0 ? 0.55f : Distance >= 2 ? 1.4f : 1f; }
        }

        public static float GrassGap
        {
            get { return Grass <= 1 ? 1.75f : 1f; }
        }

        public static string ShadowLabel
        {
            get { return Shadows <= 0 ? "Off" : Shadows == 1 ? "Near" : "Full"; }
        }

        public static string GrassLabel
        {
            get { return Grass <= 0 ? "Off" : Grass == 1 ? "Thin" : "Full"; }
        }

        public static string TreeLabel
        {
            get { return Trees <= 0 ? "Low" : "Full"; }
        }

        public static string DistanceLabel
        {
            get { return Distance <= 0 ? "Short" : Distance >= 2 ? "Far" : "Medium"; }
        }

        public static void CycleShadows(int Direction)
        {
            Shadows = Wrap(Shadows + Direction, 3);
        }

        public static void CycleGrass(int Direction)
        {
            Grass = Wrap(Grass + Direction, 3);
        }

        public static void CycleDistance(int Direction)
        {
            Distance = Wrap(Distance + Direction, 3);
        }

        public static void CycleTrees(int Direction)
        {
            Trees = Wrap(Trees + Direction, 2);
        }

        static int Wrap(int Value, int Count)
        {
            if (Value < 0)
            {
                return Count - 1;
            }

            if (Value >= Count)
            {
                return 0;
            }

            return Value;
        }
    }
}
