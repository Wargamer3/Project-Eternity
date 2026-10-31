using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.LightningSystem;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class WeatherDraw
    {
        const double StrikeHold = 1.2d;
        const double StrikeWait = 3d;

        struct Drop
        {
            public Vector3 Position;
            public float Speed;
            public Vector2 Drift;
            public bool Live;
            public bool Drip;
        }

        struct Ring
        {
            public Vector3 At;
            public float Age;
            public bool Live;
        }

        const int DropCap = 1100;
        const int DripCap = 48;
        const int RingCap = 96;
        const int RunoffCap = 64;
        const float RingLife = 0.9f;
        const int SheetGrid = 14;
        const float RainMargin = 150f;

        static readonly List<Drop> Drops = new List<Drop>();
        static readonly List<Drop> Drips = new List<Drop>();
        static readonly List<Ring> Rings = new List<Ring>();
        static readonly List<Drop> Runoff = new List<Drop>();
        static int RoofRings;
        static readonly PlayerCharacter[] WetPeople = new PlayerCharacter[16];
        static readonly Random Rng = new Random(1);
        static BasicEffect ColorEffect;
        static BasicEffect Textured;
        static Effect RippleEffect;
        static Texture2D RippleTexture;
        static Texture2D RainLine;
        static Texture2D Droplets;
        static RenderTarget2D RippleTarget;
        const int MaskSize = 128;
        static Effect StencilEffect;
        static Texture2D PlaceMask;
        static Color[] MaskPixels;
        static BasicEffect MaskEffect;

        struct RainSheet
        {
            public Camera3D Camera;
            public RenderTarget2D Stencil;
            public VertexPositionTexture[] Ground;
            public VertexPositionTexture[] Roofs;
            public bool Ready;
        }

        static readonly RainSheet[] PaneSheets = new RainSheet[4];
        static int SheetCount;
        static LightningBolt Bolt;
        static FMODSound[] Thunder;
        static bool AssetsTried;
        static bool RippleBroken;
        static double RippleTime;
        static double Show;
        static double Wait = StrikeWait;
        static bool FreshBolt;
        static bool StrikeBegan;
        static float FlashReach = 0.22f;
        static double ThunderLeft;
        static float ThunderGain;
        static Vector3 BoltStart;
        static Vector3 BoltEnd;
        static float Bright = 1f;
        static VertexPositionColorTexture[] StreakVerts;

        public static float Light
        {
            get { return Bright; }
        }

        public static void Reset()
        {
            RippleTime = 0d;
            Show = 0d;
            Wait = StrikeWait;
            FreshBolt = false;
            StrikeBegan = false;
            FlashReach = 0.22f;
            ThunderLeft = 0d;
            ThunderGain = 0f;
            Bright = 1f;
            Drips.Clear();
            Rings.Clear();
            Runoff.Clear();
            RoofRings = 0;
        }

        public static float RainAt(WorldClock Clock, MapInfo Root, float WorldX, float WorldZ)
        {
            if (Clock == null)
            {
                return 0f;
            }

            Outlook Sky = Forecast.Look(Clock, Root, WorldX, WorldZ);
            if (Forecast.Falls(Sky.Period, false))
            {
                return 1f;
            }

            if (Forecast.Falls(Sky.Dying, false) && Sky.Strength > 0f)
            {
                return Sky.Strength;
            }

            return 0f;
        }

        public static float ShellCover(WorldClock Clock, MapInfo Root, float WorldX, float WorldZ)
        {
            if (Dry(WorldX, WorldZ))
            {
                return 0f;
            }

            return RainAt(Clock, Root, WorldX, WorldZ);
        }

        public static float ShellCover(NavMapGameManager World, float WorldX, float WorldZ)
        {
            if (World == null)
            {
                return 0f;
            }

            return ShellCover(World.Clock, World.RootMapContainer, WorldX, WorldZ);
        }

        public static bool RoofInRain(WorldClock Clock, MapInfo Root, MapInfo Chunk, BuildingItem Box)
        {
            if (Clock == null || Chunk == null || Chunk.MapMapContainer == null || Box == null || Box.Kind != BuildingKind.Floor)
            {
                return false;
            }

            if (Motives.Sheltered(Chunk, Box.Position.X, Box.Position.Y, Box.Floor))
            {
                return false;
            }

            Vector3 Shift = ChunkOrigin.Shift(Chunk);
            return RainAt(Clock, Root, Box.Position.X + Shift.X, Box.Position.Y + Shift.Z) > 0f;
        }

        public static bool StrikeAllowed(WorldClock Clock, MapInfo Root, float WorldX, float WorldZ)
        {
            if (ShellCover(Clock, Root, WorldX, WorldZ) <= 0f)
            {
                return false;
            }

            Outlook Sky = Forecast.Look(Clock, Root, WorldX, WorldZ);
            return Forecast.Storm(Sky.Period) || (Forecast.Storm(Sky.Dying) && Sky.Strength > 0f);
        }

        public static float Brightness(double Visible, double Length)
        {
            if (Visible <= 0d || Length <= 0d)
            {
                return 1f;
            }

            double Age = Length - Visible;
            float Spike = 0f;
            if (Age < 0.07d)
            {
                Spike = 1f;
            }
            else if (Age < 0.13d)
            {
                Spike = 0.08f;
            }
            else if (Age < 0.32d)
            {
                Spike = 0.42f * (1f - (float)((Age - 0.13d) / 0.19d));
            }

            return 1f + Spike * FlashReach;
        }

        public static void Step(bool Allowed, double Seconds)
        {
            if (Seconds < 0d)
            {
                Seconds = 0d;
            }

            if (Show > 0d)
            {
                if (!Allowed)
                {
                    Show = 0d;
                    Bright = 1f;
                    FreshBolt = false;
                    StrikeBegan = false;
                    return;
                }

                Show -= Seconds;
                if (Show < 0d)
                {
                    Show = 0d;
                }

                Bright = Brightness(Show, StrikeHold);
                return;
            }

            Bright = 1f;
            if (!Allowed)
            {
                return;
            }

            Wait -= Seconds;
            if (Wait > 0d)
            {
                return;
            }

            Show = StrikeHold;
            Wait = 6d + Rng.NextDouble() * 6d;
            FreshBolt = true;
            StrikeBegan = true;
            Bright = Brightness(Show, StrikeHold);
        }

        public static int OpenSectors(WorldClock Clock, MapInfo Root, Vector3 Eye, float Radius, int Sectors)
        {
            if (Sectors < 1)
            {
                return 0;
            }

            int Open = 0;
            for (int Sector = 0; Sector < Sectors; Sector++)
            {
                float Angle = MathHelper.TwoPi * (Sector + 0.5f) / Sectors;
                float X = Eye.X + (float)Math.Cos(Angle) * Radius;
                float Z = Eye.Z + (float)Math.Sin(Angle) * Radius;
                if (ShellCover(Clock, Root, X, Z) > 0f)
                {
                    Open++;
                }
            }

            return Open;
        }

        public static void Simulate(NavMapGameManager World, IList<PlayerOverseer> Players, double Seconds)
        {
            if (World == null || Seconds <= 0d)
            {
                return;
            }

            Ensure(World);
            RoofRings = 0;
            bool Allowed = false;
            Vector3 Eye = Vector3.Zero;
            Vector3 Look = new Vector3(0f, 0f, 1f);
            bool HaveEye = false;
            float Pace = 0.45f;
            Camera3D ViewCamera = null;
            if (Players != null)
            {
                for (int p = 0; p < Players.Count; p++)
                {
                    PlayerOverseer Player = Players[p];
                    if (Player == null || Player.Camera == null)
                    {
                        continue;
                    }

                    Vector3 At = Player.Camera.CameraPosition3D;
                    if (!HaveEye)
                    {
                        Eye = At;
                        Look = FlatLook(Player.Camera);
                        ViewCamera = Player.Camera;
                        HaveEye = true;
                    }

                    Outlook Sky = Forecast.Look(World, At.X, At.Z);
                    TimePeriod Falling = Forecast.Falls(Sky.Period, false) ? Sky.Period : Sky.Dying;
                    if (Falling != null && Falling.WeatherType != TimePeriod.WeatherTypes.SmallRain)
                    {
                        Pace = 1.1f;
                    }

                    if (!Allowed && StrikeAllowed(World.Clock, World.RootMapContainer, At.X, At.Z))
                    {
                        Allowed = true;
                        Eye = At;
                        Look = FlatLook(Player.Camera);
                        ViewCamera = Player.Camera;
                    }
                }
            }

            if (HaveEye && ViewCamera != null)
            {
                float Near;
                float Far;
                float Spread;
                ViewBand(ViewCamera, out Near, out Far, out Spread);
                Vector3 Right = new Vector3(Look.Z, 0f, -Look.X);
                float StepTime = (float)Seconds;
                int Cap = WantedDrops();
                if (Drops.Count > Cap)
                {
                    Drops.RemoveRange(Cap, Drops.Count - Cap);
                }

                for (int i = 0; i < Drops.Count; i++)
                {
                    Drop Drop = Drops[i];
                    if (!Drop.Live)
                    {
                        continue;
                    }

                    if (Drop.Speed < 100f)
                    {
                        Drop.Speed *= 3.2f;
                    }

                    Drop.Drift = DriftOf(Drop.Speed);
                    float Before = Drop.Position.Y;
                    Drop.Position.X += Drop.Drift.X * StepTime;
                    Drop.Position.Y -= Drop.Speed * StepTime;
                    Drop.Position.Z += Drop.Drift.Y * StepTime;
                    float Ground = GroundAt(Drop.Position.X, Drop.Position.Z);
                    float Roof;
                    if (!InBand(Eye, Look, Right, Drop.Position, Near, Far, Spread))
                    {
                        Drop.Live = false;
                    }
                    else if (RoofBetween(Drop.Position.X, Drop.Position.Z, Before, Drop.Position.Y, out Roof))
                    {
                        Splash(Drop.Position.X, Roof, Drop.Position.Z);
                        Drop.Live = false;
                    }
                    else if (CeilingAbove(Drop.Position.X, Drop.Position.Z, 0) && Drop.Position.Y < RoofHeight(Drop.Position.X, Drop.Position.Z))
                    {
                        Drop.Live = false;
                    }
                    else if (Drop.Position.Y < Ground)
                    {
                        if (!CeilingAbove(Drop.Position.X, Drop.Position.Z, 0))
                        {
                            Splash(Drop.Position.X, Ground, Drop.Position.Z);
                        }

                        Drop.Live = false;
                    }

                    Drops[i] = Drop;
                }

                int Misses = 0;
                for (int i = 0; i < Drops.Count && Misses <= 48; i++)
                {
                    if (Drops[i].Live)
                    {
                        continue;
                    }

                    Drop Made;
                    if (!Place(World, Eye, Look, Right, Near, Far, Spread, out Made))
                    {
                        Misses++;
                        continue;
                    }

                    Drops[i] = Made;
                }

                while (Drops.Count < Cap && Misses <= 48)
                {
                    Drop Made;
                    if (!Place(World, Eye, Look, Right, Near, Far, Spread, out Made))
                    {
                        Misses++;
                        continue;
                    }

                    Drops.Add(Made);
                }
            }

            Shed(World, (float)Seconds);
            Pour(World, ViewCamera, (float)Seconds);
            AgeRings((float)Seconds);
            if (ThunderLeft > 0d)
            {
                ThunderLeft -= Seconds;
                if (ThunderLeft <= 0d)
                {
                    ThunderLeft = 0d;
                    PlayThunder(ThunderGain);
                }
            }

            RippleTime += Seconds * (Pace > 0.6f ? 1d : 0.45d);
            bool WasQuiet = Show <= 0d;
            Step(Allowed, Seconds);
            if (WasQuiet && Show > 0d && StrikeBegan)
            {
                StrikeBegan = false;
                float Dist;
                Vector3 End;
                if (!PickStrike(World, Eye, out End, out Dist))
                {
                    Show = 0d;
                    Bright = 1f;
                    FreshBolt = false;
                    Wait = 2d;
                }
                else
                {
                    Vector2 Gust = Climate.Now.Wind;
                    BoltEnd = End;
                    BoltStart = End + new Vector3(Gust.X * 2f, 240f, Gust.Y * 2f);
                    FlashReach = MathHelper.Clamp(1.05f - Dist / 560f, 0.1f, 0.85f);
                    Bright = Brightness(Show, StrikeHold);
                    if (Bolt != null && Bolt.Descriptor != null)
                    {
                        Bolt.Descriptor.BaseWidth = Dist < 240f ? 1.7f : 0.35f;
                    }

                    ThunderGain = MathHelper.Clamp(1.2f - Dist / 700f, 0.15f, 1f);
                    if (Dry(Eye.X, Eye.Z))
                    {
                        ThunderGain *= 0.25f;
                    }

                    ThunderLeft = Math.Max(0.08d, Dist / 340d);
                }
            }
        }

        public static void PrepareFrame(CustomSpriteBatch Batch, NavMapGameManager World)
        {
            if (Batch == null || World == null)
            {
                return;
            }

            GraphicsDevice Device = Batch.GraphicsDevice;
            Viewport Full = Device.Viewport;
            try
            {
                Ensure(World);
            }
            finally
            {
                Device.SetRenderTarget(null);
                Device.Viewport = Full;
            }
        }

        public static void All(CustomSpriteBatch Batch, Camera3D Camera, NavMapGameManager World)
        {
            if (Batch == null || Camera == null || World == null)
            {
                return;
            }

            GraphicsDevice Device = Batch.GraphicsDevice;
            Viewport Pane = Device.Viewport;
            Batch.End();
            try
            {
                Device.Viewport = Pane;
                DrawVolumes(Device, Camera, World);
                DrawBolt(Device, Camera, Pane);
            }
            finally
            {
                Device.Viewport = Pane;
                Batch.Begin();
            }
        }

        static void DrawVolumes(GraphicsDevice Device, Camera3D Camera, NavMapGameManager World)
        {
            int StreakCount = Streaks(Camera);
            List<VertexPositionColor> ColorVertices = new List<VertexPositionColor>();
            Sheets(ColorVertices, Camera, World);
            bool Pouring = Droplets != null && Climate.Now.Rain > 0.05f;
            if (StreakCount < 3 && ColorVertices.Count < 3 && !RingReady() && !Pouring && !RunoffLive())
            {
                return;
            }

            Viewport Pane = Device.Viewport;
            float Aspect = Pane.Height > 0 ? Pane.Width / (float)Pane.Height : 1f;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            BlendState PreviousBlend = Device.BlendState;
            DepthStencilState PreviousDepth = Device.DepthStencilState;
            RasterizerState PreviousRaster = Device.RasterizerState;
            SamplerState PreviousSampler = Device.SamplerStates[0];
            Device.DepthStencilState = DepthStencilState.DepthRead;
            Device.RasterizerState = RasterizerState.CullNone;
            Device.SamplerStates[0] = SamplerState.LinearWrap;
            BasicEffect Effect = TextureEffect(Device);
            Effect.View = Camera.View;
            Effect.Projection = Projection;
            Effect.World = Matrix.Identity;
            DrawRings(Device, Camera, Projection);

            if (StreakCount >= 3 && RainLine != null)
            {
                Device.BlendState = BlendState.Additive;
                Device.SamplerStates[0] = SamplerState.LinearClamp;
                Effect.Texture = RainLine;
                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserPrimitives(PrimitiveType.TriangleList, StreakVerts, 0, StreakCount / 3);
                }
            }

            if (Pouring || RunoffLive())
            {
                DrawPour(Device, Camera, Projection, World, Effect);
            }

            if (ColorVertices.Count >= 3)
            {
                Device.BlendState = BlendState.AlphaBlend;
                BasicEffect Flat = FlatEffect(Device);
                Flat.View = Camera.View;
                Flat.Projection = Projection;
                Flat.World = Matrix.Identity;
                foreach (EffectPass Pass in Flat.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserPrimitives(PrimitiveType.TriangleList, ColorVertices.ToArray(), 0, ColorVertices.Count / 3);
                }
            }

            Device.BlendState = PreviousBlend;
            Device.DepthStencilState = PreviousDepth;
            Device.RasterizerState = PreviousRaster;
            Device.SamplerStates[0] = PreviousSampler;
        }

        static int Streaks(Camera3D Camera)
        {
            if (RainLine == null)
            {
                return 0;
            }

            if (StreakVerts == null || StreakVerts.Length < (DropCap + DripCap) * 6)
            {
                StreakVerts = new VertexPositionColorTexture[(DropCap + DripCap) * 6];
            }

            Vector3 Eye = Camera.CameraPosition3D;
            int Count = 0;
            float WideRatio = RainLine.Height / (float)Math.Max(RainLine.Width, 1);
            for (int i = 0; i < Drops.Count; i++)
            {
                Count = WriteStreak(Drops[i], Eye, Count, WideRatio);
            }

            for (int i = 0; i < Drips.Count; i++)
            {
                Count = WriteStreak(Drips[i], Eye, Count, WideRatio);
            }

            return Count;
        }

        static int WriteStreak(Drop Drop, Vector3 Eye, int Count, float WideRatio)
        {
            if (!Drop.Live || Count + 6 > StreakVerts.Length)
            {
                return Count;
            }

            Vector3 Center = Drop.Position;
            float Dist = (Center - Eye).Length();
            float HalfLong;
            float Thin;
            int Alpha;
            if (Drop.Drip)
            {
                HalfLong = MathHelper.Lerp(1.4f, 3.1f, MathHelper.Clamp((Dist - 8f) / 40f, 0f, 1f));
                Thin = 0.55f;
                Alpha = 128;
            }
            else
            {
                HalfLong = MathHelper.Lerp(0.7f, 2.4f, MathHelper.Clamp((Dist - 6f) / 36f, 0f, 1f));
                Thin = Dist < 28f ? 0.4f : 1f;
                Alpha = Dist < 18f ? 50 : 86;
                if (Show > 0.05d)
                {
                    Alpha += 28;
                }
            }

            Color Rain = Color.FromNonPremultiplied(176, 196, 214, Alpha);
            Vector3 Motion = new Vector3(Drop.Drift.X, -Drop.Speed, Drop.Drift.Y);
            if (Motion.LengthSquared() < 1f)
            {
                Motion = new Vector3(0f, -1f, 0f);
            }

            Motion.Normalize();
            Vector3 Along = Motion * HalfLong;
            Vector3 Side = Vector3.Cross(Along, Center - Eye);
            if (Side.LengthSquared() < 0.0001f)
            {
                Side = new Vector3(HalfLong * WideRatio * Thin, 0f, 0f);
            }
            else
            {
                Side.Normalize();
                Side *= HalfLong * WideRatio * Thin;
            }

            Vector3 Top = Center - Along;
            Vector3 Bottom = Center + Along;
            StreakVerts[Count++] = new VertexPositionColorTexture(Top - Side, Rain, new Vector2(0f, 0f));
            StreakVerts[Count++] = new VertexPositionColorTexture(Top + Side, Rain, new Vector2(0f, 1f));
            StreakVerts[Count++] = new VertexPositionColorTexture(Bottom + Side, Rain, new Vector2(1f, 1f));
            StreakVerts[Count++] = new VertexPositionColorTexture(Top - Side, Rain, new Vector2(0f, 0f));
            StreakVerts[Count++] = new VertexPositionColorTexture(Bottom + Side, Rain, new Vector2(1f, 1f));
            StreakVerts[Count++] = new VertexPositionColorTexture(Bottom - Side, Rain, new Vector2(1f, 0f));
            return Count;
        }

        static void DrawBolt(GraphicsDevice Device, Camera3D Camera, Viewport Pane)
        {
            if (Show <= 0d || Bolt == null || Camera == null)
            {
                return;
            }

            if (FreshBolt)
            {
                Bolt.CreateNewLightningBolt(BoltStart, BoltEnd);
                FreshBolt = false;
            }

            float Aspect = Pane.Height > 0 ? Pane.Width / (float)Pane.Height : 1f;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            Bolt.SetWorldViewProjectionMatrix(Matrix.Identity, Camera.View, Projection);
            Bolt.DrawInPlace(Device);
        }

        static bool SheetReady(Camera3D Camera)
        {
            for (int i = 0; i < SheetCount; i++)
            {
                if (PaneSheets[i].Camera == Camera && PaneSheets[i].Ready)
                {
                    return true;
                }
            }

            return false;
        }

        static void ProjectSheets(GraphicsDevice Device, NavMapGameManager World)
        {
            SheetCount = 0;
            if (Device == null || World == null || World.ListLocalPlayer == null || StencilEffect == null || RippleTarget == null)
            {
                return;
            }

            int Live = 0;
            for (int i = 0; i < World.ListLocalPlayer.Count; i++)
            {
                if (World.ListLocalPlayer[i] != null)
                {
                    Live++;
                }
            }

            if (Live == 0)
            {
                return;
            }

            Viewport Full = Device.Viewport;
            Rectangle[] Panes = SplitLayout.Panes(Live, Full.Width, Full.Height, World.SecondPlayerOnLeft);
            int Pane = 0;
            for (int i = 0; i < World.ListLocalPlayer.Count && Pane < Panes.Length && SheetCount < PaneSheets.Length; i++)
            {
                PlayerOverseer Player = World.ListLocalPlayer[i];
                if (Player == null)
                {
                    continue;
                }

                Rectangle Area = Panes[Pane];
                Pane++;
                if (Player.Camera == null || Area.Width < 2 || Area.Height < 2)
                {
                    continue;
                }

                RainSheet Sheet = PaneSheets[SheetCount];
                Sheet.Camera = Player.Camera;
                Sheet.Ready = false;
                Sheet.Ground = null;
                Sheet.Roofs = null;
                if (ProjectSheet(Device, World, Player.Camera, Area, ref Sheet))
                {
                    PaneSheets[SheetCount] = Sheet;
                    SheetCount++;
                }
            }

            Device.SetRenderTarget(null);
            Device.Viewport = Full;
        }

        static bool ProjectSheet(GraphicsDevice Device, NavMapGameManager World, Camera3D Camera, Rectangle Area, ref RainSheet Sheet)
        {
            Vector3 Eye = Camera.CameraPosition3D;
            float Height = Surface(Camera.LookAtTarget.X, Camera.LookAtTarget.Z) + 1.2f;
            Vector3 A;
            Vector3 B;
            Vector3 C;
            Vector3 D;
            ViewGround(Camera, Area, Eye, Height, out A, out B, out C, out D);
            float MinX = Math.Min(Math.Min(A.X, B.X), Math.Min(C.X, D.X));
            float MaxX = Math.Max(Math.Max(A.X, B.X), Math.Max(C.X, D.X));
            float MinZ = Math.Min(Math.Min(A.Z, B.Z), Math.Min(C.Z, D.Z));
            float MaxZ = Math.Max(Math.Max(A.Z, B.Z), Math.Max(C.Z, D.Z));
            if (RainAt(World.Clock, World.RootMapContainer, (MinX + MaxX) * 0.5f, (MinZ + MaxZ) * 0.5f) <= 0f)
            {
                return false;
            }

            if (PlaceMask == null || PlaceMask.IsDisposed || PlaceMask.GraphicsDevice != Device)
            {
                PlaceMask = new Texture2D(Device, MaskSize, MaskSize, false, SurfaceFormat.Color);
            }

            FillMask(World, MinX, MinZ, MaxX, MaxZ);
            float SpanX = Math.Max(MaxX - MinX, 1f);
            float SpanZ = Math.Max(MaxZ - MinZ, 1f);
            float Scroll = (float)RippleTime * 0.15f;
            int SheetVerts = SheetGrid * SheetGrid * 6;
            if (Sheet.Ground == null || Sheet.Ground.Length != SheetVerts)
            {
                Sheet.Ground = new VertexPositionTexture[SheetVerts];
            }

            VertexPositionTexture[] MaskGround = new VertexPositionTexture[SheetVerts];
            Sheet.Roofs = RoofMeshes(World, MinX, MinZ, MaxX, MaxZ, Scroll);
            FillSheet(Sheet.Ground, MaskGround, A, B, C, D, MinX, MinZ, SpanX, SpanZ, Scroll);
            if (Sheet.Stencil == null || Sheet.Stencil.IsDisposed || Sheet.Stencil.Width != Area.Width || Sheet.Stencil.Height != Area.Height || Sheet.Stencil.GraphicsDevice != Device)
            {
                Sheet.Stencil = new RenderTarget2D(Device, Area.Width, Area.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            }

            Viewport Previous = Device.Viewport;
            Device.SetRenderTarget(Sheet.Stencil);
            Device.Viewport = new Viewport(0, 0, Area.Width, Area.Height);
            Device.Clear(Color.Transparent);
            float Aspect = Area.Width / (float)Area.Height;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            BasicEffect Blit = MaskBlit(Device);
            Blit.View = Camera.View;
            Blit.Projection = Projection;
            Blit.World = Matrix.Identity;
            Blit.Texture = PlaceMask;
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullNone;
            Device.SamplerStates[0] = SamplerState.PointClamp;
            foreach (EffectPass Pass in Blit.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.Default;
                Device.RasterizerState = RasterizerState.CullNone;
                Device.SamplerStates[0] = SamplerState.PointClamp;
                Device.DrawUserPrimitives(PrimitiveType.TriangleList, MaskGround, 0, MaskGround.Length / 3);
            }

            if (Sheet.Roofs != null && Sheet.Roofs.Length >= 3 && GameScreen.sprPixel != null)
            {
                Blit.Texture = GameScreen.sprPixel;
                foreach (EffectPass Pass in Blit.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserPrimitives(PrimitiveType.TriangleList, Sheet.Roofs, 0, Sheet.Roofs.Length / 3);
                }
            }

            Device.SetRenderTarget(null);
            Device.Viewport = Previous;
            Sheet.Ready = true;
            return true;
        }

        static void FillSheet(VertexPositionTexture[] Ground, VertexPositionTexture[] Mask, Vector3 A, Vector3 B, Vector3 C, Vector3 D, float MinX, float MinZ, float SpanX, float SpanZ, float Scroll)
        {
            int Write = 0;
            for (int Z = 0; Z < SheetGrid; Z++)
            {
                for (int X = 0; X < SheetGrid; X++)
                {
                    Vector3 P00 = SheetPoint(A, B, C, D, X, Z);
                    Vector3 P10 = SheetPoint(A, B, C, D, X + 1, Z);
                    Vector3 P11 = SheetPoint(A, B, C, D, X + 1, Z + 1);
                    Vector3 P01 = SheetPoint(A, B, C, D, X, Z + 1);
                    Write = SheetCorner(Ground, Mask, Write, P00, MinX, MinZ, SpanX, SpanZ, Scroll);
                    Write = SheetCorner(Ground, Mask, Write, P10, MinX, MinZ, SpanX, SpanZ, Scroll);
                    Write = SheetCorner(Ground, Mask, Write, P11, MinX, MinZ, SpanX, SpanZ, Scroll);
                    Write = SheetCorner(Ground, Mask, Write, P00, MinX, MinZ, SpanX, SpanZ, Scroll);
                    Write = SheetCorner(Ground, Mask, Write, P11, MinX, MinZ, SpanX, SpanZ, Scroll);
                    Write = SheetCorner(Ground, Mask, Write, P01, MinX, MinZ, SpanX, SpanZ, Scroll);
                }
            }
        }

        static Vector3 SheetPoint(Vector3 A, Vector3 B, Vector3 C, Vector3 D, int X, int Z)
        {
            float U = X / (float)SheetGrid;
            float V = Z / (float)SheetGrid;
            Vector3 Left = Vector3.Lerp(A, D, V);
            Vector3 Right = Vector3.Lerp(B, C, V);
            Vector3 At = Vector3.Lerp(Left, Right, U);
            At.Y = GroundAt(At.X, At.Z) + 0.2f;
            return At;
        }

        static int SheetCorner(VertexPositionTexture[] Ground, VertexPositionTexture[] Mask, int Write, Vector3 At, float MinX, float MinZ, float SpanX, float SpanZ, float Scroll)
        {
            Ground[Write] = new VertexPositionTexture(At, new Vector2(At.X * 0.035f, At.Z * 0.035f + Scroll));
            Mask[Write] = new VertexPositionTexture(At, new Vector2((At.X - MinX) / SpanX, (At.Z - MinZ) / SpanZ));
            return Write + 1;
        }

        static void DrawSheet(GraphicsDevice Device, Camera3D Camera, Matrix Projection)
        {
            if (StencilEffect == null || RippleTarget == null)
            {
                return;
            }

            for (int i = 0; i < SheetCount; i++)
            {
                RainSheet Sheet = PaneSheets[i];
                if (Sheet.Camera != Camera || !Sheet.Ready || Sheet.Stencil == null || Sheet.Ground == null)
                {
                    continue;
                }

                StencilEffect.Parameters["ViewProjection"].SetValue(Camera.View * Projection);
                StencilEffect.Parameters["Stencil"].SetValue(Sheet.Stencil);
                StencilEffect.Parameters["RippleMap"].SetValue(RippleTarget);
                Device.BlendState = BlendState.NonPremultiplied;
                Device.DepthStencilState = DepthStencilState.DepthRead;
                Device.RasterizerState = RasterizerState.CullNone;
                Device.SamplerStates[0] = SamplerState.LinearClamp;
                Device.SamplerStates[1] = SamplerState.LinearWrap;
                foreach (EffectPass Pass in StencilEffect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.BlendState = BlendState.NonPremultiplied;
                    Device.DepthStencilState = DepthStencilState.DepthRead;
                    Device.RasterizerState = DecalRaster();
                    Device.DrawUserPrimitives(PrimitiveType.TriangleList, Sheet.Ground, 0, Sheet.Ground.Length / 3);
                    if (Sheet.Roofs != null && Sheet.Roofs.Length >= 3)
                    {
                        Device.DrawUserPrimitives(PrimitiveType.TriangleList, Sheet.Roofs, 0, Sheet.Roofs.Length / 3);
                    }
                }

                return;
            }
        }

        static VertexPositionTexture[] RoofMeshes(NavMapGameManager World, float MinX, float MinZ, float MaxX, float MaxZ, float Scroll)
        {
            IList<MapInfo> Maps = World.UpdatedMaps;
            if (Maps == null)
            {
                return null;
            }

            List<VertexPositionTexture> Vertices = new List<VertexPositionTexture>();
            for (int i = 0; i < Maps.Count; i++)
            {
                MapInfo Chunk = Maps[i];
                if (Chunk == null || Chunk.Buildings == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Chunk);
                for (int b = 0; b < Chunk.Buildings.Count; b++)
                {
                    BuildingItem Box = Chunk.Buildings[b];
                    if (Box == null || Box.Kind != BuildingKind.Floor || Box.Floor <= 0)
                    {
                        continue;
                    }

                    float WorldX = Box.Position.X + Shift.X;
                    float WorldZ = Box.Position.Y + Shift.Z;
                    if (WorldX < MinX || WorldX > MaxX || WorldZ < MinZ || WorldZ > MaxZ)
                    {
                        continue;
                    }

                    Roof(Vertices, Box, Shift, Scroll);
                }
            }

            return Vertices.Count >= 3 ? Vertices.ToArray() : null;
        }

        static void FillMask(NavMapGameManager World, float MinX, float MinZ, float MaxX, float MaxZ)
        {
            if (MaskPixels == null || MaskPixels.Length != MaskSize * MaskSize)
            {
                MaskPixels = new Color[MaskSize * MaskSize];
            }

            for (int i = 0; i < MaskPixels.Length; i++)
            {
                MaskPixels[i] = Color.White;
            }

            float SpanX = Math.Max(MaxX - MinX, 1f);
            float SpanZ = Math.Max(MaxZ - MinZ, 1f);
            IList<MapInfo> Maps = World.UpdatedMaps;
            if (Maps != null)
            {
                for (int i = 0; i < Maps.Count; i++)
                {
                    MapInfo Chunk = Maps[i];
                    if (Chunk == null || Chunk.Buildings == null || Chunk.MapMapContainer == null)
                    {
                        continue;
                    }

                    Vector3 Shift = ChunkOrigin.Shift(Chunk);
                    for (int b = 0; b < Chunk.Buildings.Count; b++)
                    {
                        BuildingItem Box = Chunk.Buildings[b];
                        if (Box != null && Box.Kind == BuildingKind.Floor && Box.Floor > 0)
                        {
                            Punch(Box, Shift, MinX, MinZ, SpanX, SpanZ);
                        }
                    }
                }
            }

            if (PlaceMask == null || PlaceMask.IsDisposed || PlaceMask.GraphicsDevice == null)
            {
                return;
            }

            PlaceMask.SetData(MaskPixels);
        }

        static void Punch(BuildingItem Box, Vector3 Shift, float MinX, float MinZ, float SpanX, float SpanZ)
        {
            float Cose = (float)Math.Cos(Box.Angle);
            float Sine = (float)Math.Sin(Box.Angle);
            float HalfW = Box.Width * 0.5f;
            float HalfL = Box.Length * 0.5f;
            float Left = float.MaxValue;
            float Right = float.MinValue;
            float Top = float.MaxValue;
            float Bottom = float.MinValue;
            for (int Corner = 0; Corner < 4; Corner++)
            {
                float Along = (Corner & 1) == 0 ? -HalfW : HalfW;
                float Across = (Corner & 2) == 0 ? -HalfL : HalfL;
                float X = Box.Position.X + Along * Cose - Across * Sine + Shift.X;
                float Z = Box.Position.Y + Along * Sine + Across * Cose + Shift.Z;
                Left = Math.Min(Left, X);
                Right = Math.Max(Right, X);
                Top = Math.Min(Top, Z);
                Bottom = Math.Max(Bottom, Z);
            }

            int X0 = (int)Math.Floor((Left - MinX) / SpanX * MaskSize);
            int X1 = (int)Math.Ceiling((Right - MinX) / SpanX * MaskSize);
            int Z0 = (int)Math.Floor((Top - MinZ) / SpanZ * MaskSize);
            int Z1 = (int)Math.Ceiling((Bottom - MinZ) / SpanZ * MaskSize);
            if (X0 < 0)
            {
                X0 = 0;
            }

            if (Z0 < 0)
            {
                Z0 = 0;
            }

            if (X1 >= MaskSize)
            {
                X1 = MaskSize - 1;
            }

            if (Z1 >= MaskSize)
            {
                Z1 = MaskSize - 1;
            }

            for (int Z = Z0; Z <= Z1; Z++)
            {
                float WorldZ = MinZ + (Z + 0.5f) / MaskSize * SpanZ;
                float LocalY = WorldZ - Shift.Z;
                for (int X = X0; X <= X1; X++)
                {
                    float WorldX = MinX + (X + 0.5f) / MaskSize * SpanX;
                    if (Box.FootprintContains(WorldX - Shift.X, LocalY))
                    {
                        MaskPixels[Z * MaskSize + X] = Color.Transparent;
                    }
                }
            }
        }

        static void Roof(List<VertexPositionTexture> Vertices, BuildingItem Box, Vector3 Shift, float Scroll)
        {
            float Cose = (float)Math.Cos(Box.Angle);
            float Sine = (float)Math.Sin(Box.Angle);
            float HalfW = Box.Width * 0.5f;
            float HalfL = Box.Length * 0.5f;
            float Height = Box.Position.Z + Box.Height + 0.35f;
            Vector3 Corner(float Along, float Across)
            {
                float MapX = Box.Position.X + Along * Cose - Across * Sine;
                float MapY = Box.Position.Y + Along * Sine + Across * Cose;
                return new Vector3(MapX + Shift.X, Height, MapY + Shift.Z);
            }

            Vector3 A = Corner(-HalfW, -HalfL);
            Vector3 B = Corner(HalfW, -HalfL);
            Vector3 C = Corner(HalfW, HalfL);
            Vector3 D = Corner(-HalfW, HalfL);
            Vector2 Uv(Vector3 At)
            {
                return new Vector2(At.X * 0.035f, At.Z * 0.035f + Scroll);
            }

            Vertices.Add(new VertexPositionTexture(A, Uv(A)));
            Vertices.Add(new VertexPositionTexture(B, Uv(B)));
            Vertices.Add(new VertexPositionTexture(C, Uv(C)));
            Vertices.Add(new VertexPositionTexture(A, Uv(A)));
            Vertices.Add(new VertexPositionTexture(C, Uv(C)));
            Vertices.Add(new VertexPositionTexture(D, Uv(D)));
        }

        static RasterizerState SheetRaster;

        static RasterizerState DecalRaster()
        {
            if (SheetRaster == null)
            {
                SheetRaster = new RasterizerState();
                SheetRaster.CullMode = CullMode.None;
                SheetRaster.DepthBias = -0.001f;
                SheetRaster.SlopeScaleDepthBias = -1f;
            }

            return SheetRaster;
        }

        static BasicEffect MaskBlit(GraphicsDevice Device)
        {
            if (MaskEffect == null || MaskEffect.GraphicsDevice != Device)
            {
                MaskEffect = new BasicEffect(Device);
                MaskEffect.TextureEnabled = true;
                MaskEffect.VertexColorEnabled = false;
                MaskEffect.LightingEnabled = false;
            }

            return MaskEffect;
        }

        static void Sheets(List<VertexPositionColor> Vertices, Camera3D Camera, NavMapGameManager World)
        {
            Vector3 Eye = Camera.CameraPosition3D;
            Vector3 Look = FlatLook(Camera);
            float Near;
            float Far;
            float Spread;
            ViewBand(Camera, out Near, out Far, out Spread);
            float Span = Math.Max(Far - Near, 1f);
            float Sample = Near + Span * 0.55f;
            Outlook Sky = Forecast.Look(World, Eye.X + Look.X * Sample, Eye.Z + Look.Z * Sample);
            float CloudStrength = Forecast.Cloudy(Sky.Period) ? 1f : (Forecast.Cloudy(Sky.Dying) ? Sky.Strength : 0f);
            float FogStrength = Forecast.Foggy(Sky.Period, false) ? 1f : (Forecast.Foggy(Sky.Dying, false) ? Sky.Strength : 0f);
            if (CloudStrength <= 0f && FogStrength <= 0f)
            {
                return;
            }

            Color Cloud = new Color(170, 175, 185) * (0.22f * CloudStrength);
            if (Show > 0.05d && CloudStrength > 0f)
            {
                Cloud = new Color(245, 245, 255) * ((0.34f + (Bright - 1f) * 0.2f) * CloudStrength);
            }

            Color Fog = new Color(190, 195, 200) * (0.14f * FogStrength);
            for (int Band = 0; Band < 3; Band++)
            {
                float Along = Near + Span * (0.48f + Band * 0.18f);
                if (Along > Far)
                {
                    continue;
                }

                float Half = Math.Min(Math.Abs(Along) * Spread * 0.45f, Span * 0.18f);
                float X = Eye.X + Look.X * Along;
                float Z = Eye.Z + Look.Z * Along;
                if (CloudStrength > 0f)
                {
                    Horizontal(Vertices, X, Z, 220f, Half, Cloud);
                }

                if (FogStrength > 0f)
                {
                    Horizontal(Vertices, X, Z, Surface(X, Z) + 36f + Band * 16f, Half, Fog);
                }
            }
        }

        static void PaintRipples(CustomSpriteBatch Batch, GraphicsDevice Device)
        {
            if (RippleBroken || RippleEffect == null || RippleTexture == null || Device == null)
            {
                return;
            }

            bool Begun = false;
            try
            {
                if (RippleTarget == null || RippleTarget.IsDisposed || RippleTarget.GraphicsDevice != Device)
                {
                    RippleTarget = new RenderTarget2D(Device, 256, 256, false, SurfaceFormat.Color, DepthFormat.None);
                }

                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, 256, 256, 0, 0, -1);
                if (Device.UseHalfPixelOffset)
                {
                    Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0f) * Projection;
                }

                RippleEffect.Parameters["View"].SetValue(Matrix.Identity);
                RippleEffect.Parameters["Projection"].SetValue(Projection);
                RippleEffect.Parameters["World"].SetValue(Matrix.Identity);
                RippleEffect.Parameters["RippleTexture"].SetValue(RippleTexture);
                RippleEffect.Parameters["Time"].SetValue((float)RippleTime);
                RippleEffect.Parameters["TextureSize"].SetValue(256f);
                EffectParameter Intensity = RippleEffect.Parameters["RainIntensity"];
                if (Intensity != null)
                {
                    float Amount = Climate.Now.Rain;
                    if (Amount < 0.15f)
                    {
                        Amount = 0.15f;
                    }

                    Intensity.SetValue(Amount);
                }
                Device.SetRenderTarget(RippleTarget);
                Device.Clear(Color.Transparent);
                Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, RippleEffect);
                Begun = true;
                Batch.Draw(RippleTexture, new Rectangle(0, 0, 256, 256), Color.White);
                Batch.End();
                Begun = false;
                Device.SetRenderTarget(null);
            }
            catch (Exception)
            {
                if (Begun)
                {
                    try
                    {
                        Batch.End();
                    }
                    catch (Exception)
                    {
                    }
                }

                try
                {
                    Device.SetRenderTarget(null);
                }
                catch (Exception)
                {
                }

                RippleBroken = true;
                RippleTarget = null;
            }
        }

        static void Ensure(NavMapGameManager World)
        {
            if (AssetsTried || World == null || World.Content == null)
            {
                return;
            }

            AssetsTried = true;
            try
            {
                RippleTexture = World.Content.Load<Texture2D>("3D/Textures/RippleNormal");
                RippleEffect = World.Content.Load<Effect>("Shaders/Ripple");
            }
            catch (Exception)
            {
                RippleTexture = null;
                RippleEffect = null;
            }

            try
            {
                StencilEffect = World.Content.Load<Effect>("Shaders/Stencil Ripple");
            }
            catch (Exception)
            {
                try
                {
                    StencilEffect = GameScreen.ContentFallback.Load<Effect>("Shaders/Stencil Ripple");
                }
                catch (Exception)
                {
                    StencilEffect = null;
                }
            }

            try
            {
                RainLine = World.Content.Load<Texture2D>("Rain Line");
            }
            catch (Exception)
            {
                try
                {
                    RainLine = GameScreen.ContentFallback.Load<Texture2D>("Rain Line");
                }
                catch (Exception)
                {
                    RainLine = null;
                }
            }

            try
            {
                Droplets = World.Content.Load<Texture2D>("3D/Textures/Glass Droplets");
            }
            catch (Exception)
            {
                Droplets = null;
            }

            if (GameScreen.GraphicsDevice == null)
            {
                return;
            }

            try
            {
                Bolt = new LightningBolt();
                Bolt.Init(World.Content, GameScreen.GraphicsDevice, LightningDescriptor.LightningTree(Bolt));
                Bolt.Descriptor.BaseWidth = 0.55f;
                Bolt.Descriptor.JitterForwardDeviation = new ProjectEternity.Core.LightningSystem.Range(-3.5f, 3.5f);
                Bolt.Descriptor.JitterLeftDeviation = new ProjectEternity.Core.LightningSystem.Range(-3.5f, 3.5f);
                Bolt.Descriptor.JitterDeviationRadius = 2f;
                Bolt.Descriptor.IsGlowEnabled = false;
            }
            catch (Exception)
            {
                Bolt = null;
            }

            if (GameScreen.FMODSystem == null)
            {
                return;
            }

            try
            {
                Thunder = new FMODSound[3];
                Thunder[0] = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Thunder 1.wav");
                Thunder[1] = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Thunder 2.wav");
                Thunder[2] = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Thunder 3.wav");
            }
            catch (Exception)
            {
                Thunder = null;
            }
        }

        static void ViewGround(Camera3D Camera, Rectangle Area, Vector3 Eye, float Height, out Vector3 BottomLeft, out Vector3 BottomRight, out Vector3 TopRight, out Vector3 TopLeft)
        {
            float Aspect = Area.Height > 0 ? Area.Width / (float)Area.Height : 1f;
            float HalfV = (float)Math.Tan(MathHelper.PiOver4 * 0.5f) * 1.55f;
            float HalfH = HalfV * Aspect;
            Matrix Inv = Matrix.Invert(Camera.View);
            Vector3 Hit(float NdcX, float NdcY)
            {
                Vector3 Dir = Vector3.TransformNormal(new Vector3(NdcX * HalfH, NdcY * HalfV, -1f), Inv);
                if (Dir.LengthSquared() < 0.0001f)
                {
                    Dir = new Vector3(0f, -1f, 0f);
                }

                Dir.Normalize();
                if (Dir.Y < -0.08f)
                {
                    float T = (Height - Eye.Y) / Dir.Y;
                    if (T < 1f)
                    {
                        T = 1f;
                    }

                    return Eye + Dir * T;
                }

                Vector3 Flat = Dir;
                Flat.Y = 0f;
                if (Flat.LengthSquared() < 0.0001f)
                {
                    Flat = new Vector3(0f, 0f, -1f);
                }

                Flat.Normalize();
                Vector3 Far = Eye + Flat * 900f;
                Far.Y = Height;
                return Far;
            }

            BottomLeft = Hit(-1f, -1f);
            BottomRight = Hit(1f, -1f);
            TopRight = Hit(1f, 1f);
            TopLeft = Hit(-1f, 1f);
        }

        static void ViewBand(Camera3D Camera, out float Near, out float Far, out float Spread)
        {
            Vector3 Eye = Camera.CameraPosition3D;
            Vector3 Target = Camera.LookAtTarget;
            float Height = Math.Max(Eye.Y - Target.Y, 8f);
            float Flat = new Vector2(Target.X - Eye.X, Target.Z - Eye.Z).Length();
            if (Flat < 1f)
            {
                Flat = Height;
            }

            float Pitch = (float)Math.Atan2(Height, Flat);
            const float HalfVertical = 0.3926991f;
            float Down = Math.Min(Pitch + HalfVertical, 1.4f);
            float Up = Math.Max(Pitch - HalfVertical, 0.12f);
            Near = Math.Max(4f, Height / (float)Math.Tan(Down) * 0.75f);
            Far = Height / (float)Math.Tan(Up) * 1.2f;
            Spread = (float)(Math.Tan(HalfVertical) * 1.7f);
        }

        static float BandHalf(float Along, float Spread)
        {
            return Math.Abs(Along) * Spread + RainMargin;
        }

        static bool InBand(Vector3 Eye, Vector3 Look, Vector3 Right, Vector3 Position, float Near, float Far, float Spread)
        {
            Vector3 Offset = Position - Eye;
            Offset.Y = 0f;
            float Along = Vector3.Dot(Offset, Look);
            float Lateral = Vector3.Dot(Offset, Right);
            return Along >= Near - RainMargin && Along <= Far + RainMargin && Math.Abs(Lateral) <= BandHalf(Along, Spread);
        }

        static bool Place(NavMapGameManager World, Vector3 Eye, Vector3 Look, Vector3 Right, float Near, float Far, float Spread, out Drop Made)
        {
            Made = new Drop();
            float Unit = (float)Rng.NextDouble();
            float Span = Math.Max(Far - Near + RainMargin * 2f, 1f);
            float Along = (Near - RainMargin) + Unit * Span;
            float Lateral = ((float)Rng.NextDouble() * 2f - 1f) * BandHalf(Along, Spread);
            float X = Eye.X + Look.X * Along + Right.X * Lateral;
            float Z = Eye.Z + Look.Z * Along + Right.Z * Lateral;
            if (RainAt(World.Clock, World.RootMapContainer, X, Z) <= 0f)
            {
                return false;
            }

            if (CeilingAbove(X, Z, 0))
            {
                float Slab;
                if (RoofRings < 1 && Rng.NextDouble() < 0.2d && RoofBetween(X, Z, 400f, 0f, out Slab))
                {
                    Splash(X, Slab, Z);
                    RoofRings++;
                }

                return false;
            }

            float Floor = GroundAt(X, Z);
            float Top = Math.Max(Eye.Y, Floor) + 48f;
            Outlook Sky = Forecast.Look(World, X, Z);
            TimePeriod Falling = Forecast.Falls(Sky.Period, false) ? Sky.Period : Sky.Dying;
            Made.Position = new Vector3(X, Floor + 1f + (float)Rng.NextDouble() * Math.Max(Top - Floor, 8f), Z);
            float Fast = Falling != null && Falling.WeatherType == TimePeriod.WeatherTypes.SmallRain ? 120f : 190f;
            Made.Speed = Fast * (0.9f + (float)Rng.NextDouble() * 0.25f);
            Made.Drift = DriftOf(Made.Speed);
            Made.Live = true;
            return true;
        }

        static void Shed(NavMapGameManager World, float Step)
        {
            if (Step < 0f)
            {
                Step = 0f;
            }

            for (int i = 0; i < Drips.Count; i++)
            {
                Drop Drop = Drips[i];
                if (!Drop.Live)
                {
                    continue;
                }

                Drop.Position.X += Drop.Drift.X * Step;
                Drop.Position.Y -= Drop.Speed * Step;
                Drop.Position.Z += Drop.Drift.Y * Step;
                if (Drop.Position.Y < GroundAt(Drop.Position.X, Drop.Position.Z))
                {
                    Drop.Live = false;
                }

                Drips[i] = Drop;
            }

            int People = WetOutOfRain(World);
            int Want = 0;
            for (int i = 0; i < People; i++)
            {
                Want += DripBudget(WetPeople[i]);
            }

            if (Want > DripCap)
            {
                Want = DripCap;
            }

            if (Want <= 0 || People <= 0)
            {
                return;
            }

            int Live = 0;
            for (int i = 0; i < Drips.Count; i++)
            {
                if (Drips[i].Live)
                {
                    Live++;
                }
            }

            int Next = 0;
            for (int i = 0; i < Drips.Count && Live < Want; i++)
            {
                if (Drips[i].Live)
                {
                    continue;
                }

                Drop Made;
                if (!DripFrom(WetPeople[Next % People], out Made))
                {
                    Next++;
                    continue;
                }

                Drips[i] = Made;
                Live++;
                Next++;
            }

            while (Drips.Count < Want && Live < Want)
            {
                Drop Made;
                if (!DripFrom(WetPeople[Next % People], out Made))
                {
                    break;
                }

                Drips.Add(Made);
                Live++;
                Next++;
            }
        }

        static int WetOutOfRain(NavMapGameManager World)
        {
            int Count = 0;
            IList<MapInfo> Maps = World == null ? null : World.UpdatedMaps;
            if (Maps == null)
            {
                return 0;
            }

            for (int m = 0; m < Maps.Count && Count < WetPeople.Length; m++)
            {
                MapInfo Info = Maps[m];
                if (Info == null || Info.People == null)
                {
                    continue;
                }

                for (int i = 0; i < Info.People.Count && Count < WetPeople.Length; i++)
                {
                    PlayerCharacter Who = Info.People[i];
                    if (Who == null || Who.WasInWater || Who.DungeonPose || Who.WetAmount <= 0.08f)
                    {
                        continue;
                    }

                    if (Who.Life != null && Who.Life.Away != null && Who.Life.Away.Busy)
                    {
                        continue;
                    }

                    if (Who.SharedMapContex == null || Who.SharedMapContex.CurrentMapInfo != Info)
                    {
                        continue;
                    }

                    Vector3 Shift = ChunkOrigin.Shift(Info);
                    float X = Who.Position.X + Shift.X;
                    float Z = Who.Position.Y + Shift.Z;
                    bool Covered = UnderRoof(X, Z);
                    if (RainAt(World.Clock, World.RootMapContainer, X, Z) > 0.05f && !Covered)
                    {
                        continue;
                    }

                    WetPeople[Count] = Who;
                    Count++;
                }
            }

            return Count;
        }

        static int DripBudget(PlayerCharacter Who)
        {
            int Count = 3 + (int)(Who.WetAmount * 11f);
            if (Count > 14)
            {
                Count = 14;
            }

            return Count;
        }

        static bool DripFrom(PlayerCharacter Who, out Drop Made)
        {
            Made = new Drop();
            if (Who == null || Who.SharedMapContex == null || Who.SharedMapContex.CurrentMapInfo == null)
            {
                return false;
            }

            Vector3 Shift = ChunkOrigin.Shift(Who.SharedMapContex.CurrentMapInfo);
            Vector3 Feet = ChunkOrigin.ModelWorld(Who.Position, Shift);
            float Tall = Who.BodyScale > 2f ? Who.BodyScale * 1.55f : WaterPlace.CharacterHeight;
            float Crown = Who.WetLine > 2f ? Math.Min(Who.WetLine + Tall * 0.05f, Tall * 0.95f) : Tall * 0.92f;
            float Low = Crown * 0.62f;
            if (Crown - Low < 3f)
            {
                Low = Math.Max(0f, Crown - 3f);
            }

            float Rise = Low + (float)Rng.NextDouble() * Math.Max(Crown - Low, 1f);
            float Radius = Math.Max(2.5f, Tall * 0.07f);
            float Angle = (float)(Rng.NextDouble() * Math.PI * 2d);
            float Radial = (float)Rng.NextDouble() * Radius;
            Made.Position = new Vector3(
                Feet.X + (float)Math.Cos(Angle) * Radial,
                Feet.Y + Rise,
                Feet.Z + (float)Math.Sin(Angle) * Radial);
            Made.Speed = 48f + (float)Rng.NextDouble() * 36f;
            Made.Drift = new Vector2(((float)Rng.NextDouble() - 0.5f) * 8f, ((float)Rng.NextDouble() - 0.5f) * 8f);
            if (!UnderRoof(Feet.X, Feet.Z))
            {
                Made.Drift += Climate.Now.Wind * 0.2f;
            }

            Made.Live = true;
            Made.Drip = true;
            return true;
        }

        static int WantedDrops()
        {
            float Rain = Climate.Now.Rain;
            if (Rain <= 0.05f)
            {
                return 0;
            }

            int Count = (int)(DropCap * MathHelper.Clamp(Rain, 0f, 1f));
            if (Count < 80)
            {
                Count = 80;
            }

            return Count;
        }

        static Vector2 DriftOf(float Speed)
        {
            Vector2 Gust = Climate.Now.Wind;
            if (Gust.LengthSquared() < 0.01f || Speed < 1f)
            {
                return Vector2.Zero;
            }

            Gust.Normalize();
            float Lean = MathHelper.Clamp(0.12f + Climate.Now.Strength * 0.035f, 0.12f, 0.55f);
            return Gust * Speed * Lean;
        }

        static bool PickStrike(NavMapGameManager World, Vector3 Eye, out Vector3 End, out float Dist)
        {
            End = Eye;
            Dist = 0f;
            if (World == null)
            {
                return false;
            }

            for (int n = 0; n < 8; n++)
            {
                float Angle = (float)(Rng.NextDouble() * Math.PI * 2d);
                float Distance = 160f + (float)Rng.NextDouble() * 520f;
                float X = Eye.X + (float)Math.Cos(Angle) * Distance;
                float Z = Eye.Z + (float)Math.Sin(Angle) * Distance;
                if (!StrikeAllowed(World.Clock, World.RootMapContainer, X, Z) || UnderRoof(X, Z))
                {
                    continue;
                }

                End = new Vector3(X, GroundAt(X, Z), Z);
                Dist = Distance;
                return true;
            }

            return false;
        }

        static Vector3 FlatLook(Camera3D Camera)
        {
            if (Camera == null)
            {
                return new Vector3(0f, 0f, 1f);
            }

            Vector3 Look = Camera.LookAtTarget - Camera.CameraPosition3D;
            Look.Y = 0f;
            if (Look.LengthSquared() < 0.0001f)
            {
                return new Vector3(0f, 0f, 1f);
            }

            Look.Normalize();
            return Look;
        }

        static void PlayThunder(float Volume)
        {
            if (Thunder == null)
            {
                return;
            }

            int Choice = Rng.Next(Thunder.Length);
            if (Thunder[Choice] == null)
            {
                return;
            }

            try
            {
                Thunder[Choice].Play(Volume);
            }
            catch (Exception)
            {
            }
        }

        static bool RunoffLive()
        {
            for (int i = 0; i < Runoff.Count; i++)
            {
                if (Runoff[i].Live)
                {
                    return true;
                }
            }

            return false;
        }

        static void Pour(NavMapGameManager World, Camera3D Camera, float Step)
        {
            if (Step < 0f)
            {
                Step = 0f;
            }

            for (int i = 0; i < Runoff.Count; i++)
            {
                Drop Drop = Runoff[i];
                if (!Drop.Live)
                {
                    continue;
                }

                Drop.Position.X += Drop.Drift.X * Step;
                Drop.Position.Y -= Drop.Speed * Step;
                Drop.Position.Z += Drop.Drift.Y * Step;
                float Ground = GroundAt(Drop.Position.X, Drop.Position.Z);
                if (Drop.Position.Y <= Ground)
                {
                    Splash(Drop.Position.X, Ground, Drop.Position.Z);
                    Drop.Live = false;
                }

                Runoff[i] = Drop;
            }

            if (Camera == null || Droplets == null || Climate.Now.Rain <= 0.05f)
            {
                return;
            }

            int Want = (int)(28f * MathHelper.Clamp(Climate.Now.Rain, 0f, 1f));
            if (Want < 10)
            {
                Want = 10;
            }

            if (Want > RunoffCap)
            {
                Want = RunoffCap;
            }

            int Live = 0;
            for (int i = 0; i < Runoff.Count; i++)
            {
                if (Runoff[i].Live)
                {
                    Live++;
                }
            }

            if (Live >= Want)
            {
                return;
            }

            Vector3 Eye = Camera.CameraPosition3D;
            IList<MapInfo> Maps = ChunkCross.Maps;
            if (Maps == null)
            {
                return;
            }

            int Added = 0;
            for (int m = 0; m < Maps.Count && Live < Want && Added < 12; m++)
            {
                MapInfo Chunk = Maps[m];
                if (Chunk == null || Chunk.Buildings == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Chunk);
                for (int b = 0; b < Chunk.Buildings.Count && Live < Want && Added < 12; b++)
                {
                    BuildingItem Box = Chunk.Buildings[b];
                    if (Box == null || Box.Kind != BuildingKind.Floor || Box.Floor <= 0)
                    {
                        continue;
                    }

                    float MidX = Box.Position.X + Shift.X;
                    float MidZ = Box.Position.Y + Shift.Z;
                    if (Vector2.DistanceSquared(new Vector2(MidX, MidZ), new Vector2(Eye.X, Eye.Z)) > 90f * 90f)
                    {
                        continue;
                    }

                    if (CeilingAbove(MidX, MidZ, Box.Floor))
                    {
                        continue;
                    }

                    Drop Made;
                    if (!EaveDrop(Chunk.Buildings, Box, Shift, out Made))
                    {
                        continue;
                    }

                    if (Vector3.DistanceSquared(Made.Position, Eye) > 80f * 80f)
                    {
                        continue;
                    }

                    ParkRunoff(Made);
                    Live++;
                    Added++;
                }
            }
        }

        static bool EaveDrop(IList<BuildingItem> Buildings, BuildingItem Box, Vector3 Shift, out Drop Made)
        {
            Made = new Drop();
            float Cose = (float)Math.Cos(Box.Angle);
            float Sine = (float)Math.Sin(Box.Angle);
            Vector2 Across = new Vector2(-Sine, Cose);
            Vector2 Along = new Vector2(Cose, Sine);
            float HalfW = Box.Width * 0.5f;
            float HalfL = Box.Length * 0.5f;
            int Side = Rng.Next(4);
            Vector2 Out = Side < 2 ? Across * (Side == 0 ? 1f : -1f) : Along * (Side == 2 ? 1f : -1f);
            float Span = Side < 2 ? HalfW : HalfL;
            float Lip = Side < 2 ? HalfL : HalfW;
            Vector2 Local = new Vector2(Box.Position.X, Box.Position.Y) + Out * (Lip + 0.4f);
            Vector2 Neighbor = Local + Out * 3f;
            if (SameFloor(Buildings, Box, Neighbor.X, Neighbor.Y))
            {
                return false;
            }

            float Slide = ((float)Rng.NextDouble() * 2f - 1f) * Span;
            Vector2 Tangent = new Vector2(-Out.Y, Out.X);
            Local += Tangent * Slide;
            Made.Position = new Vector3(Local.X + Shift.X, Box.Position.Z + 0.15f, Local.Y + Shift.Z);
            Made.Speed = 36f + (float)Rng.NextDouble() * 28f;
            Made.Drift = Climate.Now.Wind * 0.08f;
            Made.Live = true;
            Made.Drip = true;
            return true;
        }

        static bool SameFloor(IList<BuildingItem> Buildings, BuildingItem Box, float LocalX, float LocalY)
        {
            if (Buildings == null || Box == null)
            {
                return false;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Other = Buildings[i];
                if (Other == null || Other == Box || Other.Kind != BuildingKind.Floor || Other.Floor != Box.Floor)
                {
                    continue;
                }

                if (Other.FootprintContains(LocalX, LocalY))
                {
                    return true;
                }
            }

            return false;
        }

        static void ParkRunoff(Drop Made)
        {
            for (int i = 0; i < Runoff.Count; i++)
            {
                if (!Runoff[i].Live)
                {
                    Runoff[i] = Made;
                    return;
                }
            }

            if (Runoff.Count < RunoffCap)
            {
                Runoff.Add(Made);
            }
        }

        static void DrawPour(GraphicsDevice Device, Camera3D Camera, Matrix Projection, NavMapGameManager World, BasicEffect Effect)
        {
            if (Droplets == null || Effect == null)
            {
                return;
            }

            Effect.Texture = Droplets;
            Device.BlendState = BlendState.Additive;
            Device.SamplerStates[0] = SamplerState.LinearWrap;
            Device.DepthStencilState = DepthStencilState.DepthRead;
            Device.RasterizerState = RasterizerState.CullNone;
            VertexPositionColorTexture[] Sheet = WallSheets(World, Camera);
            if (Sheet != null && Sheet.Length >= 3)
            {
                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserPrimitives(PrimitiveType.TriangleList, Sheet, 0, Sheet.Length / 3);
                }
            }

            int Count = RunoffQuads(Camera);
            if (Count >= 3)
            {
                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserPrimitives(PrimitiveType.TriangleList, RunoffVerts, 0, Count / 3);
                }
            }
        }

        static VertexPositionColorTexture[] RunoffVerts;

        static int RunoffQuads(Camera3D Camera)
        {
            if (RunoffVerts == null || RunoffVerts.Length < RunoffCap * 6)
            {
                RunoffVerts = new VertexPositionColorTexture[RunoffCap * 6];
            }

            Vector3 Eye = Camera.CameraPosition3D;
            int Count = 0;
            float Scroll = (float)RippleTime * 0.55f;
            for (int i = 0; i < Runoff.Count; i++)
            {
                Drop Drop = Runoff[i];
                if (!Drop.Live || Count + 6 > RunoffVerts.Length)
                {
                    continue;
                }

                Vector3 Center = Drop.Position;
                Vector3 ToEye = Eye - Center;
                ToEye.Y = 0f;
                if (ToEye.LengthSquared() < 0.0001f)
                {
                    ToEye = new Vector3(1f, 0f, 0f);
                }

                ToEye.Normalize();
                Vector3 Side = Vector3.Cross(Vector3.Up, ToEye) * 0.16f;
                Vector3 Up = Vector3.Up * 1.6f;
                float Column = (Math.Abs(i * 17 + (int)Center.X) % 10) / 10f;
                float V = Scroll + Center.Y * 0.04f;
                Color Ink = Color.FromNonPremultiplied(210, 225, 235, 150);
                Vector3 Top = Center + Up;
                Vector3 Bottom = Center - Up;
                RunoffVerts[Count++] = new VertexPositionColorTexture(Top - Side, Ink, new Vector2(Column, V));
                RunoffVerts[Count++] = new VertexPositionColorTexture(Top + Side, Ink, new Vector2(Column + 0.08f, V));
                RunoffVerts[Count++] = new VertexPositionColorTexture(Bottom + Side, Ink, new Vector2(Column + 0.08f, V + 0.45f));
                RunoffVerts[Count++] = new VertexPositionColorTexture(Top - Side, Ink, new Vector2(Column, V));
                RunoffVerts[Count++] = new VertexPositionColorTexture(Bottom + Side, Ink, new Vector2(Column + 0.08f, V + 0.45f));
                RunoffVerts[Count++] = new VertexPositionColorTexture(Bottom - Side, Ink, new Vector2(Column, V + 0.45f));
            }

            return Count;
        }

        static VertexPositionColorTexture[] WallSheets(NavMapGameManager World, Camera3D Camera)
        {
            if (World == null || Camera == null || Climate.Now.Rain <= 0.05f)
            {
                return null;
            }

            IList<MapInfo> Maps = ChunkCross.Maps;
            if (Maps == null)
            {
                return null;
            }

            List<VertexPositionColorTexture> Vertices = new List<VertexPositionColorTexture>();
            Vector3 Eye = Camera.CameraPosition3D;
            float Scroll = (float)RippleTime * 0.22f;
            int Faces = 0;
            for (int m = 0; m < Maps.Count && Faces < 28; m++)
            {
                MapInfo Chunk = Maps[m];
                if (Chunk == null || Chunk.Buildings == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Chunk);
                for (int b = 0; b < Chunk.Buildings.Count && Faces < 28; b++)
                {
                    BuildingItem Box = Chunk.Buildings[b];
                    if (Box == null || (Box.Kind != BuildingKind.Wall && Box.Kind != BuildingKind.Window) || Box.Width < 2f || Box.Height < 4f)
                    {
                        continue;
                    }

                    float Cose = (float)Math.Cos(Box.Angle);
                    float Sine = (float)Math.Sin(Box.Angle);
                    Vector3 Along = new Vector3(Cose, 0f, Sine);
                    Vector3 Normal = new Vector3(-Sine, 0f, Cose);
                    Vector3 Mid = new Vector3(Box.Position.X + Shift.X, Box.Position.Z + Box.Height * 0.5f, Box.Position.Y + Shift.Z);
                    if (Vector2.DistanceSquared(new Vector2(Mid.X, Mid.Z), new Vector2(Eye.X, Eye.Z)) > 70f * 70f)
                    {
                        continue;
                    }

                    float Reach = Box.Length * 0.5f + 6f;
                    bool Plus = !CeilingAbove(Mid.X + Normal.X * Reach, Mid.Z + Normal.Z * Reach, Box.Floor);
                    bool Minus = !CeilingAbove(Mid.X - Normal.X * Reach, Mid.Z - Normal.Z * Reach, Box.Floor);
                    if (!Plus && !Minus)
                    {
                        continue;
                    }

                    float Out = Box.Length * 0.5f + 0.25f;
                    if (Plus)
                    {
                        WallFace(Vertices, Mid + Normal * Out, Along, Box.Width, Box.Height, Scroll, Box.Kind == BuildingKind.Window);
                        Faces++;
                    }

                    if (Minus && Faces < 28)
                    {
                        WallFace(Vertices, Mid - Normal * Out, Along, Box.Width, Box.Height, Scroll, Box.Kind == BuildingKind.Window);
                        Faces++;
                    }
                }
            }

            return Vertices.Count >= 3 ? Vertices.ToArray() : null;
        }

        static void WallFace(List<VertexPositionColorTexture> Vertices, Vector3 Center, Vector3 Along, float Width, float Height, float Scroll, bool Window)
        {
            Vector3 Side = Along * (Width * 0.5f);
            Vector3 Up = Vector3.Up * (Height * 0.5f);
            float Tile = Math.Max(Width / 36f, 0.35f);
            float Rise = Height / 48f;
            int Alpha = Window ? 170 : 110;
            Color Ink = Color.FromNonPremultiplied(200, 220, 230, Alpha);
            Vector3 A = Center - Side + Up;
            Vector3 B = Center + Side + Up;
            Vector3 C = Center + Side - Up;
            Vector3 D = Center - Side - Up;
            Vertices.Add(new VertexPositionColorTexture(A, Ink, new Vector2(0f, Scroll)));
            Vertices.Add(new VertexPositionColorTexture(B, Ink, new Vector2(Tile, Scroll)));
            Vertices.Add(new VertexPositionColorTexture(C, Ink, new Vector2(Tile, Rise + Scroll)));
            Vertices.Add(new VertexPositionColorTexture(A, Ink, new Vector2(0f, Scroll)));
            Vertices.Add(new VertexPositionColorTexture(C, Ink, new Vector2(Tile, Rise + Scroll)));
            Vertices.Add(new VertexPositionColorTexture(D, Ink, new Vector2(0f, Rise + Scroll)));
        }

        public static bool CeilingAbove(float WorldX, float WorldZ, int Floor)
        {
            float Top;
            return LowestRoof(WorldX, WorldZ, Floor, out Top);
        }

        static bool UnderRoof(float WorldX, float WorldZ)
        {
            return CeilingAbove(WorldX, WorldZ, 0);
        }

        static float RoofHeight(float WorldX, float WorldZ)
        {
            float Top;
            return LowestRoof(WorldX, WorldZ, 0, out Top) ? Top : -100000f;
        }

        static bool RoofBetween(float WorldX, float WorldZ, float High, float Low, out float Top)
        {
            if (High < Low)
            {
                float Swap = High;
                High = Low;
                Low = Swap;
            }

            if (!LowestRoof(WorldX, WorldZ, 0, out Top))
            {
                return false;
            }

            return Top <= High + 0.4f && Top >= Low - 0.4f;
        }

        static bool LowestRoof(float WorldX, float WorldZ, int Floor, out float Top)
        {
            Top = 0f;
            bool Hit = false;
            IList<MapInfo> Maps = ChunkCross.Maps;
            if (Maps == null)
            {
                return false;
            }

            for (int m = 0; m < Maps.Count; m++)
            {
                MapInfo Chunk = Maps[m];
                if (Chunk == null || Chunk.Buildings == null || Chunk.MapMapContainer == null)
                {
                    continue;
                }

                float SpanX = Chunk.MapMapContainer.SpanX;
                float SpanY = Chunk.MapMapContainer.SpanY;
                if (SpanX <= 0f || SpanY <= 0f)
                {
                    continue;
                }

                float LocalX = WorldX - Chunk.MapMapContainer.OffsetX;
                float LocalY = WorldZ - Chunk.MapMapContainer.OffsetY;
                if (LocalX < 0f || LocalY < 0f || LocalX >= SpanX || LocalY >= SpanY)
                {
                    continue;
                }

                for (int i = 0; i < Chunk.Buildings.Count; i++)
                {
                    BuildingItem Box = Chunk.Buildings[i];
                    if (Box == null || Box.Kind != BuildingKind.Floor || Box.Floor <= Floor || !Box.FootprintContains(LocalX, LocalY))
                    {
                        continue;
                    }

                    float Thick = Box.Height > 0.5f ? Box.Height : 4f;
                    if (Thick > 8f)
                    {
                        Thick = 8f;
                    }

                    float Slab = Box.Position.Z + Thick;
                    if (!Hit || Slab < Top)
                    {
                        Top = Slab;
                        Hit = true;
                    }
                }
            }

            return Hit;
        }

        static void Splash(float X, float Y, float Z)
        {
            Ring Made = new Ring
            {
                At = new Vector3(X, Y + 0.18f, Z),
                Age = 0f,
                Live = true,
            };
            for (int i = 0; i < Rings.Count; i++)
            {
                if (!Rings[i].Live)
                {
                    Rings[i] = Made;
                    return;
                }
            }

            if (Rings.Count < RingCap)
            {
                Rings.Add(Made);
                return;
            }

            int Oldest = 0;
            for (int i = 1; i < Rings.Count; i++)
            {
                if (Rings[i].Age > Rings[Oldest].Age)
                {
                    Oldest = i;
                }
            }

            Rings[Oldest] = Made;
        }

        static void AgeRings(float Step)
        {
            if (Step < 0f)
            {
                Step = 0f;
            }

            for (int i = 0; i < Rings.Count; i++)
            {
                Ring Ring = Rings[i];
                if (!Ring.Live)
                {
                    continue;
                }

                Ring.Age += Step;
                if (Ring.Age >= RingLife)
                {
                    Ring.Live = false;
                }

                Rings[i] = Ring;
            }
        }

        static bool RingReady()
        {
            for (int i = 0; i < Rings.Count; i++)
            {
                if (Rings[i].Live)
                {
                    return true;
                }
            }

            return false;
        }

        static VertexPositionColor[] RingCorners;

        static void DrawRings(GraphicsDevice Device, Camera3D Camera, Matrix Projection)
        {
            if (!RingReady())
            {
                return;
            }

            if (RingCorners == null || RingCorners.Length < RingCap * 24)
            {
                RingCorners = new VertexPositionColor[RingCap * 24];
            }

            VertexPositionColor[] Corners = RingCorners;
            int Count = 0;
            const int Seg = 8;
            for (int i = 0; i < Rings.Count && Count + 24 <= Corners.Length; i++)
            {
                Ring Ring = Rings[i];
                if (!Ring.Live)
                {
                    continue;
                }

                float Grow = MathHelper.Clamp(Ring.Age / RingLife, 0f, 1f);
                float Radius = MathHelper.Lerp(0.35f, 2.2f, Grow);
                int Alpha = (int)(160f * (1f - Grow));
                Color Ink = Color.FromNonPremultiplied(190, 210, 220, Alpha);
                float Inner = Radius * 0.62f;
                for (int s = 0; s < Seg; s++)
                {
                    float A0 = s / (float)Seg * MathHelper.TwoPi;
                    float A1 = (s + 1) / (float)Seg * MathHelper.TwoPi;
                    Vector3 Inner0 = Ring.At + new Vector3((float)Math.Cos(A0) * Inner, 0f, (float)Math.Sin(A0) * Inner);
                    Vector3 Outer0 = Ring.At + new Vector3((float)Math.Cos(A0) * Radius, 0f, (float)Math.Sin(A0) * Radius);
                    Vector3 Outer1 = Ring.At + new Vector3((float)Math.Cos(A1) * Radius, 0f, (float)Math.Sin(A1) * Radius);
                    Vector3 Inner1 = Ring.At + new Vector3((float)Math.Cos(A1) * Inner, 0f, (float)Math.Sin(A1) * Inner);
                    Corners[Count++] = new VertexPositionColor(Inner0, Ink);
                    Corners[Count++] = new VertexPositionColor(Outer0, Ink);
                    Corners[Count++] = new VertexPositionColor(Outer1, Ink);
                    Corners[Count++] = new VertexPositionColor(Inner0, Ink);
                    Corners[Count++] = new VertexPositionColor(Outer1, Ink);
                    Corners[Count++] = new VertexPositionColor(Inner1, Ink);
                }
            }

            if (Count < 3)
            {
                return;
            }

            BasicEffect Flat = FlatEffect(Device);
            Flat.View = Camera.View;
            Flat.Projection = Projection;
            Flat.World = Matrix.Identity;
            Device.BlendState = BlendState.NonPremultiplied;
            Device.DepthStencilState = DepthStencilState.DepthRead;
            Device.RasterizerState = DecalRaster();
            foreach (EffectPass Pass in Flat.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserPrimitives(PrimitiveType.TriangleList, Corners, 0, Count / 3);
            }
        }

        static bool Dry(float WorldX, float WorldZ)
        {
            MapInfo Chunk = ChunkCross.At(WorldX, WorldZ);
            if (Chunk == null || Chunk.MapMapContainer == null)
            {
                return false;
            }

            return Motives.Sheltered(Chunk, WorldX - Chunk.MapMapContainer.OffsetX, WorldZ - Chunk.MapMapContainer.OffsetY, 0);
        }

        static float GroundAt(float WorldX, float WorldZ)
        {
            MapInfo Chunk = ChunkCross.At(WorldX, WorldZ);
            if (Chunk != null && Chunk.Heights != null && Chunk.MapMapContainer != null)
            {
                float Height;
                float SlopeX;
                float SlopeY;
                float LocalX = WorldX - Chunk.MapMapContainer.OffsetX;
                float LocalY = WorldZ - Chunk.MapMapContainer.OffsetY;
                if (Chunk.Heights.Sample(LocalX, LocalY, out Height, out SlopeX, out SlopeY))
                {
                    return Height;
                }
            }

            return Surface(WorldX, WorldZ);
        }

        static float Surface(float WorldX, float WorldZ)
        {
            MapInfo Chunk = ChunkCross.At(WorldX, WorldZ);
            if (Chunk == null || Chunk.MapMapContainer == null)
            {
                return 0f;
            }

            LifeSimMap Map = Chunk.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map == null || Map.LayerManager == null || Map.LayerManager.ListLayer == null || Map.LayerManager.ListLayer.Count == 0 || Map.TileSize.X <= 0)
            {
                return 0f;
            }

            MapLayer Ground = Map.LayerManager.ListLayer[0];
            if (Ground.ArrayTerrain == null)
            {
                return 0f;
            }

            int X = (int)((WorldX - Chunk.MapMapContainer.OffsetX) / Map.TileSize.X);
            int Y = (int)((WorldZ - Chunk.MapMapContainer.OffsetY) / Map.TileSize.Y);
            if (X < 0 || Y < 0 || X >= Ground.ArrayTerrain.GetLength(0) || Y >= Ground.ArrayTerrain.GetLength(1))
            {
                return 0f;
            }

            return Ground.ArrayTerrain[X, Y].WorldPosition.Z;
        }

        static BasicEffect TextureEffect(GraphicsDevice Device)
        {
            if (Textured == null || Textured.GraphicsDevice != Device)
            {
                Textured = new BasicEffect(Device);
                Textured.VertexColorEnabled = true;
                Textured.LightingEnabled = false;
                Textured.TextureEnabled = true;
            }

            return Textured;
        }

        static BasicEffect FlatEffect(GraphicsDevice Device)
        {
            if (ColorEffect == null || ColorEffect.GraphicsDevice != Device)
            {
                ColorEffect = new BasicEffect(Device);
                ColorEffect.VertexColorEnabled = true;
                ColorEffect.LightingEnabled = false;
                ColorEffect.TextureEnabled = false;
            }

            return ColorEffect;
        }

        static void Horizontal(List<VertexPositionColorTexture> Vertices, float X, float Z, float Height, float Half, float U, float V, float Scale, Color Color)
        {
            Vector3 A = new Vector3(X - Half, Height, Z - Half);
            Vector3 B = new Vector3(X + Half, Height, Z - Half);
            Vector3 C = new Vector3(X + Half, Height, Z + Half);
            Vector3 D = new Vector3(X - Half, Height, Z + Half);
            Quad(Vertices, A, B, C, D, new Vector2(U, V), new Vector2(U + Scale, V), new Vector2(U + Scale, V + Scale), new Vector2(U, V + Scale), Color);
        }

        static void Horizontal(List<VertexPositionColor> Vertices, float X, float Z, float Height, float Half, Color Color)
        {
            Vector3 A = new Vector3(X - Half, Height, Z - Half);
            Vector3 B = new Vector3(X + Half, Height, Z - Half);
            Vector3 C = new Vector3(X + Half, Height, Z + Half);
            Vector3 D = new Vector3(X - Half, Height, Z + Half);
            Vertices.Add(new VertexPositionColor(A, Color));
            Vertices.Add(new VertexPositionColor(B, Color));
            Vertices.Add(new VertexPositionColor(C, Color));
            Vertices.Add(new VertexPositionColor(A, Color));
            Vertices.Add(new VertexPositionColor(C, Color));
            Vertices.Add(new VertexPositionColor(D, Color));
        }

        static void Quad(List<VertexPositionColor> Vertices, Vector3 A, Vector3 B, Vector3 C, Vector3 D, Color Color)
        {
            Vertices.Add(new VertexPositionColor(A, Color));
            Vertices.Add(new VertexPositionColor(B, Color));
            Vertices.Add(new VertexPositionColor(C, Color));
            Vertices.Add(new VertexPositionColor(A, Color));
            Vertices.Add(new VertexPositionColor(C, Color));
            Vertices.Add(new VertexPositionColor(D, Color));
        }

        static void Quad(List<VertexPositionColorTexture> Vertices, Vector3 A, Vector3 B, Vector3 C, Vector3 D, Vector2 UA, Vector2 UB, Vector2 UC, Vector2 UD, Color Color)
        {
            Vertices.Add(new VertexPositionColorTexture(A, Color, UA));
            Vertices.Add(new VertexPositionColorTexture(B, Color, UB));
            Vertices.Add(new VertexPositionColorTexture(C, Color, UC));
            Vertices.Add(new VertexPositionColorTexture(A, Color, UA));
            Vertices.Add(new VertexPositionColorTexture(C, Color, UC));
            Vertices.Add(new VertexPositionColorTexture(D, Color, UD));
        }
    }
}
