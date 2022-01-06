using System;
using System.IO;
using System.Collections.Generic;
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

        private bool IsUsed;

        public HealthCrateSpawner(BattleMap Map)
            : base("HP Crate", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            IsUsed = false;
        }


        public override void Load(ContentManager Content)
        {
            sprCrate = Content.Load<Texture2D>("Maps/Props/HP Crate");
            Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), sprCrate, 1);
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
            if (PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {
                IsUsed = true;
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
            if (!IsUsed)
            {
                float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
                float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

                g.Draw(sprCrate, new Vector2(PosX, PosY), Color.White);
            }
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice)
        {
            if (!IsUsed)
            {
                Unit3D.Draw(GraphicsDevice);
            }
        }

        protected override InteractiveProp Copy()
        {
            HealthCrateSpawner NewProp = new HealthCrateSpawner(Map);

            NewProp.sprCrate = sprCrate;
            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Squad shader 3D"), sprCrate, 1);

            return NewProp;
        }
    }
}
