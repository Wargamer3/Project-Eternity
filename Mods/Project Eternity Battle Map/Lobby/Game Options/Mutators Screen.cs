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

        private Scrollbar AvailableMutatorsScrollbar;
        private Scrollbar ActiveMutatorsScrollbar;
        private TextButton AddButton;
        private TextButton AddAllButton;
        private TextButton RemoveButton;
        private TextButton RemoveAllButton;

        private IUIElement[] ArrayMenuButton;

        private SpriteFont fntText;
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntOxanimumBold;

        private Texture2D sprHighlight;

        private Texture2D sprFrameTop;
        private Texture2D sprFrameDescription;
        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        #endregion

        #region UI

        private int PanelY;
        private int PanelWidth;
        private int PanelHeight;

        private int LeftPanelX;
        private int RightPanelX;

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

            float Ratio = Constants.Height / 2160f;

            ListAvailableMutators = new List<Mutator>();
            AvailableMutatorsValue = 0;
            SelectedActiveMutatorsIndex = -1;

            PanelWidth = (int)(Constants.Width * 0.37);
            PanelHeight = (int)(Constants.Height * 0.52);

            LeftPanelX = (int)(394 * Ratio);
            PanelY = (int)(542 * Ratio);
            RightPanelX = (int)(2278 * Ratio);

        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");

            sprHighlight = Content.Load<Texture2D>("Menus/Lobby/Room/Select Highlight");

            sprFrameTop = Content.Load<Texture2D>("Menus/Lobby/Room/Frame Top");
            sprFrameDescription = Content.Load<Texture2D>("Menus/Lobby/Room/Frame Description");
            sprScrollbarBackground = Content.Load<Texture2D>("Menus/Lobby/Room/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Menus/Lobby/Room/Scrollbar Bar");
            float Ratio = Constants.Height / 2160f;

            AvailableMutatorsScrollbar = new Scrollbar(sprScrollbar, new Vector2(LeftPanelX + PanelWidth - 20, PanelY + 30), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 5, OnAvailableMutatorsScrollbarChange);
            ActiveMutatorsScrollbar = new Scrollbar(sprScrollbar, new Vector2(RightPanelX + PanelWidth - 20, PanelY + 30), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnActiveMutatorsScrollbarChange);

            int ButtonsX = (int)(1912 * Ratio);
            int ButtonsY = (int)(770 * Ratio);
            AddButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Add}}", "Menus/Lobby/Room/Button Small", new Vector2(ButtonsX, ButtonsY), 4, 1, Ratio, OnButtonOver, OnAddPressed);
            ButtonsY += (int)(132 * Ratio);
            AddAllButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Add All}}", "Menus/Lobby/Room/Button Small", new Vector2(ButtonsX, ButtonsY), 4, 1, Ratio, OnButtonOver, OnAddAllPressed);
            ButtonsY += (int)(306 * Ratio);
            RemoveButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Remove}}", "Menus/Lobby/Room/Button Small", new Vector2(ButtonsX, ButtonsY), 4, 1, Ratio, OnButtonOver, OnRemovePressed);
            ButtonsY += (int)(132 * Ratio);
            RemoveAllButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Remove All}}", "Menus/Lobby/Room/Button Small", new Vector2(ButtonsX, ButtonsY), 4, 1, Ratio, OnButtonOver, OnRemoveAllPressed);

            ArrayMenuButton = new IUIElement[]
            {
                //AvailableMutatorsScrollbar, ActiveMutatorsScrollbar,
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
            ListAvailableMutators = BattleMap.DicBattmeMapType[Room.MapModName].Params.DicMutator.Values.ToList();
            Room.ListMutator.Clear();

            foreach (string ActiveMutatorName in Room.ListMandatoryMutator)
            {
                Mutator FoundMutator;

                if (BattleMap.DicBattmeMapType[Room.MapModName].Params.DicMutator.TryGetValue(ActiveMutatorName, out FoundMutator))
                {
                    Room.ListMutator.Add(FoundMutator);
                    ListAvailableMutators.Remove(FoundMutator);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);
            int DrawX = LeftPanelX;
            int DrawY = PanelY;
            int BoxHeight = (int)(994 * Ratio);
            g.DrawStringCentered(fntOxanimumBold, "Available Mutators", new Vector2(DrawX + (sprFrameTop.Width / 2) * Ratio, DrawY), ColorText);

            DrawY += (int)(42 * Ratio);
            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.Draw(sprPixel, new Rectangle((int)(DrawX), (int)(DrawY + sprFrameTop.Height * Ratio), (int)(sprFrameTop.Width * Ratio), BoxHeight), ColorBox);
            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY + sprFrameTop.Height * Ratio + BoxHeight), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.FlipVertically, 0.9f);
            

            DrawY = PanelY + 32;
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

            DrawX = RightPanelX;
            DrawY = PanelY;
            g.DrawStringCentered(fntOxanimumBold, "Active Mutators", new Vector2(DrawX + (sprFrameTop.Width / 2) * Ratio, DrawY), ColorText);

            DrawY += (int)(42 * Ratio);
            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.Draw(sprPixel, new Rectangle((int)(DrawX), (int)(DrawY + sprFrameTop.Height * Ratio), (int)(sprFrameTop.Width * Ratio), BoxHeight), ColorBox);
            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY + sprFrameTop.Height * Ratio + BoxHeight), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.FlipVertically, 0.9f);

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

            int DescriptionBoxY = (int)(1650 * Ratio);
            int DescriptionWidth = (int)(sprFrameDescription.Width * Ratio * 0.95f);
            int DescriptionHeight = (int)(sprFrameDescription.Height * Ratio * 0.95f);
            DrawX = LeftPanelX;

            g.Draw(sprFrameDescription, new Vector2(DrawX, DescriptionBoxY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringRightAligned(fntOxanimumBold, "Mutator Details", new Vector2(3300 * Ratio, 1690 * Ratio), ColorText);

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
