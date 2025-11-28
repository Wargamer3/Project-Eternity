using System;
using System.IO;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class LocalPlayerSelectionScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumLightBigger;

        private Texture2D sprBackground;

        private TextButton CloseButton;
        private DropDownButton[] ArrayPlayerControlDropDown;
        private TextButton[] ArrayPlayerLoadProfileButton;
        private TextButton[] ArrayAddPlayerButton;
        private TextButton[] ArrayRemovePlayerButton;

        private IUIElement[] ArrayUIElement;

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumLightBigger = Content.Load<SpriteFont>("Fonts/Oxanium Light Bigger");

            sprBackground = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Popup/Popup Large");

            ArrayPlayerControlDropDown = new DropDownButton[4];
            ArrayPlayerLoadProfileButton = new TextButton[4];
            ArrayAddPlayerButton = new TextButton[3];
            ArrayRemovePlayerButton = new TextButton[3];

            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(sprBackground.Width * Ratio);
            int MenuHeight = (int)(sprBackground.Height * Ratio);
            int MenuX = Constants.Width / 2 - MenuWidth / 2;
            int MenuY = Constants.Height / 2 - MenuHeight / 2;

            CloseButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}Close}}", "Deathmatch/Lobby Menu/Popup/Button Small Grey", new Vector2((int)(MenuX + MenuWidth - 300 * Ratio), (int)(MenuY + MenuHeight - 150 * Ratio)), 4, 1, Ratio, OnButtonOver, OnConfirmButtonPressed);

            for (int P = 0; P < 4; P++)
            {
                int LocalPlayerIndex = P;
                int ActivePlayerY = (int)(MenuY + 330 * Ratio + P * 120 * Ratio);

                ArrayPlayerControlDropDown[P] = new DropDownButton(Content, "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}M&K}}", 
                    new string[] { "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}M&K}}",
                        "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}Gamepad 1}}",
                        "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}Gamepad 2}}",
                        "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}Gamepad 3}}",
                        "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}Gamepad 4}}" },
                     "Deathmatch/Lobby Menu/Interactive/Button Grey",
                    new Vector2((int)(MenuX + 636 * Ratio), ActivePlayerY), 4, 1, Ratio, OnButtonOver, (SelectedIndex, SelectedItem) => { OnControlChange(LocalPlayerIndex, SelectedIndex, SelectedItem); });

                ArrayPlayerLoadProfileButton[P] = new TextButton(Content, "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}Load}}", "Deathmatch/Lobby Menu/Interactive/Button Grey", new Vector2(MenuX + MenuWidth - 300 * Ratio, ActivePlayerY), 4, 1, Ratio, OnButtonOver, () => { OnLoadProfilePressed(LocalPlayerIndex); });
            }

            for (int P = 0; P < 3; P++)
            {
                int ActivePlayerY = (int)(MenuY + 450 * Ratio + P * 120 * Ratio);

                ArrayAddPlayerButton[P] = new TextButton(Content, "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}Add}}", "Deathmatch/Lobby Menu/Interactive/Button Grey", new Vector2(MenuX + 300 * Ratio, ActivePlayerY), 4, 1, Ratio, OnButtonOver, OnAddPlayerPressed);
                ArrayRemovePlayerButton[P] = new TextButton(Content, "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}Remove}}", "Deathmatch/Lobby Menu/Interactive/Button Grey", new Vector2(MenuX + MenuWidth - 300 * Ratio, ActivePlayerY + 120 * Ratio), 4, 1, Ratio, OnButtonOver, OnRemovePlayerPressed);
            }

            UpdateUIElements();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveUIElement in ArrayUIElement)
            {
                ActiveUIElement.Update(gameTime);
            }
        }

        private void UpdateUIElements()
        {
            if (PlayerManager.ListLocalPlayer.Count == 1)
            {
                ArrayUIElement = new IUIElement[]
                {
                    CloseButton,
                    ArrayPlayerControlDropDown[0],
                    ArrayPlayerLoadProfileButton[0],
                    ArrayAddPlayerButton[0],
                };
            }
            else if (PlayerManager.ListLocalPlayer.Count == 2)
            {
                ArrayUIElement = new IUIElement[]
                {
                    CloseButton,
                    ArrayPlayerControlDropDown[0], ArrayPlayerControlDropDown[1],
                    ArrayPlayerLoadProfileButton[0], ArrayPlayerLoadProfileButton[1],
                    ArrayAddPlayerButton[1],
                    ArrayRemovePlayerButton[0],
                };
            }
            else if (PlayerManager.ListLocalPlayer.Count == 3)
            {
                ArrayUIElement = new IUIElement[]
                {
                    CloseButton,
                    ArrayPlayerControlDropDown[0], ArrayPlayerControlDropDown[1], ArrayPlayerControlDropDown[2],
                    ArrayPlayerLoadProfileButton[0], ArrayPlayerLoadProfileButton[1], ArrayPlayerLoadProfileButton[2],
                    ArrayAddPlayerButton[2],
                    ArrayRemovePlayerButton[1],
                };
            }
            else if (PlayerManager.ListLocalPlayer.Count == 4)
            {
                ArrayUIElement = new IUIElement[]
                {
                    CloseButton,
                    ArrayPlayerControlDropDown[0], ArrayPlayerControlDropDown[1], ArrayPlayerControlDropDown[2], ArrayPlayerControlDropDown[3],
                    ArrayPlayerLoadProfileButton[0], ArrayPlayerLoadProfileButton[1], ArrayPlayerLoadProfileButton[2], ArrayPlayerLoadProfileButton[3],
                    ArrayRemovePlayerButton[2],
                };
            }

            foreach (IUIElement ActiveUIElement in ArrayUIElement)
            {
                ActiveUIElement.Unselect();
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void OnConfirmButtonPressed()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void OnAddPlayerPressed()
        {
            sndButtonClick.Play();

            OnlinePlayerBase NewPlayer = GetNewPlayer();

            PlayerManager.ListLocalPlayer.Add(NewPlayer);
            NewPlayer.LoadLocally(GameScreen.ContentFallback);
            UpdateUIElements();
        }

        protected virtual OnlinePlayerBase GetNewPlayer()
        {
            BattleMapPlayer NewPlayer = new BattleMapPlayer(PlayerManager.OnlinePlayerID, "Player " + (PlayerManager.ListLocalPlayer.Count + 1), OnlinePlayerBase.PlayerTypes.Player, false, 0, true, Color.Blue);
            
            if (!File.Exists("User data/Profiles/" + NewPlayer.SaveFileFolder + NewPlayer.Name + ".bin"))
            {
                NewPlayer.InitFirstTimeInventory();
                NewPlayer.SaveLocally();
            }

            return NewPlayer;
        }

        private void OnRemovePlayerPressed()
        {
            sndButtonClick.Play();
            PlayerManager.ListLocalPlayer.RemoveAt(PlayerManager.ListLocalPlayer.Count - 1);
            UpdateUIElements();
        }

        private void OnControlChange(int LocalPlayerIndex, int SelectedIndex, string SelectedItem)
        {
            sndButtonClick.Play();
        }

        private void OnLoadProfilePressed(int Index)
        {
            sndButtonClick.Play();

            PushScreen(new LocalPlayerProfileSelectionScreen(PlayerManager.ListLocalPlayer[Index]));
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin();
            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(sprBackground.Width * Ratio);
            int MenuHeight = (int)(sprBackground.Height * Ratio);
            float MenuX = Constants.Width / 2 - MenuWidth / 2;
            float MenuY = Constants.Height / 2 - MenuHeight / 2;

            Color WhiteText = Color.FromNonPremultiplied(233, 233, 233, 255);
            Color BlackText = Color.FromNonPremultiplied(65, 70, 65, 255);

            g.Draw(sprBackground, new Vector2(MenuX, MenuY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);

            g.DrawStringRightAligned(fntOxanimumBold, "Local Players Mangement", new Vector2(MenuX + MenuWidth - 40 * Ratio, MenuY + 70 * Ratio), WhiteText);
            g.DrawString(fntOxanimumBold, "Name", new Vector2(MenuX + 120 * Ratio, MenuY + 200 * Ratio), BlackText);
            g.DrawString(fntOxanimumBold, "Controls", new Vector2(MenuX + 500 * Ratio, MenuY + 200 * Ratio), BlackText);
            g.DrawStringMiddleAligned(fntOxanimumBold, "Profile", new Vector2(MenuX + MenuWidth - 300 * Ratio, MenuY + 200 * Ratio), BlackText);

            g.Draw(sprPixel, new Rectangle((int)(MenuX + 100 * Ratio), (int)(MenuY + 280 * Ratio), (int)(1900 * Ratio), (int)(2 * Ratio)), BlackText);
            g.Draw(sprPixel, new Rectangle((int)(MenuX + 100 * Ratio), (int)(MenuY + 750 * Ratio), (int)(1900 * Ratio), (int)(2 * Ratio)), BlackText);

            for (int P = 0; P < PlayerManager.ListLocalPlayer.Count; P++)
            {
                float ActivePlayerY = MenuY + 300 * Ratio + P * 120 * Ratio;
                g.DrawString(fntOxanimumLightBigger, PlayerManager.ListLocalPlayer[P].Name, new Vector2(MenuX + 150 * Ratio, ActivePlayerY + 5), BlackText);
            }

            foreach (IUIElement ActiveUIElement in ArrayUIElement)
            {
                ActiveUIElement.Draw(g);
            }
        }
    }
}
