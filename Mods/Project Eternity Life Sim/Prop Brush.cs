using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ConvexBrush
    {
        public List<Vector4> Planes = new List<Vector4>();

        public static ConvexBrush Box(Vector3 Center, Vector3 Size)
        {
            ConvexBrush Brush = new ConvexBrush();
            Vector3 Half = Size * 0.5f;
            Brush.Planes.Add(new Vector4(1f, 0f, 0f, Center.X + Half.X));
            Brush.Planes.Add(new Vector4(-1f, 0f, 0f, -Center.X + Half.X));
            Brush.Planes.Add(new Vector4(0f, 1f, 0f, Center.Y + Half.Y));
            Brush.Planes.Add(new Vector4(0f, -1f, 0f, -Center.Y + Half.Y));
            Brush.Planes.Add(new Vector4(0f, 0f, 1f, Center.Z + Half.Z));
            Brush.Planes.Add(new Vector4(0f, 0f, -1f, -Center.Z + Half.Z));
            return Brush;
        }

        public ConvexBrush Clone()
        {
            ConvexBrush Copy = new ConvexBrush();
            Copy.Planes.AddRange(Planes);
            return Copy;
        }

        public void Clip(Vector4 Plane)
        {
            Planes.Add(Plane);
        }

        public static List<ConvexBrush> Subtract(ConvexBrush Target, ConvexBrush Cutter)
        {
            List<ConvexBrush> Pile = new List<ConvexBrush>();
            if (Target == null || Cutter == null)
            {
                return Pile;
            }

            Pile.Add(Target.Clone());
            for (int i = 0; i < Cutter.Planes.Count; i++)
            {
                Vector4 Plane = Cutter.Planes[i];
                List<ConvexBrush> Next = new List<ConvexBrush>();
                for (int p = 0; p < Pile.Count; p++)
                {
                    ConvexBrush Front = Pile[p].Clone();
                    Front.Planes.Add(new Vector4(-Plane.X, -Plane.Y, -Plane.Z, -Plane.W));
                    ConvexBrush Back = Pile[p].Clone();
                    Back.Planes.Add(Plane);
                    if (Front.HasVolume())
                    {
                        Next.Add(Front);
                    }

                    if (Back.HasVolume())
                    {
                        Next.Add(Back);
                    }
                }

                Pile = Next;
            }

            List<ConvexBrush> Outside = new List<ConvexBrush>();
            for (int i = 0; i < Pile.Count; i++)
            {
                Vector3 Center;
                if (Pile[i].Center(out Center) && !Cutter.Contains(Center))
                {
                    Outside.Add(Pile[i]);
                }
            }

            return Outside;
        }

        public bool Contains(Vector3 Point)
        {
            for (int i = 0; i < Planes.Count; i++)
            {
                Vector4 Plane = Planes[i];
                if (Plane.X * Point.X + Plane.Y * Point.Y + Plane.Z * Point.Z > Plane.W + 0.05f)
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasVolume()
        {
            return SolidPoints().Count >= 4;
        }

        public bool Center(out Vector3 Center)
        {
            List<Vector3> Points = SolidPoints();
            Center = Vector3.Zero;
            if (Points.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < Points.Count; i++)
            {
                Center += Points[i];
            }

            Center /= Points.Count;
            return true;
        }

        public List<Vector3> SolidPoints()
        {
            List<Vector3> Points = new List<Vector3>();
            for (int i = 0; i < Planes.Count; i++)
            {
                for (int j = i + 1; j < Planes.Count; j++)
                {
                    for (int k = j + 1; k < Planes.Count; k++)
                    {
                        Vector3 Point;
                        if (!Intersect(Planes[i], Planes[j], Planes[k], out Point))
                        {
                            continue;
                        }

                        if (!Contains(Point))
                        {
                            continue;
                        }

                        bool Fresh = true;
                        for (int p = 0; p < Points.Count; p++)
                        {
                            if (Vector3.DistanceSquared(Points[p], Point) < 0.01f)
                            {
                                Fresh = false;
                                break;
                            }
                        }

                        if (Fresh)
                        {
                            Points.Add(Point);
                        }
                    }
                }
            }

            return Points;
        }

        public void Triangles(List<VertexPositionColor> Vertices, List<int> Indices, Color Tint)
        {
            List<Vector3> Points = SolidPoints();
            if (Points.Count < 4)
            {
                return;
            }

            for (int i = 0; i < Planes.Count; i++)
            {
                Vector3 Normal = new Vector3(Planes[i].X, Planes[i].Y, Planes[i].Z);
                if (Normal.LengthSquared() < 0.0001f)
                {
                    continue;
                }

                Normal.Normalize();
                List<Vector3> Face = new List<Vector3>();
                for (int p = 0; p < Points.Count; p++)
                {
                    float Distance = Normal.X * Points[p].X + Normal.Y * Points[p].Y + Normal.Z * Points[p].Z - Planes[i].W;
                    if (Math.Abs(Distance) < 0.08f)
                    {
                        Face.Add(Points[p]);
                    }
                }

                if (Face.Count < 3)
                {
                    continue;
                }

                Vector3 Origin = Face[0];
                Face.Sort(delegate (Vector3 A, Vector3 B)
                {
                    Vector3 Cross = Vector3.Cross(A - Origin, B - Origin);
                    float Side = Vector3.Dot(Cross, Normal);
                    if (Side > 0.0001f)
                    {
                        return -1;
                    }

                    if (Side < -0.0001f)
                    {
                        return 1;
                    }

                    return 0;
                });
                int Base = Vertices.Count;
                for (int p = 0; p < Face.Count; p++)
                {
                    Vertices.Add(new VertexPositionColor(Face[p], Tint));
                }

                for (int p = 1; p + 1 < Face.Count; p++)
                {
                    Indices.Add(Base);
                    Indices.Add(Base + p);
                    Indices.Add(Base + p + 1);
                }
            }
        }

        static bool Intersect(Vector4 A, Vector4 B, Vector4 C, out Vector3 Point)
        {
            Vector3 N1 = new Vector3(A.X, A.Y, A.Z);
            Vector3 N2 = new Vector3(B.X, B.Y, B.Z);
            Vector3 N3 = new Vector3(C.X, C.Y, C.Z);
            Vector3 Cross = Vector3.Cross(N2, N3);
            float Denom = Vector3.Dot(N1, Cross);
            Point = Vector3.Zero;
            if (Math.Abs(Denom) < 0.00001f)
            {
                return false;
            }

            Point = (A.W * Cross + B.W * Vector3.Cross(N3, N1) + C.W * Vector3.Cross(N1, N2)) / Denom;
            return true;
        }
    }

    public static class PropBrushMesh
    {
        static BasicEffect Effect;

        public static void Draw(GraphicsDevice Device, FurnitureItem Item, Matrix View, Matrix Projection, Vector3 Shift)
        {
            if (Device == null || Item == null || Item.Brushes == null || Item.Brushes.Count == 0)
            {
                return;
            }

            if (Effect == null)
            {
                Effect = new BasicEffect(Device);
                Effect.VertexColorEnabled = true;
                Effect.LightingEnabled = false;
            }

            List<VertexPositionColor> Vertices = new List<VertexPositionColor>();
            List<int> Indices = new List<int>();
            Color Tint = new Color(Item.Swatch);
            for (int i = 0; i < Item.Brushes.Count; i++)
            {
                Item.Brushes[i].Triangles(Vertices, Indices, Tint);
            }

            if (Indices.Count < 3)
            {
                return;
            }

            Vector3 Feet = new Vector3(Shift.X, Shift.Y, Shift.Z);
            Effect.World = Matrix.CreateTranslation(Feet);
            Effect.View = View;
            Effect.Projection = Projection;
            Effect.CurrentTechnique.Passes[0].Apply();
            Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count, Indices.ToArray(), 0, Indices.Count / 3);
        }
    }

    public static class PropBrushTool
    {
        public static FurnitureItem Ensure(MapInfo Info, Vector3 Ground)
        {
            if (Info.Furniture == null)
            {
                Info.Furniture = new List<FurnitureItem>();
            }

            FurnitureItem Nearest = null;
            float Best = 48f * 48f;
            for (int i = 0; i < Info.Furniture.Count; i++)
            {
                FurnitureItem Item = Info.Furniture[i];
                if (Item == null || Item.RoomProp)
                {
                    continue;
                }

                float DX = Item.Position.X - Ground.X;
                float DY = Item.Position.Y - Ground.Y;
                float Distance = DX * DX + DY * DY;
                if (Distance < Best)
                {
                    Best = Distance;
                    Nearest = Item;
                }
            }

            if (Nearest != null)
            {
                if (Nearest.Brushes == null)
                {
                    Nearest.Brushes = new List<ConvexBrush>();
                }

                return Nearest;
            }

            FurnitureItem Created = new FurnitureItem();
            Created.Position = new Vector3(Ground.X, Ground.Y, Ground.Z);
            Created.Width = 16f;
            Created.Height = 24f;
            Created.Length = 16f;
            Created.Swatch = new Vector3(0.55f, 0.55f, 0.52f);
            Created.Interaction = "Prop";
            Created.Brushes = new List<ConvexBrush>();
            Info.Furniture.Add(Created);
            LifeSimMap Map = Info.MapMapContainer == null ? null : Info.MapMapContainer.ActiveMap;
            if (Map != null && Map.Furniture != null && !object.ReferenceEquals(Map.Furniture, Info.Furniture))
            {
                Map.Furniture.Add(Created);
            }

            return Created;
        }

        public static void AddBox(FurnitureItem Item, Vector3 Ground)
        {
            Vector3 Center = new Vector3(Ground.X, Ground.Z + 12f, Ground.Y);
            Item.Brushes.Add(ConvexBrush.Box(Center, new Vector3(16f, 24f, 16f)));
        }

        public static void Clip(FurnitureItem Item)
        {
            if (Item.Brushes.Count == 0)
            {
                return;
            }

            ConvexBrush Last = Item.Brushes[Item.Brushes.Count - 1];
            Vector3 Center;
            if (!Last.Center(out Center))
            {
                return;
            }

            Vector3 Normal = Vector3.Normalize(new Vector3(1f, 0f, 1f));
            Last.Clip(new Vector4(Normal.X, Normal.Y, Normal.Z, Vector3.Dot(Normal, Center)));
        }

        public static void Subtract(FurnitureItem Item)
        {
            if (Item.Brushes.Count < 2)
            {
                return;
            }

            ConvexBrush Cutter = Item.Brushes[Item.Brushes.Count - 1];
            ConvexBrush Target = Item.Brushes[Item.Brushes.Count - 2];
            Item.Brushes.RemoveAt(Item.Brushes.Count - 1);
            Item.Brushes.RemoveAt(Item.Brushes.Count - 1);
            List<ConvexBrush> Pieces = ConvexBrush.Subtract(Target, Cutter);
            if (Pieces.Count == 0)
            {
                Item.Brushes.Add(Target);
                return;
            }

            Item.Brushes.AddRange(Pieces);
        }
    }
}
