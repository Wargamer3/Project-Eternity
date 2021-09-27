using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public static class PlayerManager
    {
        public static string OnlinePlayerID = string.Empty;
        public static string OnlinePlayerName = string.Empty;
        public static int OnlinePlayerLevel = 0;

        public static List<Player> ListLocalPlayer = new List<Player>();
    }
}
