﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Units
{
    /// <summary>
    /// An invisible componenent  that is placed on map. Because of transforming units the visible part need to be implemented by its parent, usually a Unit.
    /// </summary>
    public abstract class UnitMapComponent
    {
        public const float DirectionNone = -1;
        public const float DirectionDown = 0;
        public const float DirectionRight = 90;
        public const float DirectionUp = 180;
        public const float DirectionLeft = 270;

        public uint ID;

        public Point MapSize;//Size of the Unit on the map.
        private int _ActionsRemaining;//How many can this Unit move this turn.
        public bool CanMove { get { return _ActionsRemaining > 0; } }
        public int ActionsRemaining { get { return _ActionsRemaining; } set { _ActionsRemaining = value; } }
        public byte CurrentTerrainIndex;//How the Unit is currently moving.
        public bool IsUnderTerrain;
        public bool IsPlayerControlled;

        protected Vector3 _Position;
        public Vector3 LastRealPosition;//Use to limit incline
        public float Direction;
        public float Pitch;
        public bool IsOnGround;
        public bool IsStunned;//If stunned, take longer to stop moving on ground
        protected Vector3 _Speed;
        public Vector3 Position { get { return _Position; } }
        public Vector3 Speed { get { return _Speed; } set { _Speed = value; } }

        private HoldableItem _ItemHeld;
        public HoldableItem ItemHeld => _ItemHeld;
        public List<UnitMapComponent> ListTransportedUnit;
        public bool CanTransportUnit;
        public List<string> ListAllowedTransportUnitName;
        public List<string> ListParthDrop;
        public List<int> ListAttackedTeam;//List of teams that this Unit has encounter. (Used to to know if we have access to its data)
        public TagSystem TeamTags;
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract bool[,] ArrayMapSize { get; }
        public abstract bool IsActive { get; }//Decide if it can be updated or drawn.

        public void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            Draw2DOnMap(g, Position, Width, Height, UnitColor);
        }

        public abstract void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor);

        public abstract void Draw3DOnMap(Microsoft.Xna.Framework.Graphics.GraphicsDevice GraphicsDevice, Matrix View, Matrix Projection);

        public abstract void DrawExtraOnMap(CustomSpriteBatch g, Vector3 PositionY, Color UnitColor);

        public abstract void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position);

        public abstract void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay);

        public virtual void OnMenuSelect(ActionPanel PanelOwner, int ActivePlayerIndex, ActionPanelHolder ListActionMenuChoice)
        {  }

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

        public void PickupItem(HoldableItem ItemHeld)
        {
            if (_ItemHeld != null)
            {
                _ItemHeld.OnDroped(this);
            }

            _ItemHeld = ItemHeld;
            _ItemHeld.OnPickedUp(this);
        }

        public void DropItem()
        {
            if (_ItemHeld != null)
            {
                _ItemHeld.OnDroped(this);
                _ItemHeld = null;
            }
        }

        public virtual void SetPosition(Vector3 Position)
        {
            _Position = Position;
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
        public UnitMap3D Unit3DSprite;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;

        private Matrix WorldPosition;

        public override int Width { get { return SpriteMap.SpriteWidth; } }
        public override int Height { get { return SpriteMap.SpriteHeight; } }
        public override bool[,] ArrayMapSize { get { return new bool[,] { { true } }; } }
        public override bool IsActive { get { return true; } }

        public override void SetPosition(Vector3 Position)
        {
            base.SetPosition(Position);

            if (Unit3DModel == null)
            {
                Unit3DSprite.SetPosition(Position.X, Position.Y, Position.Z);
            }
            else
            {
                Matrix RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(Direction));

                Vector3 FinalPosition = new Vector3(Position.X, Position.Y, Position.Z);

                WorldPosition = RotationMatrix * Matrix.CreateTranslation(FinalPosition.X, FinalPosition.Z, FinalPosition.Y);
            }
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            SpriteMap.Draw(g, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), UnitColor);
        }

        public override void Draw3DOnMap(GraphicsDevice GraphicsDevice, Matrix View, Matrix Projection)
        {
            if (Unit3DModel == null)
            {
                Unit3DSprite.SetViewMatrix(View);

                Unit3DSprite.Draw(GraphicsDevice);
            }
            else
            {
                Unit3DModel.PlayAnimation("Idle");
                Unit3DModel.Draw(View, Projection, WorldPosition);
            }
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
