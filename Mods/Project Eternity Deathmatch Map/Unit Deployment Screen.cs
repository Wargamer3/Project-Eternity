using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using FMOD;
using ProjectEternity.Core.ControlHelper;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class UnitDeploymentScreen
    {
        public enum States { UnitSelection, StatusMenu, SubMenu }
        
        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;

        private FMODSound sndConfirm;
        private FMODSound sndSelection;
        private FMODSound sndDeny;
        private FMODSound sndCancel;

        private SpriteFont fntFinlanderFont;

        private readonly Roster PlayerRoster;

        private StatusMenuScreen StatusMenu;
        public States State;
        public int SubMenuIndex;

        private DrawableMenu UnitSelectionMenu;
        private int MaxNumberOfUnitsToSpawn;

        private List<Unit> ListVisibleUnit;
        public List<Unit> ListSelectedUnit;
        private List<Unit> ListPresentUnit;

        private Unit SelectedUnit { get { return ListVisibleUnit[UnitSelectionMenu.SelectedIndex]; } }

        public UnitDeploymentScreen(Roster PlayerRoster)
            : base()
        {
            this.PlayerRoster = PlayerRoster;
        }

        public void Open(int MaxNumberOfUnitsToSpawn)
        {
            this.MaxNumberOfUnitsToSpawn = MaxNumberOfUnitsToSpawn;
            State = States.UnitSelection;
            SubMenuIndex = 0;

            ListSelectedUnit.Clear();
            ListVisibleUnit.Clear();
            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
            for (int U = 0; U < ListPresentUnit.Count; ++U)
            {
                var ActiveUnit = ListPresentUnit[U];
                if (ActiveUnit != null)
                {
                    ListVisibleUnit.Add(ActiveUnit);
                }
            }
            UnitSelectionMenu = new DrawableMenu(DrawMenuItem, ListVisibleUnit.Count, 5);
        }

        public void Load(ContentManager Content)
        {
            sprLand = Content.Load<Texture2D>("Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Status Screen/Space");

            sndConfirm = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Cancel.mp3");

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            StatusMenu = new StatusMenuScreen(null);
            StatusMenu.Load();

            ListSelectedUnit = new List<Unit>();
            ListVisibleUnit = new List<Unit>();
        }

        public void Update(GameTime gameTime)
        {
            switch (State)
            {
                case States.UnitSelection:
                    if (InputHelper.InputConfirmPressed())
                    {
                        if (ListSelectedUnit.Contains(SelectedUnit))
                        {
                            ListSelectedUnit.Remove(SelectedUnit);
                        }
                        else if (MaxNumberOfUnitsToSpawn - ListSelectedUnit.Count > 0)
                        {
                            ListSelectedUnit.Add(SelectedUnit);
                            if (MaxNumberOfUnitsToSpawn - ListSelectedUnit.Count <= 0 || ListSelectedUnit.Count == ListVisibleUnit.Count)
                            {
                                State = States.SubMenu;
                            }
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        if (ListSelectedUnit.Count > 0)
                        {
                            UndoSelection();
                        }
                    }
                    else if (InputHelper.InputLButtonPressed())
                    {
                        StatusMenu.ActiveSquad = new Squad("", SelectedUnit);
                        StatusMenu.StatusPannel = StatusMenuScreen.StatusPannels.Unit;
                        State = States.StatusMenu;
                    }
                    else
                    {
                        UnitSelectionMenu.Update(gameTime);
                    }
                    break;

                case States.StatusMenu:
                    if (InputHelper.InputCancelPressed())
                    {
                        State = States.UnitSelection;
                    }
                    break;

                case States.SubMenu:
                    if (InputHelper.InputUpPressed())
                    {
                        SubMenuIndex -= (SubMenuIndex > 0) ? 1 : 0;
                        sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ++SubMenuIndex;

                        if (SubMenuIndex >= 4)
                        {
                            SubMenuIndex = 0;
                        }

                        sndSelection.Play();
                    }
                    break;
            }
        }

        public void UndoSelection()
        {
            int LastSelectedUnit = ListVisibleUnit.IndexOf(ListSelectedUnit[ListSelectedUnit.Count - 1]);
            int LastSelectedUnitPage = UnitSelectionMenu.GetItemPage(LastSelectedUnit);
            ListSelectedUnit.Remove(SelectedUnit);
            UnitSelectionMenu.SelectedItemIndex = LastSelectedUnit;
            UnitSelectionMenu.CurrentPage = LastSelectedUnitPage;
        }

        public void Draw(CustomSpriteBatch g)
        {
            switch (State)
            {
                case States.UnitSelection:
                    DrawMenu(g);
                    break;

                case States.StatusMenu:
                    StatusMenu.Draw(g);
                    break;

                case States.SubMenu:
                    DrawMenu(g);
                    DrawSubMenu(g);
                    break;
            }
        }

        private void DrawMenu(CustomSpriteBatch g)
        {
            GameScreen.DrawBox(g, new Vector2(10, 10), 620, 40, Color.White);
            g.DrawString(fntFinlanderFont, "Unit Select", new Vector2(15, 13), Color.White);
            GameScreen.DrawBox(g, new Vector2(10, 45), 620, 355, Color.White);
            g.DrawString(fntFinlanderFont, "Unit", new Vector2(50, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "HP", new Vector2(340, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "EN", new Vector2(400, 50), Color.Yellow);
            g.DrawString(fntFinlanderFont, "PILOT", new Vector2(440, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "LV", new Vector2(620, 50), Color.Yellow);

            int LineSpacing = 36;
            UnitSelectionMenu.DrawMenu(g, 20, 80, LineSpacing);

            g.DrawStringRightAligned(fntFinlanderFont, (MaxNumberOfUnitsToSpawn - ListSelectedUnit.Count) + " Units Left",
                new Vector2(500, 13), Color.White);

            g.DrawStringRightAligned(fntFinlanderFont, "Page " + UnitSelectionMenu.CurrentPage + "/" + UnitSelectionMenu.PageCount,
                new Vector2(620, 13), Color.White);

            g.Draw(GameScreen.sprPixel, new Rectangle(15, 77 + UnitSelectionMenu.SelectedItemIndex * LineSpacing, 611, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));

            GameScreen.DrawBox(g, new Vector2(10, 400), 620, 70, Color.White);
            g.DrawString(fntFinlanderFont, "Move Type", new Vector2(50, 400), Color.Yellow);
            int X = 100 - (SelectedUnit.ListTerrainChoices.Count - 1) * 15;
            if (SelectedUnit.ListTerrainChoices.Contains(UnitStats.TerrainAir))
            {
                g.Draw(sprSky, new Vector2(X, 435), Color.White);
                X += 30;
            }
            if (SelectedUnit.ListTerrainChoices.Contains(UnitStats.TerrainLand))
            {
                g.Draw(sprLand, new Vector2(X, 435), Color.White);
                X += 30;
            }
            if (SelectedUnit.ListTerrainChoices.Contains(UnitStats.TerrainSea))
            {
                g.Draw(sprSea, new Vector2(X, 435), Color.White);
                X += 30;
            }
            if (SelectedUnit.ListTerrainChoices.Contains(UnitStats.TerrainSpace))
            {
                g.Draw(sprSpace, new Vector2(X, 435), Color.White);
                X += 30;
            }

            g.DrawString(fntFinlanderFont, "Terrain Stat", new Vector2(420, 400), Color.Yellow);
            g.Draw(sprSky, new Vector2(400, 435), Color.White);
            g.Draw(sprLand, new Vector2(450, 435), Color.White);
            g.Draw(sprSea, new Vector2(500, 435), Color.White);
            g.Draw(sprSpace, new Vector2(550, 435), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.TerrainLetterAttribute(UnitStats.TerrainAir).ToString(), new Vector2(425, 430), Color.Yellow);
            g.DrawString(fntFinlanderFont, SelectedUnit.TerrainLetterAttribute(UnitStats.TerrainLand).ToString(), new Vector2(475, 430), Color.Yellow);
            g.DrawString(fntFinlanderFont, SelectedUnit.TerrainLetterAttribute(UnitStats.TerrainSea).ToString(), new Vector2(525, 430), Color.Yellow);
            g.DrawString(fntFinlanderFont, SelectedUnit.TerrainLetterAttribute(UnitStats.TerrainSpace).ToString(), new Vector2(575, 430), Color.Yellow);
        }

        private void DrawMenuItem(CustomSpriteBatch g, int ItemIndex, int X, int Y)
        {
            Unit ActiveUnit = ListVisibleUnit[ItemIndex];
            int SelectedUnitIndex = ListSelectedUnit.IndexOf(ActiveUnit);

            g.Draw(GameScreen.sprPixel, new Rectangle(X, Y, 30, 30), Color.Black);
            if (SelectedUnitIndex >= 0)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(X + 1, Y + 1, 28, 28), Color.Orange);
                g.DrawStringMiddleAligned(fntFinlanderFont, SelectedUnitIndex.ToString(), new Vector2(X + 14, Y), Color.White);
            }
            else
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(X + 1, Y + 1, 28, 28), Color.White);
            }
            g.Draw(ActiveUnit.SpriteMap, new Vector2(X + 30, Y), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.RelativePath, new Vector2(X + 70, Y), Color.White);
            TextHelper.DrawTextRightAligned(g, ActiveUnit.MaxHP.ToString(), new Vector2(340, Y), Color.White);
            TextHelper.DrawTextRightAligned(g, ActiveUnit.MaxEN.ToString(), new Vector2(400, Y), Color.White);

            if (ActiveUnit.Pilot != null)
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.PilotName, new Vector2(440, Y), Color.White);
                g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotLevel.ToString(), new Vector2(620, Y), Color.White);
            }
        }

        private void DrawSubMenu(CustomSpriteBatch g)
        {
            int X = (Constants.Width - 300) / 2;
            int Y = 200;
            int LineSpacing = fntFinlanderFont.LineSpacing;

            GameScreen.DrawBox(g, new Vector2(X, Y), 300, 100, Color.White);
            g.DrawString(fntFinlanderFont, "Deploy Units", new Vector2(X += 5, Y += 5), Color.White);
            g.DrawString(fntFinlanderFont, "Change Units", new Vector2(X, Y += fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, "Change Formation", new Vector2(X, Y += fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, "Intermission Screen", new Vector2(X, Y += fntFinlanderFont.LineSpacing), Color.White);

            g.Draw(GameScreen.sprPixel, new Rectangle(X, 210 + SubMenuIndex * LineSpacing, 290, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
        }
    }
}
