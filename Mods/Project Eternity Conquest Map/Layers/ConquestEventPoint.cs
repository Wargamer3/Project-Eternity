using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestEventPoint : EventPoint
    {
        public string SpawnTypeName;
        public string SpawnName;
        public Texture2D sprMapPreview;

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
            if (SpawnTypeName == "Unit")
            {
                sprMapPreview = GameScreen.ContentFallback.Load<Texture2D>("Conquest/Units/Map Sprite/" + SpawnName);
            }
        }

        public override void Save(BinaryWriter BW)
        {
            base.Save(BW);

            BW.Write(SpawnTypeName);
            BW.Write(SpawnName);
        }

        public override void Draw(CustomSpriteBatch g, Vector2 Position)
        {
            if (sprMapPreview != null)
            {
                g.Draw(sprMapPreview, Position, Color.FromNonPremultiplied(215, 215, 215, 200));
            }
        }
    }
}
