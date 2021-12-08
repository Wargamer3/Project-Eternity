using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class InteractiveProp
    {
        public readonly string PropType;//HP, EN, Ammo, weapon, pillar.
        public Vector3 Position;
        public bool[,] ArrayMapSize;
        public bool CanBlockPath;

        protected InteractiveProp(string PropType, Vector3 Position, bool[,] ArrayMapSize, bool CanBlockPath)
        {
            this.PropType = PropType;
            this.Position = Position;
            this.ArrayMapSize = ArrayMapSize;
            this.CanBlockPath = CanBlockPath;
        }

        public abstract void Load(ContentManager Content);

        public abstract void Update(GameTime gameTime);

        public abstract List<ActionPanel> OnUnitSelected(UnitMapComponent SelectedUnit);

        public abstract List<ActionPanel> OnUnitStop(UnitMapComponent StoppedUnit);

        public abstract void OnBattleEnd(Squad Attacker, Squad Defender);

        public abstract void OnTurnEnd(int PlayerIndex);

        public abstract void Draw(CustomSpriteBatch g);
    }

    public class HPCrate : InteractiveProp
    {
        private Texture2D sprCrate;

        private BattleMap Map;

        public HPCrate(Vector3 Position, BattleMap Map)
            : base("HP Crate", Position, new bool[,] { { true } }, false)
        {
            this.Map = Map;
        }

        public override void Load(ContentManager Content)
        {
            sprCrate = Content.Load<Texture2D>("Maps/Props/HP Crate");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override List<ActionPanel> OnUnitSelected(UnitMapComponent SelectedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
        }

        public override List<ActionPanel> OnUnitStop(UnitMapComponent StoppedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
        }

        public override void OnBattleEnd(Squad Attacker, Squad Defender)
        {
        }

        public override void OnTurnEnd(int PlayerIndex)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
            float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            g.Draw(sprCrate, new Vector2(PosX, PosY), Color.White);
        }
    }
}
