using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Graphics
{
    public class PolygonMesh
    {
        public struct PolygonMeshCollisionResult
        {
            public Vector3 Axis;
            public float Distance;
            public bool Collided { get { return Distance > 0; } }

            public PolygonMeshCollisionResult(Vector3 Axis, float Distance)
            {
                this.Axis = Axis;
                this.Distance = Distance;
            }
        }

        public Vector3[] ArrayVertex;
        public Vector3[] ArrayEdge;
        public Vector3[] ArrayAxis;
        public Vector3 Center;

        public PolygonMesh()
        {
            ArrayVertex = new Vector3[8]
            {
                // Calculate the position of the vertices on the top face.
                new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, -1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),

                // Calculate the position of the vertices on the bottom face.
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, 1.0f),
            };
            ArrayAxis = new Vector3[3]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
            };

            ComputerCenter();
            ComputeEdges();
        }

        public PolygonMesh(Vector3[] ArrayVertex)
        {
            this.ArrayVertex = ArrayVertex;
            ComputePerpendicularAxis();

            ComputerCenter();
            ComputeEdges();
        }

        public PolygonMesh(Vector3[] ArrayVertex, Vector3[] ArrayAxis)
        {
            this.ArrayVertex = ArrayVertex;
            this.ArrayAxis = ArrayAxis;

            ComputerCenter();
            ComputeEdges();
        }
        
        public void ComputerCenter()
        {
            float totalX = 0;
            float totalY = 0;
            float totalZ = 0;
            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                totalX += ArrayVertex[i].X;
                totalY += ArrayVertex[i].Y;
                totalZ += ArrayVertex[i].Z;
            }

            Center = new Vector3(totalX / (float)ArrayVertex.Length, totalY / (float)ArrayVertex.Length, totalZ / (float)ArrayVertex.Length);
        }

        public void ComputeEdges()
        {
            ArrayEdge = new Vector3[ArrayVertex.Length];

            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                ArrayEdge[V] = Center - ArrayVertex[V];
            }
        }

        public bool ComputePerpendicularAxis()
        {
            if (ArrayVertex.Length < 3)
            {
                return false;
            }
            else
            {
                Vector3 Vertex1;
                Vector3 Vertex2;
                Vector3 Vertex3;

                ArrayAxis = new Vector3[ArrayVertex.Length];

                for (int V = 0; V < ArrayVertex.Length; V++)
                {
                    Vertex1 = ArrayVertex[V];

                    if (V + 1 >= ArrayVertex.Length)
                        Vertex2 = ArrayVertex[0];
                    else
                        Vertex2 = ArrayVertex[V + 1];

                    if (V + 2 >= ArrayVertex.Length)
                        Vertex3 = ArrayVertex[V + 2 - ArrayVertex.Length];
                    else
                        Vertex3 = ArrayVertex[V + 2];

                    var side1 = Vertex2 - Vertex1;
                    var side2 = Vertex3 - Vertex1;

                    ArrayAxis[V] = Vector3.Cross(side1, side2);
                    ArrayAxis[V].Normalize();
                }
            }
            return true;
        }

        public void Offset(float X, float Y, float Z)
        {
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                ArrayVertex[V] = new Vector3(ArrayVertex[V].X + X, ArrayVertex[V].Y + Y, ArrayVertex[V].Z + Z);
            }
            Center.X += X;
            Center.Y += Y;
            Center.Z += Z;
        }

        public void Rotate(float Yaw, float Pitch, float Roll)
        {
            Matrix Translation1 = Matrix.CreateTranslation(-Center);
            Matrix Rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
            Matrix Translation2 = Matrix.CreateTranslation(Center);
            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                ArrayVertex[i] = Vector3.Transform(ArrayVertex[i], Translation1 * Rotation * Translation2);
                ArrayEdge[i] = Vector3.Transform(ArrayEdge[i], Translation1 * Rotation * Translation2);
            }
            for (int i = 0; i < ArrayAxis.Length; i++)
            {
                ArrayAxis[i] = Vector3.Transform(ArrayAxis[i], Rotation);
            }
        }

        public void Scale(Vector3 ScaleValue)
        {
            Matrix Translation1 = Matrix.CreateTranslation(-Center);
            Matrix Scale = Matrix.CreateScale(ScaleValue);
            Matrix Translation2 = Matrix.CreateTranslation(Center);
            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                ArrayVertex[i] = Vector3.Transform(ArrayVertex[i], Translation1 * Scale * Translation2);
            }

            ComputeEdges();
        }

        public static PolygonMeshCollisionResult PolygonCollisionSAT(PolygonMesh PolygonA, PolygonMesh PolygonB, Vector3 Speed)
        {
            bool Intersection = true;
            bool IntersectionNext = true;

            float minA = 0, maxA = 0;
            float minB = 0, maxB = 0;

            int AxisCountA = PolygonA.ArrayAxis.Length;
            int AxisCountB = PolygonB.ArrayAxis.Length;
            int EdgeCountA = PolygonA.ArrayEdge.Length;
            int EdgeCountB = PolygonB.ArrayEdge.Length;

            Vector3 Axis;
            Vector3 AxisTranslation = Vector3.Zero;
            float minIntervalDistance = float.MaxValue;
            float intervalDistance;
            Vector3[] CrossProduct = new Vector3[EdgeCountA * EdgeCountB];
            for (int EdgeIndexA = EdgeCountA - 1; EdgeIndexA >= 0; --EdgeIndexA)
            {
                for (int EdgeIndexB = EdgeCountB - 1; EdgeIndexB >= 0; --EdgeIndexB)
                {
                    CrossProduct[EdgeIndexA + EdgeIndexB * EdgeCountA] = Vector3.Cross(PolygonA.ArrayEdge[EdgeIndexA], PolygonB.ArrayEdge[EdgeIndexB]);
                }
            }

            for (int AxisIndex = 0; AxisIndex < AxisCountA + AxisCountB + CrossProduct.Length; AxisIndex++)
            {
                if (AxisIndex < AxisCountA)
                {
                    Axis = PolygonA.ArrayAxis[AxisIndex];
                }
                else if (AxisIndex < AxisCountA + AxisCountB)
                {
                    Axis = PolygonB.ArrayAxis[AxisIndex - AxisCountA];
                }
                else
                {
                    Axis = CrossProduct[AxisIndex - AxisCountA - AxisCountB];
                }

                ProjectPolygon(Axis, PolygonA, out minA, out maxA);
                ProjectPolygon(Axis, PolygonB, out minB, out maxB);

                if (IntervalDistance(minA, maxA, minB, maxB) >= 0)
                    Intersection = false;

                float AxeVitesse = Vector3.Dot(Axis, Speed);

                if (AxeVitesse < 0)
                    minA += AxeVitesse;
                else
                    maxA += AxeVitesse;

                intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance >= 0)
                    IntersectionNext = false;

                if (Intersection || IntersectionNext)
                {
                    intervalDistance = Math.Abs(intervalDistance);

                    if (intervalDistance < minIntervalDistance)
                    {
                        minIntervalDistance = intervalDistance;
                        AxisTranslation = Axis;

                        Vector3 p = PolygonA.Center - PolygonB.Center;
                        if (Vector3.Dot(p, AxisTranslation) < 0)
                            AxisTranslation = -AxisTranslation;
                    }
                }
            }
            if (IntersectionNext)
            {
                PolygonMeshCollisionResult CollisionResult = new PolygonMeshCollisionResult(AxisTranslation, minIntervalDistance);
                
                return CollisionResult;
            }

            return new PolygonMeshCollisionResult(Vector3.Zero, -1);
        }
        
        private float DistanceToEdge(Vector3 Vertex1, Vector3 Vertex2, Vector3 Axis, Vector3 Point, out bool InsideEdge)
        {
            Vector3 Edge = Vertex2 - Vertex1;
            Edge.Normalize();
            Axis.Normalize();

            float MinValue = Vector3.Dot(Edge, Vertex1);
            float MaxValue = Vector3.Dot(Edge, Vertex2);
            float PointValue = Vector3.Dot(Edge, Point);

            if (PointValue >= MinValue && PointValue <= MaxValue)
            {
                InsideEdge = true;
                return Vector3.Dot(Axis, Point) - Vector3.Dot(Axis, Vertex1);
            }
            InsideEdge = false;
            return float.MaxValue;
        }

        public static float IntervalDistance(float MinA, float MaxA, float MinB, float MaxB)
        {
            if (MinA < MinB)
                return MinB - MaxA;
            else
                return MinA - MaxB;
        }

        public static void ProjectPolygon(Vector3 Axis, PolygonMesh Polygon, out float Min, out float Max)
        {
            float DotProduct = Vector3.Dot(Axis, Polygon.ArrayVertex[0]);
            Min = DotProduct;
            Max = DotProduct;
            for (int i = 0; i < Polygon.ArrayVertex.Length; i++)
            {
                DotProduct = Vector3.Dot(Axis, Polygon.ArrayVertex[i]);
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
    }
}
