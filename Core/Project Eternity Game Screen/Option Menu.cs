using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens
{
    public sealed class OptionMenu : GameScreen
    {
        const int RowCount = 14;
        const int RowHeight = 22;

        private SpriteFont fntArial12;
        private FMODSound sndSelection;
        private FMODSound sndCancel;
        private int Cursor;
        private CustomSpriteBatch SpriteBatch;

        public OptionMenu()
            : base()
        {
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                Cursor--;
                if (Cursor < 0)
                {
                    Cursor = RowCount - 1;
                }

                sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                Cursor++;
                if (Cursor >= RowCount)
                {
                    Cursor = 0;
                }

                sndSelection.Play();
            }
            else if (InputHelper.InputLeftPressed())
            {
                Nudge(-1);
                sndSelection.Play();
            }
            else if (InputHelper.InputRightPressed() || InputHelper.InputConfirmPressed())
            {
                Nudge(1);
                sndSelection.Play();
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
                sndCancel.Play();
            }
        }

        void Nudge(int Direction)
        {
            switch (Cursor)
            {
                case 0:
                    CycleUnit(Direction);
                    break;
                case 1:
                    Constants.ShowHealthBar = !Constants.ShowHealthBar;
                    break;
                case 2:
                    CycleWindow(Direction);
                    break;
                case 3:
                    ToggleFullscreen();
                    break;
                case 4:
                    CycleScreen(Direction);
                    break;
                case 5:
                    PictureOptions.CycleShadows(Direction);
                    break;
                case 6:
                    PictureOptions.Shade = !PictureOptions.Shade;
                    break;
                case 7:
                    PictureOptions.Shafts = !PictureOptions.Shafts;
                    break;
                case 8:
                    PictureOptions.CycleGrass(Direction);
                    break;
                case 9:
                    PictureOptions.CycleTrees(Direction);
                    break;
                case 10:
                    PictureOptions.Lamps = !PictureOptions.Lamps;
                    break;
                case 11:
                    PictureOptions.CycleDistance(Direction);
                    break;
                case 12:
                    PictureOptions.FastTime = !PictureOptions.FastTime;
                    break;
                case 13:
                    PictureOptions.Msaa = !PictureOptions.Msaa;
                    ApplyMsaa();
                    break;
            }
        }

        static void ApplyMsaa()
        {
            if (Constants.graphics == null)
            {
                return;
            }

            Constants.graphics.PreferMultiSampling = PictureOptions.Msaa;
            Constants.graphics.ApplyChanges();
        }

        static void CycleUnit(int Direction)
        {
            int Next = (int)Constants.UnitRepresentationState + Direction;
            if (Next < 0)
            {
                Next = 2;
            }
            else if (Next > 2)
            {
                Next = 0;
            }

            Constants.UnitRepresentationState = (Constants.UnitRepresentationStates)Next;
        }

        void CycleWindow(int Direction)
        {
            int Next = (int)Constants.WindowType + Direction;
            if (Next < 0)
            {
                Next = (int)Constants.WindowTypes.Extend;
            }
            else if (Next > (int)Constants.WindowTypes.Extend)
            {
                Next = 0;
            }

            Constants.WindowType = (Constants.WindowTypes)Next;
            UpdateScreenSettings(SpriteBatch);
        }

        static void ToggleFullscreen()
        {
            if (Constants.WindowType == Constants.WindowTypes.Original || Constants.graphics == null)
            {
                return;
            }

            Constants.graphics.IsFullScreen = !Constants.graphics.IsFullScreen;
            Constants.graphics.ApplyChanges();
        }

        void CycleScreen(int Direction)
        {
            if (Constants.WindowType == Constants.WindowTypes.Original || Constants.graphics == null)
            {
                return;
            }

            Constants.ScreenSize += Direction;
            if (Constants.ScreenSize < 0)
            {
                Constants.ScreenSize = Constants.ScreenSizes.Length - 1;
            }
            else if (Constants.ScreenSize >= Constants.ScreenSizes.Length)
            {
                Constants.ScreenSize = 0;
            }

            ApplySize(Constants.ScreenSizes[Constants.ScreenSize]);
            UpdateScreenSettings(SpriteBatch);
        }

        static void ApplySize(string Label)
        {
            string[] Parts = Label.Split('x');
            int Width;
            int Height;
            if (Parts.Length != 2 || !int.TryParse(Parts[0].Trim(), out Width) || !int.TryParse(Parts[1].Trim(), out Height))
            {
                return;
            }

            bool WasFull = Constants.graphics.IsFullScreen;
            if (WasFull)
            {
                Constants.graphics.IsFullScreen = false;
                Constants.graphics.ApplyChanges();
            }

            Constants.graphics.PreferredBackBufferWidth = Width;
            Constants.graphics.PreferredBackBufferHeight = Height;
            Constants.graphics.ApplyChanges();
            if (WasFull)
            {
                Constants.graphics.IsFullScreen = true;
                Constants.graphics.ApplyChanges();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            SpriteBatch = g;
            g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.FromNonPremultiplied(0, 0, 0, 160));
            g.DrawString(fntArial12, "Options", new Vector2(10, 8), Color.White);
            g.Draw(sprPixel, new Rectangle(0, 36 + Cursor * RowHeight, Constants.Width, RowHeight), Color.FromNonPremultiplied(255, 255, 255, 100));

            int Y = 36;
            Row(g, "Unit representation", UnitLabel(), Y, true);
            Y += RowHeight;
            Row(g, "Show health bar", Constants.ShowHealthBar ? "On" : "Off", Y, true);
            Y += RowHeight;
            Row(g, "Window type", WindowLabel(), Y, true);
            Y += RowHeight;
            bool Display = Constants.WindowType != Constants.WindowTypes.Original;
            Row(g, "Fullscreen", Display && Constants.graphics != null && Constants.graphics.IsFullScreen ? "On" : "Off", Y, Display);
            Y += RowHeight;
            Row(g, "Screen size", Constants.ScreenSizes[Constants.ScreenSize], Y, Display);
            Y += RowHeight;
            Row(g, "Shadows", PictureOptions.ShadowLabel, Y, true);
            Y += RowHeight;
            Row(g, "Soft shade", PictureOptions.Shade ? "On" : "Off", Y, true);
            Y += RowHeight;
            Row(g, "Light shafts", PictureOptions.Shafts ? "On" : "Off", Y, PictureOptions.Shade);
            Y += RowHeight;
            Row(g, "Grass", PictureOptions.GrassLabel, Y, true);
            Y += RowHeight;
            Row(g, "Tree detail", PictureOptions.TreeLabel, Y, true);
            Y += RowHeight;
            Row(g, "Lamps", PictureOptions.Lamps ? "On" : "Off", Y, true);
            Y += RowHeight;
            Row(g, "View distance", PictureOptions.DistanceLabel, Y, true);
            Y += RowHeight;
            Row(g, "Fast time", PictureOptions.FastTime ? "On" : "Off", Y, true);
            Y += RowHeight;
            Row(g, "MSAA", PictureOptions.Msaa ? "On" : "Off", Y, true);
            g.DrawString(fntArial12, "Left and right change a setting. Cancel closes.", new Vector2(10, Y + 36), Color.LightGray);
        }

        void Row(CustomSpriteBatch g, string Name, string Value, int Y, bool Enabled)
        {
            Color Ink = Enabled ? Color.White : Color.Gray;
            Color Pick = Enabled ? Color.Red : Color.Gray;
            g.DrawString(fntArial12, Name, new Vector2(10, Y), Ink);
            g.DrawStringRightAligned(fntArial12, Value, new Vector2(Constants.Width - 10, Y), Pick);
        }

        static string UnitLabel()
        {
            if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
            {
                return "Colored";
            }

            if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.NonColored)
            {
                return "Non colored";
            }

            return "Bordered";
        }

        static string WindowLabel()
        {
            if (Constants.WindowType == Constants.WindowTypes.Original)
            {
                return "Original";
            }

            if (Constants.WindowType == Constants.WindowTypes.Streched)
            {
                return "Stretched";
            }

            if (Constants.WindowType == Constants.WindowTypes.KeeptAspectRatio)
            {
                return "Keep aspect";
            }

            return "Extend";
        }

        public static void UpdateScreenSettings(CustomSpriteBatch g)
        {
            if (Constants.graphics == null)
            {
                return;
            }

            if (Constants.WindowType == Constants.WindowTypes.Original)
            {
                Constants.graphics.IsFullScreen = false;
                Constants.ScreenSize = 0;
                Constants.graphics.PreferredBackBufferWidth = Constants.Width;
                Constants.graphics.PreferredBackBufferHeight = Constants.Height;
                Constants.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Constants.graphics.PreferredBackBufferWidth, Constants.graphics.PreferredBackBufferHeight);
                if (g != null)
                {
                    g.Scale = Matrix.CreateScale(1);
                }

                Constants.graphics.ApplyChanges();
            }
            else if (Constants.WindowType == Constants.WindowTypes.Streched)
            {
                Constants.Width = 640;
                Constants.Height = 480;
                float HeightScale = Constants.graphics.PreferredBackBufferHeight / (float)480;
                float WidthScale = Constants.graphics.PreferredBackBufferWidth / (float)640;
                if (g != null)
                {
                    g.Scale = Matrix.CreateScale(WidthScale, HeightScale, 1);
                }

                Constants.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Constants.graphics.PreferredBackBufferWidth, Constants.graphics.PreferredBackBufferHeight);
            }
            else if (Constants.WindowType == Constants.WindowTypes.KeeptAspectRatio)
            {
                Constants.Width = 640;
                Constants.Height = 480;
                float HeightScale = Constants.graphics.PreferredBackBufferHeight / (float)480;
                int ScreenWidth = (int)(Constants.graphics.PreferredBackBufferWidth - Constants.Width * HeightScale) / 2;
                if (g != null)
                {
                    g.Scale = Matrix.CreateScale(HeightScale, HeightScale, 1);
                }

                Constants.graphics.GraphicsDevice.Viewport = new Viewport(ScreenWidth, 0, Constants.graphics.PreferredBackBufferWidth - ScreenWidth, Constants.graphics.PreferredBackBufferHeight);
            }
            else if (Constants.WindowType == Constants.WindowTypes.Extend)
            {
                Constants.Width = Constants.graphics.PreferredBackBufferWidth;
                Constants.Height = Constants.graphics.PreferredBackBufferHeight;
                if (g != null)
                {
                    g.Scale = Matrix.CreateScale(1);
                }

                Constants.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Constants.Width, Constants.Height);
            }
        }
    }
}
