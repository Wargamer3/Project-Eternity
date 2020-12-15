using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class HitscanBox : AttackBox
    {
        private const float HitscanLength = 10;
        private readonly float Angle;

        public HitscanBox(float Damage, Weapon.ExplosionOptions ExplosionAttributes, RobotAnimation Owner, Vector2 Position, float Angle)
            : base(Damage, ExplosionAttributes, Owner, false)
        {
            this.Angle = Angle;
            Lifetime = 50;

            Owner.SetAttackContext(this, Owner, Angle, Position);

            float CosA = (float)Math.Cos(Angle);
            float SinA = (float)Math.Sin(Angle);
            float MinX = Position.X;
            float MinY = Position.Y;
            float MaxX = MinX + CosA * HitscanLength;
            float MaxY = MinY + SinA * HitscanLength;
            Speed = new Vector2(CosA * 100000, SinA * 100000);

            Polygon NewPolygon = new Polygon();
            NewPolygon.ArrayVertex = new Vector2[4];
            NewPolygon.ArrayVertex[0] = new Vector2(MinX, MinY);
            NewPolygon.ArrayVertex[1] = new Vector2(MaxX, MaxY);
            NewPolygon.ArrayVertex[2] = new Vector2(MaxX, MaxY);
            NewPolygon.ArrayVertex[3] = new Vector2(MinX, MinY);

            NewPolygon.ComputePerpendicularAxis();
            NewPolygon.ComputerCenter();

            ListCollisionPolygon = new List<Polygon>(1) { NewPolygon };
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Owner.SetAttackContext(this, Owner, Angle, ListCollisionPolygon[0].Center);
        }

        public override void SetAngle(float Angle)
        {
        }

        public void FinalizeCollision(Vector2 FinalMovement, Vector2 CollisionPoint)
        {
            if (Speed != Vector2.Zero)
            {
                ListCollisionPolygon[0].ArrayVertex[1] = CollisionPoint;
                ListCollisionPolygon[0].ArrayVertex[2] = CollisionPoint;
                Speed = Vector2.Zero;
            }
        }

        public override void OnCollision(PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, out Vector2 CollisionPoint)
        {
            float MinDistance = float.MaxValue;
            Vector2 FinalCollisionPoint = ListCollisionPolygon[0].ArrayVertex[2];

            for (int I = 0; I < FinalCollisionPolygon.ArrayIndex.Length; I += 3)
            {
                Vector2 Vertex1 = FinalCollisionPolygon.ArrayVertex[FinalCollisionPolygon.ArrayIndex[I]];
                Vector2 Vertex2 = FinalCollisionPolygon.ArrayVertex[FinalCollisionPolygon.ArrayIndex[I + 1]];
                Vector2 Vertex3 = FinalCollisionPolygon.ArrayVertex[FinalCollisionPolygon.ArrayIndex[I + 2]];

                CollisionPoint = Vector2.Zero;

                if (Polygon.DoLinesIntersect(
                    ListCollisionPolygon[0].ArrayVertex[0],
                    ListCollisionPolygon[0].ArrayVertex[2] + Speed,
                    Vertex1,
                    Vertex2,
                    ref CollisionPoint))
                {
                    float DistanceToPoint = (ListCollisionPolygon[0].ArrayVertex[0] - CollisionPoint).Length();

                    if (DistanceToPoint < MinDistance)
                    {
                        MinDistance = DistanceToPoint;
                        FinalCollisionPoint = CollisionPoint;
                    }
                }

                if (Polygon.DoLinesIntersect(
                    ListCollisionPolygon[0].ArrayVertex[0],
                    ListCollisionPolygon[0].ArrayVertex[2] + Speed,
                    Vertex2,
                    Vertex3,
                    ref CollisionPoint))
                {
                    float DistanceToPoint = (ListCollisionPolygon[0].ArrayVertex[0] - CollisionPoint).Length();

                    if (DistanceToPoint < MinDistance)
                    {
                        MinDistance = DistanceToPoint;
                        FinalCollisionPoint = CollisionPoint;
                    }
                }

                if (Polygon.DoLinesIntersect(
                    ListCollisionPolygon[0].ArrayVertex[0],
                    ListCollisionPolygon[0].ArrayVertex[2] + Speed,
                    Vertex3,
                    Vertex1,
                    ref CollisionPoint))
                {
                    float DistanceToPoint = (ListCollisionPolygon[0].ArrayVertex[0] - CollisionPoint).Length();

                    if (DistanceToPoint < MinDistance)
                    {
                        MinDistance = DistanceToPoint;
                        FinalCollisionPoint = CollisionPoint;
                    }
                }
            }

            Vector2 MovementCorection = FinalCollisionResult.Axis * FinalCollisionResult.Distance;
            Vector2 FinalMovement = Speed + MovementCorection;
            CollisionPoint = Polygon.GetCollisionPointFromLine(ListCollisionPolygon[0].ArrayVertex[0], ListCollisionPolygon[0].ArrayVertex[2], Speed, FinalCollisionPolygon, out _);

            FinalizeCollision(FinalMovement, FinalCollisionPoint);
        }

        public override void DrawRegular(CustomSpriteBatch g)
        {
            GameScreen.DrawLine(g, ListCollisionPolygon[0].ArrayVertex[0], ListCollisionPolygon[0].ArrayVertex[2], Color.Black, 3);
        }
    }
}
