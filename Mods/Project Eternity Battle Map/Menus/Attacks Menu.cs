﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class AttacksMenu : GameScreen
    {
        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;
        private Texture2D sprRanged;
        private Texture2D sprMelee;

        private Texture2D sprAttackPropertiesBack;
        private Texture2D sprAttackPropertiesP;
        private Texture2D sprAttackPropertiesB;
        private Texture2D sprAttackPropertiesS;

        private Texture2D sprAttackTypeBack;
        private Texture2D sprAttackTypeALL;
        private Texture2D sprAttackTypeMAP;
        private Texture2D sprAttackTypePLA;

        private FormulaParser ActiveParser;

        public AttacksMenu(FormulaParser ActiveParser)
        {
            this.ActiveParser = ActiveParser;
        }

        public AttacksMenu(Texture2D sprLand, Texture2D sprSea, Texture2D sprSky, Texture2D sprSpace,
                            Texture2D sprAttackPropertiesBack, Texture2D sprAttackPropertiesP, Texture2D sprAttackPropertiesB, Texture2D sprAttackPropertiesS,
                            Texture2D sprAttackTypeBack, Texture2D sprAttackTypeALL, Texture2D sprAttackTypeMAP, Texture2D sprAttackTypePLA)
        {
            this.sprLand = sprLand;
            this.sprSea = sprSea;
            this.sprSky = sprSky;
            this.sprSpace = sprSpace;

            this.sprAttackPropertiesBack = sprAttackPropertiesBack;
            this.sprAttackPropertiesP = sprAttackPropertiesP;
            this.sprAttackPropertiesB = sprAttackPropertiesB;
            this.sprAttackPropertiesS = sprAttackPropertiesS;

            this.sprAttackTypeBack = sprAttackTypeBack;
            this.sprAttackTypeALL = sprAttackTypeALL;
            this.sprAttackTypeMAP = sprAttackTypeMAP;
            this.sprAttackTypePLA = sprAttackTypePLA;
        }

        public override void Load()
        {
            sprLand = Content.Load<Texture2D>("Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Status Screen/Space");
            sprRanged = Content.Load<Texture2D>("Status Screen/Ranged");
            sprMelee = Content.Load<Texture2D>("Status Screen/Melee");

            sprAttackPropertiesBack = Content.Load<Texture2D>("Battle/Attack Select/Properties Back");
            sprAttackPropertiesP = Content.Load<Texture2D>("Battle/Attack Select/Properties P");
            sprAttackPropertiesB = Content.Load<Texture2D>("Battle/Attack Select/Properties B");
            sprAttackPropertiesS = Content.Load<Texture2D>("Battle/Attack Select/Properties S");

            sprAttackTypeBack = Content.Load<Texture2D>("Battle/Attack Select/Type Back");
            sprAttackTypeALL = Content.Load<Texture2D>("Battle/Attack Select/Type ALL");
            sprAttackTypeMAP = Content.Load<Texture2D>("Battle/Attack Select/Type MAP");
            sprAttackTypePLA = Content.Load<Texture2D>("Battle/Attack Select/Type PLA");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public void DrawTopAttackPanel(CustomSpriteBatch g, SpriteFont ActiveFont, Unit ActiveUnit, int CursorIndex, bool DrawAll = true)
        {
            Color ColorBrush;
            string Hit;
            string Crit;
            int XStart = (Constants.Width - 630) / 2;
            int YStep = 25;
            int YStart = 115;

            g.Draw(ActiveUnit.SpriteMap, new Vector2(XStart + 15, 50), Color.White);
            g.DrawString(ActiveFont, ActiveUnit.RelativePath, new Vector2(XStart + 55, 50), Color.White);
            g.Draw(sprPixel, new Rectangle(XStart + 55, 75, (int)ActiveFont.MeasureString(ActiveUnit.RelativePath).X, 1), Color.FromNonPremultiplied(173, 216, 230, 190));

            DrawBox(g, new Vector2(XStart, YStart - 30), 630, 200, Color.White);
            g.DrawString(ActiveFont, "Attack", new Vector2(XStart + 45, YStart - 25), Color.Yellow);
            if (DrawAll)
            {
                g.DrawString(ActiveFont, "Power", new Vector2(XStart + 315, YStart - 25), Color.Yellow);
                g.DrawString(ActiveFont, "Range", new Vector2(XStart + 415, YStart - 25), Color.Yellow);
                g.DrawString(ActiveFont, "Hit", new Vector2(XStart + 515, YStart - 25), Color.Yellow);
                g.DrawString(ActiveFont, "Crit", new Vector2(XStart + 565, YStart - 25), Color.Yellow);
            }

            for (int i = Math.Max(0, CursorIndex - 8), CurPos = 0; i < ActiveUnit.ListAttack.Count && CurPos <= 8; i++, CurPos++)
            {
                if (ActiveUnit.ListAttack[i].CanAttack)
                    ColorBrush = Color.White;
                else
                    ColorBrush = Color.Gray;

                if (ActiveUnit.ListAttack[i].Accuracy < 0)
                    Hit = ActiveUnit.ListAttack[i].Accuracy.ToString();
                else
                    Hit = "+" + ActiveUnit.ListAttack[i].Accuracy;

                if (ActiveUnit.ListAttack[i].Critical < 0)
                    Crit = ActiveUnit.ListAttack[i].Critical.ToString();
                else
                    Crit = "+" + ActiveUnit.ListAttack[i].Critical;

                //Draw the weapon list.
                if (ActiveUnit.ListAttack[i].AttackType == "Melee")
                    g.Draw(sprMelee, new Vector2(XStart + 7, YStart + CurPos * YStep + 5), Color.White);
                else
                    g.Draw(sprRanged, new Vector2(XStart + 7, YStart + CurPos * YStep + 5), Color.White);

                int CurrentX = XStart + 35;
                g.DrawString(ActiveFont, ActiveUnit.ListAttack[i].ItemName, new Vector2(XStart + 35, YStart + CurPos * YStep), ColorBrush);
                CurrentX += 5 + (int)ActiveFont.MeasureString(ActiveUnit.ListAttack[i].ItemName).X;

                if ((ActiveUnit.ListAttack[i].Sec & WeaponSecondaryProperty.PostMovement) == WeaponSecondaryProperty.PostMovement)
                {
                    g.Draw(sprAttackPropertiesP, new Vector2(CurrentX, YStart + 11 + CurPos * YStep), Color.White);
                    CurrentX += 12;
                }

                if (ActiveUnit.ListAttack[i].Pri == WeaponPrimaryProperty.MAP)
                {
                    g.Draw(sprAttackTypeMAP, new Vector2(CurrentX, YStart + 11 + CurPos * YStep), Color.White);
                    CurrentX += 42;
                }
                if (ActiveUnit.ListAttack[i].Pri == WeaponPrimaryProperty.PLA)
                {
                    g.Draw(sprAttackTypePLA, new Vector2(CurrentX, YStart + 11 + CurPos * YStep), Color.White);
                    CurrentX += 42;
                }
                if (ActiveUnit.ListAttack[i].Pri == WeaponPrimaryProperty.ALL)
                {
                    g.Draw(sprAttackTypeALL, new Vector2(CurrentX, YStart + 11 + CurPos * YStep), Color.White);
                    CurrentX += 42;
                }

                if (DrawAll)
                {
                    g.DrawString(ActiveFont, ActiveUnit.ListAttack[i].GetPower(ActiveUnit, ActiveParser).ToString(),
                        new Vector2(XStart + 315, YStart + CurPos * YStep), ColorBrush);

                    int RangeMaximum = ActiveUnit.ListAttack[i].RangeMaximum;
                    if (RangeMaximum > 1)
                        RangeMaximum += ActiveUnit.Boosts.RangeModifier;

                    g.DrawString(ActiveFont, ActiveUnit.ListAttack[i].RangeMinimum + "- " + RangeMaximum,
                        new Vector2(XStart + 425, YStart + CurPos * YStep), ColorBrush);
                    g.DrawStringRightAligned(ActiveFont, Hit, new Vector2(XStart + 555, YStart + CurPos * YStep), ColorBrush);
                    g.DrawString(ActiveFont, Crit, new Vector2(XStart + 570, YStart + CurPos * YStep), ColorBrush);
                }
            }
        }

        public void DrawAttackPanel(CustomSpriteBatch g, SpriteFont ActiveFont, Unit ActiveUnit, int CursorIndex)
        {
            int XStart = (Constants.Width - 630) / 2;
            int YStep = 25;
            int YStart = 115;

            DrawTopAttackPanel(g, ActiveFont, ActiveUnit, CursorIndex);
            //Draw the information of the selected weapon.
            DrawBox(g, new Vector2(XStart, YStart + 170), 630, 50, Color.White);
            g.DrawString(ActiveFont, "Ammo", new Vector2(XStart + 13, YStart + 178), Color.Yellow);
            g.DrawStringRightAligned(ActiveFont, ActiveUnit.ListAttack[CursorIndex].Ammo + "/" + (ActiveUnit.ListAttack[CursorIndex].MaxAmmo + ActiveUnit.Boosts.AmmoMaxModifier),
                new Vector2(XStart + 150, YStart + 178), Color.White);

            g.DrawString(ActiveFont, "Will", new Vector2(XStart + 156, YStart + 178), Color.Yellow);
            g.DrawStringRightAligned(ActiveFont, ActiveUnit.ListAttack[CursorIndex].MoraleRequirement + "(" + ActiveUnit.PilotMorale + ")",
                new Vector2(XStart + 305, YStart + 178), Color.White);

            g.DrawString(ActiveFont, "Energy", new Vector2(XStart + 305, YStart + 178), Color.Yellow);
            g.DrawStringRightAligned(ActiveFont, ActiveUnit.ListAttack[CursorIndex].ENCost.ToString(),
                new Vector2(XStart + 430, YStart + 178), Color.White);
            g.Draw(sprSky, new Vector2(XStart + 442, YStart + 182), Color.White);
            g.DrawString(ActiveFont, ActiveUnit.ListAttack[CursorIndex].DicTerrainAttribute[UnitStats.TerrainAir].ToString(),
                new Vector2(XStart + 467, YStart + 178), Color.White);
            g.Draw(sprLand, new Vector2(XStart + 488, YStart + 182), Color.White);
            g.DrawString(ActiveFont, ActiveUnit.ListAttack[CursorIndex].DicTerrainAttribute[UnitStats.TerrainLand].ToString(),
                new Vector2(XStart + 513, YStart + 178), Color.White);
            g.Draw(sprSea, new Vector2(XStart + 534, YStart + 182), Color.White);
            g.DrawString(ActiveFont, ActiveUnit.ListAttack[CursorIndex].DicTerrainAttribute[UnitStats.TerrainSea].ToString(),
                new Vector2(XStart + 559, YStart + 178), Color.White);
            g.Draw(sprSpace, new Vector2(XStart + 580, YStart + 182), Color.White);
            g.DrawString(ActiveFont, ActiveUnit.ListAttack[CursorIndex].DicTerrainAttribute[UnitStats.TerrainSpace].ToString(),
                new Vector2(XStart + 605, YStart + 178), Color.White);

            DrawBox(g, new Vector2(XStart, YStart + 220), 630, 140, Color.White);
            //Draw the cursor.
            g.Draw(sprPixel, new Rectangle(XStart + 6, 120 + CursorIndex * YStep, 618, YStep), Color.FromNonPremultiplied(255, 255, 255, 127));

            Vector2 BoxPosition = new Vector2(XStart + 10, YStart + 223);
            for (int A = 0; A < 4; A++)
            {
                string TextToDraw = "------";
                if (A < ActiveUnit.ListAttack[CursorIndex].ArrayAttackAttributes.Length)
                {
                    TextToDraw = ActiveUnit.ListAttack[CursorIndex].ArrayAttackAttributes[A].Name;
                    TextHelper.DrawText(g, ActiveUnit.ListAttack[CursorIndex].ArrayAttackAttributes[A].Description,
                        new Vector2(BoxPosition.X + 20, BoxPosition.Y + 22 + A * 31), Color.White);
                }
                g.DrawString(ActiveFont, TextToDraw,
                    new Vector2(BoxPosition.X, BoxPosition.Y + A * 31), Color.White);
            }
        }
    }
}
