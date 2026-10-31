using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public struct NoisePrint
    {
        public float Loudness;
        public float Radius;
        public float Background;
        public float HearingRadius;
    }

    public static class Noise
    {
        public const float WalkLoudness = 45f;
        public const float MaxLoudness = 100f;
        public const float MaxRadius = 64f;
        const float IndoorFloor = 6f;

        public static float StanceOf(Pose Pose)
        {
            switch (Pose)
            {
                case Pose.Crouch:
                    return 0.45f;
                case Pose.Crawl:
                    return 0.25f;
                case Pose.Sit:
                    return 0.2f;
                case Pose.Climb:
                    return 0.85f;
                case Pose.MidAir:
                    return 1.15f;
                case Pose.Incapacitated:
                    return 0f;
                default:
                    return 1f;
            }
        }

        public static float SurfaceOf(string Name)
        {
            SurfaceItem Card = WallCatalog.Find(Name);
            if (Card == null || Card.Use != SurfaceUse.Floor || Card.Footstep <= 0f)
            {
                return 1f;
            }

            return Card.Footstep;
        }

        public static float LoudnessOf(float HorizontalSpeed, float WalkSpeed, Pose Pose, float Surface)
        {
            if (HorizontalSpeed < 0f)
            {
                HorizontalSpeed = 0f;
            }

            if (WalkSpeed < 1f)
            {
                WalkSpeed = 1f;
            }

            if (Surface < 0f)
            {
                Surface = 0f;
            }

            float Loudness = HorizontalSpeed / WalkSpeed * WalkLoudness * StanceOf(Pose) * Surface;
            if (Loudness < 0f)
            {
                Loudness = 0f;
            }

            if (Loudness > MaxLoudness)
            {
                Loudness = MaxLoudness;
            }

            return Loudness;
        }

        public static float RadiusOf(float Loudness)
        {
            if (Loudness < 0f)
            {
                Loudness = 0f;
            }

            if (Loudness > MaxLoudness)
            {
                Loudness = MaxLoudness;
            }

            return MaxRadius * Loudness / MaxLoudness;
        }

        public static float HearingOf(float Loudness, float Radius, float Background)
        {
            if (Loudness <= 0.01f || Radius <= 0f)
            {
                return 0f;
            }

            float Fraction = 1f - Background / Loudness;
            if (Fraction < 0f)
            {
                Fraction = 0f;
            }

            return Radius * Fraction;
        }

        public static float FloorOf(TimePeriod.WeatherTypes Weather)
        {
            switch (Weather)
            {
                case TimePeriod.WeatherTypes.LightSnow:
                    return 18f;
                case TimePeriod.WeatherTypes.Windy:
                    return 28f;
                case TimePeriod.WeatherTypes.SmallRain:
                    return 40f;
                case TimePeriod.WeatherTypes.HeavySnow:
                    return 45f;
                case TimePeriod.WeatherTypes.HeavyRain:
                    return 58f;
                case TimePeriod.WeatherTypes.Sandstorm:
                    return 70f;
                case TimePeriod.WeatherTypes.Thunderstorm:
                    return 78f;
                case TimePeriod.WeatherTypes.Distortion:
                case TimePeriod.WeatherTypes.Minovsky:
                case TimePeriod.WeatherTypes.Getter:
                    return 22f;
                default:
                    return 8f;
            }
        }

        public static float Blend(TimePeriod Period, TimePeriod Dying, float Strength)
        {
            float Now = Period == null ? FloorOf(TimePeriod.WeatherTypes.Regular) : FloorOf(Period.WeatherType);
            if (Dying == null || Strength <= 0f)
            {
                return Now;
            }

            if (Strength > 1f)
            {
                Strength = 1f;
            }

            float Was = FloorOf(Dying.WeatherType);
            return Now + (Was - Now) * Strength;
        }

        public static float Muffle(float Outdoor, bool Sheltered)
        {
            if (!Sheltered)
            {
                return Outdoor;
            }

            float Quiet = Outdoor * 0.25f;
            if (Quiet < IndoorFloor)
            {
                Quiet = IndoorFloor;
            }

            return Quiet;
        }

        public static string PlateUnder(IList<BuildingItem> Buildings, float X, float Y, int Floor)
        {
            if (Buildings == null)
            {
                return null;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Box = Buildings[i];
                if (Box == null || Box.Kind != BuildingKind.Floor || Box.Floor != Floor)
                {
                    continue;
                }

                if (Box.FootprintContains(X, Y))
                {
                    return Box.Interaction ?? "";
                }
            }

            return null;
        }

        public static float SnowOnGround(Outlook Sky, bool Sheltered)
        {
            if (Sheltered)
            {
                return 1f;
            }

            float Now = SnowFactor(Sky.Period);
            if (Sky.Dying == null || Sky.Strength <= 0f)
            {
                return Now;
            }

            if (Sky.Strength > 1f)
            {
                Sky.Strength = 1f;
            }

            float Was = SnowFactor(Sky.Dying);
            return Now + (Was - Now) * Sky.Strength;
        }

        public static float SurfaceAt(IList<BuildingItem> Buildings, float X, float Y, float Z, Outlook Sky, bool Sheltered)
        {
            string Plate = PlateUnder(Buildings, X, Y, BuildFloor.Index(Z));
            if (Plate != null)
            {
                return SurfaceOf(Plate);
            }

            return SnowOnGround(Sky, Sheltered);
        }

        public static void Stamp(PlayerCharacter Who, float HorizontalSpeed)
        {
            if (Who == null)
            {
                return;
            }

            IList<BuildingItem> Buildings;
            Outlook Sky;
            bool Sheltered;
            ReadPlace(Who, out Buildings, out Sky, out Sheltered);
            float Surface = SurfaceAt(Buildings, Who.Position.X, Who.Position.Y, Who.Position.Z, Sky, Sheltered);
            if (PlateUnder(Buildings, Who.Position.X, Who.Position.Y, BuildFloor.Index(Who.Position.Z)) == null && Who.SharedMapContex != null)
            {
                MapInfo Here = Who.SharedMapContex.CurrentMapInfo;
                Vector3 Shift = ChunkOrigin.Shift(Here);
                GroundCover Cover = SurfaceData.At(Who.Position.X + Shift.X, Who.Position.Y + Shift.Z);
                if (Cover.Hit)
                {
                    Surface *= Cover.Step;
                }
            }
            float Background = Muffle(Blend(Sky.Period, Sky.Dying, Sky.Strength), Sheltered);
            float Walk = Who.BaseMoveSpeed * Who.MovePace;
            float Loudness = LoudnessOf(HorizontalSpeed, Walk, Who.CurrentPose, Surface);
            float Radius = RadiusOf(Loudness);
            NoisePrint Print = new NoisePrint();
            Print.Loudness = Loudness;
            Print.Radius = Radius;
            Print.Background = Background;
            Print.HearingRadius = HearingOf(Loudness, Radius, Background);
            Who.Noise = Print;
        }

        static void ReadPlace(PlayerCharacter Who, out IList<BuildingItem> Buildings, out Outlook Sky, out bool Sheltered)
        {
            Buildings = null;
            Sky = new Outlook();
            Sheltered = false;
            if (Who.SharedMapContex == null)
            {
                return;
            }

            MapInfo Here = Who.SharedMapContex.CurrentMapInfo;
            Buildings = Here == null ? null : Here.Buildings;
            NavMapGameManager World = Who.SharedMapContex.RootMapContainer;
            if (World == null || World.Clock == null || Here == null)
            {
                return;
            }

            Sheltered = Motives.Sheltered(Here, Who.Position.X, Who.Position.Y, BuildFloor.Index(Who.Position.Z));
            Vector3 Shift = ChunkOrigin.Shift(Here);
            Sky = Forecast.Look(World, Who.Position.X + Shift.X, Who.Position.Y + Shift.Z);
        }

        static float SnowFactor(TimePeriod Period)
        {
            if (Period == null)
            {
                return 1f;
            }

            if (Period.WeatherType == TimePeriod.WeatherTypes.LightSnow)
            {
                return 0.5f;
            }

            if (Period.WeatherType == TimePeriod.WeatherTypes.HeavySnow)
            {
                return 0.35f;
            }

            return 1f;
        }
    }

    public static class NoiseBar
    {
        static readonly Color Track = new Color(18, 22, 36, 230);
        static readonly Color Edge = new Color(168, 186, 220, 230);
        static readonly Color Green = new Color(42, 186, 58);
        static readonly Color Red = new Color(196, 38, 42);
        static readonly Color Tick = new Color(240, 220, 120);

        public static void Draw(CustomSpriteBatch g, PlayerCharacter Who, bool Raised)
        {
            Texture2D Pixel = GameScreen.sprPixel;
            if (g == null || Pixel == null || Who == null || g.GraphicsDevice == null)
            {
                return;
            }

            Viewport Pane = g.GraphicsDevice.Viewport;
            int BarW = 200;
            int BarH = 16;
            int Margin = 18;
            if (Pane.Width < BarW + Margin * 2 || Pane.Height < BarH + Margin * 2)
            {
                return;
            }

            int X = Pane.Width - Margin - BarW;
            int Y = Pane.Height - Margin - BarH;
            if (Raised)
            {
                int NeedsH = 112;
                if (NeedsH > Pane.Height / 3)
                {
                    NeedsH = Pane.Height / 3;
                }

                Y = Pane.Height - NeedsH - 12 - 8 - BarH;
            }

            if (Y < 4)
            {
                Y = 4;
            }

            Rounded(g, Pixel, new Rectangle(X - 2, Y - 2, BarW + 4, BarH + 4), 8, Edge);
            Rounded(g, Pixel, new Rectangle(X, Y, BarW, BarH), 8, Track);

            int InnerX = X + 3;
            int InnerY = Y + 3;
            int InnerW = BarW - 6;
            int InnerH = BarH - 6;
            float Loudness = Who.Noise.Loudness;
            float Background = Who.Noise.Background;
            if (Loudness < 0f)
            {
                Loudness = 0f;
            }

            if (Loudness > Noise.MaxLoudness)
            {
                Loudness = Noise.MaxLoudness;
            }

            if (Background < 0f)
            {
                Background = 0f;
            }

            if (Background > Noise.MaxLoudness)
            {
                Background = Noise.MaxLoudness;
            }

            int Fill = (int)Math.Round(InnerW * Loudness / Noise.MaxLoudness);
            int Mark = (int)Math.Round(InnerW * Background / Noise.MaxLoudness);
            if (Fill < 0)
            {
                Fill = 0;
            }

            if (Fill > InnerW)
            {
                Fill = InnerW;
            }

            if (Mark < 0)
            {
                Mark = 0;
            }

            if (Mark > InnerW)
            {
                Mark = InnerW;
            }

            int Quiet = Fill < Mark ? Fill : Mark;
            if (Quiet > 0)
            {
                g.Draw(Pixel, new Rectangle(InnerX, InnerY, Quiet, InnerH), Green);
            }

            if (Fill > Quiet)
            {
                g.Draw(Pixel, new Rectangle(InnerX + Quiet, InnerY, Fill - Quiet, InnerH), Red);
            }

            g.Draw(Pixel, new Rectangle(InnerX + Mark - 1, Y - 3, 2, BarH + 6), Tick);

            if (TextHelper.fntShadowFont != null && Y >= 20)
            {
                TextHelper.DrawText(g, "Noise", new Vector2(X, Y - 18), Color.White);
            }
        }

        static void Rounded(CustomSpriteBatch g, Texture2D Pixel, Rectangle Box, int Radius, Color Color)
        {
            if (Radius > Box.Width / 2)
            {
                Radius = Box.Width / 2;
            }

            if (Radius > Box.Height / 2)
            {
                Radius = Box.Height / 2;
            }

            if (Radius < 1)
            {
                g.Draw(Pixel, Box, Color);
                return;
            }

            g.Draw(Pixel, new Rectangle(Box.X, Box.Y + Radius, Box.Width, Box.Height - Radius * 2), Color);
            for (int Row = 0; Row < Radius; Row++)
            {
                int Inset = Cap(Radius, Row);
                g.Draw(Pixel, new Rectangle(Box.X + Inset, Box.Y + Row, Box.Width - Inset * 2, 1), Color);
                g.Draw(Pixel, new Rectangle(Box.X + Inset, Box.Bottom - 1 - Row, Box.Width - Inset * 2, 1), Color);
            }
        }

        static int Cap(int Radius, int Row)
        {
            float FromCenter = Radius - Row - 0.5f;
            float Inside = Radius * Radius - FromCenter * FromCenter;
            if (Inside < 0f)
            {
                Inside = 0f;
            }

            return (int)Math.Ceiling(Radius - Math.Sqrt(Inside));
        }
    }
}
