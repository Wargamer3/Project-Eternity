using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core
{
    public class CollisionZone3D<T> where T : class, ICollisionObjectHolder3D<T>
    {
        public LinkedList<T> ListObjectInZoneAndOverlappingParents;

        public CollisionZone3D<T>[] ArraySubZone;

        int ZoneX;
        int ZoneY;
        int SubdivisionSize;

        public CollisionZone3D(int Size, int NumberOfSubdivisions, int ZoneX, int ZoneY)
        {
            this.ZoneX = ZoneX;
            this.ZoneY = ZoneY;

            ListObjectInZoneAndOverlappingParents = new LinkedList<T>();
            ArraySubZone = new CollisionZone3D<T>[NumberOfSubdivisions * NumberOfSubdivisions];
            if (NumberOfSubdivisions > 0)
            {
                SubdivisionSize = Size / NumberOfSubdivisions;

                for (int X = 0; X < NumberOfSubdivisions; ++X)
                {
                    for (int Y = 0; Y < NumberOfSubdivisions; ++Y)
                    {
                        ArraySubZone[X + Y * NumberOfSubdivisions] = new CollisionZone3D<T>(SubdivisionSize, 0, ZoneX + X * NumberOfSubdivisions, ZoneY + Y * NumberOfSubdivisions);
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
                int[] Keys = GetKeysFromCircle(NewObject.Collision.Position, NewObject.Collision.Radius);

                for (int K = 0; K < Keys.Length; ++K)
                {
                    if (Keys[K] >= 0 && Keys[K] < ArraySubZone.Length)
                    {
                        ArraySubZone[Keys[K]].AddToZone(NewObject);
                    }
                }
            }

            ListObjectInZoneAndOverlappingParents.AddLast(NewObject);
        }

        public void Move(T ObjectToMove, Vector2 Offset)
        {
            ObjectToMove.Collision.Position += Offset;
            CollisionZone3D<T> Root = ObjectToMove.Collision.ListParent.First.Value;

            foreach (CollisionZone3D<T> ActiveParent in ObjectToMove.Collision.ListParent)
            {
                ActiveParent.ListObjectInZoneAndOverlappingParents.Remove(ObjectToMove);
            }
            ObjectToMove.Collision.ListParent.Clear();
            Root.AddToZone(ObjectToMove);
        }

        public void Remove(T ObjectToRemove)
        {
            foreach (CollisionZone3D<T> ActiveParent in ObjectToRemove.Collision.ListParent)
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
            foreach (CollisionZone3D<T> ActiveParent in ActiveObject.Collision.ListParent)
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

        public HashSet<T> GetCollidableObjects<V>(CollisionObject2D<V> CollisionBox)
            where V : class, ICollisionObjectHolder2D<V>
        {
            HashSet<T> SetUniqueObject = new HashSet<T>();
            int[] Keys = GetKeysFromCircle(CollisionBox.Position, CollisionBox.Radius);
            for (int K = 0; K < Keys.Length; ++K)
            {
                if (Keys[K] >= 0)
                {
                    foreach (T ActiveObstacle in ArraySubZone[Keys[K]].ListObjectInZoneAndOverlappingParents)
                    {
                        SetUniqueObject.Add(ActiveObstacle);
                    }
                }
            }

            return SetUniqueObject;
        }
    }
}
