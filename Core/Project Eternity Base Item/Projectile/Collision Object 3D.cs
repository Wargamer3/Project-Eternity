using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core
{
    public sealed class CollisionObject3D<T> where T : class, ICollisionObjectHolder3D<T>
    {
        public List<Polygon> ListCollisionPolygon;
        public LinkedList<CollisionZone3D<T>> ListParent;
        public Vector2 Position;
        public float Radius;

        public CollisionObject3D()
        {
            ListCollisionPolygon = new List<Polygon>();
            ListParent = new LinkedList<CollisionZone3D<T>>();
        }

        public CollisionObject3D(Vector2[] ArrayVertex, int MaxWidth, int MaxHeight)
        {
            ListCollisionPolygon = new List<Polygon>() { new Polygon(ArrayVertex, MaxWidth, MaxHeight) };
            ListParent = new LinkedList<CollisionZone3D<T>>();

            ComputeCenterAndRadius();
        }

        public void ComputeCenterAndRadius()
        {
            float TotalX = 0;
            float TotalY = 0;
            float MinX = float.MaxValue;
            float MinY = float.MaxValue;
            float MaxX = float.MinValue;
            float MaxY = float.MinValue;

            for (int P = 0; P < ListCollisionPolygon.Count; ++P)
            {
                ListCollisionPolygon[P].ComputerCenter();

                if (ListCollisionPolygon[P].Center.X - ListCollisionPolygon[P].Radius < MinX)
                    MinX = ListCollisionPolygon[P].Center.X - ListCollisionPolygon[P].Radius;
                if (ListCollisionPolygon[P].Center.X + ListCollisionPolygon[P].Radius > MaxX)
                    MaxX = ListCollisionPolygon[P].Center.X + ListCollisionPolygon[P].Radius;

                if (ListCollisionPolygon[P].Center.Y - ListCollisionPolygon[P].Radius < MinY)
                    MinY = ListCollisionPolygon[P].Center.Y - ListCollisionPolygon[P].Radius;
                if (ListCollisionPolygon[P].Center.Y + ListCollisionPolygon[P].Radius > MaxY)
                    MaxY = ListCollisionPolygon[P].Center.Y + ListCollisionPolygon[P].Radius;

                TotalX += ListCollisionPolygon[P].Center.X;
                TotalY += ListCollisionPolygon[P].Center.Y;
            }

            Position = new Vector2(TotalX / ListCollisionPolygon.Count, TotalY / ListCollisionPolygon.Count);
            Radius = Math.Max(MaxX - MinX, MaxY - MinY) / 2f;
        }

        public bool CollideWith<V>(CollisionObject2D<V> Other, Vector2 OtherSpeed, out PolygonCollisionResult FinalCollisionResult, out Polygon FinalCollisionPolygon, out Polygon FinalOtherPolygon)
            where V : class, ICollisionObjectHolder2D<V>
        {
            FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);
            FinalCollisionPolygon = null;
            FinalOtherPolygon = null;

            foreach (Polygon CollisionPolygon in ListCollisionPolygon)
            {
                foreach (Polygon OtherPolygon in Other.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(OtherPolygon, CollisionPolygon, OtherSpeed);

                    if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                    {
                        FinalCollisionResult = CollisionResult;
                        FinalCollisionPolygon = CollisionPolygon;
                        FinalOtherPolygon = OtherPolygon;
                    }
                }
            }

            if (FinalCollisionResult.Distance >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
