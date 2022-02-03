using System;
using System.IO;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class LocalPlayerSelectionScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private BoxButton CloseButton;
        private DropDownButton[] ArrayPlayerControlDropDown;
        private BoxButton[] ArrayPlayerLoadProfileButton;
        private BoxButton[] ArrayAddPlayerButton;
        private BoxButton[] ArrayRemovePlayerButton;

        private IUIElement[] ArrayUIElement;

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            ArrayPlayerControlDropDown = new DropDownButton[4];
            ArrayPlayerLoadProfileButton = new BoxButton[4];
            ArrayAddPlayerButton = new BoxButton[3];
            ArrayRemovePlayerButton = new BoxButton[3];

            int MenuWidth = (int)(Constants.Width * 0.45);
            int MenuHeight = (int)(Constants.Height * 0.45);
            int MenuX = Constants.Width / 2 - MenuWidth / 2;
            int MenuY = Constants.Height / 2 - MenuHeight / 2;

            CloseButton = new BoxButton(new Rectangle((int)MenuX + 10, (int)MenuY + MenuHeight + 5, 70, 30), fntArial12, "Confirm", OnButtonOver, OnConfirmButtonPressed);

            for (int P = 0; P < 4; P++)
            {
                int LocalPlayerIndex = P;
                int ActivePlayerY = MenuY + 75 + P * 40;

                ArrayPlayerControlDropDown[P] = new DropDownButton(new Rectangle(MenuX + 150, ActivePlayerY, 95, 30), fntArial12, "M&K",
                    new string[] { "M&K", "Gamepad 1", "Gamepad 2", "Gamepad 3", "Gamepad 4" }, OnButtonOver, () => { OnPlayerControlChange(LocalPlayerIndex); });

                ArrayPlayerLoadProfileButton[P] = new BoxButton(new Rectangle(MenuX + MenuWidth - 70, ActivePlayerY, 60, 30), fntArial12, "Load", OnButtonOver, () => { OnLoadProfilePressed(LocalPlayerIndex); });
            }

            for (int P = 0; P < 3; P++)
            {
                int ActivePlayerY = MenuY + 115 + P * 40;

                ArrayAddPlayerButton[P] = new BoxButton(new Rectangle(MenuX + 10, ActivePlayerY, 60, 30), fntArial12, "Add", OnButtonOver, OnAddPlayerPressed);
                ArrayRemovePlayerButton[P] = new BoxButton(new Rectangle(MenuX + MenuWidth - 70, ActivePlayerY + 40, 60, 30), fntArial12, "Remove", OnButtonOver, OnRemovePlayerPressed);
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

            BattleMapPlayer NewPlayer = new BattleMapPlayer(PlayerManager.OnlinePlayerID, "Player " + (PlayerManager.ListLocalPlayer.Count + 1), BattleMapPlayer.PlayerTypes.Online, false, 0, true, Color.Blue);

            if (!File.Exists("User data/Profiles/Battle Map/" + NewPlayer.Name + ".bin"))
            {
                NewPlayer.InitFirstTimeInventory();
                NewPlayer.SaveLocally();
            }

            PlayerManager.ListLocalPlayer.Add(NewPlayer);
            NewPlayer.LoadLocally(GameScreen.ContentFallback);
            UpdateUIElements();
        }

        private void OnRemovePlayerPressed()
        {
            sndButtonClick.Play();
            PlayerManager.ListLocalPlayer.RemoveAt(PlayerManager.ListLocalPlayer.Count - 1);
            UpdateUIElements();
        }

        private void OnPlayerControlChange(int Index)
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
            int MenuWidth = (int)(Constants.Width * 0.45);
            int MenuHeight = (int)(Constants.Height * 0.45);
            float MenuX = Constants.Width / 2 - MenuWidth / 2;
            float MenuY = Constants.Height / 2 - MenuHeight / 2;

            DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);

            DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, 40, Color.White);
            g.DrawString(fntArial12, "Local Players Mangement", new Vector2(MenuX + 10, MenuY + 10), Color.White);
            g.DrawString(fntArial12, "Name", new Vector2(MenuX + 10, MenuY + 42), Color.White);
            g.DrawString(fntArial12, "Controls", new Vector2(MenuX + 150, MenuY + 42), Color.White);
            g.DrawString(fntArial12, "Profile", new Vector2(MenuX + MenuWidth - 60, MenuY + 42), Color.White);

            for (int P = 0; P < PlayerManager.ListLocalPlayer.Count; P++)
            {
                float ActivePlayerY = MenuY + 75 + P * 40;
                g.DrawString(fntArial12, PlayerManager.ListLocalPlayer[P].Name, new Vector2(MenuX + 10, ActivePlayerY + 5), Color.White);
            }

            DrawBox(g, new Vector2(MenuX, MenuY + MenuHeight), MenuWidth, 40, Color.White);

            foreach (IUIElement ActiveUIElement in ArrayUIElement)
            {
                ActiveUIElement.Draw(g);
            }
        }
    }
}
