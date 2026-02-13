using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class HealthVialSmallSpawner : InteractiveProp
    {
        private Texture2D sprVial;

        private readonly BattleMap Map;

        private bool IsUsed;
        private int TurnUsed;
        private int TurnRemaining;

        public HealthVialSmallSpawner(BattleMap Map)
            : base("HP Vial Small", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            IsUsed = false;
        }


        public override void LoadPreset(ContentManager Content)
        {
            if (Content != null)
            {
                sprVial = Content.Load<Texture2D>("Animations/Sprites/Spawn Weapons/Health Vial Small");
                Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Billboard 3D"), sprVial, 1, 0f);
            }
        }

        public override void DoLoad(BinaryReader BR)
        {
        }

        public override void DoSave(BinaryWriter BW)
        {
        }

        public void HealSquad(Unit UnitToHeal)
        {
            if (UnitToHeal.HP < UnitToHeal.MaxHP)
            {
                UnitToHeal.HealUnit(UnitToHeal.MaxHP / 10);
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
                HealSquad(SelectedUnit.CurrentLeader);
            }
        }

        public override void OnMovedOverBeforeStop(Unit SelectedUnit, Vector3 PositionMovedOn, UnitMapComponent PositionStoppedOn)
        {
            if (!IsUsed && PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                HealSquad(SelectedUnit);
            }
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (!IsUsed && StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {
                HealSquad(StoppedUnit.CurrentLeader);
            }
        }

        public override void OnUnitStop(Unit StoppedUnit, UnitMapComponent UnitPosition)
        {
            if (!IsUsed && UnitPosition.X == Position.X && UnitPosition.Y == Position.Y)
            {
                HealSquad(StoppedUnit);
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
                float PosX = Position.X - Map.Camera2DPosition.X;
                float PosY = Position.Y - Map.Camera2DPosition.Y;

                g.Draw(sprVial, new Vector2(PosX, PosY), Color.White);
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
            HealthVialSmallSpawner NewProp = new HealthVialSmallSpawner(Map);

            NewProp.sprVial = sprVial;
            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Unit3D.UnitEffect3D, sprVial, 1, 0f);

            return NewProp;
        }
    }
}
