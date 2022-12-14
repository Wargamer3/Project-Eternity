using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;

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
        private Mutator ActiveMutator;

        private readonly RoomInformations Room;
        private readonly GameOptionsScreen Owner;

        public GameOptionsMutatorsScreen(RoomInformations Room, GameOptionsScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;

            AvailableMutatorsValue = 0;
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

            AvailableMutatorsScrollbar = new BoxScrollbar(new Vector2(LeftPanelX + PanelWidth - 20, PanelY + 30), PanelHeight - 30, 5, OnAvailableMutatorsScrollbarChange);
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
                Room.ListMutator.Add(ListAvailableMutators[SelectedAvailableMutatorsIndex]);
                ListAvailableMutators.RemoveAt(SelectedAvailableMutatorsIndex);
            }
        }

        private void OnAddAllPressed()
        {
            if (SelectedAvailableMutatorsIndex >= 0)
            {
                Owner.sndButtonClick.Play();
                Room.ListMutator.AddRange(ListAvailableMutators);
                ListAvailableMutators.Clear();
            }
        }

        private void OnRemovePressed()
        {
            if (SelectedActiveMutatorsIndex >= 0)
            {
                Owner.sndButtonClick.Play();
                ListAvailableMutators.Add(Room.ListMutator[SelectedActiveMutatorsIndex]);
                Room.ListMutator.RemoveAt(SelectedActiveMutatorsIndex);
            }
        }

        private void OnRemoveAllPressed()
        {
            if (SelectedActiveMutatorsIndex >= 0)
            {
                Owner.sndButtonClick.Play();
                ListAvailableMutators.AddRange(Room.ListMutator);
                Room.ListMutator.Clear();
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
            if (CurrentActiveMutatorsIndex >= 0 && CurrentActiveMutatorsIndex < Room.ListMutator.Count && MouseHelper.InputLeftButtonPressed()
                && MouseHelper.MouseStateCurrent.X >= RightPanelX && MouseHelper.MouseStateCurrent.X < RightPanelX + PanelWidth - 20)
            {
                SelectedActiveMutatorsIndex = CurrentActiveMutatorsIndex;
                ActiveMutator = Room.ListMutator[CurrentActiveMutatorsIndex];
            }

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        public void UpdateMutators()
        {
            ListAvailableMutators = BattleMap.DicBattmeMapType[Room.MapType].Params.DicMutator.Values.ToList();
            Room.ListMutator.Clear();

            foreach (string ActiveMutatorName in Room.ListMandatoryMutator)
            {
                Mutator FoundMutator;

                if (BattleMap.DicBattmeMapType[Room.MapType].Params.DicMutator.TryGetValue(ActiveMutatorName, out FoundMutator))
                {
                    Room.ListMutator.Add(FoundMutator);
                    ListAvailableMutators.Remove(FoundMutator);
                }
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
            for (int M = AvailableMutatorsValue; M < Room.ListMutator.Count; M++)
            {
                Mutator ActiveMutator = Room.ListMutator[M];

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
