using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlatform
    {
        public BattleMap PlatformMap;

        public AIContainer PlatformAI;
        public Vector3 Position;
        public float Yaw;
        public float Pitch;
        public float Roll;
    }
}
