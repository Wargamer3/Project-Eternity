using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class LocalPlayerProfileSelectionScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;


        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumLightBigger;

        private Texture2D sprBackground;
        private Texture2D sprSelector;

        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        private Scrollbar ProfilesScrollbar;
        private TextButton ConfirmButton;
        private TextButton CancelButton;

        private IUIElement[] ArrayUIElement;

        private int SelectedProfileIndex;
        private int MapScrollbarValue;
        private List<string> ListPlayerProfile;
        private readonly OnlinePlayerBase ActivePlayer;

        public LocalPlayerProfileSelectionScreen(OnlinePlayerBase ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumLightBigger = Content.Load<SpriteFont>("Fonts/Oxanium Light Bigger");

            sprBackground = Content.Load<Texture2D>("Menus/Lobby/Popup/Popup Large");
            sprSelector = Content.Load<Texture2D>("Menus/Lobby/Popup/Popup Selector");

            sprScrollbarBackground = Content.Load<Texture2D>("Menus/Lobby/Interactive/Scrollbar Frame");
            sprScrollbar = Content.Load<Texture2D>("Menus/Lobby/Interactive/Scrollbar Bar");

            ListPlayerProfile = ActivePlayer.GetProfileNames();

            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(sprBackground.Width * Ratio);
            int MenuHeight = (int)(sprBackground.Height * Ratio);
            int MenuX = Constants.Width / 2 - MenuWidth / 2;
            int MenuY = Constants.Height / 2 - MenuHeight / 2;

            ProfilesScrollbar = new Scrollbar(sprScrollbar, new Vector2(MenuX + MenuWidth - 120 * Ratio, MenuY + 250 * Ratio), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnProfileScrollbarChange);

            ConfirmButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}Confirm}}", "Menus/Lobby/Popup/Button Small Blue", new Vector2((int)(MenuX + MenuWidth - 700 * Ratio), (int)(MenuY + MenuHeight - 150 * Ratio)), 4, 1, Ratio, OnButtonOver, OnConfirmButtonPressed);

            CancelButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}Close}}", "Menus/Lobby/Popup/Button Small Grey", new Vector2((int)(MenuX + MenuWidth - 300 * Ratio), (int)(MenuY + MenuHeight - 150 * Ratio)), 4, 1, Ratio, OnButtonOver, OnCancelButtonPressed);

            ArrayUIElement = new IUIElement[]
            {
                ConfirmButton, CancelButton, ProfilesScrollbar,
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveUIElement in ArrayUIElement)
            {
                ActiveUIElement.Update(gameTime);
            }

            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(sprBackground.Width * Ratio);
            int MenuHeight = (int)(sprBackground.Height * Ratio);
            float MenuX = Constants.Width / 2 - MenuWidth / 2;
            float MenuY = Constants.Height / 2 - MenuHeight / 2;

            float DrawY = MenuY + 250 * Ratio;
            for (int CurrentIndex = MapScrollbarValue; CurrentIndex < ListPlayerProfile.Count; CurrentIndex++)
            {
                if (InputHelper.InputConfirmPressed()
                    &&MouseHelper.MouseStateCurrent.X >= MenuX + 120 * Ratio && MouseHelper.MouseStateCurrent.X < MenuX + MenuWidth - 150 * Ratio
                    && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 120 * Ratio)
                {
                    SelectedProfileIndex = CurrentIndex;
                }

                DrawY += 120 * Ratio;
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void OnConfirmButtonPressed()
        {
            sndButtonClick.Play();

            ActivePlayer.Name = ListPlayerProfile[SelectedProfileIndex];
            ActivePlayer.LoadLocally(GameScreen.ContentFallback);

            RemoveScreen(this);
        }

        private void OnCancelButtonPressed()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void OnProfileScrollbarChange(float ScrollbarValue)
        {
            MapScrollbarValue = (int)ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(sprBackground.Width * Ratio);
            int MenuHeight = (int)(sprBackground.Height * Ratio);
            float MenuX = Constants.Width / 2 - MenuWidth / 2;
            float MenuY = Constants.Height / 2 - MenuHeight / 2;

            Color WhiteText = Color.FromNonPremultiplied(233, 233, 233, 255);
            Color BlackText = Color.FromNonPremultiplied(65, 70, 65, 255);

            g.Draw(sprBackground, new Vector2(MenuX, MenuY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);

            g.DrawStringRightAligned(fntOxanimumBold, "Select Profile", new Vector2(MenuX + MenuWidth - 40 * Ratio, MenuY + 70 * Ratio), WhiteText);

            g.Draw(sprPixel, new Rectangle((int)(MenuX + 100 * Ratio), (int)(MenuY + 210 * Ratio), (int)(1900 * Ratio), (int)(2 * Ratio)), BlackText);
            g.Draw(sprPixel, new Rectangle((int)(MenuX + 100 * Ratio), (int)(MenuY + 750 * Ratio), (int)(1900 * Ratio), (int)(2 * Ratio)), BlackText);

            float DrawY = MenuY + 250 * Ratio;

            for (int CurrentIndex = MapScrollbarValue; CurrentIndex < ListPlayerProfile.Count; CurrentIndex++)
            {
                string ActiveProfileName = ListPlayerProfile[CurrentIndex];

                if (CurrentIndex == SelectedProfileIndex ||
                    (MouseHelper.MouseStateCurrent.X >= MenuX + 120 * Ratio && MouseHelper.MouseStateCurrent.X < MenuX + MenuWidth - 150 * Ratio
                    && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 120 * Ratio))
                {
                    g.Draw(sprSelector, new Vector2((int)(MenuX + 120 * Ratio), (int)DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);
                    g.DrawString(fntOxanimumBold, ActiveProfileName, new Vector2(MenuX + 150 * Ratio, DrawY + 24 * Ratio), WhiteText);
                }
                else
                {
                    g.DrawString(fntOxanimumBold, ActiveProfileName, new Vector2(MenuX + 150 * Ratio, DrawY + 24 * Ratio), BlackText);
                }

                DrawY += 120 * Ratio;
                if (DrawY >= MenuY + MenuHeight)
                {
                    break;
                }
            }

            foreach (IUIElement ActiveUIElement in ArrayUIElement)
            {
                ActiveUIElement.Draw(g);
            }
        }
    }
}
