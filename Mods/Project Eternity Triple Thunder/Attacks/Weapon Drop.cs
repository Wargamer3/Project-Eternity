using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class WeaponDrop : Projectile
    {
        public readonly Texture2D sprWeapon;

        public uint ID;
        public readonly string WeaponName;
        protected readonly Layer CurrentLayer;
        public Vector2 GravityVector { get { return CurrentLayer.GravityVector; } }

        public WeaponDrop(Texture2D sprWeapon, string WeaponName, Layer CurrentLayer, Vector2 Position, float Angle)
            : base()
        {
            this.sprWeapon = sprWeapon;
            this.WeaponName = WeaponName;
            this.CurrentLayer = CurrentLayer;

            Speed = new Vector2((float)Math.Cos(Angle) * 7, (float)Math.Sin(Angle) * 7);

            float MinX = Position.X - sprWeapon.Width / 2f;
            float MinY = Position.Y - sprWeapon.Height / 2f;
            float MaxX = MinX + sprWeapon.Width;
            float MaxY = MinY + sprWeapon.Height;


            Polygon NewPolygon = new Polygon();
            NewPolygon.ArrayVertex = new Vector2[4];
            NewPolygon.ArrayVertex[0] = new Vector2(MinX, MinY);
            NewPolygon.ArrayVertex[1] = new Vector2(MaxX, MaxY);
            NewPolygon.ArrayVertex[2] = new Vector2(MaxX, MaxY);
            NewPolygon.ArrayVertex[3] = new Vector2(MinX, MinY);

            NewPolygon.ComputePerpendicularAxis();
            NewPolygon.ComputerCenter();

            Collision.ListCollisionPolygon = new List<Polygon>(1) { NewPolygon };
            Collision.ComputeCenterAndRadius();
            this.Collision.Position = Position;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Speed += GravityVector;
        }

        public virtual void Move(GameTime gameTime)
        {
            foreach (Polygon ActivePolygon in Collision.ListCollisionPolygon)
            {
                ActivePolygon.Offset(Speed.X, Speed.Y);
            }

            Collision.Position += Speed;
        }

        public override void SetAngle(float Angle)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprWeapon, Collision.Position, Color.White);
        }
    }
}
