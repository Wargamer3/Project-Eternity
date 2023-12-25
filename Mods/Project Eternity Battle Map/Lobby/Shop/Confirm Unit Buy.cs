using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.UI;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopConfirmBuyUnitScreen : GameScreen
    {
        private EmptyBoxButton BuyButton;
        private EmptyBoxButton CancelButton;

        private IUIElement[] ArrayUIElement;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntFinlanderFont;

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;

        private BattleMapPlayerInventory Inventory;
        private UnlockableUnit UnitToBuy;

        private double AnimationProgression;

        int MenuSizeX;
        int MenuSizeY;
        int DrawX;
        int DrawY;

        public ShopConfirmBuyUnitScreen(BattleMapPlayerInventory Inventory, UnlockableUnit UnitToBuy)
        {
            this.Inventory = Inventory;
            this.UnitToBuy = UnitToBuy;

            MenuSizeX = Constants.Width / 3;
            MenuSizeY = Constants.Height / 2;
            DrawX = Constants.Width / 2 - MenuSizeX / 2;
            DrawY = MenuSizeY / 2;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            sprLand = Content.Load<Texture2D>("Menus/Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Menus/Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Menus/Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Menus/Status Screen/Space");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            BuyButton = new EmptyBoxButton(new Rectangle(DrawX + 50, DrawY + MenuSizeY - 100, 100, 50), fntArial12, "BUY", OnButtonOver, BuyUnit);
            CancelButton = new EmptyBoxButton(new Rectangle(DrawX + MenuSizeX - 150, DrawY + MenuSizeY - 100, 100, 50), fntArial12, "CANCEL", OnButtonOver, Cancel);

            ArrayUIElement = new IUIElement[]
            {
                BuyButton, CancelButton,
            };
        }

        public override void Update(GameTime gameTime)
        {
            AnimationProgression += gameTime.ElapsedGameTime.TotalSeconds * 2d;
            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void BuyUnit()
        {
            sndButtonClick.Play();
            if (Inventory.DicOwnedUnit.ContainsKey(UnitToBuy.UnitToBuy.RelativePath))
            {
                Inventory.DicOwnedUnit[UnitToBuy.UnitToBuy.RelativePath].QuantityOwned++;
            }
            else
            {
                Inventory.DicOwnedUnit.Add(UnitToBuy.UnitToBuy.RelativePath, new UnitInfo(UnitToBuy.UnitToBuy, 1));
            }

            RemoveScreen(this);
        }


        private void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }


        public override void Draw(CustomSpriteBatch g)
        {
            int MenuSizeX = 760;
            int MenuSizeY = Constants.Height / 2;
            int DrawX = Constants.Width / 2 - MenuSizeX / 2;
            int DrawY = MenuSizeY / 2;

            g.Draw(sprPixel, new Rectangle(DrawX, DrawY, MenuSizeX, MenuSizeY),
                Color.FromNonPremultiplied(
                    (int)(ShopScreen.BackgroundColor.R * 0.9),
                    (int)(ShopScreen.BackgroundColor.G * 0.9),
                    (int)(ShopScreen.BackgroundColor.B * 0.9), 240));

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), MenuSizeX, MenuSizeY, 7, MenuSizeX / 21, AnimationProgression);

            g.DrawStringCentered(fntArial12, "BUY A UNIT", new Vector2(DrawX + MenuSizeX / 2, DrawY + 25), Color.White);

            g.Draw(sprPixel, new Rectangle((int)(DrawX + MenuSizeX * 0.05f), DrawY + 50, (int)(MenuSizeX * 0.9f), 1), Color.White);

            g.Draw(UnitToBuy.UnitToBuy.SpriteMap, new Vector2(DrawX + MenuSizeX / 2 - UnitToBuy.UnitToBuy.SpriteMap.Width / 2, DrawY + 70), Color.White);

            g.DrawStringCentered(fntArial12, UnitToBuy.UnitToBuy.ItemName.ToUpper(), new Vector2(DrawX + MenuSizeX / 2, DrawY + 135), Color.White);

            g.DrawStringCentered(fntArial12, UnitToBuy.UnitToBuy.Price + " cr", new Vector2(DrawX + MenuSizeX / 2, DrawY + 175), Color.White);

            DrawSelectedUnitStats(g, DrawX + 20, DrawY + 220, UnitToBuy.UnitToBuy);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }

        private void DrawSelectedUnitStats(CustomSpriteBatch g, int DrawX, int DrawY, Unit ActiveUnit)
        {
            int BottomHeight = 182;
            DrawEmptyBox(g, new Vector2(DrawX, DrawY - 5), 240, BottomHeight);

            int DistanceBetweenText = 16;
            int MenuOffset = (int)(DistanceBetweenText * 0.5);

            #region Stats

            g.DrawString(fntFinlanderFont, "HP", new Vector2(DrawX + 15, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxHP.ToString(), new Vector2(DrawX + 222, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.White);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(DrawX + 15, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxEN.ToString(), new Vector2(DrawX + 222, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);

            g.DrawString(fntFinlanderFont, "Armor", new Vector2(DrawX + 15, DrawY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.Armor.ToString(), new Vector2(DrawX + 95, DrawY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.White);
            g.DrawString(fntFinlanderFont, "Mobility", new Vector2(DrawX + 15, DrawY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.Mobility.ToString(), new Vector2(DrawX + 115, DrawY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.White);

            DrawX += 250;
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 206, BottomHeight);
            DrawX += 10;
            g.DrawString(fntFinlanderFont, "MV", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxMovement.ToString(), new Vector2(DrawX + 90, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.White);
            g.DrawString(fntFinlanderFont, "Size", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, UnitStats.ListUnitSize[ActiveUnit.SizeIndex], new Vector2(DrawX + 90, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, "Move Type", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);

            #endregion

            int DrawOffset = 0;
            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainAirIndex))
            {
                g.Draw(sprSky, new Vector2(DrawX + DrawOffset, DrawY + 132), Color.White);
                DrawOffset += 50;
            }
            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainLandIndex))
            {
                g.Draw(sprLand, new Vector2(DrawX + DrawOffset, DrawY + 132), Color.White);
                DrawOffset += 50;
            }
            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainSeaIndex))
            {
                g.Draw(sprSea, new Vector2(DrawX + DrawOffset, DrawY + 132), Color.White);
                DrawOffset += 50;
            }
            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainSpaceIndex))
            {
                g.Draw(sprSpace, new Vector2(DrawX + DrawOffset, DrawY + 132), Color.White);
            }

            DrawX += 200;
            int CurrentY = DrawY + 52;

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 130, BottomHeight);

            #region Terrain

            g.DrawString(fntFinlanderFont, "Terrain", new Vector2(DrawX + 15, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);

            DrawX += 11;

            g.Draw(sprSky, new Vector2(DrawX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainAirIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainAirIndex).ToString(), new Vector2(DrawX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(DrawX + 34, CurrentY), Color.White);
            }

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprLand, new Vector2(DrawX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainLandIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainLandIndex).ToString(), new Vector2(DrawX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(DrawX + 34, CurrentY), Color.White);
            }

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprSea, new Vector2(DrawX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainSeaIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSeaIndex).ToString(), new Vector2(DrawX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(DrawX + 34, CurrentY), Color.White);
            }

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprSpace, new Vector2(DrawX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainSpaceIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSpaceIndex).ToString(), new Vector2(DrawX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(DrawX + 34, CurrentY), Color.White);
            }

            #endregion

            DrawX += 116;

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 130, BottomHeight);

            g.DrawString(fntFinlanderFont, "Rank", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.QualityRank, new Vector2(DrawX + 120, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.White);
            g.DrawString(fntFinlanderFont, "Spawn", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 3), Color.Yellow);
            g.DrawString(fntFinlanderFont, "Cost", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 4), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.SpawnCost.ToString(), new Vector2(DrawX + 120, DrawY - MenuOffset + 10 + DistanceBetweenText * 4), Color.White);
        }
    }
}
