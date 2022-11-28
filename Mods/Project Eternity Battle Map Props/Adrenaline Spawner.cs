using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class AdrenalineSpawner : InteractiveProp
    {
        private Texture2D sprAdrenaline;

        private readonly BattleMap Map;

        private bool IsUsed;
        private int TurnUsed;
        private int TurnRemaining;

        public AdrenalineSpawner(BattleMap Map)
            : base("Adrenaline", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            IsUsed = false;
        }


        public override void Load(ContentManager Content)
        {
            if (Content != null)
            {
                sprAdrenaline = Content.Load<Texture2D>("Animations/Sprites/Spawn Weapons/Adrenaline");
                Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Billboard 3D"), sprAdrenaline, 1);
            }
        }

        public override void DoLoad(BinaryReader BR)
        {
        }

        public override void DoSave(BinaryWriter BW)
        {
        }

        public void RefillAdrenaline(Squad SquadToHeal)
        {
            if (SquadToHeal.CurrentLeader.Pilot.SP < SquadToHeal.CurrentLeader.Pilot.MaxSP)
            {
                SquadToHeal.CurrentLeader.RefillSP(10);
                IsUsed = true;
                TurnUsed = Map.ActivePlayerIndex;
                TurnRemaining = 3;
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnUnitSelected(ActionPanel PanelOwner, Squad SelectedUnit)
        {
        }

        public override void OnUnitBeforeStop(ActionPanel PanelOwner, Squad StoppedUnit, Vector3 PositionToStopOn)
        {
        }

        public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
            if (!IsUsed && PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                RefillAdrenaline(SelectedUnit);
            }
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (!IsUsed && StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {
                RefillAdrenaline(StoppedUnit);
            }
        }

        public override void OnBattleEnd(Squad Attacker, Squad Defender)
        {
        }

        public override void OnTurnEnd(int PlayerIndex)
        {
            if (IsUsed && TurnUsed == PlayerIndex && --TurnRemaining <= 0)
            {
                IsUsed = false;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (!IsUsed)
            {
                float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
                float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

                g.Draw(sprAdrenaline, new Vector2(PosX, PosY), Color.White);
            }
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g)
        {
            if (!IsUsed)
            {
                Unit3D.SetViewMatrix(View);
                Unit3D.Draw(GraphicsDevice);
            }
        }

        protected override InteractiveProp Copy()
        {
            AdrenalineSpawner NewProp = new AdrenalineSpawner(Map);

            NewProp.sprAdrenaline = sprAdrenaline;
            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Unit3D.UnitEffect3D, sprAdrenaline, 1);

            return NewProp;
        }
    }
}
