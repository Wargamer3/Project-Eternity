using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class HealthCrateSpawner : InteractiveProp
    {
        private Texture2D sprCrate;

        private readonly BattleMap Map;

        public HealthCrateSpawner(BattleMap Map)
            : base("HP Crate", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
        }


        public override void Load(ContentManager Content)
        {
            sprCrate = Content.Load<Texture2D>("Maps/Props/HP Crate");
        }

        public override void DoLoad(BinaryReader BR)
        {
        }

        public override void DoSave(BinaryWriter BW)
        {
        }

        public void HealSquad(Squad SquadToHeal)
        {
            SquadToHeal.CurrentLeader.HealUnit(SquadToHeal.CurrentLeader.MaxHP / 2);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override List<ActionPanel> OnUnitSelected(Squad SelectedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
        }

        public override List<ActionPanel> OnUnitBeforeStop(Squad StoppedUnit, Vector3 PositionToStopOn)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
        }

        public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {

            }
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

        protected override InteractiveProp Copy()
        {
            HealthCrateSpawner NewProp = new HealthCrateSpawner(Map);

            NewProp.sprCrate = sprCrate;

            return NewProp;
        }
    }
}
