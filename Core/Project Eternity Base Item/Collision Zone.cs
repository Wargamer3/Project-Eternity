using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core
{
    public interface ICollisionObject<T> where T : class, ICollisionObject<T>
    {
        CollisionObject<T> Collision {get;}
    }

    public sealed class CollisionObject<T> where T : class, ICollisionObject<T>
    {
        public List<Polygon> ListCollisionPolygon;
        public LinkedList<CollisionZone<T>> ListParent;
        public Vector2 Center;
        public float Radius;

        public CollisionObject(Vector2[] ArrayVertex, int MaxWidth, int MaxHeight)
        {
            ListCollisionPolygon = new List<Polygon>() { new Polygon(ArrayVertex, MaxWidth, MaxHeight) };
            ListParent = new LinkedList<CollisionZone<T>>();

            Init();
        }

        public void Init()
        {
            float totalX = 0;
            float totalY = 0;
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

                totalX += ListCollisionPolygon[P].Center.X;
                totalY += ListCollisionPolygon[P].Center.Y;
            }

            Center = new Vector2(totalX / ListCollisionPolygon.Count, totalY / ListCollisionPolygon.Count);
            Radius = Math.Max(MaxX - MinX, MaxY - MinY) / 2f;
        }

        public bool CollideWith<V>(CollisionObject<V> Other, Vector2 OtherSpeed, out PolygonCollisionResult FinalCollisionResult, out Polygon FinalCollisionPolygon, out Polygon FinalOtherPolygon)
            where V : class, ICollisionObject<V>
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

    public class CollisionZone<T> where T : class, ICollisionObject<T>
    {
        public LinkedList<T> ListObjectInZoneAndOverlappingParents;

        public CollisionZone<T>[] ArraySubZone;

        int ZoneX;
        int ZoneY;
        int SubdivisionSize;

        public CollisionZone(int Size, int NumberOfSubdivisions, int ZoneX, int ZoneY)
        {
            this.ZoneX = ZoneX;
            this.ZoneY = ZoneY;

            ListObjectInZoneAndOverlappingParents = new LinkedList<T>();
            ArraySubZone = new CollisionZone<T>[NumberOfSubdivisions * NumberOfSubdivisions];
            if (NumberOfSubdivisions > 0)
            {
                SubdivisionSize = Size / NumberOfSubdivisions;

                for (int X = 0; X < NumberOfSubdivisions; ++X)
                {
                    for (int Y = 0; Y < NumberOfSubdivisions; ++Y)
                    {
                        ArraySubZone[X + Y * NumberOfSubdivisions] = new CollisionZone<T>(SubdivisionSize, 0, ZoneX + X * NumberOfSubdivisions, ZoneY + Y * NumberOfSubdivisions);
                    }
                }
            }
        }

        private int[] GetKeysFromCircle(Vector2 Position, float Radius)
        {
            int RelativeX = (int)Position.X - ZoneX;
            int RelativeY = (int)Position.Y - ZoneY;
            int MinX = (int)(RelativeX - Radius) / SubdivisionSize;
            int MaxX = (int)(RelativeX + Radius) / SubdivisionSize;
            int MinY = (int)(RelativeY - Radius) / SubdivisionSize;
            int MaxY = (int)(RelativeY + Radius) / SubdivisionSize;

            if (MinX == MaxX && MinY == MaxY)
                return new int[1] { MinX };

            int LineWidth = SubdivisionSize;
            int XDiff = MaxX - MinX + 1;
            int ArraySize = XDiff * (MaxY - MinY + 1);
            int[] ReturnValue = new int[ArraySize];
            int Index = 0;
            int Value = MinX;
            for (int i = 0; i < ArraySize; ++i)
            {
                ReturnValue[Index++] = Value++;
                if (Value > MaxX)
                {
                    Value += LineWidth - XDiff;
                    MaxX += LineWidth;
                }
            }

            return ReturnValue;
        }

        public void AddToZone(T NewObject)
        {
            NewObject.Collision.ListParent.AddLast(this);

            if (ArraySubZone.Length > 0)
            {
                int[] Keys = GetKeysFromCircle(NewObject.Collision.Center, NewObject.Collision.Radius);

                for (int K = 0; K < Keys.Length; ++K)
                {
                    ArraySubZone[Keys[K]].AddToZone(NewObject);
                }
            }

            ListObjectInZoneAndOverlappingParents.AddLast(NewObject);
        }

        public void Move(T ObjectToMove, Vector2 Offset)
        {
            ObjectToMove.Collision.Center += Offset;
            CollisionZone<T> Root = ObjectToMove.Collision.ListParent.First.Value;

            foreach (CollisionZone<T> ActiveParent in ObjectToMove.Collision.ListParent)
            {
                ActiveParent.ListObjectInZoneAndOverlappingParents.Remove(ObjectToMove);
            }
            ObjectToMove.Collision.ListParent.Clear();
            Root.AddToZone(ObjectToMove);
        }

        public void Remove(T ObjectToRemove)
        {
            foreach (CollisionZone<T> ActiveParent in ObjectToRemove.Collision.ListParent)
            {
                ActiveParent.ListObjectInZoneAndOverlappingParents.Remove(ObjectToRemove);
            }
            ObjectToRemove.Collision.ListParent.Clear();
        }

        public void Clear()
        {
            LinkedListNode<T> ActiveObject = ListObjectInZoneAndOverlappingParents.First;
            while (ActiveObject != null)
            {
                LinkedListNode<T> NextObject = ActiveObject.Next;
                Remove(ActiveObject.Value);
                ActiveObject = NextObject;
            }
        }

        public HashSet<T> GetCollidableObjects(T ActiveObject)
        {
            HashSet<T> SetUniqueObject = new HashSet<T>();
            //Remove duplicates.
            foreach (CollisionZone<T> ActiveParent in ActiveObject.Collision.ListParent)
            {
                foreach (T ActiveParentObject in ActiveParent.ListObjectInZoneAndOverlappingParents)
                {
                    if (ActiveObject != ActiveParentObject)
                    {
                        SetUniqueObject.Add(ActiveParentObject);
                    }
                }
            }

            return SetUniqueObject;
        }

        public HashSet<T> GetCollidableObjects<V>(CollisionObject<V> CollisionBox)
            where V : class, ICollisionObject<V>
        {
            HashSet<T> SetUniqueObject = new HashSet<T>();
            int[] Keys = GetKeysFromCircle(CollisionBox.Center, CollisionBox.Radius);
            for (int K = 0; K < Keys.Length; ++K)
            {
                foreach (T ActiveObstacle in ArraySubZone[Keys[K]].ListObjectInZoneAndOverlappingParents)
                {
                    SetUniqueObject.Add(ActiveObstacle);
                }
            }

            return SetUniqueObject;
        }
    }
}
