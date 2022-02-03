using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class LocalPlayerProfileSelectionScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private BoxScrollbar ProfilesScrollbar;
        private BoxButton ConfirmButton;
        private BoxButton CancelButton;

        private IUIElement[] ArrayUIElement;

        private int SelectedProfileIndex;
        private int MapScrollbarValue;
        private List<string> ListPlayerProfile;
        private readonly BattleMapPlayer ActivePlayer;

        public LocalPlayerProfileSelectionScreen(BattleMapPlayer ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            ListPlayerProfile = BattleMapPlayer.GetProfileNames();

            int MenuWidth = (int)(Constants.Width * 0.45);
            int MenuHeight = (int)(Constants.Height * 0.45);
            float MenuX = Constants.Width / 2 - MenuWidth / 2;
            float MenuY = Constants.Height / 2 - MenuHeight / 2;

            ProfilesScrollbar = new BoxScrollbar(new Vector2(MenuX + MenuWidth - 20, MenuY), MenuHeight, Math.Max(0, ListPlayerProfile.Count -  9), OnProfileScrollbarChange);
            ConfirmButton = new BoxButton(new Rectangle((int)MenuX + 10, (int)MenuY + MenuHeight + 5, 70, 30), fntArial12, "Confirm", OnButtonOver, OnConfirmButtonPressed);
            CancelButton = new BoxButton(new Rectangle((int)MenuX + MenuWidth - 80, (int)MenuY + MenuHeight + 5, 70, 30), fntArial12, "Cancel", OnButtonOver, OnCancelButtonPressed);

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

            int MenuWidth = (int)(Constants.Width * 0.45);
            int MenuHeight = (int)(Constants.Height * 0.45);
            float MenuX = Constants.Width / 2 - MenuWidth / 2;
            float MenuY = Constants.Height / 2 - MenuHeight / 2;

            float DrawY = MenuY + 45;
            for (int CurrentIndex = MapScrollbarValue; CurrentIndex < ListPlayerProfile.Count; CurrentIndex++)
            {
                if (InputHelper.InputConfirmPressed()
                    &&MouseHelper.MouseStateCurrent.X >= MenuX && MouseHelper.MouseStateCurrent.X < MenuX + MenuWidth
                    && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 25)
                {
                    SelectedProfileIndex = CurrentIndex;
                }

                DrawY += 25;
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
            int MenuWidth = (int)(Constants.Width * 0.45);
            int MenuHeight = (int)(Constants.Height * 0.45);
            float MenuX = Constants.Width / 2 - MenuWidth / 2;
            float MenuY = Constants.Height / 2 - MenuHeight / 2;

            DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);

            DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, 40, Color.White);
            g.DrawString(fntArial12, "Select Profile", new Vector2(MenuX + 10, MenuY + 10), Color.White);

            float DrawY = MenuY + 45;

            for (int CurrentIndex = MapScrollbarValue; CurrentIndex < ListPlayerProfile.Count; CurrentIndex++)
            {
                string ActiveMap = ListPlayerProfile[CurrentIndex];
                g.DrawString(fntArial12, ActiveMap, new Vector2(MenuX + 10, DrawY), Color.White);

                if (CurrentIndex == SelectedProfileIndex)
                {
                    g.Draw(sprPixel, new Rectangle((int)MenuX + 5, (int)DrawY, MenuWidth - 25, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                }

                if (MouseHelper.MouseStateCurrent.X >= MenuX && MouseHelper.MouseStateCurrent.X < MenuX + MenuWidth
                    && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 25)
                {
                    g.Draw(sprPixel, new Rectangle((int)MenuX + 5, (int)DrawY, MenuWidth - 25, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                }

                DrawY += 25;
                if (DrawY >= MenuY + MenuHeight)
                {
                    break;
                }
            }

            DrawBox(g, new Vector2(MenuX, MenuY + MenuHeight), MenuWidth, 40, Color.White);

            foreach (IUIElement ActiveUIElement in ArrayUIElement)
            {
                ActiveUIElement.Draw(g);
            }
        }
    }
}
