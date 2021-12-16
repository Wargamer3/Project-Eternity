using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsMutatorsScreen : GameScreen
    {
        #region Ressources

        private BoxScrollbar AvailableMutatorsScrollbar;
        private BoxScrollbar ActiveMutatorsScrollbar;
        private BoxButton AddButton;
        private BoxButton AddAllButton;
        private BoxButton RemoveButton;
        private BoxButton RemoveAllButton;

        private IUIElement[] ArrayMenuButton;

        private SpriteFont fntText;

        #endregion

        #region UI

        private int PanelY;
        private int PanelWidth;
        private int PanelHeight;

        private int LeftPanelX;
        private int RightPanelX;

        private int ButtonsWidth;
        private int ButtonsHeight;
        private int ButtonsX;
        private int ButtonsY;

        #endregion

        private int AvailableMutatorsValue;
        private int SelectedAvailableMutatorsIndex;
        private int ActiveMutatorsValue;
        private int SelectedActiveMutatorsIndex;

        private List<Mutator> ListAvailableMutators;
        private List<Mutator> ListActiveMutators;
        private Mutator ActiveMutator;

        private readonly RoomInformations Room;
        private readonly GameOptionsScreen Owner;

        public GameOptionsMutatorsScreen(RoomInformations Room, GameOptionsScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;

            ListAvailableMutators = new List<Mutator>();
            ListActiveMutators = new List<Mutator>();

            AvailableMutatorsValue = -1;
            SelectedActiveMutatorsIndex = -1;

            PanelY = (int)(Constants.Height * 0.15);
            PanelWidth = (int)(Constants.Width * 0.37);
            PanelHeight = (int)(Constants.Height * 0.52);

            LeftPanelX = (int)(Constants.Width * 0.03);
            RightPanelX = Constants.Width - LeftPanelX - PanelWidth;

            ButtonsWidth = (int)(Constants.Width * 0.13);
            ButtonsHeight = 30;
            ButtonsX = Constants.Width / 2 - ButtonsWidth / 2;
            ButtonsY = PanelY + (PanelHeight - ButtonsHeight * 7) / 2;
        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");
            for (int i = 0; i < 40; ++i)
            {
                ListAvailableMutators.Add(new Mutator("Test" + i, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum"));
            }
            AvailableMutatorsScrollbar = new BoxScrollbar(new Vector2(LeftPanelX + PanelWidth - 20, PanelY + 30), PanelHeight - 30, ListAvailableMutators.Count, OnAvailableMutatorsScrollbarChange);
            ActiveMutatorsScrollbar = new BoxScrollbar(new Vector2(RightPanelX + PanelWidth - 20, PanelY + 30), PanelHeight - 30, 10, OnActiveMutatorsScrollbarChange);
            
            AddButton = new BoxButton(new Rectangle(ButtonsX, ButtonsY, ButtonsWidth, ButtonsHeight), fntText, "Add", OnButtonOver, OnAddPressed);
            ButtonsY += 40;
            AddAllButton = new BoxButton(new Rectangle(ButtonsX, ButtonsY, ButtonsWidth, ButtonsHeight), fntText, "Add All", OnButtonOver, OnAddAllPressed);
            ButtonsY += 90;
            RemoveButton = new BoxButton(new Rectangle(ButtonsX, ButtonsY, ButtonsWidth, ButtonsHeight), fntText, "Remove", OnButtonOver, OnRemovePressed);
            ButtonsY += 40;
            RemoveAllButton = new BoxButton(new Rectangle(ButtonsX, ButtonsY, ButtonsWidth, ButtonsHeight), fntText, "Remove All", OnButtonOver, OnRemoveAllPressed);

            ArrayMenuButton = new IUIElement[]
            {
                AvailableMutatorsScrollbar, ActiveMutatorsScrollbar,
                AddButton, AddAllButton, RemoveButton, RemoveAllButton,
            };
        }

        private void OnAvailableMutatorsScrollbarChange(float ScrollbarValue)
        {
            AvailableMutatorsValue = (int)ScrollbarValue;
        }

        private void OnActiveMutatorsScrollbarChange(float ScrollbarValue)
        {
            ActiveMutatorsValue = (int)ScrollbarValue;
        }

        private void OnButtonOver()
        {
            Owner.sndButtonOver.Play();
        }

        private void OnAddPressed()
        {
            if (SelectedAvailableMutatorsIndex >= 0)
            {
                Owner.sndButtonClick.Play();
                ListActiveMutators.Add(ListAvailableMutators[SelectedAvailableMutatorsIndex]);
                ListAvailableMutators.RemoveAt(SelectedAvailableMutatorsIndex);
            }
        }

        private void OnAddAllPressed()
        {
            if (SelectedAvailableMutatorsIndex >= 0)
            {
                Owner.sndButtonClick.Play();
                ListActiveMutators.AddRange(ListAvailableMutators);
                ListAvailableMutators.Clear();
            }
        }

        private void OnRemovePressed()
        {
            if (SelectedActiveMutatorsIndex >= 0)
            {
                Owner.sndButtonClick.Play();
                ListAvailableMutators.Add(ListActiveMutators[SelectedActiveMutatorsIndex]);
                ListActiveMutators.RemoveAt(SelectedActiveMutatorsIndex);
            }
        }

        private void OnRemoveAllPressed()
        {
            if (SelectedActiveMutatorsIndex >= 0)
            {
                Owner.sndButtonClick.Play();
                ListAvailableMutators.AddRange(ListActiveMutators);
                ListActiveMutators.Clear();
            }
        }

        public override void Update(GameTime gameTime)
        {
            float DrawY = PanelY + 32;
            int CurrentAllMutatorsIndex = (int)(MouseHelper.MouseStateCurrent.Y - DrawY) / 20 + AvailableMutatorsValue;
            if (CurrentAllMutatorsIndex >= 0 && CurrentAllMutatorsIndex < ListAvailableMutators.Count && MouseHelper.InputLeftButtonPressed()
                && MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth - 20)
            {
                SelectedAvailableMutatorsIndex = CurrentAllMutatorsIndex;
                ActiveMutator = ListAvailableMutators[CurrentAllMutatorsIndex];
            }

            int CurrentActiveMutatorsIndex = (int)(MouseHelper.MouseStateCurrent.Y - DrawY) / 20 + ActiveMutatorsValue;
            if (CurrentActiveMutatorsIndex >= 0 && CurrentActiveMutatorsIndex < ListActiveMutators.Count && MouseHelper.InputLeftButtonPressed()
                && MouseHelper.MouseStateCurrent.X >= RightPanelX && MouseHelper.MouseStateCurrent.X < RightPanelX + PanelWidth - 20)
            {
                SelectedActiveMutatorsIndex = CurrentActiveMutatorsIndex;
                ActiveMutator = ListActiveMutators[CurrentActiveMutatorsIndex];
            }

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(LeftPanelX, PanelY), PanelWidth, PanelHeight, Color.White);
            DrawBox(g, new Vector2(LeftPanelX, PanelY), PanelWidth, 30, Color.White);
            DrawBox(g, new Vector2(LeftPanelX + PanelWidth - 132, PanelY + 3), 127, 24, Color.White);
            g.DrawStringRightAligned(fntText, "Available Mutators", new Vector2(LeftPanelX + PanelWidth - 15, PanelY + 6), Color.White);

            float DrawY = PanelY + 32;
            for (int M = AvailableMutatorsValue; M < ListAvailableMutators.Count; M++)
            {
                Mutator ActiveMutator = ListAvailableMutators[M];

                g.DrawString(fntText, ActiveMutator.Name, new Vector2(LeftPanelX + 5, DrawY), Color.White);

                if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth - 20
                    && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20)
                {
                    g.Draw(sprPixel, new Rectangle(LeftPanelX + 5, (int)DrawY, PanelWidth - 30, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                }

                if (M == SelectedAvailableMutatorsIndex)
                {
                    g.Draw(sprPixel, new Rectangle(LeftPanelX + 5, (int)DrawY, PanelWidth - 30, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                }

                DrawY += 20;

                if (DrawY >= PanelY + PanelHeight)
                {
                    break;
                }
            }

            DrawBox(g, new Vector2(RightPanelX, PanelY), PanelWidth, PanelHeight, Color.White);
            DrawBox(g, new Vector2(RightPanelX, PanelY), PanelWidth, 30, Color.White);
            DrawBox(g, new Vector2(RightPanelX + PanelWidth - 115, PanelY + 3), 110, 24, Color.White);
            g.DrawStringRightAligned(fntText, "Active Mutators", new Vector2(RightPanelX + PanelWidth - 15, PanelY + 6), Color.White);

            DrawY = PanelY + 32;
            for (int M = AvailableMutatorsValue; M < ListActiveMutators.Count; M++)
            {
                Mutator ActiveMutator = ListActiveMutators[M];

                g.DrawString(fntText, ActiveMutator.Name, new Vector2(RightPanelX + 5, DrawY), Color.White);

                if (MouseHelper.MouseStateCurrent.X >= RightPanelX && MouseHelper.MouseStateCurrent.X < RightPanelX + PanelWidth - 20
                    && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20)
                {
                    g.Draw(sprPixel, new Rectangle(RightPanelX + 5, (int)DrawY, PanelWidth - 30, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                }

                if (M == SelectedActiveMutatorsIndex)
                {
                    g.Draw(sprPixel, new Rectangle(RightPanelX + 5, (int)DrawY, PanelWidth - 30, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                }

                DrawY += 20;

                if (DrawY >= PanelY + PanelHeight)
                {
                    break;
                }
            }

            int DescriptionBoxY = PanelY + PanelHeight + 20;
            int DescriptionWidth = (int)(Constants.Width * 0.94);
            int DescriptionHeight = (int)(Constants.Height * 0.2);

            DrawBox(g, new Vector2(LeftPanelX, DescriptionBoxY), DescriptionWidth, DescriptionHeight, Color.White);
            DrawBox(g, new Vector2(LeftPanelX, DescriptionBoxY), DescriptionWidth, 30, Color.White);
            DrawBox(g, new Vector2(LeftPanelX + DescriptionWidth - 115, DescriptionBoxY + 3), 110, 24, Color.White);
            g.DrawStringRightAligned(fntText, "Mutator Details", new Vector2(LeftPanelX + DescriptionWidth - 15, DescriptionBoxY + 6), Color.White);

            if (ActiveMutator != null)
            {
                TextHelper.DrawTextMultiline(g, fntText, TextHelper.FitToWidth(fntText, ActiveMutator.Description, DescriptionWidth - 15),
                    TextHelper.TextAligns.Left, LeftPanelX + DescriptionWidth / 2, DescriptionBoxY + 30, DescriptionWidth - 15);
            }

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }

        public override string ToString()
        {
            return "Mutators";
        }
    }
}
