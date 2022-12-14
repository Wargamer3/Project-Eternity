using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class PointOfViewBox : AttackBox
    {
        public PointOfViewBox(Vector2 Position, Vector2 Destination)
            : base(0, new ExplosionOptions(), null, false)
        {
            float MinX = Position.X;
            float MinY = Position.Y;
            float MaxX = Destination.X;
            float MaxY = Destination.Y;
            Speed = Vector2.Zero;

            Polygon NewPolygon = new Polygon();
            NewPolygon.ArrayVertex = new Vector2[4];
            NewPolygon.ArrayVertex[0] = new Vector2(MinX, MinY);
            NewPolygon.ArrayVertex[1] = new Vector2(MaxX, MaxY);
            NewPolygon.ArrayVertex[2] = new Vector2(MaxX, MaxY);
            NewPolygon.ArrayVertex[3] = new Vector2(MinX, MinY);

            NewPolygon.ComputePerpendicularAxis();

            NewPolygon.ComputerCenter();

            Collision.ListCollisionPolygon = new List<Polygon>(1) { NewPolygon };
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void SetAngle(float Angle)
        {
            throw new NotImplementedException();
        }

        public override void DrawRegular(CustomSpriteBatch g)
        {
            GameScreen.DrawLine(g, Collision.ListCollisionPolygon[0].ArrayVertex[0], Collision.ListCollisionPolygon[0].ArrayVertex[2], Color.Black, 3);
        }

        public override void OnCollision(PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, out Vector2 CollisionPoint)
        {
            throw new NotImplementedException();
        }
    }
}
