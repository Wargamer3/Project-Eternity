using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum WaterLook
    {
        Tropical,
        Northern,
        Cave,
    }

    public enum WaterKind
    {
        Pool,
        River,
        Lake,
    }

    public struct WaterLookStyle
    {
        public Vector4 Shallow;
        public Vector4 Deep;
        public Vector4 Strengths;
        public float Specular;

        public static WaterLookStyle Of(WaterLook Look)
        {
            WaterLookStyle Style = new WaterLookStyle();
            Style.Shallow.W = 1f;
            Style.Deep.W = 1f;
            if (Look == WaterLook.Northern)
            {
                Style.Shallow = new Vector4(0.62f, 0.74f, 0.82f, 1f);
                Style.Deep = new Vector4(0.02f, 0.08f, 0.22f, 1f);
                Style.Strengths = new Vector4(0.25f, 0.8f, 1f, 0f);
                Style.Specular = 0.7f;
                return Style;
            }

            if (Look == WaterLook.Cave)
            {
                Style.Shallow = new Vector4(0.05f, 0.08f, 0.06f, 1f);
                Style.Deep = new Vector4(0.01f, 0.02f, 0.02f, 1f);
                Style.Strengths = new Vector4(0.15f, 0.35f, 0f, 0f);
                Style.Specular = 0.15f;
                return Style;
            }

            Style.Shallow = new Vector4(0.18f, 0.72f, 0.48f, 1f);
            Style.Deep = new Vector4(0.02f, 0.35f, 0.38f, 1f);
            Style.Strengths = new Vector4(0.8f, 1f, 0.5f, 1f);
            Style.Specular = 0.85f;
            return Style;
        }

        public static WaterLookStyle Average(WaterLookStyle A, WaterLookStyle B, int Count, int Next)
        {
            float Weight = Count / (float)Next;
            float Added = 1f / Next;
            WaterLookStyle Mixed = new WaterLookStyle();
            Mixed.Shallow = A.Shallow * Weight + B.Shallow * Added;
            Mixed.Deep = A.Deep * Weight + B.Deep * Added;
            Mixed.Strengths = A.Strengths * Weight + B.Strengths * Added;
            Mixed.Specular = A.Specular * Weight + B.Specular * Added;
            return Mixed;
        }
    }

    public class WaterVolume
    {
        public WaterKind Kind;
        public WaterLook Look;
        public Vector3 Start;
        public Vector3 End;
        public float Radius;
        public float Width;
        public float Depth;
        public float Speed;
        public float Cell;
        public int[] LakeCells;
        HashSet<int> LakeLookup;
        HeightField BasinGround;
        int BasinSig;
        bool[] BasinWet;
        float[] BasinDepth;
        int BasinW;
        int BasinH;
        float BasinStep;

        public bool Footprint(float X, float Y, out float Surface, out Vector2 Flow)
        {
            Surface = Start.Z;
            Flow = Vector2.Zero;
            if (Kind == WaterKind.Lake)
            {
                if (LakeCells != null && Cell > 0.01f)
                {
                    int CellX = (int)Math.Floor(X / Cell + 0.5f);
                    int CellY = (int)Math.Floor(Y / Cell + 0.5f);
                    for (int DY = -2; DY <= 2; DY++)
                    {
                        for (int DX = -2; DX <= 2; DX++)
                        {
                            if (Holds(CellX + DX, CellY + DY))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                float LakeX = X - Start.X;
                float LakeY = Y - Start.Y;
                return LakeX * LakeX + LakeY * LakeY <= Radius * Radius;
            }

            if (Kind == WaterKind.Pool)
            {
                float DX = X - Start.X;
                float DY = Y - Start.Y;
                return DX * DX + DY * DY <= Radius * Radius;
            }

            Vector2 A = new Vector2(Start.X, Start.Y);
            Vector2 B = new Vector2(End.X, End.Y);
            Vector2 AB = B - A;
            float LengthSquared = AB.LengthSquared();
            float T = 0f;
            if (LengthSquared > 0.0001f)
            {
                T = MathHelper.Clamp(Vector2.Dot(new Vector2(X, Y) - A, AB) / LengthSquared, 0f, 1f);
            }

            Vector2 Closest = A + AB * T;
            float OffX = X - Closest.X;
            float OffY = Y - Closest.Y;
            float Half = Width * 0.5f;
            if (OffX * OffX + OffY * OffY > Half * Half)
            {
                return false;
            }

            Surface = MathHelper.Lerp(Start.Z, End.Z, T);
            if (LengthSquared > 0.0001f)
            {
                float Pace = Speed;
                if (Pace > 0.5f && Pace < 36f)
                {
                    Pace = 36f;
                }

                Flow = AB / (float)Math.Sqrt(LengthSquared) * Pace;
            }

            return true;
        }

        public bool Column(float FeetZ, float BodyHeight, float Surface)
        {
            return FeetZ <= Surface + 1f && FeetZ + BodyHeight >= Surface - Depth;
        }

        public float SurfaceAt(float X, float Y)
        {
            if (Kind != WaterKind.River)
            {
                return Start.Z;
            }

            Vector2 A = new Vector2(Start.X, Start.Y);
            Vector2 B = new Vector2(End.X, End.Y);
            Vector2 AB = B - A;
            float LengthSquared = AB.LengthSquared();
            float T = 0f;
            if (LengthSquared > 0.0001f)
            {
                T = MathHelper.Clamp(Vector2.Dot(new Vector2(X, Y) - A, AB) / LengthSquared, 0f, 1f);
            }

            return MathHelper.Lerp(Start.Z, End.Z, T);
        }

        public void SetLakeCells(int[] Cells)
        {
            LakeCells = Cells;
            LakeLookup = null;
        }

        public float DistanceToCourse(float X, float Y)
        {
            Vector2 A = new Vector2(Start.X, Start.Y);
            Vector2 B = new Vector2(End.X, End.Y);
            Vector2 AB = B - A;
            float LengthSquared = AB.LengthSquared();
            float T = 0f;
            if (LengthSquared > 0.0001f)
            {
                T = MathHelper.Clamp(Vector2.Dot(new Vector2(X, Y) - A, AB) / LengthSquared, 0f, 1f);
            }

            Vector2 Closest = A + AB * T;
            float OffX = X - Closest.X;
            float OffY = Y - Closest.Y;
            return (float)Math.Sqrt(OffX * OffX + OffY * OffY);
        }

        public bool Covers(HeightField Ground, float X, float Y)
        {
            return Covers(Ground, X, Y, -0.05f);
        }

        public bool Covers(HeightField Ground, float X, float Y, float DryAbove)
        {
            if (Ground == null)
            {
                return false;
            }

            float Surface = SurfaceAt(X, Y);
            float Height;
            float SlopeX;
            float SlopeY;
            if (!Ground.Sample(X, Y, out Height, out SlopeX, out SlopeY) || Height >= Surface + DryAbove)
            {
                return false;
            }

            if (Kind == WaterKind.River)
            {
                return DistanceToCourse(X, Y) <= Math.Max(Width, 4f) * 0.55f;
            }

            if (Kind != WaterKind.Lake || LakeCells == null || Cell <= 0.01f)
            {
                return false;
            }

            bool[] Wet;
            float[] Depths;
            int WidthN;
            int HeightN;
            float Step;
            if (!PrepareBasin(Ground, out Wet, out Depths, out WidthN, out HeightN, out Step))
            {
                return false;
            }

            int X0 = (int)Math.Floor(X / Step);
            int Y0 = (int)Math.Floor(Y / Step);
            for (int DY = 0; DY <= 1; DY++)
            {
                for (int DX = 0; DX <= 1; DX++)
                {
                    int NX = X0 + DX;
                    int NY = Y0 + DY;
                    if (NX >= 0 && NY >= 0 && NX < WidthN && NY < HeightN && Wet[NY * WidthN + NX])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool PrepareBasin(HeightField Ground, out bool[] Wet, out float[] Depths, out int WidthN, out int HeightN, out float Step)
        {
            Wet = null;
            Depths = null;
            WidthN = 0;
            HeightN = 0;
            Step = 4f;
            if (Ground == null || Kind != WaterKind.Lake || LakeCells == null || LakeCells.Length == 0 || Cell <= 0.01f || Ground.SamplesX < 2 || Ground.SamplesZ < 2 || Ground.Spacing <= 0.01f)
            {
                return false;
            }

            int Sig = LakeCells.Length;
            for (int i = 0; i < LakeCells.Length; i++)
            {
                Sig = Sig * 31 + LakeCells[i];
            }

            Sig = Sig * 31 + Ground.Revision;
            Sig = Sig * 31 + Start.Z.GetHashCode();
            Sig = Sig * 31 + Cell.GetHashCode();
            if (BasinGround == Ground && BasinSig == Sig && BasinWet != null && BasinDepth != null)
            {
                Wet = BasinWet;
                Depths = BasinDepth;
                WidthN = BasinW;
                HeightN = BasinH;
                Step = BasinStep;
                return BasinW >= 2 && BasinH >= 2;
            }

            float LimitX = (Ground.SamplesX - 1) * Ground.Spacing;
            float LimitY = (Ground.SamplesZ - 1) * Ground.Spacing;
            Step = 4f;
            while (Step < LimitX && Step < LimitY && (LimitX / Step + 1f) * (LimitY / Step + 1f) > 128f * 128f)
            {
                Step *= 2f;
            }

            WidthN = (int)Math.Floor(LimitX / Step + 0.001f) + 1;
            HeightN = (int)Math.Floor(LimitY / Step + 0.001f) + 1;
            if (WidthN < 2 || HeightN < 2)
            {
                return false;
            }

            int Count = WidthN * HeightN;
            Wet = new bool[Count];
            Depths = new float[Count];
            bool[] Open = new bool[Count];
            int[] Queue = new int[Count];
            int Head = 0;
            int Tail = 0;
            float Surface = Start.Z;
            for (int Y = 0; Y < HeightN; Y++)
            {
                for (int X = 0; X < WidthN; X++)
                {
                    int At = Y * WidthN + X;
                    float WorldX = X * Step;
                    float WorldY = Y * Step;
                    float Height;
                    float SlopeX;
                    float SlopeY;
                    if (!Ground.Sample(WorldX, WorldY, out Height, out SlopeX, out SlopeY))
                    {
                        Depths[At] = -1f;
                        continue;
                    }

                    Depths[At] = Surface - Height;
                    if (Depths[At] <= 0.05f || !BesideSaved(WorldX, WorldY))
                    {
                        continue;
                    }

                    Open[At] = true;
                    if (OnSaved(WorldX, WorldY))
                    {
                        Wet[At] = true;
                        Queue[Tail++] = At;
                    }
                }
            }

            while (Head < Tail)
            {
                int At = Queue[Head++];
                int X = At % WidthN;
                int Y = At / WidthN;
                for (int Side = 0; Side < 4; Side++)
                {
                    int NX = X + (Side == 0 ? 1 : Side == 1 ? -1 : 0);
                    int NY = Y + (Side == 2 ? 1 : Side == 3 ? -1 : 0);
                    if (NX < 0 || NY < 0 || NX >= WidthN || NY >= HeightN)
                    {
                        continue;
                    }

                    int Next = NY * WidthN + NX;
                    if (Wet[Next] || !Open[Next])
                    {
                        continue;
                    }

                    Wet[Next] = true;
                    Queue[Tail++] = Next;
                }
            }

            BasinGround = Ground;
            BasinSig = Sig;
            BasinWet = Wet;
            BasinDepth = Depths;
            BasinW = WidthN;
            BasinH = HeightN;
            BasinStep = Step;
            return true;
        }

        bool OnSaved(float X, float Y)
        {
            int CellX = (int)Math.Floor(X / Cell + 0.5f);
            int CellY = (int)Math.Floor(Y / Cell + 0.5f);
            return Holds(CellX, CellY);
        }

        bool BesideSaved(float X, float Y)
        {
            int CellX = (int)Math.Floor(X / Cell + 0.5f);
            int CellY = (int)Math.Floor(Y / Cell + 0.5f);
            for (int DY = -1; DY <= 1; DY++)
            {
                for (int DX = -1; DX <= 1; DX++)
                {
                    if (Holds(CellX + DX, CellY + DY))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool Holds(int X, int Z)
        {
            if (X < 0 || Z < 0 || LakeCells == null)
            {
                return false;
            }

            if (LakeLookup == null || LakeLookup.Count != LakeCells.Length)
            {
                LakeLookup = new HashSet<int>(LakeCells);
            }

            return LakeLookup.Contains(X | (Z << 16));
        }
    }

    public struct WaterSample
    {
        public bool Hit;
        public float Surface;
        public float Bed;
        public Vector2 Flow;
    }

    public struct WaterShift
    {
        public Vector3 Position;
        public bool InWater;
        public float Surface;
        public float Submerged;
    }

    public struct WaterVertex : IVertexType
    {
        public Vector3 Position;
        public Vector2 WorldXZ;
        public Vector2 Flow;
        public Vector4 Shallow;
        public Vector4 Deep;
        public Vector4 Strengths;

        public static readonly VertexDeclaration Declaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(28, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(44, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(60, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4));

        public VertexDeclaration VertexDeclaration
        {
            get { return Declaration; }
        }
    }

    public class WaterSheet
    {
        public WaterVertex[] Vertices = Array.Empty<WaterVertex>();
        public int[] Indices = Array.Empty<int>();
    }

    public class WaterRing
    {
        public Vector2 Center;
        public float Born;
        public float Strength;
    }

    public class WaterPoint
    {
        public Vector2 At;
        public float Born;
    }

    public class WaterWake
    {
        public float Touch;
        public Vector2 Head;
        public float HeadBorn;
        public bool HasHead;
        public readonly List<WaterPoint> Points = new List<WaterPoint>();
    }

    public static class WaterTrails
    {
        public const int SlotCount = 48;
        public const float Spacing = 5f;
        public const float Life = 12f;

        static readonly List<WaterWake> Live = new List<WaterWake>();
        static readonly Dictionary<object, WaterWake> ByOwner = new Dictionary<object, WaterWake>();
        static readonly Dictionary<object, bool> Swimming = new Dictionary<object, bool>();

        public static bool IsSwimming(object Owner)
        {
            bool Value;
            return Owner != null && Swimming.TryGetValue(Owner, out Value) && Value;
        }

        public static void SetSwimming(object Owner, bool Value)
        {
            if (Owner == null)
            {
                return;
            }

            Swimming[Owner] = Value;
        }

        public static void Mark(object Owner, Vector2 WorldAt, float Now)
        {
            if (Owner == null)
            {
                return;
            }

            WaterWake Trail;
            if (!ByOwner.TryGetValue(Owner, out Trail) || Trail == null)
            {
                Trail = new WaterWake();
                ByOwner[Owner] = Trail;
                Live.Add(Trail);
            }

            Trail.Touch = Now;
            Trail.Head = WorldAt;
            Trail.HeadBorn = Now;
            Trail.HasHead = true;
            int Last = Trail.Points.Count - 1;
            if (Last < 0 || Vector2.Distance(Trail.Points[Last].At, WorldAt) >= Spacing)
            {
                AddPoint(Trail, WorldAt, Now);
            }
        }

        static void AddPoint(WaterWake Trail, Vector2 WorldAt, float Now)
        {
            WaterPoint Point = new WaterPoint();
            Point.At = WorldAt;
            Point.Born = Now;
            Trail.Points.Add(Point);
            if (Trail.Points.Count > SlotCount - 1)
            {
                Trail.Points.RemoveAt(0);
            }
        }

        public static void Age(float Now)
        {
            for (int i = Live.Count - 1; i >= 0; i--)
            {
                WaterWake Trail = Live[i];
                for (int p = Trail.Points.Count - 1; p >= 0; p--)
                {
                    if (Now - Trail.Points[p].Born >= Life)
                    {
                        Trail.Points.RemoveAt(p);
                    }
                }

                if (Trail.Points.Count == 0)
                {
                    Live.RemoveAt(i);
                    Drop(Trail);
                }
            }
        }

        static void Drop(WaterWake Trail)
        {
            List<object> Gone = null;
            foreach (KeyValuePair<object, WaterWake> Pair in ByOwner)
            {
                if (Pair.Value != Trail)
                {
                    continue;
                }

                if (Gone == null)
                {
                    Gone = new List<object>();
                }

                Gone.Add(Pair.Key);
            }

            if (Gone == null)
            {
                return;
            }

            for (int i = 0; i < Gone.Count; i++)
            {
                ByOwner.Remove(Gone[i]);
            }
        }

        public static int Pack(Vector2 Origin, Vector4[] Upload)
        {
            int Written = 0;
            for (int w = Live.Count - 1; w >= 0 && Written < Upload.Length; w--)
            {
                WaterWake Trail = Live[w];
                if (Trail.Points.Count == 0)
                {
                    continue;
                }

                int Room = Upload.Length - Written;
                int Start = Math.Max(0, Trail.Points.Count - Room);
                for (int p = Start; p < Trail.Points.Count && Written < Upload.Length; p++)
                {
                    float Link = p == Start ? 1f : 0f;
                    Vector2 At = Trail.Points[p].At - Origin;
                    Upload[Written] = new Vector4(At.X, At.Y, Trail.Points[p].Born, Link);
                    Written++;
                }

                if (Trail.HasHead && Written > 0 && Written < Upload.Length)
                {
                    Vector2 At = Trail.Head - Origin;
                    Vector2 Previous = new Vector2(Upload[Written - 1].X, Upload[Written - 1].Y);
                    if (Vector2.Distance(Previous, At) > 0.25f)
                    {
                        Upload[Written] = new Vector4(At.X, At.Y, Trail.HeadBorn, 0f);
                        Written++;
                    }
                }
            }

            for (int i = Written; i < Upload.Length; i++)
            {
                Upload[i] = Vector4.Zero;
            }

            return Written;
        }
    }

    public static class WaterSession
    {
        public static WaterVolume Preview;
    }

    public static class WaterPlace
    {
        public const float PoolRadius = 48f;
        public const float RiverWidth = 32f;
        public const float Depth = 24f;
        public const float Speed = 36f;
        public const float Quad = 16f;
        public const float CharacterHeight = 40f;
        public const float SwimBody = 20f;
        public const float CharacterDensity = 0.85f;
        public const float HeavyDensity = 1.4f;
        public const float LightDensity = 0.4f;
        public const float DrySeconds = 60f;
        public const float RingLife = 1.5f;
        public const int RingCap = 8;

        public static WaterVolume MakePool(Vector3 Ground, WaterLook Look)
        {
            WaterVolume Pool = new WaterVolume();
            Pool.Kind = WaterKind.Pool;
            Pool.Look = Look;
            Pool.Start = Ground;
            Pool.End = Ground;
            Pool.Radius = PoolRadius;
            Pool.Width = PoolRadius * 2f;
            Pool.Depth = Depth;
            Pool.Speed = 0f;
            return Pool;
        }

        public static WaterVolume MakeRiver(Vector3 Start, Vector3 End, WaterLook Look, bool AllowShort)
        {
            float DX = End.X - Start.X;
            float DY = End.Y - Start.Y;
            if (!AllowShort && DX * DX + DY * DY < 16f)
            {
                return null;
            }

            WaterVolume River = new WaterVolume();
            River.Kind = WaterKind.River;
            River.Look = Look;
            River.Start = Start;
            River.End = End;
            River.Radius = 0f;
            River.Width = RiverWidth;
            River.Depth = Depth;
            River.Speed = Speed;
            return River;
        }

        public static bool RemoveAt(IList<WaterVolume> Volumes, Vector3 Ground)
        {
            if (Volumes == null)
            {
                return false;
            }

            for (int i = Volumes.Count - 1; i >= 0; i--)
            {
                WaterVolume Volume = Volumes[i];
                float Surface;
                Vector2 Flow;
                if (Volume != null && Volume.Footprint(Ground.X, Ground.Y, out Surface, out Flow))
                {
                    Volumes.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public static WaterSample Sample(IList<WaterVolume> Volumes, Vector3 Feet, float BodyHeight, HeightField Ground)
        {
            WaterSample Found = new WaterSample();
            Found.Surface = float.MinValue;
            Found.Bed = float.MaxValue;
            if (Volumes == null || BodyHeight <= 0.01f)
            {
                return Found;
            }

            Vector2 Sum = Vector2.Zero;
            int Count = 0;
            for (int i = 0; i < Volumes.Count; i++)
            {
                WaterVolume Volume = Volumes[i];
                if (Volume == null)
                {
                    continue;
                }

                float Surface;
                Vector2 Flow;
                bool Inside = Volume.Footprint(Feet.X, Feet.Y, out Surface, out Flow);
                if (Ground != null && (Volume.Kind == WaterKind.Lake || Volume.Kind == WaterKind.River))
                {
                    Inside = Volume.Covers(Ground, Feet.X, Feet.Y);
                    if (Inside)
                    {
                        Surface = Volume.SurfaceAt(Feet.X, Feet.Y);
                    }
                }

                if (!Inside || !Volume.Column(Feet.Z, BodyHeight, Surface))
                {
                    continue;
                }

                Sum += Flow;
                Count++;
                if (Surface > Found.Surface)
                {
                    Found.Surface = Surface;
                }

                float Bed = Surface - Volume.Depth;
                if (Ground != null)
                {
                    float Terrain;
                    float GradeX;
                    float GradeY;
                    if (Ground.Sample(Feet.X, Feet.Y, out Terrain, out GradeX, out GradeY))
                    {
                        Bed = Terrain;
                    }
                }

                if (Bed < Found.Bed)
                {
                    Found.Bed = Bed;
                }
            }

            if (Count == 0)
            {
                return new WaterSample();
            }

            Found.Hit = true;
            Found.Flow = Sum / Count;
            return Found;
        }

        public static Vector2 FlowAt(IList<WaterVolume> Volumes, Vector3 Point)
        {
            return Sample(Volumes, Point, 1f, null).Flow;
        }

        public static WaterLookStyle LookAt(IList<WaterVolume> Volumes, Vector3 Point)
        {
            WaterLookStyle Mixed = new WaterLookStyle();
            int Count = 0;
            if (Volumes == null)
            {
                return WaterLookStyle.Of(WaterLook.Tropical);
            }

            for (int i = 0; i < Volumes.Count; i++)
            {
                WaterVolume Volume = Volumes[i];
                float Surface;
                Vector2 Flow;
                if (Volume == null || !Volume.Footprint(Point.X, Point.Y, out Surface, out Flow) || !Volume.Column(Point.Z, 1f, Surface))
                {
                    continue;
                }

                WaterLookStyle Style = WaterLookStyle.Of(Volume.Look);
                Count++;
                if (Count == 1)
                {
                    Mixed = Style;
                }
                else
                {
                    Mixed = WaterLookStyle.Average(Mixed, Style, Count - 1, Count);
                }
            }

            if (Count == 0)
            {
                return WaterLookStyle.Of(WaterLook.Tropical);
            }

            return Mixed;
        }

        public static float DensityOf(FurnitureItem Box)
        {
            if (Box != null && Box.Kind == BoxKind.Long)
            {
                return HeavyDensity;
            }

            return LightDensity;
        }

        public static void Soak(ref float Line, ref float Amount, bool InWater, float SurfaceAboveFeet, float Dt)
        {
            if (InWater)
            {
                if (SurfaceAboveFeet > Line)
                {
                    Line = SurfaceAboveFeet;
                }

                Amount = 1f;
                return;
            }

            if (Amount <= 0f)
            {
                return;
            }

            Amount -= Dt / DrySeconds;
            if (Amount <= 0f)
            {
                Amount = 0f;
                Line = 0f;
            }
        }

        public static WaterShift Move(IList<WaterVolume> Volumes, Vector3 Feet, float BodyHeight, float Density, bool WasInside, float Dt, HeightField Ground)
        {
            WaterShift Shift = new WaterShift();
            Shift.Position = Feet;
            WaterSample Here = Sample(Volumes, Feet, BodyHeight, Ground);
            if (!Here.Hit)
            {
                return Shift;
            }

            float Fraction = Density < 1f ? Density : 1f;
            float Bottom = Here.Bed;
            bool HasBottom = false;
            if (Ground != null)
            {
                float Terrain;
                float SlopeX;
                float SlopeY;
                if (Ground.Sample(Feet.X, Feet.Y, out Terrain, out SlopeX, out SlopeY))
                {
                    Bottom = Terrain;
                    HasBottom = true;
                }
            }

            float Draft = Density < 1f ? Density * BodyHeight : BodyHeight;
            float Depth = Here.Surface - Bottom;
            float Target = Feet.Z;
            if (Density < 1f && Depth >= Draft)
            {
                Target = Here.Surface - Draft;
                if (HasBottom && Target < Bottom)
                {
                    Target = Bottom;
                }
            }
            else if (Density >= 1f)
            {
                Target = HasBottom ? Math.Max(Here.Bed, Bottom) : Here.Bed;
            }
            else if (Target < Bottom)
            {
                Target = Bottom;
            }

            Shift.Position.Z = Target;
            Shift.Position.X += Here.Flow.X * Fraction * Dt;
            Shift.Position.Y += Here.Flow.Y * Fraction * Dt;
            WaterSample After = Sample(Volumes, Shift.Position, BodyHeight, Ground);
            if (!After.Hit)
            {
                Shift.Position.Z = Feet.Z;
                return Shift;
            }

            Shift.InWater = true;
            Shift.Surface = Here.Surface;
            Shift.Submerged = Fraction;
            return Shift;
        }

        public static bool Over(MapInfo Map, float LocalX, float LocalY)
        {
            if (Map == null || Map.Water == null || Map.Heights == null)
            {
                return false;
            }

            for (int i = 0; i < Map.Water.Count; i++)
            {
                WaterVolume Volume = Map.Water[i];
                if (Volume != null && Volume.Covers(Map.Heights, LocalX, LocalY))
                {
                    return true;
                }
            }

            return false;
        }

        public static void DriftProps(MapInfo Map, float Dt)
        {
            if (Map == null || Map.Furniture == null || Map.Tracks == null)
            {
                return;
            }

            for (int i = 0; i < Map.Furniture.Count; i++)
            {
                FurnitureItem Box = Map.Furniture[i];
                if (Box == null || Box.IsRope)
                {
                    continue;
                }

                bool Was = Map.Tracks.Inside(Box);
                if (!Was && (Map.Water == null || Map.Water.Count == 0))
                {
                    continue;
                }

                Vector2 Before = new Vector2(Box.Position.X, Box.Position.Y);
                WaterShift Shift = Move(Map.Water, Box.Position, Box.Height, DensityOf(Box), Was, Dt, Map.Heights);
                if (!Was && !Shift.InWater)
                {
                    continue;
                }

                Box.Position = Shift.Position;
                Vector2 After = new Vector2(Shift.Position.X, Shift.Position.Y);
                Map.Tracks.Note(Box, After, Shift.InWater, Vector2.Distance(Before, After) >= 4f, Dt, ChunkOrigin.Shift(Map));
            }
        }

        public static void DriftPeople(MapInfo Map, IList<PlayerCharacter> People, float Dt, PhysicsManager Physics)
        {
            if (Map == null || People == null || Map.Tracks == null)
            {
                return;
            }

            for (int i = 0; i < People.Count; i++)
            {
                PlayerCharacter Who = People[i];
                if (Who == null || (Physics != null && Physics.Drives(Who)))
                {
                    continue;
                }

                StepPerson(Map, Who, Dt);
            }
        }

        public static void StepPerson(MapInfo Map, PlayerCharacter Who, float Dt)
        {
            if (Map == null || Who == null || Map.Tracks == null)
            {
                return;
            }

            bool Was = Who.WasInWater;
            if (!Was && (Map.Water == null || Map.Water.Count == 0))
            {
                return;
            }

            Vector3 Feet = Who.Position;
            Vector2 Before = new Vector2(Feet.X, Feet.Y);
            WaterShift Shift = Move(Map.Water, Feet, SwimBody, CharacterDensity, Was, Dt, Map.Heights);
            if (!Was && !Shift.InWater)
            {
                return;
            }

            Who.Position = Shift.Position;
            Who.WasInWater = Shift.InWater;
            Soak(ref Who.WetLine, ref Who.WetAmount, Shift.InWater, Shift.InWater ? Shift.Surface - Shift.Position.Z : 0f, Dt);
            Vector2 After = new Vector2(Shift.Position.X, Shift.Position.Y);
            Map.Tracks.Note(Who, After, Shift.InWater, Vector2.Distance(Before, After) > 0.02f, Dt, ChunkOrigin.Shift(Map));
        }
    }

    public class WaterTool
    {
        public WaterLook Look;
        public bool HasStart;
        public Vector3 Start;

        public WaterVolume Confirm(IList<WaterVolume> Volumes, Vector3 Ground, bool River)
        {
            if (Volumes == null)
            {
                return null;
            }

            if (!River)
            {
                HasStart = false;
                WaterVolume Pool = WaterPlace.MakePool(Ground, Look);
                Volumes.Add(Pool);
                return Pool;
            }

            if (!HasStart)
            {
                HasStart = true;
                Start = Ground;
                return null;
            }

            WaterVolume Made = WaterPlace.MakeRiver(Start, Ground, Look, false);
            if (Made == null)
            {
                return null;
            }

            HasStart = false;
            Volumes.Add(Made);
            return Made;
        }

        public void Cancel()
        {
            HasStart = false;
        }

        public WaterVolume Ghost(Vector3 Ground)
        {
            if (!HasStart)
            {
                return null;
            }

            return WaterPlace.MakeRiver(Start, Ground, Look, true);
        }
    }

    public class WaterTracks
    {
        public readonly List<WaterRing> Rings = new List<WaterRing>();
        public readonly List<WaterWake> Wakes = new List<WaterWake>();
        readonly Dictionary<object, BodyTrack> Bodies = new Dictionary<object, BodyTrack>();

        class BodyTrack
        {
            public bool Inside;
            public bool Has;
            public Vector2 Last;
        }

        public bool Inside(object Owner)
        {
            BodyTrack Track;
            return Owner != null && Bodies.TryGetValue(Owner, out Track) && Track.Inside;
        }

        public void Tick(float Dt)
        {
            Age(Dt);
        }

        public void Note(object Owner, Vector2 At, bool InWater, bool Moving, float Dt, Vector3 Origin)
        {
            if (Owner == null)
            {
                return;
            }

            BodyTrack Track;
            if (!Bodies.TryGetValue(Owner, out Track))
            {
                Track = new BodyTrack();
                Bodies.Add(Owner, Track);
            }

            if (Track.Has)
            {
                Moving = Vector2.Distance(Track.Last, At) > 0.02f;
            }

            bool Entered = InWater && !WaterTrails.IsSwimming(Owner);
            WaterTrails.SetSwimming(Owner, InWater);
            Track.Last = At;
            Track.Has = true;
            Track.Inside = InWater;
            if (!InWater)
            {
                Track.Has = false;
                return;
            }

            if (Entered)
            {
                AddRing(At);
            }

            if (!Moving)
            {
                return;
            }

            WaterTrails.Mark(Owner, new Vector2(At.X + Origin.X, At.Y + Origin.Z), WaterDraw.Time);
        }

        void Age(float Dt)
        {
            float Now = WaterDraw.Time;
            for (int i = Rings.Count - 1; i >= 0; i--)
            {
                if (Now - Rings[i].Born >= WaterPlace.RingLife)
                {
                    Rings.RemoveAt(i);
                }
            }
        }

        void AddRing(Vector2 At)
        {
            while (Rings.Count >= WaterPlace.RingCap)
            {
                Rings.RemoveAt(0);
            }

            WaterRing Ring = new WaterRing();
            Ring.Center = At;
            Ring.Born = WaterDraw.Time;
            Ring.Strength = 1f;
            Rings.Add(Ring);
        }
    }

    public static class WaterMesh
    {

        public static List<WaterSheet> Build(IList<WaterVolume> Volumes, HeightField Ground, MapInfo Chunk = null)
        {
            List<WaterSheet> Sheets = new List<WaterSheet>();
            if (Volumes == null || Volumes.Count == 0)
            {
                return Sheets;
            }

            for (int i = 0; i < Volumes.Count; i++)
            {
                WaterVolume Volume = Volumes[i];
                if (Volume == null)
                {
                    continue;
                }

                if (Volume.Kind == WaterKind.Pool)
                {
                    Sheets.Add(Disc(Volume, Volumes));
                }
                else if (Volume.Kind == WaterKind.River)
                {
                    WaterSheet Bank = Contour(Volume, Ground, Volumes);
                    Sheets.Add(Bank != null && Bank.Vertices.Length > 0 ? Bank : Ribbon(Volume, Volumes));
                }
                else
                {
                    WaterSheet Shore = Lake(Volume, Ground, Volumes);
                    if (Shore != null && Shore.Vertices.Length > 0)
                    {
                        Sheets.Add(Shore);
                    }
                }
            }

            return Sheets;
        }

        static WaterSheet Lake(WaterVolume Volume, HeightField Ground, IList<WaterVolume> Volumes)
        {
            bool[] Wet;
            float[] Depths;
            int W;
            int H;
            float Step;
            if (Volume == null || !Volume.PrepareBasin(Ground, out Wet, out Depths, out W, out H, out Step))
            {
                return null;
            }

            ShoreNode[] Nodes = new ShoreNode[W * H];
            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int At = Y * W + X;
                    Nodes[At].X = X * Step;
                    Nodes[At].Y = Y * Step;
                    Nodes[At].Depth = Depths[At];
                    Nodes[At].Wet = Wet[At];
                }
            }

            List<WaterVertex> Vertices = new List<WaterVertex>();
            List<int> Indices = new List<int>();
            for (int Y = 0; Y < H - 1; Y++)
            {
                for (int X = 0; X < W - 1; X++)
                {
                    ShoreNode A = Nodes[Y * W + X];
                    ShoreNode B = Nodes[Y * W + X + 1];
                    ShoreNode C = Nodes[(Y + 1) * W + X + 1];
                    ShoreNode D = Nodes[(Y + 1) * W + X];
                    int Mask = (A.Wet ? 1 : 0) | (B.Wet ? 2 : 0) | (C.Wet ? 4 : 0) | (D.Wet ? 8 : 0);
                    if (Mask == 0)
                    {
                        continue;
                    }

                    EmitCell(Vertices, Indices, Volumes, Volume, A, B, C, D, Mask);
                }
            }

            if (Vertices.Count < 3 || Indices.Count < 3)
            {
                return null;
            }

            return Pack(Vertices, Indices);
        }


        static WaterSheet Disc(WaterVolume Pool, IList<WaterVolume> Volumes)
        {
            int Rings = Math.Max(1, (int)Math.Ceiling(Pool.Radius / WaterPlace.Quad));
            int Segments = Math.Max(8, (int)Math.Ceiling(Math.PI * 2f * Pool.Radius / WaterPlace.Quad));
            List<WaterVertex> Vertices = new List<WaterVertex>();
            List<int> Indices = new List<int>();
            Vertices.Add(Fill(Volumes, Pool.Start.X, Pool.Start.Y, Pool.Start.Z, 0.85f));
            for (int Ring = 1; Ring <= Rings; Ring++)
            {
                float Reach = Pool.Radius * Ring / Rings;
                float Alpha = Ring == Rings ? 0.15f : 0.85f;
                for (int Segment = 0; Segment < Segments; Segment++)
                {
                    float Angle = MathHelper.TwoPi * Segment / Segments;
                    float X = Pool.Start.X + (float)Math.Cos(Angle) * Reach;
                    float Y = Pool.Start.Y + (float)Math.Sin(Angle) * Reach;
                    Vertices.Add(Fill(Volumes, X, Y, Pool.Start.Z, Alpha));
                }
            }

            for (int Ring = 1; Ring <= Rings; Ring++)
            {
                for (int Segment = 0; Segment < Segments; Segment++)
                {
                    int Next = (Segment + 1) % Segments;
                    int Outer = 1 + (Ring - 1) * Segments + Segment;
                    int OuterNext = 1 + (Ring - 1) * Segments + Next;
                    if (Ring == 1)
                    {
                        Indices.Add(0);
                        Indices.Add(Outer);
                        Indices.Add(OuterNext);
                    }
                    else
                    {
                        int Inner = 1 + (Ring - 2) * Segments + Segment;
                        int InnerNext = 1 + (Ring - 2) * Segments + Next;
                        Indices.Add(Inner);
                        Indices.Add(Outer);
                        Indices.Add(OuterNext);
                        Indices.Add(Inner);
                        Indices.Add(OuterNext);
                        Indices.Add(InnerNext);
                    }
                }
            }

            return Pack(Vertices, Indices);
        }

        static WaterSheet Ribbon(WaterVolume River, IList<WaterVolume> Volumes)
        {
            Vector2 Along = new Vector2(River.End.X - River.Start.X, River.End.Y - River.Start.Y);
            float Length = Along.Length();
            if (Length < 0.001f)
            {
                return new WaterSheet();
            }

            Along /= Length;
            Vector2 Side = new Vector2(-Along.Y, Along.X);
            int Steps = Math.Max(1, (int)Math.Ceiling(Length / WaterPlace.Quad));
            List<WaterVertex> Vertices = new List<WaterVertex>();
            List<int> Indices = new List<int>();
            for (int Step = 0; Step <= Steps; Step++)
            {
                float T = Step / (float)Steps;
                Vector2 Center = new Vector2(MathHelper.Lerp(River.Start.X, River.End.X, T), MathHelper.Lerp(River.Start.Y, River.End.Y, T));
                float Height = MathHelper.Lerp(River.Start.Z, River.End.Z, T);
                for (int Across = 0; Across < 3; Across++)
                {
                    float Offset = (Across - 1) * River.Width * 0.5f;
                    float Alpha = Across == 1 ? 0.85f : 0.15f;
                    Vertices.Add(Fill(Volumes, Center.X + Side.X * Offset, Center.Y + Side.Y * Offset, Height, Alpha));
                }
            }

            for (int Step = 0; Step < Steps; Step++)
            {
                int Row = Step * 3;
                Indices.Add(Row);
                Indices.Add(Row + 3);
                Indices.Add(Row + 4);
                Indices.Add(Row);
                Indices.Add(Row + 4);
                Indices.Add(Row + 1);
                Indices.Add(Row + 1);
                Indices.Add(Row + 4);
                Indices.Add(Row + 5);
                Indices.Add(Row + 1);
                Indices.Add(Row + 5);
                Indices.Add(Row + 2);
            }

            return Pack(Vertices, Indices);
        }

        struct ShoreNode
        {
            public float X;
            public float Y;
            public float Depth;
            public bool Wet;
        }


        static WaterSheet Contour(WaterVolume Volume, HeightField Ground, IList<WaterVolume> Volumes)
        {
            if (Volume == null || Ground == null)
            {
                return null;
            }

            float MinX;
            float MinY;
            float MaxX;
            float MaxY;
            if (!ShoreBounds(Volume, Ground, out MinX, out MinY, out MaxX, out MaxY))
            {
                return null;
            }

            float Step = 4f;
            float SpanX = MaxX - MinX;
            float SpanY = MaxY - MinY;
            if (SpanX < Step || SpanY < Step)
            {
                return null;
            }

            while (SpanX / Step * (SpanY / Step) > 4096f)
            {
                Step *= 2f;
            }

            int X0 = (int)Math.Floor(MinX / Step);
            int Y0 = (int)Math.Floor(MinY / Step);
            int X1 = (int)Math.Ceiling(MaxX / Step);
            int Y1 = (int)Math.Ceiling(MaxY / Step);
            int W = X1 - X0 + 1;
            int H = Y1 - Y0 + 1;
            if (W < 2 || H < 2 || W > 96 || H > 96)
            {
                return null;
            }

            ShoreNode[] Nodes = new ShoreNode[W * H];
            bool[] Seed = new bool[Nodes.Length];
            float OriginX = X0 * Step;
            float OriginY = Y0 * Step;
            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int At = Y * W + X;
                    float WorldX = OriginX + X * Step;
                    float WorldY = OriginY + Y * Step;
                    Nodes[At].X = WorldX;
                    Nodes[At].Y = WorldY;
                    float Height;
                    float SlopeX;
                    float SlopeY;
                    if (!Ground.Sample(WorldX, WorldY, out Height, out SlopeX, out SlopeY))
                    {
                        Nodes[At].Depth = -1f;
                        continue;
                    }

                    Nodes[At].Depth = Volume.SurfaceAt(WorldX, WorldY) - Height;
                    if (Nodes[At].Depth > 0.05f && ShoreSeed(Volume, WorldX, WorldY))
                    {
                        Seed[At] = true;
                    }
                }
            }

            bool Limit = Volume.Kind == WaterKind.River;
            float Reach = Math.Max(Volume.Width, 4f) * 0.55f;
            int ShoreHops = Math.Max(1, (int)Math.Ceiling(32f / Step));
            int[] Hops = new int[Nodes.Length];
            for (int i = 0; i < Hops.Length; i++)
            {
                Hops[i] = -1;
            }

            Queue<int> Open = new Queue<int>();
            for (int i = 0; i < Seed.Length; i++)
            {
                if (!Seed[i])
                {
                    continue;
                }

                Nodes[i].Wet = true;
                Hops[i] = 0;
                Open.Enqueue(i);
            }

            if (Open.Count == 0)
            {
                return null;
            }

            while (Open.Count > 0)
            {
                int At = Open.Dequeue();
                if (!Limit && Hops[At] >= ShoreHops)
                {
                    continue;
                }

                int X = At % W;
                int Y = At / W;
                for (int Side = 0; Side < 4; Side++)
                {
                    int NX = X + (Side == 0 ? 1 : Side == 1 ? -1 : 0);
                    int NY = Y + (Side == 2 ? 1 : Side == 3 ? -1 : 0);
                    if (NX < 0 || NY < 0 || NX >= W || NY >= H)
                    {
                        continue;
                    }

                    int Next = NY * W + NX;
                    if (Nodes[Next].Wet || Nodes[Next].Depth <= 0.05f)
                    {
                        continue;
                    }

                    if (Limit && Volume.DistanceToCourse(Nodes[Next].X, Nodes[Next].Y) > Reach)
                    {
                        continue;
                    }

                    Nodes[Next].Wet = true;
                    Hops[Next] = Hops[At] + 1;
                    Open.Enqueue(Next);
                }
            }

            List<WaterVertex> Vertices = new List<WaterVertex>();
            List<int> Indices = new List<int>();
            for (int Y = 0; Y < H - 1; Y++)
            {
                for (int X = 0; X < W - 1; X++)
                {
                    ShoreNode A = Nodes[Y * W + X];
                    ShoreNode B = Nodes[Y * W + X + 1];
                    ShoreNode C = Nodes[(Y + 1) * W + X + 1];
                    ShoreNode D = Nodes[(Y + 1) * W + X];
                    int Mask = (A.Wet ? 1 : 0) | (B.Wet ? 2 : 0) | (C.Wet ? 4 : 0) | (D.Wet ? 8 : 0);
                    if (Mask == 0)
                    {
                        continue;
                    }

                    EmitCell(Vertices, Indices, Volumes, Volume, A, B, C, D, Mask);
                }
            }

            if (Vertices.Count < 3 || Indices.Count < 3)
            {
                return null;
            }

            return Pack(Vertices, Indices);
        }


        static bool ShoreBounds(WaterVolume Volume, HeightField Ground, out float MinX, out float MinY, out float MaxX, out float MaxY)
        {
            MinX = float.MaxValue;
            MinY = float.MaxValue;
            MaxX = float.MinValue;
            MaxY = float.MinValue;
            if (Volume.Kind == WaterKind.Lake && Ground != null && Ground.SamplesX >= 2 && Ground.SamplesZ >= 2 && Ground.Spacing > 0.01f)
            {
                MinX = 0f;
                MinY = 0f;
                MaxX = (Ground.SamplesX - 1) * Ground.Spacing;
                MaxY = (Ground.SamplesZ - 1) * Ground.Spacing;
                return MaxX > MinX && MaxY > MinY;
            }

            if (Volume.Kind == WaterKind.Lake && Volume.LakeCells != null && Volume.Cell > 0.01f)
            {
                for (int i = 0; i < Volume.LakeCells.Length; i++)
                {
                    float X = (Volume.LakeCells[i] & 65535) * Volume.Cell;
                    float Y = (Volume.LakeCells[i] >> 16) * Volume.Cell;
                    MinX = Math.Min(MinX, X);
                    MinY = Math.Min(MinY, Y);
                    MaxX = Math.Max(MaxX, X);
                    MaxY = Math.Max(MaxY, Y);
                }

                float Pad = Volume.Cell * 2f;
                MinX -= Pad;
                MinY -= Pad;
                MaxX += Pad;
                MaxY += Pad;
                return MaxX > MinX && MaxY > MinY;
            }

            float Reach = Volume.Kind == WaterKind.River ? Volume.Width : Math.Max(Volume.Radius, Volume.Width);
            MinX = Math.Min(Volume.Start.X, Volume.End.X) - Reach;
            MinY = Math.Min(Volume.Start.Y, Volume.End.Y) - Reach;
            MaxX = Math.Max(Volume.Start.X, Volume.End.X) + Reach;
            MaxY = Math.Max(Volume.Start.Y, Volume.End.Y) + Reach;
            return Reach > 1f;
        }

        static bool ShoreSeed(WaterVolume Volume, float X, float Y)
        {
            if (Volume.Kind == WaterKind.River)
            {
                return Volume.DistanceToCourse(X, Y) <= Math.Max(4f, Volume.Width * 0.25f);
            }

            if (Volume.LakeCells == null || Volume.Cell <= 0.01f)
            {
                float DX = X - Volume.Start.X;
                float DY = Y - Volume.Start.Y;
                return DX * DX + DY * DY <= Volume.Radius * Volume.Radius;
            }

            float Reach = Volume.Cell * 0.55f;
            float ReachSq = Reach * Reach;
            for (int i = 0; i < Volume.LakeCells.Length; i++)
            {
                float CX = (Volume.LakeCells[i] & 65535) * Volume.Cell;
                float CY = (Volume.LakeCells[i] >> 16) * Volume.Cell;
                float DX = X - CX;
                float DY = Y - CY;
                if (DX * DX + DY * DY <= ReachSq)
                {
                    return true;
                }
            }

            return false;
        }

        static void EmitCell(List<WaterVertex> Vertices, List<int> Indices, IList<WaterVolume> Volumes, WaterVolume Volume, ShoreNode A, ShoreNode B, ShoreNode C, ShoreNode D, int Mask)
        {
            Vector2 P0 = new Vector2(A.X, A.Y);
            Vector2 P1 = new Vector2(B.X, B.Y);
            Vector2 P2 = new Vector2(C.X, C.Y);
            Vector2 P3 = new Vector2(D.X, D.Y);
            Vector2 Bottom = Cross(P0, P1, A.Depth, B.Depth);
            Vector2 Right = Cross(P1, P2, B.Depth, C.Depth);
            Vector2 Top = Cross(P3, P2, D.Depth, C.Depth);
            Vector2 Left = Cross(P0, P3, A.Depth, D.Depth);
            switch (Mask)
            {
                case 1:
                    Tri(Vertices, Indices, Volumes, Volume, P0, Bottom, Left, false, true, true);
                    break;
                case 2:
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, P1, Right, true, false, true);
                    break;
                case 4:
                    Tri(Vertices, Indices, Volumes, Volume, Right, P2, Top, true, false, true);
                    break;
                case 8:
                    Tri(Vertices, Indices, Volumes, Volume, Left, Top, P3, true, true, false);
                    break;
                case 3:
                    Tri(Vertices, Indices, Volumes, Volume, P0, P1, Right, false, false, true);
                    Tri(Vertices, Indices, Volumes, Volume, P0, Right, Left, false, true, true);
                    break;
                case 5:
                    Tri(Vertices, Indices, Volumes, Volume, P0, Bottom, Left, false, true, true);
                    Tri(Vertices, Indices, Volumes, Volume, Right, P2, Top, true, false, true);
                    break;
                case 6:
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, P1, P2, true, false, false);
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, P2, Top, true, false, true);
                    break;
                case 7:
                    Tri(Vertices, Indices, Volumes, Volume, P0, P1, P2, false, false, false);
                    Tri(Vertices, Indices, Volumes, Volume, P0, P2, Top, false, false, true);
                    Tri(Vertices, Indices, Volumes, Volume, P0, Top, Left, false, true, true);
                    break;
                case 9:
                    Tri(Vertices, Indices, Volumes, Volume, P0, Bottom, P3, false, true, false);
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, Top, P3, true, true, false);
                    break;
                case 10:
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, P1, Right, true, false, true);
                    Tri(Vertices, Indices, Volumes, Volume, Left, Top, P3, true, true, false);
                    break;
                case 11:
                    Tri(Vertices, Indices, Volumes, Volume, P0, P1, Right, false, false, true);
                    Tri(Vertices, Indices, Volumes, Volume, P0, Right, Top, false, true, true);
                    Tri(Vertices, Indices, Volumes, Volume, P0, Top, P3, false, true, false);
                    break;
                case 12:
                    Tri(Vertices, Indices, Volumes, Volume, Left, Right, P2, true, true, false);
                    Tri(Vertices, Indices, Volumes, Volume, Left, P2, P3, true, false, false);
                    break;
                case 13:
                    Tri(Vertices, Indices, Volumes, Volume, P0, Bottom, Right, false, true, true);
                    Tri(Vertices, Indices, Volumes, Volume, P0, Right, P2, false, true, false);
                    Tri(Vertices, Indices, Volumes, Volume, P0, P2, P3, false, false, false);
                    break;
                case 14:
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, P1, P2, true, false, false);
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, P2, P3, true, false, false);
                    Tri(Vertices, Indices, Volumes, Volume, Bottom, P3, Left, true, false, true);
                    break;
                case 15:
                    Tri(Vertices, Indices, Volumes, Volume, P0, P1, P2, false, false, false);
                    Tri(Vertices, Indices, Volumes, Volume, P0, P2, P3, false, false, false);
                    break;
            }
        }

        static Vector2 Cross(Vector2 A, Vector2 B, float DepthA, float DepthB)
        {
            float Den = DepthA - DepthB;
            float T = Math.Abs(Den) < 0.0001f ? 0.5f : DepthA / Den;
            return Vector2.Lerp(A, B, MathHelper.Clamp(T, 0f, 1f));
        }

        static void Tri(List<WaterVertex> Vertices, List<int> Indices, IList<WaterVolume> Volumes, WaterVolume Volume, Vector2 A, Vector2 B, Vector2 C, bool EdgeA, bool EdgeB, bool EdgeC)
        {
            int Base = Vertices.Count;
            Vertices.Add(Fill(Volumes, A.X, A.Y, Volume.SurfaceAt(A.X, A.Y), EdgeA ? 0.15f : 0.85f));
            Vertices.Add(Fill(Volumes, B.X, B.Y, Volume.SurfaceAt(B.X, B.Y), EdgeB ? 0.15f : 0.85f));
            Vertices.Add(Fill(Volumes, C.X, C.Y, Volume.SurfaceAt(C.X, C.Y), EdgeC ? 0.15f : 0.85f));
            Indices.Add(Base);
            Indices.Add(Base + 1);
            Indices.Add(Base + 2);
        }


        static WaterVertex Fill(IList<WaterVolume> Volumes, float X, float Y, float Height, float Alpha)
        {
            Vector3 Point = new Vector3(X, Y, Height);
            WaterLookStyle Style = WaterPlace.LookAt(Volumes, Point);
            WaterVertex Vertex = new WaterVertex();
            Vertex.Position = new Vector3(X, Height, Y);
            Vertex.WorldXZ = new Vector2(X, Y);
            Vertex.Flow = WaterPlace.FlowAt(Volumes, Point);
            Vertex.Shallow = Style.Shallow;
            Vertex.Shallow.W = Alpha;
            Vertex.Deep = Style.Deep;
            Vertex.Deep.W = Style.Specular;
            Vertex.Strengths = Style.Strengths;
            return Vertex;
        }


        static WaterSheet Pack(List<WaterVertex> Vertices, List<int> Indices)
        {
            WaterSheet Sheet = new WaterSheet();
            Sheet.Vertices = Vertices.ToArray();
            Sheet.Indices = Indices.ToArray();
            return Sheet;
        }
    }
}
