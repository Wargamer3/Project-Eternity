using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public abstract class UnitMapComponent
    {
        public uint ID;

        public Point MapSize;//Size of the Unit on the map.
        private int _ActionsRemaining;//How many can this Unit move this turn.
        public bool CanMove { get { return _ActionsRemaining > 0; } }
        public int ActionsRemaining { get { return _ActionsRemaining; } set { _ActionsRemaining = value; } }
        public string CurrentMovement;//How the Unit is currently moving.
        public bool IsFlying;
        public bool IsUnderTerrain;
        public bool IsPlayerControlled;

        protected Vector3 _Position;
        protected int _LayerIndex;
        public Vector3 Position { get { return _Position; } }
        public int LayerIndex { get { return _LayerIndex; } }

        public UnitMap3D Unit3D;
        public List<UnitMapComponent> ListTransportedUnit;
        public bool CanTransportUnit;
        public List<string> ListAllowedTransportUnitName;
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract bool[,] ArrayMapSize { get; }
        public abstract bool IsActive { get; }//Decide if it can be updated or drawn.

        public void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            Draw2DOnMap(g, Position, Width, Height, UnitColor);
        }

        public abstract void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor);

        public abstract void DrawExtraOnMap(CustomSpriteBatch g, Vector3 PositionY, Color UnitColor);

        public abstract void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position);
        public abstract void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay);
        
        public virtual List<ActionPanel> OnMenuSelect(ActionPanelHolder ListActionMenuChoice)
        { return null; }

        public void StartTurn()
        {
            ActionsRemaining = 1;
        }

        public void EndTurn()
        {
            if (ActionsRemaining > 0)
            {
                --ActionsRemaining;
            }
        }

        public void SetLayerIndex(int LayerIndex)
        {
            _LayerIndex = LayerIndex;
        }

        public void SetPosition(Vector3 Position)
        {
            _Position = Position;

            if (Unit3D != null)
            {
                Unit3D.SetPosition(Position.X, Height, Position.Y);
            }
        }

        protected void DrawHealthBar(CustomSpriteBatch g, Vector3 Position, int HP, int MaxHP, int HealthBarWidth)
        {
            g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)Position.X, (int)Position.Y - 10, HealthBarWidth, 10), Color.Black);
            g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)Position.X + 1, (int)Position.Y - 9, (int)(HP * (HealthBarWidth - 2) / (float)MaxHP), 8), Color.Green);
        }

        public float X
        {
            get { return _Position.X; }
        }

        public float Y
        {
            get { return _Position.Y; }
        }

        public float Z
        {
            get { return _Position.Z; }
        }
    }

    public class StandaloneUnitMapCompontent : UnitMapComponent
    {
        public AnimatedSprite SpriteMap;

        public override int Width { get { return SpriteMap.SpriteWidth; } }
        public override int Height { get { return SpriteMap.SpriteHeight; } }
        public override bool[,] ArrayMapSize { get { return new bool[,] { { true } }; } }
        public override bool IsActive { get { return true; } }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            SpriteMap.Draw(g, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), UnitColor);
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
        }

        public override void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position)
        {
        }

        public override void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay)
        {
        }
    }
}
