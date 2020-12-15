using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens
{
    public sealed class OptionMenu : GameScreen
    {
        private enum MenuChoices { UnitRepresentation, HealthBar, WindowType, Fullscreen, ScreenSize };

        private SpriteFont fntArial12;
        private FMODSound sndSelection;
        private FMODSound sndCancel;

        private MenuChoices MenuChoice;
        private CustomSpriteBatch SpriteBatch;

        public OptionMenu()
            : base()
        {
            MenuChoice = 0;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputLeftPressed())
            {
                switch (MenuChoice)
                {
                    case MenuChoices.UnitRepresentation:
                        Constants.UnitRepresentationState--;
                        if (Constants.UnitRepresentationState < 0)
                            Constants.UnitRepresentationState = Constants.UnitRepresentationStates.Colored;
                        break;

                    case MenuChoices.HealthBar:
                        Constants.ShowHealthBar = !Constants.ShowHealthBar;
                        break;

                    #region Window Type

                    case MenuChoices.WindowType:
                        Constants.WindowType--;
                        if (Constants.WindowType < 0)
                            Constants.WindowType = Constants.WindowTypes.Extend;

                        UpdateScreenSettings(SpriteBatch);
                        break;

                    #endregion

                    case MenuChoices.Fullscreen:
                        if (Constants.WindowType == Constants.WindowTypes.Original)
                            break;
                        Constants.graphics.IsFullScreen = !Constants.graphics.IsFullScreen;
                        Constants.graphics.ApplyChanges();
                        break;

                    case MenuChoices.ScreenSize:
                        if (Constants.WindowType == Constants.WindowTypes.Original)
                            break;
                        Constants.ScreenSize--;
                        if (Constants.ScreenSize < 0)
                            Constants.ScreenSize = Constants.ScreenSizes.Length - 1;

                        switch (Constants.ScreenSizes[Constants.ScreenSize])
                        {
                            case "640 x 480":
                                Constants.graphics.PreferredBackBufferWidth = 640;
                                Constants.graphics.PreferredBackBufferHeight = 480;
                                break;

                            case "1920 x 1080":
                                Constants.graphics.PreferredBackBufferWidth = 1920;
                                Constants.graphics.PreferredBackBufferHeight = 1080;
                                break;
                        }
                        Constants.graphics.ApplyChanges();

                        UpdateScreenSettings(SpriteBatch);
                        break;
                }
                sndSelection.Play();
            }
            else if (InputHelper.InputRightPressed() || InputHelper.InputConfirmPressed())
            {
                switch (MenuChoice)
                {
                    case MenuChoices.UnitRepresentation:
                        Constants.UnitRepresentationState++;
                        if ((int)Constants.UnitRepresentationState > 2)
                            Constants.UnitRepresentationState = Constants.UnitRepresentationStates.NonColoredWithBorder;
                        break;

                    case MenuChoices.HealthBar:
                        Constants.ShowHealthBar = !Constants.ShowHealthBar;
                        break;

                    #region Window Type

                    case MenuChoices.WindowType:
                        Constants.WindowType++;
                        if (Constants.WindowType > Constants.WindowTypes.Extend)
                            Constants.WindowType = Constants.WindowTypes.Original;

                        UpdateScreenSettings(SpriteBatch);
                        break;

                    #endregion

                    case MenuChoices.Fullscreen:
                        if (Constants.WindowType == Constants.WindowTypes.Original)
                            break;
                        Constants.graphics.IsFullScreen = !Constants.graphics.IsFullScreen;
                        Constants.graphics.ApplyChanges();
                        break;

                    case MenuChoices.ScreenSize:
                        if (Constants.WindowType == Constants.WindowTypes.Original)
                            break;
                        Constants.ScreenSize++;
                        if (Constants.ScreenSize >= Constants.ScreenSizes.Length)
                            Constants.ScreenSize = 0;

                        switch (Constants.ScreenSizes[Constants.ScreenSize])
                        {
                            case "640 x 480":
                                Constants.graphics.PreferredBackBufferWidth = 640;
                                Constants.graphics.PreferredBackBufferHeight = 480;
                                break;

                            case "1920 x 1080":
                                Constants.graphics.PreferredBackBufferWidth = 1920;
                                Constants.graphics.PreferredBackBufferHeight = 1080;
                                break;
                        }
                        Constants.graphics.ApplyChanges();

                        UpdateScreenSettings(SpriteBatch);
                        break;
                }
                sndSelection.Play();
            }
            else if (InputHelper.InputUpPressed())
            {
                MenuChoice--;
                sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                MenuChoice++;
                sndSelection.Play();
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
                sndCancel.Play();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            SpriteBatch = g;
            int Y = 50;
            g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.FromNonPremultiplied(0, 0, 0, 160));

            #region Unit representation

            g.Draw(sprPixel, new Rectangle(0, 50 + (int)MenuChoice * 20, Constants.Width, 20), Color.FromNonPremultiplied(255, 255, 255, 100));

            g.DrawString(fntArial12, "Unit representation", new Vector2(10, Y), Color.White);

            if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                g.DrawStringRightAligned(fntArial12, "Colored", new Vector2(Constants.Width - 10, Y), Color.Red);
            else if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.NonColored)
                g.DrawStringRightAligned(fntArial12, "Non colored", new Vector2(Constants.Width - 10, Y), Color.White);
            else if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.NonColoredWithBorder)
            {
                g.DrawStringRightAligned(fntArial12, "Non colored with border", new Vector2(Constants.Width - 10 - 1, Y), Color.Red);
                g.DrawStringRightAligned(fntArial12, "Non colored with border", new Vector2(Constants.Width - 10 + 1, Y), Color.Red);
                g.DrawStringRightAligned(fntArial12, "Non colored with border", new Vector2(Constants.Width - 10, Y - 1), Color.Red);
                g.DrawStringRightAligned(fntArial12, "Non colored with border", new Vector2(Constants.Width - 10, Y + 1), Color.Red);

                g.DrawStringRightAligned(fntArial12, "Non colored with border", new Vector2(Constants.Width - 10, Y), Color.White);
            }

            #endregion

            Y += 20;

            #region Health bar

            g.DrawString(fntArial12, "Show health bar", new Vector2(10, Y), Color.White);

            if (Constants.ShowHealthBar)
                g.DrawString(fntArial12, "On", new Vector2(Constants.Width - 10, Y), Color.Red);
            else
                g.DrawString(fntArial12, "Off", new Vector2(Constants.Width - 10, Y), Color.Red);

            #endregion

            Y += 20;

            #region Window type

            g.DrawString(fntArial12, "Window type", new Vector2(10, Y), Color.White);

            if (Constants.WindowType == Constants.WindowTypes.Original)
                g.DrawString(fntArial12, "Original", new Vector2(Constants.Width - 10, Y), Color.Red);
            else if (Constants.WindowType == Constants.WindowTypes.Streched)
                g.DrawString(fntArial12, "Streched", new Vector2(Constants.Width - 10, Y), Color.Red);
            else if (Constants.WindowType == Constants.WindowTypes.KeeptAspectRatio)
                g.DrawString(fntArial12, "Keept aspect ratio", new Vector2(Constants.Width - 10, Y), Color.Red);
            else if (Constants.WindowType == Constants.WindowTypes.Extend)
                g.DrawString(fntArial12, "Extend", new Vector2(Constants.Width - 10, Y), Color.Red);

            #endregion

            Y += 20;

            #region Fullscreen

            g.DrawString(fntArial12, "Fullscreen", new Vector2(10, Y), Color.White);

            if (Constants.WindowType == Constants.WindowTypes.Original)
                g.DrawStringRightAligned(fntArial12, "Off", new Vector2(Constants.Width - 10, Y), Color.Gray);
            else if (Constants.graphics.IsFullScreen)
                g.DrawStringRightAligned(fntArial12, "On", new Vector2(Constants.Width - 10, Y), Color.Red);
            else
                g.DrawStringRightAligned(fntArial12, "Off", new Vector2(Constants.Width - 10, Y), Color.Red);

            #endregion

            Y += 20;

            #region Screen Size

            g.DrawString(fntArial12, "Screen Size", new Vector2(10, Y), Color.White);

            if (Constants.WindowType == Constants.WindowTypes.Original)
                g.DrawStringRightAligned(fntArial12, Constants.ScreenSizes[Constants.ScreenSize], new Vector2(Constants.Width - 10, Y), Color.Gray);
            else
                g.DrawStringRightAligned(fntArial12, Constants.ScreenSizes[Constants.ScreenSize], new Vector2(Constants.Width - 10, Y), Color.Red);

            #endregion
        }

        public static void UpdateScreenSettings(CustomSpriteBatch g)
        {
            if (Constants.WindowType == Constants.WindowTypes.Original)
            {
                Constants.graphics.IsFullScreen = false;
                Constants.Width = 640;
                Constants.Height = 480;
                Constants.ScreenSize = 0;
                Constants.graphics.PreferredBackBufferWidth = Constants.Width;
                Constants.graphics.PreferredBackBufferHeight = Constants.Height;
                Constants.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, 640, 480);
                g.Scale = Matrix.CreateScale(1);
                Constants.graphics.ApplyChanges();
            }
            else if (Constants.WindowType == Constants.WindowTypes.Streched)
            {
                Constants.Width = 640;
                Constants.Height = 480;

                float HeightScale = Constants.graphics.PreferredBackBufferHeight / (float)480;
                float WidthScale = Constants.graphics.PreferredBackBufferWidth / (float)640;
                g.Scale = Matrix.CreateScale(WidthScale, HeightScale, 1);
                Constants.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Constants.graphics.PreferredBackBufferWidth, Constants.graphics.PreferredBackBufferHeight);
            }
            else if (Constants.WindowType == Constants.WindowTypes.KeeptAspectRatio)
            {
                Constants.Width = 640;
                Constants.Height = 480;

                float HeightScale = Constants.graphics.PreferredBackBufferHeight / (float)480;
                int ScreenWidth = (int)(Constants.graphics.PreferredBackBufferWidth - Constants.Width * HeightScale) / 2;
                g.Scale = Matrix.CreateScale(HeightScale, HeightScale, 1);

                Constants.graphics.GraphicsDevice.Viewport = new Viewport(ScreenWidth, 0, Constants.graphics.PreferredBackBufferWidth - ScreenWidth, Constants.graphics.PreferredBackBufferHeight);
            }
            else if (Constants.WindowType == Constants.WindowTypes.Extend)
            {
                Constants.Width = Constants.graphics.PreferredBackBufferWidth;
                Constants.Height = Constants.graphics.PreferredBackBufferHeight;
                g.Scale = Matrix.CreateScale(1);

                Constants.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Constants.Width, Constants.Height);
            }
        }
    }
}
