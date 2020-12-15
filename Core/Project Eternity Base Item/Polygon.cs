using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public struct PolygonCollisionResult
    {
        public Vector2 Axis;
        public float Distance;

        public PolygonCollisionResult(Vector2 Axis, float Distance)
        {
            this.Axis = Axis;
            this.Distance = Distance;
        }
    }

    public class PolygonTriangle
    {
        public enum SelectionTypes { None, Vertex, Edge, Triangle, Polygon };

        public SelectionTypes SelectionType;

        public short VertexIndex;
        public short TriangleIndex;
        public short EdgeIndex1;
        public short EdgeIndex2;
        public Polygon ActivePolygon;

        public PolygonTriangle()
        {
            SelectionType = SelectionTypes.None;
            VertexIndex = -1;
            TriangleIndex = -1;

            EdgeIndex1 = VertexIndex;

            if ((VertexIndex - TriangleIndex) < 2)
                EdgeIndex2 = (short)(VertexIndex + 1);
            else
                EdgeIndex2 = TriangleIndex;
        }

        public PolygonTriangle(SelectionTypes SelectionType, Polygon ActivePolygon, short TriangleIndex, short VertexIndex)
        {
            this.SelectionType = SelectionType;
            this.ActivePolygon = ActivePolygon;
            this.VertexIndex = VertexIndex;
            this.TriangleIndex = TriangleIndex;

            EdgeIndex1 = VertexIndex;

            if ((VertexIndex - TriangleIndex) < 2)
                EdgeIndex2 = (short)(VertexIndex + 1);
            else
                EdgeIndex2 = TriangleIndex;
        }

        public static PolygonTriangle EdgeSelection(Polygon ActivePolygon, short EdgeIndex)
        {
            return new PolygonTriangle(SelectionTypes.Edge, ActivePolygon, (short)(EdgeIndex - (EdgeIndex % 3)), EdgeIndex);
        }

        public static PolygonTriangle TriangleSelection(Polygon ActivePolygon, short TriangleIndex)
        {
            return new PolygonTriangle(SelectionTypes.Triangle, ActivePolygon, TriangleIndex, TriangleIndex);
        }

        public static PolygonTriangle VertexSelection(Polygon ActivePolygon, int VertexIndex)
        {
            return new PolygonTriangle(SelectionTypes.Vertex, ActivePolygon, (short)(VertexIndex - (VertexIndex % 3)), (short)VertexIndex);
        }

        public void Move(float OffsetX, float OffsetY)
        {
            switch (SelectionType)
            {
                case SelectionTypes.Vertex:
                    ActivePolygon.ArrayVertex[VertexIndex].X += OffsetX;
                    ActivePolygon.ArrayVertex[VertexIndex].Y += OffsetY;
                    break;

                case SelectionTypes.Edge:
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[EdgeIndex1]].X += OffsetX;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[EdgeIndex1]].Y += OffsetY;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[EdgeIndex2]].X += OffsetX;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[EdgeIndex2]].Y += OffsetY;
                    break;

                case SelectionTypes.Triangle:
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[TriangleIndex]].X += OffsetX;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[TriangleIndex]].Y += OffsetY;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[TriangleIndex + 1]].X += OffsetX;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[TriangleIndex + 1]].Y += OffsetY;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[TriangleIndex + 2]].X += OffsetX;
                    ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[TriangleIndex + 2]].Y += OffsetY;
                    break;

                case SelectionTypes.Polygon:
                    ActivePolygon.Offset(OffsetX, OffsetY);
                    break;
            }
            ActivePolygon.ComputePerpendicularAxis();
            ActivePolygon.UpdateWorldPosition(Vector2.Zero, 0);
        }

        public void Draw(CustomSpriteBatch g, GraphicsDevice GraphicsDevice, Texture2D sprPixel)
        {
            if (ActivePolygon == null)
                return;

            Draw(g, GraphicsDevice, sprPixel, this);
        }

        public static void Draw(CustomSpriteBatch g, GraphicsDevice GraphicsDevice, Texture2D sprPixel, PolygonTriangle Selection)
        {
            if (Selection.SelectionType == SelectionTypes.Polygon)
            {
                Selection.ActivePolygon.Draw(GraphicsDevice);
            }
            else
            {
                for (int I = 0; I < Selection.ActivePolygon.ArrayIndex.Length; I += 3)
                {
                    Vector2 Vertex1 = Selection.ActivePolygon.ArrayVertex[Selection.ActivePolygon.ArrayIndex[I]];
                    Vector2 Vertex2 = Selection.ActivePolygon.ArrayVertex[Selection.ActivePolygon.ArrayIndex[I + 1]];

                    if (Selection.EdgeIndex1 == I && Selection.EdgeIndex2 == (I + 1) && Selection.SelectionType == SelectionTypes.Edge)
                        g.DrawLine(sprPixel, Vertex1, Vertex2, Color.Red, 1);
                    else
                        g.DrawLine(sprPixel, Vertex1, Vertex2, Color.Black, 1);

                    if (I + 2 < Selection.ActivePolygon.ArrayIndex.Length)
                    {
                        Vector2 Vertex3 = Selection.ActivePolygon.ArrayVertex[Selection.ActivePolygon.ArrayIndex[I + 2]];
                        
                        if (Selection.EdgeIndex1 == (I + 1) && Selection.EdgeIndex2 == (I + 2) && Selection.SelectionType == SelectionTypes.Edge)
                            g.DrawLine(sprPixel, Vertex2, Vertex3, Color.Red, 1);
                        else
                            g.DrawLine(sprPixel, Vertex2, Vertex3, Color.Black, 1);

                        if (Selection.EdgeIndex1 == (I + 2) && Selection.EdgeIndex2 == I && Selection.SelectionType == SelectionTypes.Edge)
                            g.DrawLine(sprPixel, Vertex3, Vertex1, Color.Red, 1);
                        else
                            g.DrawLine(sprPixel, Vertex3, Vertex1, Color.Black, 1);
                    }
                }

                for (int V = 0; V < Selection.ActivePolygon.ArrayVertex.Length; V++)
                {
                    if (Selection.SelectionType == SelectionTypes.Vertex && Selection.VertexIndex == V)
                    {
                        g.Draw(sprPixel, new Rectangle((int)Selection.ActivePolygon.ArrayVertex[V].X - 2,
                                                        (int)Selection.ActivePolygon.ArrayVertex[V].Y - 2, 5, 5), Color.Red);
                    }
                    else
                    {
                        g.Draw(sprPixel, new Rectangle((int)Selection.ActivePolygon.ArrayVertex[V].X - 2,
                                                        (int)Selection.ActivePolygon.ArrayVertex[V].Y - 2, 5, 5), Color.Black);
                    }
                }

                //Triangle Selected.
                if (Selection.SelectionType == SelectionTypes.Triangle)
                {
                    Selection.ActivePolygon.DrawTriangle(GraphicsDevice, Selection.VertexIndex);
                }
            }
        }
    }

    public class Polygon
    {
        public Vector2[] ArrayVertex;
        public Vector2[] ArrayAxis;
        public Vector2 Center;
        public float Radius;

        public short[] ArrayIndex;
        public Vector2[] ArrayUVCoordinates;
        public Color[] ArrayColorPoints;

        public int VertexCount { get { return ArrayVertex.Length; } }

        public int TriangleCount;

        /// <summary>
        /// Vertex translated by position and rotated by Rotation.
        /// </summary>
        public VertexPositionColorTexture[] ArrayFinalVertex;

        public Polygon()
        {
            ArrayIndex = new short[0];
        }

        public Polygon(Vector2[] ArrayVertex, float MaxWidth, float MaxHeight)
        {
            this.ArrayVertex = ArrayVertex;
            TriangleCount = 2;

            ArrayIndex = new short[6];
            ArrayIndex[0] = 0;
            ArrayIndex[1] = 1;
            ArrayIndex[2] = 2;
            ArrayIndex[3] = 0;
            ArrayIndex[4] = 2;
            ArrayIndex[5] = 3;
            ComputePerpendicularAxis();
            ComputeColorAndUVCoordinates(MaxWidth, MaxHeight);
            UpdateWorldPosition(Vector2.Zero, 0);
        }

        public Polygon(Vector2[] ArrayVertex, short[] ArrayIndex, float MaxWidth, float MaxHeight)
        {
            this.ArrayVertex = ArrayVertex;
            this.ArrayIndex = ArrayIndex;
            TriangleCount = ArrayIndex.Length / 3;

            ComputePerpendicularAxis();
            ComputeColorAndUVCoordinates(MaxWidth, MaxHeight);
            UpdateWorldPosition(Vector2.Zero, 0);
        }

        public Polygon(Polygon Copy)
        {
            ArrayVertex = new Vector2[Copy.ArrayVertex.Length];
            Array.Copy(Copy.ArrayVertex, ArrayVertex, ArrayVertex.Length);

            ArrayIndex = new short[Copy.ArrayIndex.Length];
            Array.Copy(Copy.ArrayIndex, ArrayIndex, ArrayIndex.Length);

            ArrayUVCoordinates = new Vector2[Copy.ArrayUVCoordinates.Length];
            Array.Copy(Copy.ArrayUVCoordinates, ArrayUVCoordinates, ArrayUVCoordinates.Length);

            ArrayColorPoints = new Color[Copy.ArrayColorPoints.Length];
            Array.Copy(Copy.ArrayColorPoints, ArrayColorPoints, ArrayColorPoints.Length);

            ArrayFinalVertex = new VertexPositionColorTexture[Copy.ArrayFinalVertex.Length];
            Array.Copy(Copy.ArrayFinalVertex, ArrayFinalVertex, ArrayFinalVertex.Length);

            TriangleCount = Copy.TriangleCount;
        }

        public Polygon(BinaryReader BR, float MaxWidth, float MaxHeight)
        {
            float MinX = float.MaxValue;
            float MaxX = float.MinValue;
            float MinY = float.MaxValue;
            float MaxY = float.MinValue;

            int ArrayVertexLength = BR.ReadInt32();
            ArrayVertex = new Vector2[ArrayVertexLength];
            for (int V = 0; V < ArrayVertexLength; V++)
            {
                int VertexX = BR.ReadInt32();
                int VertexY = BR.ReadInt32();
                ArrayVertex[V] = new Vector2(VertexX, VertexY);

                if (VertexX < MinX)
                    MinX = VertexX;
                if (VertexX > MaxX)
                    MaxX = VertexX;

                if (VertexY < MinY)
                    MinY = VertexY;
                if (VertexY > MaxY)
                    MaxY = VertexY;
            }

            int ArrayIndiceLength = BR.ReadInt32();
            ArrayIndex = new short[ArrayIndiceLength];
            for (int V = 0; V < ArrayIndiceLength; V++)
            {
                ArrayIndex[V] = BR.ReadInt16();
            }
            TriangleCount = ArrayIndex.Length / 3;

            ComputeColorAndUVCoordinates(MaxWidth, MaxHeight);
            ComputePerpendicularAxis();
            UpdateWorldPosition(Vector2.Zero, 0);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ArrayVertex.Length);
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                BW.Write((int)ArrayVertex[V].X);
                BW.Write((int)ArrayVertex[V].Y);
            }
            BW.Write(ArrayIndex.Length);
            for (int I = 0; I < ArrayIndex.Length; I++)
            {
                BW.Write(ArrayIndex[I]);
            }
        }

        public void ComputerCenter()
        {
            float totalX = 0;
            float totalY = 0;
            float MinX = float.MaxValue;
            float MinY = float.MaxValue;
            float MaxX = float.MinValue;
            float MaxY = float.MinValue;

            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                if (ArrayVertex[i].X < MinX)
                    MinX = ArrayVertex[i].X;
                if (ArrayVertex[i].X > MaxX)
                    MaxX = ArrayVertex[i].X;

                if (ArrayVertex[i].Y < MinY)
                    MinY = ArrayVertex[i].Y;
                if (ArrayVertex[i].Y > MaxY)
                    MaxY = ArrayVertex[i].Y;

                totalX += ArrayVertex[i].X;
                totalY += ArrayVertex[i].Y;
            }

            Center = new Vector2(totalX / ArrayVertex.Length, totalY / ArrayVertex.Length);
            Radius = Math.Max(MaxX - MinX, MaxY - MinY) / 2;
        }

        public bool ComputePerpendicularAxis()
        {
            if (ArrayVertex.Length < 2)
                return false;
            else if (ArrayVertex.Length == 2)
            {
                ArrayAxis = new Vector2[1] { ArrayVertex[1] - ArrayVertex[0] };
            }
            else
            {
                Vector2 Vertex1;
                Vector2 Vertex2;
                ArrayAxis = new Vector2[ArrayVertex.Length];

                for (int V = 0; V < ArrayVertex.Length; V++)
                {
                    Vertex1 = ArrayVertex[V];

                    if (V + 1 >= ArrayVertex.Length)
                        Vertex2 = ArrayVertex[0];
                    else
                        Vertex2 = ArrayVertex[V + 1];

                    ArrayAxis[V] = Vertex2 - Vertex1;
                    ArrayAxis[V] = new Vector2(-ArrayAxis[V].Y, ArrayAxis[V].X);
                    ArrayAxis[V].Normalize();
                }
            }
            return true;
        }

        public PolygonTriangle PolygonCollisionWithMouse(int PosX, int PosY)
        {
            short ClosestEdgeIndex;
            float MinDistanceToEdge;

            //Select Vertex.
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                if (PosX >= ArrayVertex[V].X - 2 && PosX <= ArrayVertex[V].X + 2
                    && PosY >= ArrayVertex[V].Y - 2 && PosY <= ArrayVertex[V].Y + 2)
                {
                    return PolygonTriangle.VertexSelection(this, V);
                }
            }

            short SelectedTriangleIndex = (short) PolygonCollisionPerTriangle(PosX, PosY, out ClosestEdgeIndex, out MinDistanceToEdge);

            if (SelectedTriangleIndex >= 0)
            {
                return PolygonTriangle.TriangleSelection(this, SelectedTriangleIndex);
            }

            if (MinDistanceToEdge < 5)
            {
                return PolygonTriangle.EdgeSelection(this, ClosestEdgeIndex);
            }

            return new PolygonTriangle();
        }

        public int PolygonCollisionPerTriangle(int PosX, int PosY, out short ClosestEdgeIndex, out float MinDistanceToEdge)
        {
            Vector2 CollisionPoint = new Vector2(PosX, PosY);
            MinDistanceToEdge = float.MaxValue;
            ClosestEdgeIndex = -1;
            int TriangleFoundIndex = -1;

            for (int I = 0; I < ArrayIndex.Length; I += 3)
            {
                Vector2 Vertex1 = ArrayVertex[ArrayIndex[I]];
                Vector2 Vertex2 = ArrayVertex[ArrayIndex[I + 1]];
                Vector2 Vertex3 = ArrayVertex[ArrayIndex[I + 2]];
                int EdgeCount = 0;

                bool InsideEdge;
                float DistanceToEdgeValue = DistanceToEdge(Vertex1, Vertex2, CollisionPoint, out InsideEdge);
                if (InsideEdge)
                {
                    if (DistanceToEdgeValue <= 0)
                    {
                        ++EdgeCount;
                    }
                    if (Math.Abs(DistanceToEdgeValue) < MinDistanceToEdge)
                    {
                        MinDistanceToEdge = Math.Abs(DistanceToEdgeValue);
                        ClosestEdgeIndex = (short)(I + 0);
                    }
                }
                if (Math.Abs(DistanceToEdgeValue) < MinDistanceToEdge)
                {
                    MinDistanceToEdge = Math.Abs(DistanceToEdgeValue);
                    ClosestEdgeIndex = (short)(I + 0);
                }

                DistanceToEdgeValue = DistanceToEdge(Vertex2, Vertex3, CollisionPoint, out InsideEdge);
                if (InsideEdge)
                {
                    if (DistanceToEdgeValue <= 0)
                    {
                        ++EdgeCount;
                    }
                    if (Math.Abs(DistanceToEdgeValue) < MinDistanceToEdge)
                    {
                        MinDistanceToEdge = Math.Abs(DistanceToEdgeValue);
                        ClosestEdgeIndex = (short)(I + 1);
                    }
                }
                if (Math.Abs(DistanceToEdgeValue) < MinDistanceToEdge)
                {
                    MinDistanceToEdge = Math.Abs(DistanceToEdgeValue);
                    ClosestEdgeIndex = (short)(I + 1);
                }

                DistanceToEdgeValue = DistanceToEdge(Vertex3, Vertex1, CollisionPoint, out InsideEdge);
                if (InsideEdge)
                {
                    if (DistanceToEdgeValue <= 0)
                    {
                        ++EdgeCount;
                    }
                    if (Math.Abs(DistanceToEdgeValue) < MinDistanceToEdge)
                    {
                        MinDistanceToEdge = Math.Abs(DistanceToEdgeValue);
                        ClosestEdgeIndex = (short)(I + 2);
                    }
                }
                if (Math.Abs(DistanceToEdgeValue) < MinDistanceToEdge)
                {
                    MinDistanceToEdge = Math.Abs(DistanceToEdgeValue);
                    ClosestEdgeIndex = (short)(I + 2);
                }

                if (EdgeCount == 3)
                    TriangleFoundIndex = I;
            }

            if (TriangleFoundIndex >= 0)
                return TriangleFoundIndex;
            else
                return -1;
        }

        public bool PolygonCollisionPerTriangle(float PosX, float PosY)
        {
            Vector2 CollisionPoint = new Vector2(PosX, PosY);

            for (int I = 0; I < ArrayIndex.Length; I += 3)
            {
                Vector2 Point1 = new Vector2(ArrayFinalVertex[ArrayIndex[I]].Position.X, ArrayFinalVertex[ArrayIndex[I]].Position.Y);
                Vector2 Point2 = new Vector2(ArrayFinalVertex[ArrayIndex[I + 1]].Position.X, ArrayFinalVertex[ArrayIndex[I + 1]].Position.Y);
                Vector2 Point3 = new Vector2(ArrayFinalVertex[ArrayIndex[I + 2]].Position.X, ArrayFinalVertex[ArrayIndex[I + 2]].Position.Y);
                bool InsideEdge1;
                bool InsideEdge2;
                bool InsideEdge3;

                DistanceToEdge(Point1, Point2, CollisionPoint, out InsideEdge1);
                DistanceToEdge(Point2, Point3, CollisionPoint, out InsideEdge2);
                DistanceToEdge(Point3, Point1, CollisionPoint, out InsideEdge3);

                if (InsideEdge1 && InsideEdge2 && InsideEdge3)
                    return true;
            }

            return false;
        }

        public float DistanceToEdge(Vector2 Vertex1, Vector2 Vertex2, Vector2 Point, out bool InsideEdge)
        {
            Vector2 Edge = Vertex2 - Vertex1;
            Vector2 Axis = new Vector2(-Edge.Y, Edge.X);
            Edge.Normalize();
            Axis.Normalize();

            float MinValue = Vector2.Dot(Edge, Vertex1);
            float MaxValue = Vector2.Dot(Edge, Vertex2);
            float PointValue = Vector2.Dot(Edge, Point);

            if (PointValue >= MinValue && PointValue <= MaxValue)
            {
                InsideEdge = true;
                return Vector2.Dot(Axis, Point) - Vector2.Dot(Axis, Vertex1);
            }
            InsideEdge = false;
            return float.MaxValue;
        }

        public static Vector2 GetCollisionPointFromLine(Vector2 NearVertex, Vector2 FarVertex, Vector2 Speed, Polygon FinalCollisionPolygon, out Vector2 CollisionLineAxis)
        {
            float MinDistance = float.MaxValue;
            Vector2 FinalCollisionPoint = FarVertex;
            CollisionLineAxis = Vector2.Zero;

            for (int I = 0; I < FinalCollisionPolygon.ArrayIndex.Length; I += 3)
            {
                Vector2 Vertex1 = FinalCollisionPolygon.ArrayVertex[FinalCollisionPolygon.ArrayIndex[I]];
                Vector2 Vertex2 = FinalCollisionPolygon.ArrayVertex[FinalCollisionPolygon.ArrayIndex[I + 1]];
                Vector2 Vertex3 = FinalCollisionPolygon.ArrayVertex[FinalCollisionPolygon.ArrayIndex[I + 2]];

                Vector2 CollisionPoint = Vector2.Zero;

                if (Polygon.DoLinesIntersect(
                    NearVertex,
                    FarVertex + Speed,
                    Vertex1,
                    Vertex2,
                    ref CollisionPoint))
                {
                    float DistanceToPoint = (NearVertex - CollisionPoint).Length();

                    if (DistanceToPoint < MinDistance)
                    {
                        CollisionLineAxis = Vertex2 - Vertex1;
                        MinDistance = DistanceToPoint;
                        FinalCollisionPoint = CollisionPoint;
                    }
                }

                if (Polygon.DoLinesIntersect(
                    NearVertex,
                    FarVertex + Speed,
                    Vertex2,
                    Vertex3,
                    ref CollisionPoint))
                {
                    float DistanceToPoint = (NearVertex - CollisionPoint).Length();

                    if (DistanceToPoint < MinDistance)
                    {
                        CollisionLineAxis = Vertex3 - Vertex2;
                        MinDistance = DistanceToPoint;
                        FinalCollisionPoint = CollisionPoint;
                    }
                }

                if (Polygon.DoLinesIntersect(
                    NearVertex,
                    FarVertex + Speed,
                    Vertex3,
                    Vertex1,
                    ref CollisionPoint))
                {
                    float DistanceToPoint = (NearVertex - CollisionPoint).Length();

                    if (DistanceToPoint < MinDistance)
                    {
                        CollisionLineAxis = Vertex1 - Vertex3;
                        MinDistance = DistanceToPoint;
                        FinalCollisionPoint = CollisionPoint;
                    }
                }
            }

            return FinalCollisionPoint;
        }

        public static PolygonCollisionResult PolygonCollisionSAT(Polygon PolygonA, Polygon PolygonB, Vector2 PolygonASpeed)
        {
            bool Intersection = true;
            bool IntersectionNext = true;

            float minA = 0, maxA = 0;
            float minB = 0, maxB = 0;

            int AxisCountA = PolygonA.ArrayAxis.Length;
            int AxisCountB = PolygonB.ArrayAxis.Length;

            Vector2 Axis;
            Vector2 AxisTranslation = Vector2.Zero;
            float minIntervalDistance = float.MaxValue;

            for (int AxisIndex = 0; AxisIndex < AxisCountA + AxisCountB; AxisIndex++)
            {
                if (AxisIndex < AxisCountA)
                {
                    Axis = PolygonA.ArrayAxis[AxisIndex];
                }
                else
                {
                    Axis = PolygonB.ArrayAxis[AxisIndex - AxisCountA];
                }

                ProjectPolygon(Axis, PolygonA, out minA, out maxA);
                ProjectPolygon(Axis, PolygonB, out minB, out maxB);

                if (IntervalDistance(minA, maxA, minB, maxB) >= 0)
                    Intersection = false;

                float AxeVitesse = Vector2.Dot(Axis, PolygonASpeed);

                if (AxeVitesse < 0)
                    minA += AxeVitesse;
                else
                    maxA += AxeVitesse;

                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance >= 0)
                    IntersectionNext = false;

                if (Intersection || IntersectionNext)
                {
                    intervalDistance = Math.Abs(intervalDistance);

                    if (intervalDistance < minIntervalDistance)
                    {
                        minIntervalDistance = intervalDistance;
                        AxisTranslation = Axis;

                        Vector2 p = PolygonA.Center - PolygonB.Center;
                        if (Vector2.Dot(p, AxisTranslation) < 0)
                            AxisTranslation = -AxisTranslation;
                    }
                }
            }
            if (IntersectionNext)
            {
                PolygonCollisionResult CollisionResult = new PolygonCollisionResult(AxisTranslation, minIntervalDistance);

                //Vector2 MovementCorection = AxisTranslation * minIntervalDistance;
                //Vector2 FinalPosition = Position + Speed + MovementCorection;
                return CollisionResult;
            }

            return new PolygonCollisionResult(Vector2.Zero, -1);
        }

        public static PolygonCollisionResult PolygonCollisionSAT(Polygon PolygonA, Polygon PolygonB, Vector2 PolygonASpeed, Vector2 PolygonAOffset)
        {
            bool Intersection = true;
            bool IntersectionNext = true;

            float minA = 0, maxA = 0;
            float minB = 0, maxB = 0;

            int AxisCountA = PolygonA.ArrayAxis.Length;
            int AxisCountB = PolygonB.ArrayAxis.Length;

            Vector2 Axis;
            Vector2 AxisTranslation = Vector2.Zero;
            float minIntervalDistance = float.MaxValue;

            for (int AxisIndex = 0; AxisIndex < AxisCountA + AxisCountB; AxisIndex++)
            {
                if (AxisIndex < AxisCountA)
                {
                    Axis = PolygonA.ArrayAxis[AxisIndex];
                }
                else
                {
                    Axis = PolygonB.ArrayAxis[AxisIndex - AxisCountA];
                }

                ProjectPolygon(Axis, PolygonA, out minA, out maxA);
                ProjectPolygon(Axis, PolygonB, out minB, out maxB);

                if (IntervalDistance(minA, maxA, minB, maxB) >= 0)
                    Intersection = false;

                float AxeVitesse = Vector2.Dot(Axis, PolygonASpeed);

                if (AxeVitesse < 0)
                    minA += AxeVitesse;
                else
                    maxA += AxeVitesse;

                float AxeOffset = Vector2.Dot(Axis, PolygonAOffset);

                maxA += AxeOffset;
                minA += AxeOffset;

                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance >= 0)
                    IntersectionNext = false;

                if (Intersection || IntersectionNext)
                {
                    intervalDistance = Math.Abs(intervalDistance);

                    if (intervalDistance < minIntervalDistance)
                    {
                        minIntervalDistance = intervalDistance;
                        AxisTranslation = Axis;

                        Vector2 p = PolygonA.Center - PolygonB.Center;
                        if (Vector2.Dot(p, AxisTranslation) < 0)
                            AxisTranslation = -AxisTranslation;
                    }
                }
            }
            if (IntersectionNext)
            {
                PolygonCollisionResult CollisionResult = new PolygonCollisionResult(AxisTranslation, minIntervalDistance);

                //Vector2 MovementCorection = AxisTranslation * minIntervalDistance;
                //Vector2 FinalPosition = Position + Speed + MovementCorection;
                return CollisionResult;
            }

            return new PolygonCollisionResult(Vector2.Zero, -1);
        }

        public static PolygonCollisionResult PolygonCollisionSAT(Polygon PolygonA, Polygon PolygonB, Vector2 Speed, out PolygonCollisionResult PolygonBResult)
        {
            PolygonBResult = new PolygonCollisionResult();
            bool Intersection = true;
            bool IntersectionNext = true;

            float minA = 0, maxA = 0;
            float minB = 0, maxB = 0;

            int AxisCountA = PolygonA.ArrayAxis.Length;
            int AxisCountB = PolygonB.ArrayAxis.Length;

            Vector2 Axis;
            Vector2 AxisTranslation = Vector2.Zero;
            float minIntervalDistance = float.MaxValue;

            for (int AxisIndex = 0; AxisIndex < AxisCountA + AxisCountB; AxisIndex++)
            {
                if (AxisIndex < AxisCountA)
                {
                    Axis = PolygonA.ArrayAxis[AxisIndex];
                }
                else
                {
                    Axis = PolygonB.ArrayAxis[AxisIndex - AxisCountA];
                }

                ProjectPolygon(Axis, PolygonA, out minA, out maxA);
                ProjectPolygon(Axis, PolygonB, out minB, out maxB);

                if (IntervalDistance(minA, maxA, minB, maxB) >= 0)
                    Intersection = false;

                float AxeVitesse = Vector2.Dot(Axis, Speed);

                if (AxeVitesse < 0)
                    minA += AxeVitesse;
                else
                    maxA += AxeVitesse;

                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance >= 0)
                    IntersectionNext = false;

                if (Intersection || IntersectionNext)
                {
                    intervalDistance = Math.Abs(intervalDistance);
                    if (intervalDistance < minIntervalDistance)
                    {
                        minIntervalDistance = intervalDistance;
                        AxisTranslation = Axis;

                        Vector2 p = PolygonA.Center - PolygonB.Center;
                        if (Vector2.Dot(p, AxisTranslation) < 0)
                            AxisTranslation = -AxisTranslation;

                        if (AxisIndex >= AxisCountA)
                            PolygonBResult = new PolygonCollisionResult(AxisTranslation, minIntervalDistance);

                    }
                }
            }

            if (IntersectionNext)
            {
                PolygonCollisionResult CollisionResult = new PolygonCollisionResult(AxisTranslation, minIntervalDistance);

                //Vector2 MovementCorection = AxisTranslation * minIntervalDistance;
                //Vector2 FinalPosition = Position + Speed + MovementCorection;
                return CollisionResult;
            }

            return new PolygonCollisionResult(Vector2.Zero, -1);
        }

        public static bool CheckPolygon(Vector2 StartPoint, Vector2 EndPoint, Polygon PolygonToCheck, ref Vector2 ptIntersection, ref Vector2 CollisionVertex1, ref Vector2 CollisionVertex2)
        {
            for (int V = 0; V < PolygonToCheck.ArrayVertex.Length; V++)
            {
                bool Collision = false;
                CollisionVertex1 = PolygonToCheck.ArrayVertex[V];

                if (V + 1 < PolygonToCheck.ArrayVertex.Length)
                {
                    CollisionVertex2 = PolygonToCheck.ArrayVertex[V + 1];
                }
                else
                {
                    CollisionVertex2 = PolygonToCheck.ArrayVertex[0];
                }
                Collision = DoLinesIntersect(StartPoint, EndPoint, CollisionVertex1, CollisionVertex2, ref ptIntersection);

                if (Collision)
                    return true;
            }
            return false;
        }

        public void AddVertexOnEdge(Vector2 NewVertex, short PreviousVertexIndex, short NextVertexIndex)
        {
            Vector2[] NewArrayVertex = new Vector2[ArrayVertex.Length + 1];
            Array.Copy(ArrayVertex, NewArrayVertex, ArrayVertex.Length);
            NewArrayVertex[NewArrayVertex.Length - 1] = NewVertex;

            int LastIndex = ArrayIndex.Length;
            short[] NewArrayIndices = new short[ArrayIndex.Length + 3];
            Array.Copy(ArrayIndex, NewArrayIndices, ArrayIndex.Length);

            NewArrayIndices[LastIndex] = ArrayIndex[PreviousVertexIndex];
            NewArrayIndices[LastIndex + 1] = (short)VertexCount;
            NewArrayIndices[LastIndex + 2] = ArrayIndex[NextVertexIndex];

            Vector2 NewUV = ComputeUVFromEdgeNeighbours(ArrayIndex[PreviousVertexIndex], ArrayIndex[NextVertexIndex], NewVertex);
            Vector2[] NewArrayUV = new Vector2[ArrayUVCoordinates.Length + 1];
            Array.Copy(ArrayUVCoordinates, NewArrayUV, ArrayUVCoordinates.Length);

            NewArrayUV[ArrayUVCoordinates.Length] = NewUV;

            Color[] NewArrayColor = new Color[ArrayColorPoints.Length + 1];
            Array.Copy(ArrayColorPoints, NewArrayColor, ArrayColorPoints.Length);

            NewArrayColor[ArrayColorPoints.Length] = ArrayColorPoints[ArrayIndex[PreviousVertexIndex]];

            ArrayVertex = NewArrayVertex;
            ArrayIndex = NewArrayIndices;
            ArrayUVCoordinates = NewArrayUV;
            ArrayColorPoints = NewArrayColor;

            TriangleCount = ArrayIndex.Length / 3;

            float MinX = float.MaxValue;
            float MaxX = float.MinValue;
            float MinY = float.MaxValue;
            float MaxY = float.MinValue;

            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                if (ArrayVertex[i].X < MinX)
                    MinX = ArrayVertex[i].X;
                if (ArrayVertex[i].X > MaxX)
                    MaxX = ArrayVertex[i].X;

                if (ArrayVertex[i].Y < MinY)
                    MinY = ArrayVertex[i].Y;
                if (ArrayVertex[i].Y > MaxY)
                    MaxY = ArrayVertex[i].Y;
            }
        }

        public void SplitEdge(Vector2 NewVertex, int SelectedEdgeArrayIndex, short PreviousVertexIndex, short NextVertexIndex)
        {
            short FirstIndex = 0;
            short SecondIndex = 0;
            short ThirdIndex = 0;
            if (ArrayIndex[SelectedEdgeArrayIndex] == PreviousVertexIndex)
            {
                FirstIndex = ArrayIndex[SelectedEdgeArrayIndex];
                SecondIndex = ArrayIndex[SelectedEdgeArrayIndex + 1];
                ThirdIndex = ArrayIndex[SelectedEdgeArrayIndex + 2];
            }
            else if (ArrayIndex[SelectedEdgeArrayIndex + 1] == PreviousVertexIndex)
            {
                FirstIndex = ArrayIndex[SelectedEdgeArrayIndex + 1];
                SecondIndex = ArrayIndex[SelectedEdgeArrayIndex + 2];
                ThirdIndex = ArrayIndex[SelectedEdgeArrayIndex];
            }
            else if (ArrayIndex[SelectedEdgeArrayIndex + 2] == PreviousVertexIndex)
            {
                FirstIndex = ArrayIndex[SelectedEdgeArrayIndex + 2];
                SecondIndex = ArrayIndex[SelectedEdgeArrayIndex];
                ThirdIndex = ArrayIndex[SelectedEdgeArrayIndex + 1];
            }

            Vector2[] NewArrayVertex = new Vector2[ArrayVertex.Length + 1];
            Array.Copy(ArrayVertex, NewArrayVertex, ArrayVertex.Length);
            NewArrayVertex[ArrayVertex.Length] = NewVertex;
            short NewVertexIndex = (short)ArrayVertex.Length;

            short[] ArrayNewTriangleIndex1 = new short[3];
            ArrayNewTriangleIndex1[0] = FirstIndex;
            ArrayNewTriangleIndex1[1] = NewVertexIndex;
            ArrayNewTriangleIndex1[2] = ThirdIndex;
            short[] ArrayNewTriangleIndex2 = new short[3];
            ArrayNewTriangleIndex2[0] = NewVertexIndex;
            ArrayNewTriangleIndex2[1] = SecondIndex;
            ArrayNewTriangleIndex2[2] = ThirdIndex;

            int LastIndex = ArrayIndex.Length;
            short[] NewArrayIndices = new short[ArrayIndex.Length + 3];
            Array.Copy(ArrayIndex, NewArrayIndices, ArrayIndex.Length);

            NewArrayIndices[SelectedEdgeArrayIndex] = ArrayNewTriangleIndex1[0];
            NewArrayIndices[SelectedEdgeArrayIndex + 1] = ArrayNewTriangleIndex1[1];
            NewArrayIndices[SelectedEdgeArrayIndex + 2] = ArrayNewTriangleIndex1[2];
            NewArrayIndices[LastIndex] = ArrayNewTriangleIndex2[0];
            NewArrayIndices[LastIndex + 1] = ArrayNewTriangleIndex2[1];
            NewArrayIndices[LastIndex + 2] = ArrayNewTriangleIndex2[2];

            Vector2 NewUV = ComputeUVFromEdgeNeighbours(ArrayIndex[PreviousVertexIndex], ArrayIndex[NextVertexIndex], NewVertex);
            Vector2[] NewArrayUV = new Vector2[ArrayUVCoordinates.Length + 1];
            Array.Copy(ArrayUVCoordinates, NewArrayUV, ArrayUVCoordinates.Length);

            NewArrayUV[ArrayUVCoordinates.Length] = NewUV;

            Color[] NewArrayColor = new Color[ArrayColorPoints.Length + 1];
            Array.Copy(ArrayColorPoints, NewArrayColor, ArrayColorPoints.Length);

            NewArrayColor[ArrayColorPoints.Length] = ArrayColorPoints[ArrayIndex[PreviousVertexIndex]];

            ArrayVertex = NewArrayVertex;
            ArrayIndex = NewArrayIndices;
            ArrayUVCoordinates = NewArrayUV;
            ArrayColorPoints = NewArrayColor;

            TriangleCount = ArrayIndex.Length / 3;
        }

        public void ReplaceVertex(int VertexIndexCurrent, short VertexIndexNew)
        {
            //Replace Indices
            for (int I = 0; I < ArrayIndex.Length; I++)
            {
                if (ArrayIndex[I] == VertexIndexCurrent)
                {
                    ArrayIndex[I] = VertexIndexNew;
                }
            }
            //Remove Vertex.
            Vector2[] NewArrayVertex = new Vector2[ArrayVertex.Length - 1];
            Array.Copy(ArrayVertex, 0, NewArrayVertex, 0, VertexIndexCurrent);
            Array.Copy(ArrayVertex, VertexIndexCurrent + 1, NewArrayVertex,
                VertexIndexCurrent, ArrayVertex.Length - VertexIndexCurrent - 1);

            Vector2[] NewArrayUV = new Vector2[ArrayUVCoordinates.Length - 1];
            Array.Copy(ArrayUVCoordinates, 0, NewArrayUV, 0, VertexIndexCurrent);
            Array.Copy(ArrayUVCoordinates, VertexIndexCurrent + 1, NewArrayUV,
                VertexIndexCurrent, ArrayUVCoordinates.Length - VertexIndexCurrent - 1);

            Color[] NewArrayColor = new Color[ArrayColorPoints.Length - 1];
            Array.Copy(ArrayColorPoints, 0, NewArrayColor, 0, VertexIndexCurrent);
            Array.Copy(ArrayColorPoints, VertexIndexCurrent + 1, NewArrayColor,
                VertexIndexCurrent, ArrayColorPoints.Length - VertexIndexCurrent - 1);

            //Update Indices.
            for (int I = 0; I < ArrayIndex.Length; I++)
            {
                if (ArrayIndex[I] > VertexIndexCurrent)
                    ArrayIndex[I]--;
            }

            ArrayVertex = NewArrayVertex;
            ArrayUVCoordinates = NewArrayUV;
            ArrayColorPoints = NewArrayColor;
            TriangleCount = ArrayIndex.Length / 3;
        }

        public void RemoveVertex(int VertexIndexCurrent)
        {
            int InsertionIndex = 0;
            short[] NewArrayIndices = new short[ArrayIndex.Length];

            //Remove Triangles.
            for (int I = 0; I < ArrayIndex.Length; I += 3)
            {
                if (ArrayIndex[I] != VertexIndexCurrent
                    && ArrayIndex[I + 1] != VertexIndexCurrent
                    && ArrayIndex[I + 2] != VertexIndexCurrent)
                {
                    NewArrayIndices[InsertionIndex] = ArrayIndex[I];
                    NewArrayIndices[InsertionIndex + 1] = ArrayIndex[I + 1];
                    NewArrayIndices[InsertionIndex + 2] = ArrayIndex[I + 2];
                    InsertionIndex += 3;
                }
            }
            Array.Resize(ref NewArrayIndices, InsertionIndex);
            ArrayIndex = NewArrayIndices;

            //Remove Vertex.
            Vector2[] NewArrayVertex = new Vector2[ArrayVertex.Length - 1];
            Array.Copy(ArrayVertex, 0, NewArrayVertex, 0, VertexIndexCurrent);
            Array.Copy(ArrayVertex, VertexIndexCurrent + 1, NewArrayVertex,
                VertexIndexCurrent, ArrayVertex.Length - VertexIndexCurrent - 1);

            //Update Indices.
            for (int I = 0; I < ArrayIndex.Length; I++)
            {
                if (ArrayIndex[I] > VertexIndexCurrent)
                    ArrayIndex[I]--;
            }

            ArrayVertex = NewArrayVertex;
            TriangleCount = ArrayIndex.Length / 3;
        }

        public void RemoveTriangle(int TriangleIndexCurrent)
        {
            short[] NewArrayIndices = new short[ArrayIndex.Length - 3];

            //Remove Triangle.
            Array.Copy(ArrayIndex, 0, NewArrayIndices, 0, TriangleIndexCurrent);
            Array.Copy(ArrayIndex, TriangleIndexCurrent + 3, NewArrayIndices,
                TriangleIndexCurrent, ArrayIndex.Length - TriangleIndexCurrent - 3);

            ArrayIndex = NewArrayIndices;
            TriangleCount--;

            RemoveUnlinkedVertex();
        }

        public void RemoveUnlinkedVertex()
        {
            int InsertionIndex = 0;

            Vector2[] NewArrayVertex = new Vector2[ArrayVertex.Length];
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                if (ArrayIndex.Contains((short)V))
                {
                    NewArrayVertex[InsertionIndex] = ArrayVertex[V];
                    ++InsertionIndex;
                }
            }
            Array.Resize(ref NewArrayVertex, InsertionIndex);
            ArrayVertex = NewArrayVertex;
        }

        public Polygon SplitPolygon(Vector2 SplitterVertex1, Vector2 SplitterVertex2)
        {
            List<Vector2> NewPolygon1 = new List<Vector2>();
            List<Vector2> NewPolygon2 = new List<Vector2>();
            List<Vector2> NewPolygonUV1 = new List<Vector2>();
            List<Vector2> NewPolygonUV2 = new List<Vector2>();
            List<short> NewPolygonIndex1 = new List<short>();
            List<short> NewPolygonIndex2 = new List<short>();
            List<Color> NewColorPoints1 = new List<Color>();
            List<Color> NewColorPoints2 = new List<Color>();
            short[] ArrayPolygonVertexReference1 = new short[ArrayVertex.Length];
            short[] ArrayPolygonVertexReference2 = new short[ArrayVertex.Length];

            Vector2 SplittingLine = SplitterVertex2 - SplitterVertex1;
            Vector2 SplittingAxis = new Vector2(-SplittingLine.Y, SplittingLine.X);
            SplittingAxis.Normalize();

            float SplitterVertexValue = Vector2.Dot(SplittingAxis, SplitterVertex1);
            float[] ArrayVertexValue = new float[ArrayVertex.Length];

            //Split vertex between 2 polygons.
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                float VertexValue = ArrayVertexValue[V] = Vector2.Dot(SplittingAxis, ArrayVertex[V]);
                if (VertexValue < SplitterVertexValue)
                {
                    ArrayPolygonVertexReference1[V] = (short)NewPolygon1.Count;
                    ArrayPolygonVertexReference2[V] = -1;
                    NewPolygon1.Add(ArrayVertex[V]);
                    NewPolygonUV1.Add(ArrayUVCoordinates[V]);
                    NewColorPoints1.Add(ArrayColorPoints[V]);
                }
                else if (VertexValue > SplitterVertexValue)
                {
                    ArrayPolygonVertexReference1[V] = -1;
                    ArrayPolygonVertexReference2[V] = (short)NewPolygon2.Count;
                    NewPolygon2.Add(ArrayVertex[V]);
                    NewPolygonUV2.Add(ArrayUVCoordinates[V]);
                    NewColorPoints2.Add(ArrayColorPoints[V]);
                }
                else
                {
                    ArrayPolygonVertexReference1[V] = (short)NewPolygon1.Count;
                    NewPolygon1.Add(ArrayVertex[V]);
                    NewPolygonUV1.Add(ArrayUVCoordinates[V]);
                    NewColorPoints1.Add(ArrayColorPoints[V]);
                    ArrayPolygonVertexReference2[V] = (short)NewPolygon2.Count;
                    NewPolygon2.Add(ArrayVertex[V]);
                    NewPolygonUV2.Add(ArrayUVCoordinates[V]);
                    NewColorPoints2.Add(ArrayColorPoints[V]);
                }
            }

            for (int I = 0; I < ArrayIndex.Length; I += 3)
            {
                List<short> ListSplitedEdgeIndex = new List<short>();

                float[] ArrayTriangleVertexValue = new float[3];
                for (int V = 0; V < 3; ++V)
                {
                    ArrayTriangleVertexValue[V] = ArrayVertexValue[ArrayIndex[I + V]];
                }
                for (int V = 0; V < 3; ++V)
                {
                    float MinEdgeValue = Math.Min(ArrayTriangleVertexValue[V], ArrayTriangleVertexValue[(V + 1) % 3]);
                    float MaxEdgeValue = Math.Max(ArrayTriangleVertexValue[V], ArrayTriangleVertexValue[(V + 1) % 3]);

                    if (Polygon.IntervalDistance(MinEdgeValue, MaxEdgeValue, SplitterVertexValue, SplitterVertexValue) <= 0)
                    {
                        ListSplitedEdgeIndex.Add((short)V);
                    }
                }

                if (ListSplitedEdgeIndex.Count == 0)// No edges to split.
                {
                    if (ArrayVertexValue[ArrayIndex[I]] < SplitterVertexValue)
                    {
                        for (int V = 0; V < 3; ++V)
                        {
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[ArrayIndex[I + V]]);
                        }
                    }
                    else
                    {
                        for (int V = 0; V < 3; ++V)
                        {
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[ArrayIndex[I + V]]);
                        }
                    }
                    if (NewPolygonIndex1.Contains(-1) || NewPolygonIndex2.Contains(-1))
                    {
                        return null;
                    }
                }
                else if (ListSplitedEdgeIndex.Count == 1)// Split one edge and one point.
                {
                }

                #region Split 2 edges

                else if (ListSplitedEdgeIndex.Count == 2)// Split 2 edges.
                {
                    int UnslicedEdgeIndex = 3 - ListSplitedEdgeIndex[0] - ListSplitedEdgeIndex[1];
                    short UnslicedEdgeVertex = ArrayIndex[I + UnslicedEdgeIndex];
                    float UnslicedVertexValue1 = ArrayTriangleVertexValue[UnslicedEdgeIndex];
                    float UnslicedVertexValue2 = 0;

                    if (UnslicedEdgeIndex < 2)
                        UnslicedVertexValue2 = (short)(ArrayTriangleVertexValue[UnslicedEdgeIndex + 1]);
                    else
                        UnslicedVertexValue2 = ArrayTriangleVertexValue[0];

                    short SlicedEdge1Vertex1 = ArrayIndex[I + ListSplitedEdgeIndex[0]];
                    short SlicedEdge1Vertex2;
                    short SlicedEdge2Vertex1 = ArrayIndex[I + ListSplitedEdgeIndex[1]];
                    short SlicedEdge2Vertex2;

                    if (ListSplitedEdgeIndex[0] < 2)
                        SlicedEdge1Vertex2 = ArrayIndex[I + ListSplitedEdgeIndex[0] + 1];
                    else
                        SlicedEdge1Vertex2 = ArrayIndex[I];

                    if (ListSplitedEdgeIndex[1] < 2)
                        SlicedEdge2Vertex2 = ArrayIndex[I + ListSplitedEdgeIndex[1] + 1];
                    else
                        SlicedEdge2Vertex2 = ArrayIndex[I];

                    Vector2 SlicedEdge1 = ArrayVertex[SlicedEdge1Vertex2] - ArrayVertex[SlicedEdge1Vertex1];
                    Vector2 SlicedEdge2 = ArrayVertex[SlicedEdge2Vertex2] - ArrayVertex[SlicedEdge2Vertex1];

                    short[] Polygon1PointOfIntersectionIndex = new short[2];
                    short[] Polygon2PointOfIntersectionIndex = new short[2];
                    Polygon1PointOfIntersectionIndex[0] = (short)NewPolygon1.Count;
                    Polygon2PointOfIntersectionIndex[0] = (short)NewPolygon2.Count;
                    Vector2 PointOfIntersection1 = LineIntersectionPoint(ArrayVertex[SlicedEdge1Vertex1], ArrayVertex[SlicedEdge1Vertex2],
                        SplitterVertex1, SplitterVertex2);
                    Vector2 PointOfIntersectionUV1 = ComputeUVFromEdgeNeighbours(SlicedEdge1Vertex1, SlicedEdge1Vertex2, PointOfIntersection1);

                    NewPolygon1.Add(PointOfIntersection1);
                    NewPolygon2.Add(PointOfIntersection1);
                    NewPolygonUV1.Add(PointOfIntersectionUV1);
                    NewPolygonUV2.Add(PointOfIntersectionUV1);
                    NewColorPoints1.Add(ArrayColorPoints[0]);
                    NewColorPoints2.Add(ArrayColorPoints[0]);

                    Polygon1PointOfIntersectionIndex[1] = (short)NewPolygon1.Count;
                    Polygon2PointOfIntersectionIndex[1] = (short)NewPolygon2.Count;
                    Vector2 PointOfIntersection2 = LineIntersectionPoint(ArrayVertex[SlicedEdge2Vertex1], ArrayVertex[SlicedEdge2Vertex2],
                        SplitterVertex1, SplitterVertex2);
                    Vector2 PointOfIntersectionUV2 = ComputeUVFromEdgeNeighbours(SlicedEdge2Vertex1, SlicedEdge2Vertex2, PointOfIntersection2);

                    NewPolygon1.Add(PointOfIntersection2);
                    NewPolygon2.Add(PointOfIntersection2);
                    NewPolygonUV1.Add(PointOfIntersectionUV2);
                    NewPolygonUV2.Add(PointOfIntersectionUV2);
                    NewColorPoints1.Add(ArrayColorPoints[0]);
                    NewColorPoints2.Add(ArrayColorPoints[0]);

                    if (ArrayTriangleVertexValue[ListSplitedEdgeIndex[0]] < SplitterVertexValue)
                    {
                        //Entire edge is on the same side, make 2 triangles.
                        if (UnslicedVertexValue1 < SplitterVertexValue && UnslicedVertexValue2 < SplitterVertexValue)
                        {
                            //First triangle.
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[SlicedEdge1Vertex1]);
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[0]);
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[UnslicedEdgeVertex]);
                            //Second triangle.
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[0]);
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[1]);
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[UnslicedEdgeVertex]);

                            //Other side triangle.
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[0]);
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[SlicedEdge2Vertex1]);
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[1]);
                        }
                        else
                        {
                            //First triangle.
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[SlicedEdge1Vertex1]);
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[0]);
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[1]);

                            //First other side triangle.
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[0]);
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[SlicedEdge1Vertex2]);
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[1]);
                            //Second other side triangle.
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[1]);
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[UnslicedEdgeVertex]);
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[SlicedEdge2Vertex1]);
                        }
                    }
                    else if (ArrayTriangleVertexValue[ListSplitedEdgeIndex[0]] > SplitterVertexValue)
                    {
                        //Entire edge is on the same side, make 2 triangles.
                        if (UnslicedVertexValue1 > SplitterVertexValue && UnslicedVertexValue2 > SplitterVertexValue)
                        {
                            //First triangle.
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[SlicedEdge1Vertex1]);
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[0]);
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[UnslicedEdgeVertex]);
                            //Second triangle.
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[0]);
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[1]);
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[UnslicedEdgeVertex]);

                            //Other side triangle.
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[0]);
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[SlicedEdge2Vertex1]);
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[1]);
                        }
                        else
                        {
                            //First triangle.
                            NewPolygonIndex2.Add(ArrayPolygonVertexReference2[SlicedEdge1Vertex1]);
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[0]);
                            NewPolygonIndex2.Add(Polygon2PointOfIntersectionIndex[1]);

                            //First other side triangle.
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[0]);
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[SlicedEdge2Vertex1]);
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[1]);
                            //Second other side triangle.
                            NewPolygonIndex1.Add(Polygon1PointOfIntersectionIndex[0]);
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[UnslicedEdgeVertex]);
                            NewPolygonIndex1.Add(ArrayPolygonVertexReference1[SlicedEdge2Vertex1]);
                        }
                    }
                    if (NewPolygonIndex1.Contains(-1) || NewPolygonIndex2.Contains(-1))
                    {
                        return null;
                    }
                }

                #endregion

                else
                {
                    throw new Exception("An error occured while cutting the polygon.");
                }
            }

            #region Create new polygons

            if (NewPolygon1.Count == 0 || NewPolygon2.Count == 0)
            {
                return null;
            }
            else
            {
                ArrayVertex = NewPolygon1.ToArray();
                ArrayIndex = NewPolygonIndex1.ToArray();
                ArrayUVCoordinates = NewPolygonUV1.ToArray();
                ArrayColorPoints = NewColorPoints1.ToArray();

                TriangleCount = ArrayIndex.Length / 3;
                UpdateWorldPosition(Vector2.Zero, 0f);

                if (NewPolygon2.Count > 0)
                {
                    Polygon NewPolygon = new Polygon()
                    {
                        ArrayVertex = NewPolygon2.ToArray(),
                        ArrayIndex = NewPolygonIndex2.ToArray(),
                        ArrayUVCoordinates = NewPolygonUV2.ToArray(),
                        ArrayColorPoints = NewColorPoints2.ToArray(),
                        TriangleCount = NewPolygonIndex2.Count / 3,
                    };
                    NewPolygon.UpdateWorldPosition(Vector2.Zero, 0f);

                    return NewPolygon;
                }
                else
                {
                    return null;
                }
            }

            #endregion
        }

        public void SplitTriangle(Vector2 NewVertex, int TriangleIndex)
        {
            int LastIndex = ArrayIndex.Length;

            Vector2[] NewArrayVertex = new Vector2[ArrayVertex.Length + 1];
            Vector2[] NewArrayUVCoordinates = new Vector2[ArrayUVCoordinates.Length + 1];
            short[] NewArrayIndices = new short[ArrayIndex.Length + 6];
            Color[] NewArrayColor = new Color[ArrayColorPoints.Length + 1];

            Array.Copy(ArrayVertex, NewArrayVertex, ArrayVertex.Length);
            Array.Copy(ArrayUVCoordinates, NewArrayUVCoordinates, ArrayUVCoordinates.Length);
            Array.Copy(ArrayIndex, NewArrayIndices, ArrayIndex.Length);
            Array.Copy(ArrayColorPoints, NewArrayColor, ArrayColorPoints.Length);

            NewArrayVertex[NewArrayVertex.Length - 1] = NewVertex;
            NewArrayUVCoordinates[NewArrayUVCoordinates.Length - 1] = ComputeUVFromEdgeNeighbours(TriangleIndex, NewVertex);

            NewArrayIndices[LastIndex] = ArrayIndex[TriangleIndex];
            NewArrayIndices[LastIndex + 1] = ArrayIndex[TriangleIndex + 1];
            NewArrayIndices[LastIndex + 2] = (short)VertexCount;

            NewArrayIndices[LastIndex + 3] = ArrayIndex[TriangleIndex + 1];
            NewArrayIndices[LastIndex + 4] = ArrayIndex[TriangleIndex + 2];
            NewArrayIndices[LastIndex + 5] = (short)VertexCount;

            NewArrayIndices[TriangleIndex] = ArrayIndex[TriangleIndex + 2];
            NewArrayIndices[TriangleIndex + 1] = ArrayIndex[TriangleIndex];
            NewArrayIndices[TriangleIndex + 2] = (short)VertexCount;


            NewArrayColor[ArrayColorPoints.Length] = ArrayColorPoints[ArrayIndex[TriangleIndex]];

            ArrayVertex = NewArrayVertex;
            ArrayIndex = NewArrayIndices;
            ArrayUVCoordinates = NewArrayUVCoordinates;
            ArrayColorPoints = NewArrayColor;

            TriangleCount = ArrayIndex.Length / 3;
        }

        public void FuseOverlappingVertex()
        {
            for (int V1 = ArrayVertex.Length - 1; V1 >= 0; --V1)
            {
                for (int V2 = ArrayVertex.Length - 1; V2 >= 0; --V2)
                {
                    if (V1 == V2)
                    {
                        continue;
                    }

                    Vector2 CurrentVertex = ArrayVertex[V1];
                    Vector2 OtherVertex = ArrayVertex[V2];

                    if (Vector2.Distance(CurrentVertex, OtherVertex) < 0.0001f)
                    {
                        ReplaceVertex(V1, (short)V2);
                        break;
                    }
                }
            }
        }

        public Vector2 ComputeUVFromEdgeNeighbours(short NeighbourIndex1, short NeighbourIndex2, Vector2 PointToCompute)
        {
            Vector2 Edge = ArrayVertex[NeighbourIndex2] - ArrayVertex[NeighbourIndex1];
            Edge.Normalize();
            float MinValue = Vector2.Dot(Edge, ArrayVertex[NeighbourIndex1]);
            float MaxValue = Vector2.Dot(Edge, ArrayVertex[NeighbourIndex2]);
            float PointValue = Vector2.Dot(Edge, PointToCompute);
            float PercentIncrease = ((PointValue - MinValue) / (MaxValue - MinValue));
            Vector2 UVDifference = ArrayUVCoordinates[NeighbourIndex2] - ArrayUVCoordinates[NeighbourIndex1];

            Vector2 NewUV = ArrayUVCoordinates[NeighbourIndex1] + UVDifference * PercentIncrease;
            return NewUV;
        }

        public Vector2 ComputeUVFromEdgeNeighbours(int TriangleIndex, Vector2 PointToCompute)
        {
            float PointValueX = PointToCompute.X;
            float PointValueY = PointToCompute.Y;

            float MinPosX = float.MaxValue;
            float MaxPosX = float.MinValue;
            float MinPosY = float.MaxValue;
            float MaxPosY = float.MinValue;

            float MinUVX = float.MaxValue;
            float MaxUVX = float.MinValue;
            float MinUVY = float.MaxValue;
            float MaxUVY = float.MinValue;

            for (int i = 0; i < 3; i++)
            {
                short ActiveIndex = ArrayIndex[TriangleIndex + i];

                if (ArrayVertex[ActiveIndex].X < MinPosX)
                    MinPosX = ArrayVertex[ActiveIndex].X;
                if (ArrayVertex[ActiveIndex].X > MaxPosX)
                    MaxPosX = ArrayVertex[ActiveIndex].X;

                if (ArrayVertex[ActiveIndex].Y < MinPosY)
                    MinPosY = ArrayVertex[ActiveIndex].Y;
                if (ArrayVertex[ActiveIndex].Y > MaxPosY)
                    MaxPosY = ArrayVertex[ActiveIndex].Y;

                if (ArrayUVCoordinates[ActiveIndex].X < MinUVX)
                    MinUVX = ArrayUVCoordinates[ActiveIndex].X;
                if (ArrayUVCoordinates[ActiveIndex].X > MaxUVX)
                    MaxUVX = ArrayUVCoordinates[ActiveIndex].X;

                if (ArrayUVCoordinates[ActiveIndex].Y < MinUVY)
                    MinUVY = ArrayUVCoordinates[ActiveIndex].Y;
                if (ArrayUVCoordinates[ActiveIndex].Y > MaxUVY)
                    MaxUVY = ArrayUVCoordinates[ActiveIndex].Y;
            }

            float PercentIncreaseX = ((PointValueX - MinPosX) / (MaxPosX - MinPosX));
            float PercentIncreaseY = ((PointValueY - MinPosY) / (MaxPosY - MinPosY));
            float UVDifferenceX = MaxUVX - MinUVX;
            float UVDifferenceY = MaxUVY - MinUVY;
            Vector2 NewUV = new Vector2(MinUVX + UVDifferenceX * PercentIncreaseX, MinUVY + UVDifferenceY * PercentIncreaseY);

            return NewUV;
        }

        //https://social.msdn.microsoft.com/forums/vstudio/en-US/4eb3423e-eb81-4977-8ce5-5a568d53fd9b/get-the-intersection-point-of-two-lines
        // Warning: Seems to be broken if the distance between points is too great
        private static Vector2 LineIntersectionPoint(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            double x21 = p2.X - p1.X;
            double y21 = p2.Y - p1.Y;
            double x31 = q1.X - p1.X;
            double y31 = q1.Y - p1.Y;
            double x43 = q2.X - q1.X;
            double y43 = q2.Y - q1.Y;

            double paramDenominator = x43 * y21 - x21 * y43;

            if (paramDenominator == 0)
            {
                return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            }

            double s = (x43 * y31 - x31 * y43) / paramDenominator;
            double t = (x21 * y31 - x31 * y21) / paramDenominator;
            bool intersectionIsOnLines = (s >= 0 && s <= 1 && t >= 0 && t <= 1);

            return new Vector2((float)(p1.X + (p2.X - p1.X) * s), (float)(p1.Y + (p2.Y - p1.Y) * s));
        }

        //http://paulbourke.net/geometry/
        /// <summary>
        /// This is based off an explanation and expanded math presented by Paul Bourke:
        ///
        /// It takes two lines as inputs and returns true if they intersect, false if they
        /// don't.
        /// If they do, ptIntersection returns the point where the two lines intersect.
        /// </summary>
        /// <param name="L1">The first line</param>
        /// <param name="L2">The second line</param>
        /// <param name="ptIntersection">The point where both lines intersect (if they do).</param>
        /// <returns></returns>
        /// <remarks>See http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/</remarks>
        public static bool DoLinesIntersect(Vector2 StartPoint1, Vector2 EndPoint1, Vector2 StartPoint2, Vector2 EndPoint2, ref Vector2 ptIntersection)
        {
            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (EndPoint2.Y - StartPoint2.Y) * (EndPoint1.X - StartPoint1.X)
               -
               (EndPoint2.X - StartPoint2.X) * (EndPoint1.Y - StartPoint1.Y);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (EndPoint2.X - StartPoint2.X) * (StartPoint1.Y - StartPoint2.Y)
               -
               (EndPoint2.Y - StartPoint2.Y) * (StartPoint1.X - StartPoint2.X);

            double n_b =
               (EndPoint1.X - StartPoint1.X) * (StartPoint1.Y - StartPoint2.Y)
               -
               (EndPoint1.Y - StartPoint1.Y) * (StartPoint1.X - StartPoint2.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.
            // If n_a and n_b were both equal to zero the lines would be on top of each
            // other (coincidental).  This check is not done because it is not
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;

            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                ptIntersection.X = StartPoint1.X + (float)(ua * (EndPoint1.X - StartPoint1.X));
                ptIntersection.Y = StartPoint1.Y + (float)(ua * (EndPoint1.Y - StartPoint1.Y));
                return true;
            }
            return false;
        }

        public void Offset(float X, float Y)
        {
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                ArrayVertex[V] = new Vector2(ArrayVertex[V].X + X, ArrayVertex[V].Y + Y);
            }
            Center.X += X;
            Center.Y += Y;
        }

        public void Scale(float Scale)
        {
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                Vector2 DistanceFromCenter = ArrayVertex[V] - Center;
                ArrayVertex[V] = Center + DistanceFromCenter * Scale;
            }
        }

        public static float IntervalDistance(float MinA, float MaxA, float MinB, float MaxB)
        {
            if (MinA < MinB)
                return MinB - MaxA;
            else
                return MinA - MaxB;
        }

        public static void ProjectPolygon(Vector2 Axis, Polygon Polygon, out float Min, out float Max)
        {
            float DotProduct = Vector2.Dot(Axis, Polygon.ArrayVertex[0]);
            Min = DotProduct;
            Max = DotProduct;
            for (int i = 0; i < Polygon.ArrayVertex.Length; i++)
            {
                DotProduct = Vector2.Dot(Axis, Polygon.ArrayVertex[i]);
                if (DotProduct < Min)
                {
                    Min = DotProduct;
                }
                else if (DotProduct > Max)
                {
                    Max = DotProduct;
                }
            }
        }

        public static PolygonCollisionResult ProjectPolygon(Vector2 PerpendicularAxis, Vector2 ParallelAxis, Vector2 Vertex, Vector2 EndPoint, Polygon Polygon)
        {
            int CollisionDetected = 0;
            float minIntervalDistance = float.MaxValue;
            Vector2 AxisTranslation = Vector2.Zero;

            for (int V = 0; V < Polygon.ArrayVertex.Length; V++)
            {
                Vector2 ActiveVertex = Polygon.ArrayVertex[V];
                Vector2 ActiveEndVertex;
                if (V + 1 < Polygon.ArrayVertex.Length)
                    ActiveEndVertex = Polygon.ArrayVertex[V + 1];
                else
                    ActiveEndVertex = Polygon.ArrayVertex[0];

                float minA = Vector2.Dot(PerpendicularAxis, Vertex);
                float maxA = minA;
                float minB = Vector2.Dot(PerpendicularAxis, ActiveVertex);
                float maxB = Vector2.Dot(PerpendicularAxis, ActiveEndVertex);

                float ParallelMinA = 0, ParallelMaxA = 0;
                float ParallelMinB = 0, ParallelMaxB = 0;

                ParallelMinA = Vector2.Dot(ParallelAxis, Vertex);
                ParallelMaxA = Vector2.Dot(ParallelAxis, EndPoint);
                ParallelMinB = Vector2.Dot(ParallelAxis, ActiveVertex);
                ParallelMaxB = ParallelMinA;

                if (minB < minA && maxB < minA)
                {//No collisions
                }
                //Perpenducular collision detected.
                else if (minB <= minA && maxB >= maxA)
                {
                    if (ParallelMaxA < ParallelMinA)
                    {
                        float Temp = ParallelMaxA;
                        ParallelMaxA = ParallelMinA;
                        ParallelMinA = Temp;
                    }

                    if ((ParallelMinB < ParallelMinA && ParallelMaxB < ParallelMinA) || (ParallelMinB > ParallelMaxA && ParallelMaxB > ParallelMaxA))
                    {//No collisions
                    }
                    //Parallel collision detected.
                    else
                    {
                        CollisionDetected++;
                        float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                        intervalDistance = Math.Abs(intervalDistance);

                        if (intervalDistance < minIntervalDistance)
                        {
                            minIntervalDistance = intervalDistance;
                            AxisTranslation = PerpendicularAxis;
                        }
                    }
                }
            }

            if (CollisionDetected > 0)
            {
                PolygonCollisionResult CollisionResult = new PolygonCollisionResult(AxisTranslation, minIntervalDistance);

                return CollisionResult;
            }
            else
                return new PolygonCollisionResult(Vector2.Zero, -1);
        }

        public void UpdateWorldPosition(Vector2 Translation, float Angle)
        {
            Matrix TransformationMatrix = Matrix.CreateRotationZ(Angle) * Matrix.CreateTranslation(Translation.X, Translation.Y, 0);

            Vector2[] tempPoints = new Vector2[VertexCount];

            Vector2.Transform(ArrayVertex, ref TransformationMatrix, tempPoints);

            ArrayFinalVertex = new VertexPositionColorTexture[VertexCount];

            // for every point we need to assembly the cache
            for (int i = 0; i < VertexCount; i++)
            {
                ArrayFinalVertex[i].Position = new Vector3(tempPoints[i], 0);
                ArrayFinalVertex[i].TextureCoordinate = ArrayUVCoordinates[i];
                ArrayFinalVertex[i].Color = Color.White;
            }
        }

        public void ComputeColorAndUVCoordinates(float MaxWidth, float MaxHeight)
        {
            ArrayUVCoordinates = new Vector2[VertexCount];
            ArrayColorPoints = new Color[VertexCount];

            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                ArrayUVCoordinates[i] = new Vector2(ArrayVertex[i].X / MaxWidth, ArrayVertex[i].Y / MaxHeight);
            }
        }

        public void Draw(GraphicsDevice g)
        {
            g.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, ArrayFinalVertex, 0, VertexCount, ArrayIndex, 0, TriangleCount);
        }

        public void DrawTriangle(GraphicsDevice g, int TriangleIndex)
        {
            g.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, ArrayFinalVertex, 0, 3, ArrayIndex, TriangleIndex, 1);
        }
    }
}
