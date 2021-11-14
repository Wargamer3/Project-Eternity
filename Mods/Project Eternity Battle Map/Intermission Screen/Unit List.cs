using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnitListScreen : GameScreen
    {
        protected Texture2D sprMapMenuBackground;

        private Texture2D sprBarLargeBackground;
        private Texture2D sprBarLargeEN;
        private Texture2D sprBarLargeHP;

        protected Texture2D sprLand;
        protected Texture2D sprSea;
        protected Texture2D sprSky;
        protected Texture2D sprSpace;

        protected FMODSound sndConfirm;
        protected FMODSound sndSelection;
        protected FMODSound sndDeny;
        protected FMODSound sndCancel;

        protected SpriteFont fntFinlanderFont;
        protected StatusMenuScreen StatusMenu;

        protected readonly Roster PlayerRoster;

        protected int Stage;

        public DrawableMenu UnitSelectionMenu;

        private List<Unit> ListPresentUnit;


        public Unit SelectedUnit { get { return ListPresentUnit[UnitSelectionMenu.SelectedIndex]; } }

        public UnitListScreen(Roster PlayerRoster)
            : base()
        {
            this.PlayerRoster = PlayerRoster;

            Stage = 0;
        }

        public override void Load()
        {
            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

            UnitSelectionMenu = new DrawableMenu(DrawMenuItem, ListPresentUnit.Count, 5);

            sprMapMenuBackground = Content.Load<Texture2D>("Status Screen/Background Black");

            sprBarLargeBackground = Content.Load<Texture2D>("Battle/Bars/Large Bar");
            sprBarLargeEN = Content.Load<Texture2D>("Battle/Bars/Large Energy");
            sprBarLargeHP = Content.Load<Texture2D>("Battle/Bars/Large Health");

            sprLand = Content.Load<Texture2D>("Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Status Screen/Space");

            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            StatusMenu = new StatusMenuScreen(null);
            StatusMenu.Load();
        }

        public override void Update(GameTime gameTime)
        {
            if (Stage == 0)
            {
                if (InputHelper.InputCancelPressed())
                {
                    RemoveScreen(this);
                }
                else if (InputHelper.InputConfirmPressed() && ListPresentUnit.Count > 0)
                {
                    StatusMenu.ActiveSquad = new Squad("", SelectedUnit);
                    StatusMenu.StatusPannel = StatusMenuScreen.StatusPannels.Unit;
                    Stage = 1;
                }
                else
                {
                    UnitSelectionMenu.Update(gameTime);
                }
            }
            else if (Stage == 1)
            {
                if (InputHelper.InputCancelPressed())
                {
                    Stage = 0;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (Stage == 0)
            {
                DrawMenu(g);
                g.DrawString(fntFinlanderFont, "Unit List", new Vector2(120, 10), Color.White);
            }
            else if (Stage == 1)
            {
                StatusMenu.Draw(g);
            }
        }

        public void DrawMenu(CustomSpriteBatch g)
        {
            g.Draw(sprMapMenuBackground, new Vector2(0, 0), Color.White);
            DrawBox(g, new Vector2(10, 45), 420, 300, Color.White);
            g.DrawString(fntFinlanderFont, "Unit", new Vector2(20, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "HP", new Vector2(300, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "EN", new Vector2(400, 50), Color.Yellow);
            DrawBox(g, new Vector2(430, 45), 200, 300, Color.White);
            g.DrawString(fntFinlanderFont, "PILOT", new Vector2(440, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "LV", new Vector2(620, 50), Color.Yellow);

            int LineSpacing = 35;
            UnitSelectionMenu.DrawMenu(g, 20, 80, LineSpacing);

            g.DrawString(fntFinlanderFont, UnitSelectionMenu.CurrentPage + "/" + UnitSelectionMenu.PageCount, new Vector2(420, 10), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(20, 80 + UnitSelectionMenu.SelectedItemIndex * LineSpacing, 601, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
        }

        public void DrawMenuItem(CustomSpriteBatch g, int ItemIndex, int X, int Y)
        {
            Unit ActiveUnit = ListPresentUnit[ItemIndex];
            g.Draw(ActiveUnit.SpriteMap, new Vector2(X, Y), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.ItemName, new Vector2(X + 40, Y), Color.White);
            TextHelper.DrawTextRightAligned(g, ActiveUnit.MaxHP.ToString(), new Vector2(300, Y + 5), Color.White);
            TextHelper.DrawTextRightAligned(g, ActiveUnit.MaxEN.ToString(), new Vector2(400, Y + 5), Color.White);

            if (ActiveUnit.Pilot != null)
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.PilotName, new Vector2(440, Y), Color.White);
                g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotLevel.ToString(), new Vector2(620, Y), Color.White);
            }
        }
    }
}