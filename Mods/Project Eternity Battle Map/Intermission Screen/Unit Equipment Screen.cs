using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.MultiForm;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    //http://imgur.com/a/Y2kQO
    public sealed class UnitEquipmentScreen : GameScreen
    {
        private Texture2D sprMapMenuBackground;

        private Texture2D sprBarLargeBackground;
        private Texture2D sprBarLargeEN;
        private Texture2D sprBarLargeHP;

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

        private int Stage;
        private int LineSpacing = 35;

        private int CurrentMaxHP;
        private int CurrentMaxEN;
        private int CurrentMaxArmor;
        private int CurrentMaxMobility;
        private int CurrentMaxMV;
        private Dictionary<byte, byte> DicTerrainLetterAttribute;
        private List<UnitMultiForm> ListUnit;

        private DrawableMenu UnitSelectionMenu;
        private DrawableMenu EquipmentSelectionMenu;

        private List<Unit> ListPresentUnit;

        public UnitMultiForm SelectedUnit { get { return ListUnit[UnitSelectionMenu.SelectedIndex]; } }

        public UnitEquipmentScreen(Roster PlayerRoster)
            : base()
        {
            this.PlayerRoster = PlayerRoster;

            Stage = 0;
            DicTerrainLetterAttribute = new Dictionary<byte, byte>();
        }

        public override void Load()
        {
            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

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

            ListUnit = new List<UnitMultiForm>();
            for (int U = 0; U < ListPresentUnit.Count; ++U)
            {
                var ActiveUnit = ListPresentUnit[U] as UnitMultiForm;
                if (ActiveUnit != null)
                {
                    ListUnit.Add(ActiveUnit);
                }
            }

            UnitSelectionMenu = new DrawableMenu(DrawMenuUnit, ListUnit.Count, 5);
        }

        public override void Update(GameTime gameTime)
        {
            switch (Stage)
            {
                case 0:
                    if (InputHelper.InputCancelPressed())
                    {
                        RemoveScreen(this);
                    }
                    else if (InputHelper.InputConfirmPressed() && ListPresentUnit.Count > 0)
                    {
                        Stage = 1;
                        EquipmentSelectionMenu = new DrawableMenu(DrawMenuEquipment, SelectedUnit.ArrayUnitStat.Length, 5);
                        GoToPartChange();
                    }
                    else
                    {
                        UnitSelectionMenu.Update(gameTime);
                    }
                    break;

                case 1:
                    if (InputHelper.InputConfirmPressed())
                    {
                        GoToPartChange();
                        Stage = 0;
                    }
                    else
                    {
                        EquipmentSelectionMenu.Update(gameTime);
                    }

                    if (InputHelper.InputUpPressed())
                    {
                        SelectedUnit.ChangeUnit(EquipmentSelectionMenu.SelectedItemIndex);
                        UpdatePartsEffects();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        SelectedUnit.ChangeUnit(EquipmentSelectionMenu.SelectedItemIndex);
                        UpdatePartsEffects();
                    }
                    break;
            }
        }

        private void UpdatePartsEffects()
        {
            SelectedUnit.ResetBoosts();
            SelectedUnit.ActivePassiveBuffs();
        }

        private void GoToPartChange()
        {
            CurrentMaxHP = SelectedUnit.MaxHP;
            CurrentMaxEN = SelectedUnit.MaxEN;
            CurrentMaxArmor = SelectedUnit.Armor;
            CurrentMaxMobility = SelectedUnit.Mobility;
            CurrentMaxMV = SelectedUnit.MaxMovement;

            DicTerrainLetterAttribute = new Dictionary<byte, byte>(SelectedUnit.DicRankByMovement);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            g.DrawString(fntFinlanderFont, "Unit Parts", new Vector2(10, 10), Color.White);

            switch (Stage)
            {
                case 0:
                    DrawMenu(g);
                    g.DrawString(fntFinlanderFont, "Unit List", new Vector2(120, 10), Color.White);
                    break;

                case 1:
                    DrawPartMenu(g, false);
                    break;
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

            UnitSelectionMenu.DrawMenu(g, 20, 80, LineSpacing);
            g.DrawString(fntFinlanderFont, UnitSelectionMenu.CurrentPage + "/" + UnitSelectionMenu.PageCount, new Vector2(420, 10), Color.White);
            g.Draw(BattleMap.sprPixel, new Rectangle(20, 80 + UnitSelectionMenu.SelectedItemIndex * LineSpacing, 601, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
        }

        private void DrawUnitInfo(CustomSpriteBatch g, int StartX)
        {
            g.DrawString(fntFinlanderFont, SelectedUnit.RelativePath, new Vector2(StartX + 15, 40), Color.White);
            g.Draw(SelectedUnit.SpriteUnit, new Vector2(StartX + 30, 100), Color.White);

            int Y = 155 + LineSpacing * 4;

            Y += 15;
            DrawBox(g, new Vector2(StartX + 10, Y + LineSpacing), 310, 215, Color.White);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(StartX + 15, Y += LineSpacing), Color.White);
            DrawStatChange(g, CurrentMaxHP, SelectedUnit.MaxHP, StartX + 150, Y);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(StartX + 15, Y += LineSpacing), Color.White);
            DrawStatChange(g, CurrentMaxEN, SelectedUnit.MaxEN, StartX + 150, Y);
            g.DrawString(fntFinlanderFont, "Armor", new Vector2(StartX + 15, Y += LineSpacing), Color.White);
            DrawStatChange(g, CurrentMaxArmor, SelectedUnit.Armor, StartX + 150, Y);
            g.DrawString(fntFinlanderFont, "Mobility", new Vector2(StartX + 15, Y += LineSpacing), Color.White);
            DrawStatChange(g, CurrentMaxMobility, SelectedUnit.Mobility, StartX + 150, Y);
            g.DrawString(fntFinlanderFont, "MV", new Vector2(StartX + 15, Y += LineSpacing), Color.White);
            DrawStatChange(g, CurrentMaxMV, SelectedUnit.MaxMovement, StartX + 150, Y);

            g.DrawString(fntFinlanderFont, "Move Type", new Vector2(StartX + 15, Y += LineSpacing), Color.White);
            if (DicTerrainLetterAttribute.ContainsKey(UnitStats.TerrainAirIndex))
                g.Draw(sprSky, new Vector2(StartX + 150, Y + 7), Color.White);
            else
                g.Draw(sprLand, new Vector2(StartX + 150, Y + 7), Color.White);

            if (SelectedUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainAirIndex))
                g.Draw(sprSky, new Vector2(StartX + 230, Y + 7), Color.White);
            else
                g.Draw(sprLand, new Vector2(StartX + 230, Y + 7), Color.White);

            g.DrawString(fntFinlanderFont, "Terrain", new Vector2(StartX + 15, Y += LineSpacing), Color.White);

            DrawTerrainChange(g, UnitStats.TerrainAirIndex, sprSky, StartX + 40, Y + 28);
            DrawTerrainChange(g, UnitStats.TerrainLandIndex, sprLand, StartX + 90, Y + 28);
            DrawTerrainChange(g, UnitStats.TerrainSeaIndex, sprSea, StartX + 140, Y + 28);
            DrawTerrainChange(g, UnitStats.TerrainSpaceIndex, sprSpace, StartX + 190, Y + 28);
        }

        private void DrawPartMenu(CustomSpriteBatch g, bool ShowListPartCursor)
        {
            DrawUnitInfo(g, 0);
            DrawBox(g, new Vector2(325, 260), 310, 215, Color.White);

            EquipmentSelectionMenu.DrawMenu(g, 330, 260, 18);

            g.DrawString(fntFinlanderFont, EquipmentSelectionMenu.CurrentPage + "/" + EquipmentSelectionMenu.PageCount,
                new Vector2(420, 210), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(330, 265+ EquipmentSelectionMenu.SelectedItemIndex * fntFinlanderFont.LineSpacing, 300, fntFinlanderFont.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
        }

        private void DrawStatChange(CustomSpriteBatch g, int OldStat, int NewStat, int X, int Y)
        {
            Color DrawColor = Color.White;
            g.DrawString(fntFinlanderFont, OldStat.ToString(), new Vector2(X, Y), Color.White);
            if (NewStat < OldStat)
                DrawColor = Color.Red;
            else if (NewStat > OldStat)
                DrawColor = Color.Green;

            g.DrawString(fntFinlanderFont, NewStat.ToString(), new Vector2(X + 80, Y), DrawColor);
        }

        private void DrawTerrainChange(CustomSpriteBatch g, byte TerrainIndex, Texture2D Sprite, int X, int Y)
        {
            g.Draw(Sprite, new Vector2(X, Y), Color.White);
            g.DrawString(fntFinlanderFont, DicTerrainLetterAttribute[TerrainIndex].ToString(), new Vector2(X + 25, Y + 4), Color.White);

            g.Draw(Sprite, new Vector2(X, Y + 46), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.TerrainLetterAttribute(TerrainIndex).ToString(), new Vector2(X + 25, Y + 50), Color.White);
        }

        public void DrawMenuUnit(CustomSpriteBatch g, int ItemIndex, int X, int Y)
        {
            Unit ActiveUnit = SelectedUnit;
            g.Draw(ActiveUnit.SpriteMap, new Vector2(X, Y), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.RelativePath, new Vector2(X + 40, Y), Color.White);
            TextHelper.DrawTextRightAligned(g, ActiveUnit.MaxHP.ToString(), new Vector2(300, Y), Color.White);
            TextHelper.DrawTextRightAligned(g, ActiveUnit.MaxEN.ToString(), new Vector2(400, Y), Color.White);

            if (ActiveUnit.Pilot != null)
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.PilotName, new Vector2(440, Y), Color.White);
                g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotLevel.ToString(), new Vector2(620, Y), Color.White);
            }
        }

        public void DrawMenuEquipment(CustomSpriteBatch g, int ItemIndex, int X, int Y)
        {
            UnitMultiForm.EquipmentInformations ActiveEquipement = SelectedUnit.ArrayUnitStat[ItemIndex];

            g.DrawString(fntFinlanderFont, ActiveEquipement.EquipmentName, new Vector2(X, Y), Color.White);
        }
    }
}
