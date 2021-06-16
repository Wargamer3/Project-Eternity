using Microsoft.Xna.Framework;

namespace ProjectEternity.Core
{
    public class Constants
    {
        public enum UnitRepresentationStates { NonColoredWithBorder = 0, NonColored = 1, Colored = 2 };
        public enum WindowTypes { Original, Streched, KeeptAspectRatio, Extend };
        public static string[] ScreenSizes = new string[] { "640 x 480", "800 x 600", "1024 x 768", "1920 x 1080" };

        public static float Ratio = 640 / (float)480;
        public static int ScreenSize = 0;

        public static WindowTypes WindowType = WindowTypes.Original;
        public static GraphicsDeviceManager graphics;
        public static int Width = 640;//1920
        public static int Height = 480;//1080
        public static int Money = 0;
        public static bool ShowAnimation = false;
        public static bool ShowBattleRecap = true;
        public static UnitRepresentationStates UnitRepresentationState = UnitRepresentationStates.NonColoredWithBorder;
        public static bool ShowHealthBar = false;
        public static double TotalGameTime;
    }
}