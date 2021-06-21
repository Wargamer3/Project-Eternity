using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelUnitList : ActionPanelDeathmatch
    {
        private Texture2D sprBarLargeBackground;
        private Texture2D sprBarLargeEN;
        private Texture2D sprBarLargeHP;
        private Texture2D sprMapMenuBackground;

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;

        private SpriteFont fntFinlanderFont;

        private int MapUnitListChoice;
        private int MapUnitListCurrentPage;
        private int MapUnitListCurrentPageMax;
        private const int UnitListMaxPerPage = 5;
        private List<Tuple<Unit, Vector3>> ListMapMenuUnitPosition;

        public ActionPanelUnitList(DeathmatchMap Map,
            Texture2D sprBarLargeBackground, Texture2D sprBarLargeEN, Texture2D sprBarLargeHP, Texture2D sprMapMenuBackground,
            Texture2D sprLand, Texture2D sprSea, Texture2D sprSky, Texture2D sprSpace,
            SpriteFont fntFinlanderFont)
            : base("Unit List", Map, false)
        {
            this.sprBarLargeBackground = sprBarLargeBackground;
            this.sprBarLargeEN = sprBarLargeEN;
            this.sprBarLargeHP = sprBarLargeHP;
            this.sprMapMenuBackground = sprMapMenuBackground;

            this.sprLand = sprLand;
            this.sprSea = sprSea;
            this.sprSky = sprSky;
            this.sprSpace = sprSpace;

            this.fntFinlanderFont = fntFinlanderFont;

            ListMapMenuUnitPosition = new List<Tuple<Unit, Vector3>>();
        }

        public override void OnSelect()
        {
            MapUnitListChoice = 0;
            ListMapMenuUnitPosition.Clear();
            foreach (Squad ActiveSquad in Map.ListPlayer[Map.ActivePlayerIndex].ListSquad)
            {
                ListMapMenuUnitPosition.Add(new Tuple<Unit, Vector3>(ActiveSquad.CurrentLeader, ActiveSquad.Position));
                if (ActiveSquad.CurrentWingmanA != null)
                    ListMapMenuUnitPosition.Add(new Tuple<Unit, Vector3>(ActiveSquad.CurrentWingmanA, ActiveSquad.Position));
                if (ActiveSquad.CurrentWingmanB != null)
                    ListMapMenuUnitPosition.Add(new Tuple<Unit, Vector3>(ActiveSquad.CurrentWingmanB, ActiveSquad.Position));
            }
            MapUnitListCurrentPageMax = (int)Math.Ceiling(ListMapMenuUnitPosition.Count / (double)UnitListMaxPerPage);
            MapUnitListCurrentPage = 1;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputCancelPressed())
            {
                RemoveFromPanelList(this);
            }
            else if (InputHelper.InputConfirmPressed())
            {
                Map.CursorPosition = ListMapMenuUnitPosition[MapUnitListChoice].Item2;
                Map.CursorPositionVisible = Map.CursorPosition;
                Map.PushScreen(new BattleMapScreen.CenterOnSquadCutscene(Map.CenterCamera, Map, Map.CursorPosition));
            }
            else if (InputHelper.InputUpPressed())
            {
                MapUnitListChoice -= (MapUnitListChoice > 0) ? 1 : 0;
            }
            else if (InputHelper.InputDownPressed())
            {
                ++MapUnitListChoice;
                if (MapUnitListChoice >= UnitListMaxPerPage)
                    MapUnitListChoice = UnitListMaxPerPage - 1;
                else if ((MapUnitListCurrentPage - 1) * UnitListMaxPerPage + MapUnitListChoice >= ListMapMenuUnitPosition.Count)
                    MapUnitListChoice = (ListMapMenuUnitPosition.Count - 1) % UnitListMaxPerPage;
            }
            else if (InputHelper.InputLeftPressed())
            {
                MapUnitListCurrentPage -= (MapUnitListCurrentPage > 1) ? 1 : 0;
            }
            else if (InputHelper.InputRightPressed())
            {
                MapUnitListCurrentPage += (MapUnitListCurrentPage < MapUnitListCurrentPageMax) ? 1 : 0;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprMapMenuBackground, new Vector2(0, 0), Color.White);
            Unit ActiveUnit;
            int LineSpacing = 40;
            GameScreen.DrawBox(g, new Vector2(10, 5), 300, 40, Color.Black);
            g.DrawString(fntFinlanderFont, "Unit List", new Vector2(120, 10), Color.White);
            g.DrawString(fntFinlanderFont, MapUnitListCurrentPage + "/" + MapUnitListCurrentPageMax, new Vector2(420, 10), Color.White);
            GameScreen.DrawBox(g, new Vector2(10, 45), 620, 300, Color.White);
            g.DrawString(fntFinlanderFont, "Unit Name", new Vector2(70, 50), Color.Yellow);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(400, 50), Color.Yellow);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(510, 50), Color.Yellow);

            int UnitIndex = (MapUnitListCurrentPage - 1) * UnitListMaxPerPage;
            for (int U = 0; U + UnitIndex < ListMapMenuUnitPosition.Count && U < UnitListMaxPerPage; U++)
            {
                ActiveUnit = ListMapMenuUnitPosition[U + UnitIndex].Item1;
                g.Draw(ActiveUnit.SpriteMap, new Vector2(70, 90 + U * LineSpacing), Color.White);
                g.DrawString(fntFinlanderFont, ActiveUnit.RelativePath, new Vector2(110, 85 + U * LineSpacing), Color.White);
                GameScreen.DrawBar(g, sprBarLargeBackground, sprBarLargeHP, new Vector2(380, 100 + U * LineSpacing), ActiveUnit.HP, ActiveUnit.MaxHP);
                GameScreen.DrawTextRightAligned(g, ActiveUnit.HP + "/" + ActiveUnit.MaxHP, new Vector2(485, 95 + U * LineSpacing), Color.White);
                GameScreen.DrawBar(g, sprBarLargeBackground, sprBarLargeEN, new Vector2(505, 100 + U * LineSpacing), ActiveUnit.EN, ActiveUnit.MaxEN);
                GameScreen.DrawTextRightAligned(g, ActiveUnit.EN + "/" + ActiveUnit.MaxEN, new Vector2(610, 95 + U * LineSpacing), Color.White);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle(30, 75 + MapUnitListChoice * LineSpacing, 590, 45), Color.FromNonPremultiplied(255, 255, 255, 127));

            GameScreen.DrawBox(g, new Vector2(10, 395), 620, 80, Color.White);
            ActiveUnit = ListMapMenuUnitPosition[MapUnitListChoice].Item1;
            g.Draw(ActiveUnit.SpriteMap, new Vector2(20, 405), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.RelativePath, new Vector2(60, 400), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.PilotName, new Vector2(305, 400), Color.White);
            g.DrawString(fntFinlanderFont, "Pilot", new Vector2(250, 400), Color.Yellow);
            g.DrawString(fntFinlanderFont, "Lvl", new Vector2(445, 400), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.PilotLevel.ToString(), new Vector2(490, 400), Color.White);
            g.DrawString(fntFinlanderFont, "Will", new Vector2(530, 400), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.PilotMorale.ToString(), new Vector2(580, 400), Color.White);

            g.DrawString(fntFinlanderFont, "HP", new Vector2(15, 430), Color.Yellow);
            GameScreen.DrawBar(g, sprBarLargeBackground, sprBarLargeHP, new Vector2(80, 440), ActiveUnit.HP, ActiveUnit.MaxHP);
            GameScreen.DrawTextRightAligned(g, ActiveUnit.HP + "/" + ActiveUnit.MaxHP, new Vector2(185, 435), Color.White);

            g.DrawString(fntFinlanderFont, "EN", new Vector2(200, 430), Color.Yellow);
            GameScreen.DrawBar(g, sprBarLargeBackground, sprBarLargeEN, new Vector2(255, 440), ActiveUnit.EN, ActiveUnit.MaxEN);
            GameScreen.DrawTextRightAligned(g, ActiveUnit.EN + "/" + ActiveUnit.MaxEN, new Vector2(360, 435), Color.White);

            g.DrawString(fntFinlanderFont, "MV", new Vector2(375, 430), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.MaxMovement.ToString(), new Vector2(420, 430), Color.White);
            int CurrentX = 445;

            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainAir))
            {
                g.Draw(sprSky, new Vector2(CurrentX, 435), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainLand))
            {
                g.Draw(sprLand, new Vector2(CurrentX, 435), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainSea))
            {
                g.Draw(sprSea, new Vector2(CurrentX, 435), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainSpace))
            {
                g.Draw(sprSpace, new Vector2(CurrentX, 435), Color.White);
                CurrentX += 50;
            }
        }
    }
}
