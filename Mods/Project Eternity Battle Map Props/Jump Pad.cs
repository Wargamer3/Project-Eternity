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
    public class JumpPad : InteractiveProp
    {
        private Texture2D sprFlag;

        private readonly BattleMap Map;

        private bool IsUsed;

        public JumpPad(BattleMap Map)
            : base("JumpPad", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            IsUsed = false;
        }


        public override void LoadPreset(ContentManager Content)
        {
            if (Content != null)
            {
                sprFlag = Content.Load<Texture2D>("Animations/Sprites/Spawn Weapons/Flag Red");
                Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Billboard 3D"), sprFlag, 1, 0f);
            }
        }

        public override void DoLoad(BinaryReader BR)
        {
        }

        public override void DoSave(BinaryWriter BW)
        {
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
            if (PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnMovedOverBeforeStop(Unit SelectedUnit, Vector3 PositionMovedOn, UnitMapComponent PositionStoppedOn)
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

        public override void OnUnitStop(Unit StoppedUnit, UnitMapComponent UnitPosition)
        {
            if (UnitPosition.X == Position.X && UnitPosition.Y == Position.Y)
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
                float PosX = Position.X - Map.Camera2DPosition.X;
                float PosY = Position.Y - Map.Camera2DPosition.Y;

                g.Draw(sprFlag, new Vector2(PosX, PosY), Color.White);
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
            JumpPad NewProp = new JumpPad(Map);

            NewProp.sprFlag = sprFlag;
            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Unit3D.UnitEffect3D, sprFlag, 1, 0f);

            return NewProp;
        }
    }
}
