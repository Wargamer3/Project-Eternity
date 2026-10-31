using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class HeightField
    {
        public int SamplesX;
        public int SamplesZ;
        public float Spacing;
        public int Revision;
        public float[] Heights;
        public Vector4[] Blends;
        public HeightVertex[] DrawVertices;
        public int[] DrawIndices;
        public Vector3[] PhysicsVertices;
        public int[] PhysicsIndices;

        public static HeightField Create(float Size, float Spacing, float Height)
        {
            int Samples = Math.Max(2, (int)Math.Round(Size / Spacing) + 1);
            HeightField Field = new HeightField();
            Field.SamplesX = Samples;
            Field.SamplesZ = Samples;
            Field.Spacing = Spacing;
            int Count = Samples * Samples;
            Field.Heights = new float[Count];
            Field.Blends = new Vector4[Count];
            for (int i = 0; i < Count; i++)
            {
                Field.Heights[i] = Height;
                Field.Blends[i] = new Vector4(1f, 0f, 0f, 0f);
            }

            Field.Rebuild();
            return Field;
        }

        public int Index(int X, int Z)
        {
            return Z * SamplesX + X;
        }

        public bool Inside(int X, int Z)
        {
            return X >= 0 && Z >= 0 && X < SamplesX && Z < SamplesZ;
        }

        public float At(int X, int Z)
        {
            return Heights[Index(X, Z)];
        }

        public bool Sample(float LocalX, float LocalY, out float Height, out float SlopeX, out float SlopeY)
        {
            Height = 0f;
            SlopeX = 0f;
            SlopeY = 0f;
            if (Heights == null || Spacing <= 0.01f)
            {
                return false;
            }

            float GX = LocalX / Spacing;
            float GY = LocalY / Spacing;
            int X0 = (int)Math.Floor(GX);
            int Y0 = (int)Math.Floor(GY);
            int X1 = X0 + 1;
            int Y1 = Y0 + 1;
            if (!Inside(X0, Y0) || !Inside(X1, Y1))
            {
                if (!Inside(X0, Y0))
                {
                    return false;
                }

                Height = At(X0, Y0);
                return true;
            }

            float TX = GX - X0;
            float TY = GY - Y0;
            float H00 = At(X0, Y0);
            float H10 = At(X1, Y0);
            float H01 = At(X0, Y1);
            float H11 = At(X1, Y1);
            float X0H = H00 + (H10 - H00) * TX;
            float X1H = H01 + (H11 - H01) * TX;
            Height = X0H + (X1H - X0H) * TY;
            SlopeX = ((H10 - H00) * (1f - TY) + (H11 - H01) * TY) / Spacing;
            SlopeY = ((H01 - H00) * (1f - TX) + (H11 - H10) * TX) / Spacing;
            return true;
        }

        public Vector4 ReadBlend(float LocalX, float LocalY)
        {
            Vector4 Grass = new Vector4(1f, 0f, 0f, 0f);
            if (Blends == null || Spacing <= 0.01f)
            {
                return Grass;
            }

            float GX = LocalX / Spacing;
            float GY = LocalY / Spacing;
            int X0 = (int)Math.Floor(GX);
            int Y0 = (int)Math.Floor(GY);
            if (!Inside(X0, Y0))
            {
                return Grass;
            }

            int X1 = Math.Min(SamplesX - 1, X0 + 1);
            int Y1 = Math.Min(SamplesZ - 1, Y0 + 1);
            float TX = MathHelper.Clamp(GX - X0, 0f, 1f);
            float TY = MathHelper.Clamp(GY - Y0, 0f, 1f);
            Vector4 Left = Vector4.Lerp(Blends[Index(X0, Y0)], Blends[Index(X1, Y0)], TX);
            Vector4 Right = Vector4.Lerp(Blends[Index(X0, Y1)], Blends[Index(X1, Y1)], TX);
            return Vector4.Lerp(Left, Right, TY);
        }

        public const float StandClearance = 4f;

        public static float Feet(MapInfo Here, float LocalX, float LocalY)
        {
            if (Here == null || Here.Heights == null)
            {
                return 0f;
            }

            float Ground;
            float SlopeX;
            float SlopeY;
            if (!Here.Heights.Sample(LocalX, LocalY, out Ground, out SlopeX, out SlopeY))
            {
                return 0f;
            }

            return Ground;
        }

        public static float View(MapInfo Here, float LocalX, float LocalY, int Floor)
        {
            float Story = BuildFloor.Surface(Floor);
            if (Floor > 0)
            {
                return Story;
            }

            return Here != null && Here.Heights != null ? Feet(Here, LocalX, LocalY) : Story;
        }

        public static float Stand(MapInfo Here, float LocalX, float LocalY)
        {
            float Ground = Feet(Here, LocalX, LocalY);
            float Surface = View(Here, LocalX, LocalY, 0);
            if (Surface > Ground)
            {
                Ground = Surface;
            }

            return Ground + StandClearance;
        }

        public void Rebuild()
        {
            Revision++;
            if (Heights == null || SamplesX < 2 || SamplesZ < 2)
            {
                DrawVertices = Array.Empty<HeightVertex>();
                DrawIndices = Array.Empty<int>();
                PhysicsVertices = Array.Empty<Vector3>();
                PhysicsIndices = Array.Empty<int>();
                return;
            }

            int Count = SamplesX * SamplesZ;
            Vector3[] Normals = new Vector3[Count];
            for (int Z = 0; Z < SamplesZ; Z++)
            {
                for (int X = 0; X < SamplesX; X++)
                {
                    float Left = Inside(X - 1, Z) ? At(X - 1, Z) : At(X, Z);
                    float Right = Inside(X + 1, Z) ? At(X + 1, Z) : At(X, Z);
                    float Down = Inside(X, Z - 1) ? At(X, Z - 1) : At(X, Z);
                    float Up = Inside(X, Z + 1) ? At(X, Z + 1) : At(X, Z);
                    Vector3 Normal = new Vector3(Left - Right, Spacing * 2f, Down - Up);
                    if (Normal.LengthSquared() < 0.0001f)
                    {
                        Normal = Vector3.Up;
                    }
                    else
                    {
                        Normal.Normalize();
                    }

                    Normals[Index(X, Z)] = Normal;
                }
            }

            DrawVertices = new HeightVertex[Count];
            PhysicsVertices = new Vector3[Count];
            for (int Z = 0; Z < SamplesZ; Z++)
            {
                for (int X = 0; X < SamplesX; X++)
                {
                    int AtIndex = Index(X, Z);
                    Vector3 Position = new Vector3(X * Spacing, Heights[AtIndex], Z * Spacing);
                    PhysicsVertices[AtIndex] = Position;
                    DrawVertices[AtIndex] = new HeightVertex(Position, Normals[AtIndex], Blends[AtIndex]);
                }
            }

            int QuadsX = SamplesX - 1;
            int QuadsZ = SamplesZ - 1;
            DrawIndices = new int[QuadsX * QuadsZ * 6];
            int Write = 0;
            for (int Z = 0; Z < QuadsZ; Z++)
            {
                for (int X = 0; X < QuadsX; X++)
                {
                    int A = Index(X, Z);
                    int B = Index(X + 1, Z);
                    int C = Index(X, Z + 1);
                    int D = Index(X + 1, Z + 1);
                    DrawIndices[Write++] = A;
                    DrawIndices[Write++] = C;
                    DrawIndices[Write++] = B;
                    DrawIndices[Write++] = B;
                    DrawIndices[Write++] = C;
                    DrawIndices[Write++] = D;
                }
            }

            PhysicsIndices = DrawIndices;
        }

        public void CopyEdge(HeightField Neighbor, int Side)
        {
            if (Neighbor == null || SamplesX != Neighbor.SamplesX || SamplesZ != Neighbor.SamplesZ)
            {
                return;
            }

            if (Side == 0)
            {
                for (int Z = 0; Z < SamplesZ; Z++)
                {
                    int From = Index(SamplesX - 1, Z);
                    int To = Neighbor.Index(0, Z);
                    Neighbor.Heights[To] = Heights[From];
                    Neighbor.Blends[To] = Blends[From];
                }
            }
            else if (Side == 1)
            {
                for (int Z = 0; Z < SamplesZ; Z++)
                {
                    int From = Index(0, Z);
                    int To = Neighbor.Index(Neighbor.SamplesX - 1, Z);
                    Neighbor.Heights[To] = Heights[From];
                    Neighbor.Blends[To] = Blends[From];
                }
            }
            else if (Side == 2)
            {
                for (int X = 0; X < SamplesX; X++)
                {
                    int From = Index(X, SamplesZ - 1);
                    int To = Neighbor.Index(X, 0);
                    Neighbor.Heights[To] = Heights[From];
                    Neighbor.Blends[To] = Blends[From];
                }
            }
            else if (Side == 3)
            {
                for (int X = 0; X < SamplesX; X++)
                {
                    int From = Index(X, 0);
                    int To = Neighbor.Index(X, Neighbor.SamplesZ - 1);
                    Neighbor.Heights[To] = Heights[From];
                    Neighbor.Blends[To] = Blends[From];
                }
            }
        }

        public void Raise(float LocalX, float LocalY, float Radius, float Amount)
        {
            Stamp(LocalX, LocalY, Radius, delegate (int Index, float Weight)
            {
                Heights[Index] += Amount * Weight;
            });
        }

        public void Smooth(float LocalX, float LocalY, float Radius)
        {
            if (Heights == null)
            {
                return;
            }

            float[] Copy = (float[])Heights.Clone();
            int MinX;
            int MaxX;
            int MinZ;
            int MaxZ;
            Window(LocalX, LocalY, Radius, out MinX, out MaxX, out MinZ, out MaxZ);
            for (int Z = MinZ; Z <= MaxZ; Z++)
            {
                for (int X = MinX; X <= MaxX; X++)
                {
                    float DX = X * Spacing - LocalX;
                    float DY = Z * Spacing - LocalY;
                    float Distance = (float)Math.Sqrt(DX * DX + DY * DY);
                    if (Distance > Radius)
                    {
                        continue;
                    }

                    float Sum = 0f;
                    int Count = 0;
                    for (int NZ = Z - 1; NZ <= Z + 1; NZ++)
                    {
                        for (int NX = X - 1; NX <= X + 1; NX++)
                        {
                            if (!Inside(NX, NZ))
                            {
                                continue;
                            }

                            Sum += Copy[Index(NX, NZ)];
                            Count++;
                        }
                    }

                    if (Count > 0)
                    {
                        float Weight = 1f - Distance / Radius;
                        int AtIndex = Index(X, Z);
                        Heights[AtIndex] = Copy[AtIndex] + (Sum / Count - Copy[AtIndex]) * Weight;
                    }
                }
            }
        }

        public void Flatten(float LocalX, float LocalY, float Radius)
        {
            float Height;
            float SlopeX;
            float SlopeY;
            if (!Sample(LocalX, LocalY, out Height, out SlopeX, out SlopeY))
            {
                return;
            }

            Stamp(LocalX, LocalY, Radius, delegate (int Index, float Weight)
            {
                Heights[Index] += (Height - Heights[Index]) * Weight;
            });
        }

        public void LevelPad(float OriginX, float OriginY, float WorldMinX, float WorldMinY, float WorldMaxX, float WorldMaxY, float Height, float Blend)
        {
            if (Heights == null || Spacing <= 0.01f)
            {
                return;
            }

            float[] Copy = (float[])Heights.Clone();
            float Skirt = Blend > 0f ? Blend : 0f;
            for (int Z = 0; Z < SamplesZ; Z++)
            {
                for (int X = 0; X < SamplesX; X++)
                {
                    float WorldX = OriginX + X * Spacing;
                    float WorldY = OriginY + Z * Spacing;
                    float DX = 0f;
                    if (WorldX < WorldMinX)
                    {
                        DX = WorldMinX - WorldX;
                    }
                    else if (WorldX > WorldMaxX)
                    {
                        DX = WorldX - WorldMaxX;
                    }

                    float DY = 0f;
                    if (WorldY < WorldMinY)
                    {
                        DY = WorldMinY - WorldY;
                    }
                    else if (WorldY > WorldMaxY)
                    {
                        DY = WorldY - WorldMaxY;
                    }

                    float Dist = (float)Math.Sqrt(DX * DX + DY * DY);
                    if (Dist >= Skirt && Skirt > 0f)
                    {
                        continue;
                    }

                    float Along = Skirt <= 0.01f ? 0f : Dist / Skirt;
                    float Fade = Along * Along * (3f - 2f * Along);
                    int At = Index(X, Z);
                    Heights[At] = Copy[At] + (Height - Copy[At]) * (1f - Fade);
                }
            }
        }

        public void Paint(float LocalX, float LocalY, float Radius, Vector4 Channel)
        {
            Stamp(LocalX, LocalY, Radius, delegate (int Index, float Weight)
            {
                Vector4 Current = Blends[Index];
                Blends[Index] = new Vector4(
                    Current.X + (Channel.X - Current.X) * Weight,
                    Current.Y + (Channel.Y - Current.Y) * Weight,
                    Current.Z + (Channel.Z - Current.Z) * Weight,
                    Current.W + (Channel.W - Current.W) * Weight);
            });
        }

        public bool TouchesEdge(float LocalX, float LocalY, float Radius, out bool East, out bool West, out bool North, out bool South)
        {
            float SpanX = (SamplesX - 1) * Spacing;
            float SpanZ = (SamplesZ - 1) * Spacing;
            East = LocalX + Radius >= SpanX;
            West = LocalX - Radius <= 0f;
            North = LocalY + Radius >= SpanZ;
            South = LocalY - Radius <= 0f;
            return East || West || North || South;
        }

        void Stamp(float LocalX, float LocalY, float Radius, Action<int, float> Apply)
        {
            if (Heights == null || Radius <= 0.01f)
            {
                return;
            }

            int MinX;
            int MaxX;
            int MinZ;
            int MaxZ;
            Window(LocalX, LocalY, Radius, out MinX, out MaxX, out MinZ, out MaxZ);
            for (int Z = MinZ; Z <= MaxZ; Z++)
            {
                for (int X = MinX; X <= MaxX; X++)
                {
                    float DX = X * Spacing - LocalX;
                    float DY = Z * Spacing - LocalY;
                    float Distance = (float)Math.Sqrt(DX * DX + DY * DY);
                    if (Distance > Radius)
                    {
                        continue;
                    }

                    Apply(Index(X, Z), 1f - Distance / Radius);
                }
            }
        }

        public void Weld(HeightField Beside, int Side)
        {
            if (Beside == null || DrawVertices == null || Beside.DrawVertices == null)
            {
                return;
            }

            if (SamplesX != Beside.SamplesX || SamplesZ != Beside.SamplesZ || SamplesX < 3 || SamplesZ < 3)
            {
                return;
            }

            if (Side == 0)
            {
                int X = SamplesX - 1;
                for (int Z = 0; Z < SamplesZ; Z++)
                {
                    float Down = Z > 0 ? At(X, Z - 1) : At(X, Z);
                    float Up = Z + 1 < SamplesZ ? At(X, Z + 1) : At(X, Z);
                    Vector3 Normal = Slope(At(X - 1, Z), Beside.At(1, Z), Down, Up);
                    WriteNormal(X, Z, Normal);
                    Beside.WriteNormal(0, Z, Normal);
                }
            }
            else if (Side == 1)
            {
                for (int Z = 0; Z < SamplesZ; Z++)
                {
                    float Down = Z > 0 ? At(0, Z - 1) : At(0, Z);
                    float Up = Z + 1 < SamplesZ ? At(0, Z + 1) : At(0, Z);
                    Vector3 Normal = Slope(Beside.At(Beside.SamplesX - 2, Z), At(1, Z), Down, Up);
                    WriteNormal(0, Z, Normal);
                    Beside.WriteNormal(Beside.SamplesX - 1, Z, Normal);
                }
            }
            else if (Side == 2)
            {
                int Z = SamplesZ - 1;
                for (int X = 0; X < SamplesX; X++)
                {
                    float Left = X > 0 ? At(X - 1, Z) : At(X, Z);
                    float Right = X + 1 < SamplesX ? At(X + 1, Z) : At(X, Z);
                    Vector3 Normal = Slope(Left, Right, At(X, Z - 1), Beside.At(X, 1));
                    WriteNormal(X, Z, Normal);
                    Beside.WriteNormal(X, 0, Normal);
                }
            }
            else if (Side == 3)
            {
                for (int X = 0; X < SamplesX; X++)
                {
                    float Left = X > 0 ? At(X - 1, 0) : At(X, 0);
                    float Right = X + 1 < SamplesX ? At(X + 1, 0) : At(X, 0);
                    Vector3 Normal = Slope(Left, Right, Beside.At(X, Beside.SamplesZ - 2), At(X, 1));
                    WriteNormal(X, 0, Normal);
                    Beside.WriteNormal(X, Beside.SamplesZ - 1, Normal);
                }
            }
        }

        Vector3 Slope(float Left, float Right, float Down, float Up)
        {
            Vector3 Normal = new Vector3(Left - Right, Spacing * 2f, Down - Up);
            if (Normal.LengthSquared() < 0.0001f)
            {
                return Vector3.Up;
            }

            Normal.Normalize();
            return Normal;
        }

        void WriteNormal(int X, int Z, Vector3 Normal)
        {
            int AtIndex = Index(X, Z);
            HeightVertex Vertex = DrawVertices[AtIndex];
            Vertex.Normal = Normal;
            DrawVertices[AtIndex] = Vertex;
        }

        void Window(float LocalX, float LocalY, float Radius, out int MinX, out int MaxX, out int MinZ, out int MaxZ)
        {
            MinX = Math.Max(0, (int)Math.Floor((LocalX - Radius) / Spacing));
            MaxX = Math.Min(SamplesX - 1, (int)Math.Ceiling((LocalX + Radius) / Spacing));
            MinZ = Math.Max(0, (int)Math.Floor((LocalY - Radius) / Spacing));
            MaxZ = Math.Min(SamplesZ - 1, (int)Math.Ceiling((LocalY + Radius) / Spacing));
        }
    }

    public struct HeightVertex : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Blend;
        public float Wet;
        public float Shade;
        public float Open;

        public static readonly VertexDeclaration Declaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(40, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(44, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(48, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3));

        public HeightVertex(Vector3 Position, Vector3 Normal, Vector4 Blend)
        {
            this.Position = Position;
            this.Normal = Normal;
            this.Blend = Blend;
            Wet = 0f;
            Shade = 1f;
            Open = 1f;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return Declaration; }
        }
    }
}
