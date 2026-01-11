using System;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestEventPoint : EventPoint
    {
        public string SpawnTypeName;
        public string SpawnName;

        public ConquestEventPoint(BinaryReader BR)
            : base(BR)
        {
            SpawnTypeName = BR.ReadString();
            SpawnName = BR.ReadString();
        }

        public ConquestEventPoint(Vector3 Position, string Tag, byte ColorRed, byte ColorGreen, byte ColorBlue, string SpawnTypeName, string SpawnName)
            : base(Position, Tag, ColorRed, ColorGreen, ColorBlue)
        {
            this.SpawnTypeName = SpawnTypeName;
            this.SpawnName = SpawnName;
        }

        public override void Save(BinaryWriter BW)
        {
            base.Save(BW);

            BW.Write(SpawnTypeName);
            BW.Write(SpawnName);
        }
    }
}
