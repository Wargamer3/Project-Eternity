using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    //http://imgur.com/a/2kw7R
    public sealed class DefaultUnitUpgradesScreen : GameScreen
    {
        private enum UpgradeChoices { HP, EN, Armor, Mobility, Attacks }

        private Texture2D sprMapMenuBackground;
        private Texture2D sprBackWall;
        private Texture2D sprFacingWall;
        private Texture2D sprFloor;
        private Texture2D sprInnerWall;
        private Texture2D sprOverhang;

        private SpriteFont fntFinlanderFont;

        private UpgradeChoices UpgradeChoice;
        private AttacksMenu AttackPicker;

        private int HPUpgradeCount { get { return ArrayUpgradeCount[0]; } set { ArrayUpgradeCount[0] = value; } }
        private int ENUpgradeCount { get { return ArrayUpgradeCount[1]; } set { ArrayUpgradeCount[1] = value; } }
        private int ArmorUpgradeCount { get { return ArrayUpgradeCount[2]; } set { ArrayUpgradeCount[2] = value; } }
        private int MobilityUpgradeCount { get { return ArrayUpgradeCount[3]; } set { ArrayUpgradeCount[3] = value; } }
        private int AttackUpgradeCount { get { return ArrayUpgradeCount[4]; } set { ArrayUpgradeCount[4] = value; } }
        private int[] ArrayUpgradeCount;

        private Dictionary<UnitStats.AttackUpgradesCosts, int[]> DicUpgradeCost;
        private int[] ArrayUpgradeCost;
        private int UpgradeTotalCost;

        private readonly Unit SelectedUnit;
        private readonly List<Attack> ListAttack;
        private readonly List<Unit> ListPresentUnit;

        private  int SelectedUnitIndex;
        private  int PreviousUnitIndex;
        private  int NextUnitIndex;
        private bool RotateRight;
        private int SelectedNextUnitIndex = -1;
        private GameScreen NextUpgradeScreen;

        private readonly FormulaParser ActiveParser;
        private float HangarPosition = 320;
        private BasicEffect basicEffect;
        private VertexPositionTexture[] quad = new VertexPositionTexture[6];

        public DefaultUnitUpgradesScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
            : base()
        {
            this.ListPresentUnit = ListPresentUnit;
            this.SelectedUnitIndex = SelectedUnitIndex;

            if (ListPresentUnit.Count > 1)
            {
                PreviousUnitIndex = SelectedUnitIndex  - 1;
                if (PreviousUnitIndex < 0)
                {
                    PreviousUnitIndex = ListPresentUnit.Count - 1;
                }

                NextUnitIndex = SelectedUnitIndex + 1;
                if (NextUnitIndex >= ListPresentUnit.Count)
                {
                    NextUnitIndex = 0;
                }
            }

            SelectedUnit = ListPresentUnit[SelectedUnitIndex];
            ListAttack = SelectedUnit.ListAttack;
            this.ActiveParser = ActiveParser;

            ArrayUpgradeCount = new int[5] { 0, 0, 0, 0, 0 };

            DicUpgradeCost = new Dictionary<UnitStats.AttackUpgradesCosts, int[]>();
            DicUpgradeCost.Add(UnitStats.AttackUpgradesCosts.Cheapest, new int[]    { 0, 8000, 11000, 15000, 20000, 26000, 33000, 41000, 50000, 60000, 71000 });
            DicUpgradeCost.Add(UnitStats.AttackUpgradesCosts.Cheap, new int[]       { 0, 10000, 14000, 19000, 25000, 32000, 40000, 49000, 59000, 70000, 82000 });
            DicUpgradeCost.Add(UnitStats.AttackUpgradesCosts.Normal, new int[]      { 0, 12000, 17000, 23000, 30000, 38000, 47000, 57000, 68000, 80000, 93000 });
            DicUpgradeCost.Add(UnitStats.AttackUpgradesCosts.Expensive, new int[]   { 0, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 72000, 80000, 88000 });

            ArrayUpgradeCost = DicUpgradeCost[SelectedUnit.UnitStat.AttackUpgradesCost];
        }

        public override void Load()
        {
            sprMapMenuBackground = Content.Load<Texture2D>("Menus/Status Screen/Background Black");
            sprBackWall = Content.Load<Texture2D>("Menus/Intermission Screens/Hangar/Back Wall");
            sprFacingWall = Content.Load<Texture2D>("Menus/Intermission Screens/Hangar/Facing Wall");
            sprFloor = Content.Load<Texture2D>("Menus/Intermission Screens/Hangar/Floor");
            sprInnerWall = Content.Load<Texture2D>("Menus/Intermission Screens/Hangar/Inner Wall");
            sprOverhang = Content.Load<Texture2D>("Menus/Intermission Screens/Hangar/Overhang");

            AttackPicker = new AttacksMenu(ActiveParser);
            AttackPicker.Load();
            AttackPicker.Reset(SelectedUnit, ListAttack);

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            float CenterX = 320;
            float Top = 0;
            Vector2 TopLeft = new Vector2(CenterX - sprFloor.Width, Top);
            Vector2 TopRight = new Vector2(CenterX + sprFloor.Width, Top);
            Vector2 BottomLeft = new Vector2(CenterX - sprFloor.Width, Top + sprFloor.Height * 2);
            Vector2 BottomRight = new Vector2(CenterX + sprFloor.Width, Top + sprFloor.Height * 2);

            quad[0] = new VertexPositionTexture(new Vector3(TopLeft, 0f), new Vector2(0f, 0f));
            quad[1] = new VertexPositionTexture(new Vector3(BottomRight, 0f), new Vector2(1f, 1f));
            quad[2] = new VertexPositionTexture(new Vector3(BottomLeft, 0f), new Vector2(0f, 1f));

            quad[3] = new VertexPositionTexture(new Vector3(BottomRight, 0f), new Vector2(1f, 1f));
            quad[4] = new VertexPositionTexture(new Vector3(TopLeft, 0f), new Vector2(0f, 0f));
            quad[5] = new VertexPositionTexture(new Vector3(TopRight, 0f), new Vector2(1f, 0f));

            SetUpBasicEffect();
        }

        public void SetUpBasicEffect()
        {
            basicEffect = new BasicEffect(GameScreen.GraphicsDevice);
            basicEffect.VertexColorEnabled = false;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = sprFloor;

            Matrix View = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, -1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = HalfPixelOffset * Projection;

            basicEffect.World = Matrix.Identity;
            basicEffect.View = View;
            basicEffect.Projection = Projection;
        }

        public override void Update(GameTime gameTime)
        {
            if (NextUpgradeScreen != null)
            {
                if (RotateRight)
                {
                    HangarPosition -= 5;

                    if (SelectedNextUnitIndex == SelectedUnitIndex && HangarPosition <= 320)
                    {
                        RemoveScreen(this);
                        PushScreen(NextUpgradeScreen);
                    }
                    else if (HangarPosition < 0)
                    {
                        HangarPosition += 480;

                        PreviousUnitIndex = SelectedUnitIndex;
                        SelectedUnitIndex = NextUnitIndex;
                        NextUnitIndex = SelectedUnitIndex + 1;

                        if (NextUnitIndex >= ListPresentUnit.Count)
                        {
                            NextUnitIndex = 0;
                        }
                    }
                }
                else
                {
                    HangarPosition += 5;
                    if (HangarPosition > 480)
                    {
                        HangarPosition -= 480;
                        RemoveScreen(this);
                        PushScreen(NextUpgradeScreen);
                    }
                }
            }
            else if (InputHelper.InputLButtonPressed())
            {
                SelectedNextUnitIndex = PreviousUnitIndex;
                RotateRight = false;

                GameScreen CustomizeScreen = ListPresentUnit[SelectedNextUnitIndex].GetCustomizeScreen(ListPresentUnit, SelectedNextUnitIndex, ActiveParser);

                if (CustomizeScreen == null)
                {
                    CustomizeScreen = new DefaultUnitUpgradesScreen(ListPresentUnit, SelectedNextUnitIndex, ActiveParser);
                }

                NextUpgradeScreen = CustomizeScreen;
            }
            else if (InputHelper.InputRButtonPressed())
            {
                SelectedNextUnitIndex = NextUnitIndex;
                RotateRight = true;

                GameScreen CustomizeScreen = ListPresentUnit[SelectedNextUnitIndex].GetCustomizeScreen(ListPresentUnit, SelectedNextUnitIndex, ActiveParser);

                if (CustomizeScreen == null)
                {
                    CustomizeScreen = new DefaultUnitUpgradesScreen(ListPresentUnit, SelectedNextUnitIndex, ActiveParser);
                }

                NextUpgradeScreen = CustomizeScreen;
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (UpgradeTotalCost <= Constants.Money)
                {
                    SelectedUnit.UnitStat.HPUpgrades.Value += HPUpgradeCount;
                    SelectedUnit.UnitStat.ENUpgrades.Value += ENUpgradeCount;
                    SelectedUnit.UnitStat.ArmorUpgrades.Value += ArmorUpgradeCount;
                    SelectedUnit.UnitStat.MobilityUpgrades.Value += MobilityUpgradeCount;
                    SelectedUnit.UnitStat.AttackUpgrades.Value += AttackUpgradeCount;

                    HPUpgradeCount = 0;
                    ENUpgradeCount = 0;
                    ArmorUpgradeCount = 0;
                    MobilityUpgradeCount = 0;
                    AttackUpgradeCount = 0;
                    Constants.Money -= UpgradeTotalCost;
                    UpdateCost();
                }
            }
            else if (InputHelper.InputUpPressed())
            {
                if (UpgradeChoice != UpgradeChoices.HP)
                    --UpgradeChoice;
                else
                    UpgradeChoice = UpgradeChoices.Attacks;
            }
            else if (InputHelper.InputDownPressed())
            {
                if (UpgradeChoice != UpgradeChoices.Attacks)
                    ++UpgradeChoice;
                else
                    UpgradeChoice = UpgradeChoices.HP;
            }
            else if (InputHelper.InputLeftPressed())
            {
                if (ArrayUpgradeCount[(int)UpgradeChoice] > 0)
                {
                    --ArrayUpgradeCount[(int)UpgradeChoice];

                    UpdateCost();
                }
            }
            else if (InputHelper.InputRightPressed())
            {
                if (ArrayUpgradeCount[(int)UpgradeChoice] < 10 - SelectedUnit.UnitStat.ArrayUpgrade[(int)UpgradeChoice].Value)
                {
                    ++ArrayUpgradeCount[(int)UpgradeChoice];

                    UpdateCost();
                }
            }
        }

        private void UpdateCost()
        {
            UpgradeTotalCost = 0;

            for (int i = 0; i < HPUpgradeCount; ++i)
            {
                UpgradeTotalCost += 2000 + (i + SelectedUnit.UnitStat.HPUpgrades.Value) * 2000;
            }
            for (int i = 0; i < ENUpgradeCount; ++i)
            {
                UpgradeTotalCost += 1000 + (i + SelectedUnit.UnitStat.ENUpgrades.Value) * 1000;
            }
            for (int i = 0; i < ArmorUpgradeCount; ++i)
            {
                UpgradeTotalCost += 2000 + (i + SelectedUnit.UnitStat.ArmorUpgrades.Value) * 2000;
            }
            for (int i = 0; i < MobilityUpgradeCount; ++i)
            {
                UpgradeTotalCost += 2000 + (i + SelectedUnit.UnitStat.MobilityUpgrades.Value) * 2000;
            }

            UpgradeTotalCost += ArrayUpgradeCost[AttackUpgradeCount + SelectedUnit.UnitStat.AttackUpgrades.Value];
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            g.Draw(sprMapMenuBackground, new Vector2(0, 0), Color.White);
            DrawCarousel(g);
            DrawUpgradeMenu(g);
        }
        
        private void DrawUpgradeMenu(CustomSpriteBatch g)
        {
            g.DrawString(fntFinlanderFont, "UNIT UPGRADE", new Vector2(10, 10), Color.White);

            g.Draw(SelectedUnit.SpriteMap, new Vector2(20, 50), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.RelativePath, new Vector2(60, 50), Color.White);
            g.Draw(sprPixel, new Rectangle(60, 75, (int)fntFinlanderFont.MeasureString(SelectedUnit.RelativePath).X, 1), Color.FromNonPremultiplied(173, 216, 230, 190));

            int Y = 350;
            DrawBox(g, new Vector2(5, Y), 450, 120, Color.Black);
            DrawBox(g, new Vector2(455, Y), 180, 120, Color.Black);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(15, Y += 5), Color.White);

            g.DrawString(fntFinlanderFont, SelectedUnit.MaxHP.ToString(), new Vector2(150, Y), Color.White);
            SelectedUnit.UnitStat.HPUpgrades.Value += HPUpgradeCount;
            g.DrawString(fntFinlanderFont, SelectedUnit.MaxHP.ToString(), new Vector2(240, Y), Color.White);
            SelectedUnit.UnitStat.HPUpgrades.Value -= HPUpgradeCount;
            DrawUpgradeBar(g, Y, SelectedUnit.UnitStat.HPUpgrades.Value, HPUpgradeCount);


            g.DrawString(fntFinlanderFont, "EN", new Vector2(15, Y += fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.MaxEN.ToString(), new Vector2(150, Y), Color.White);
            SelectedUnit.UnitStat.ENUpgrades.Value += ENUpgradeCount;
            g.DrawString(fntFinlanderFont, SelectedUnit.MaxEN.ToString(), new Vector2(240, Y), Color.White);
            SelectedUnit.UnitStat.ENUpgrades.Value -= ENUpgradeCount;
            DrawUpgradeBar(g, Y, SelectedUnit.UnitStat.ENUpgrades.Value, ENUpgradeCount);

            g.DrawString(fntFinlanderFont, "Armor", new Vector2(15, Y += fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.Armor.ToString(), new Vector2(150, Y), Color.White);
            SelectedUnit.UnitStat.ArmorUpgrades.Value += ArmorUpgradeCount;
            g.DrawString(fntFinlanderFont, SelectedUnit.Armor.ToString(), new Vector2(240, Y), Color.White);
            SelectedUnit.UnitStat.ArmorUpgrades.Value -= ArmorUpgradeCount;
            DrawUpgradeBar(g, Y, SelectedUnit.UnitStat.ArmorUpgrades.Value, ArmorUpgradeCount);

            g.DrawString(fntFinlanderFont, "Mobility", new Vector2(15, Y += fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.Mobility.ToString(), new Vector2(150, Y), Color.White);
            SelectedUnit.UnitStat.MobilityUpgrades.Value += MobilityUpgradeCount;
            g.DrawString(fntFinlanderFont, SelectedUnit.Mobility.ToString(), new Vector2(240, Y), Color.White);
            SelectedUnit.UnitStat.MobilityUpgrades.Value -= MobilityUpgradeCount;
            DrawUpgradeBar(g, Y, SelectedUnit.UnitStat.MobilityUpgrades.Value, MobilityUpgradeCount);

            g.DrawString(fntFinlanderFont, "Attacks", new Vector2(15, Y += fntFinlanderFont.LineSpacing), Color.White);
            DrawUpgradeBar(g, Y, SelectedUnit.UnitStat.AttackUpgrades.Value, AttackUpgradeCount);

            if (UpgradeChoice == UpgradeChoices.Attacks)
            {
                AttackPicker.DrawTopAttackPanel(g, fntFinlanderFont, false);

                int YStart = 115;
                int YStep = 25;

                g.DrawString(fntFinlanderFont, "Power", new Vector2(350, YStart - 25), Color.Yellow);
                for (int i = 0; i < ListAttack.Count && i <= 8; i++)
                {
                    int CurrentPower = ListAttack[i].GetPower(SelectedUnit, ActiveParser);
                    SelectedUnit.UnitStat.AttackUpgrades.Value += AttackUpgradeCount;
                    int NextPower = ListAttack[i].GetPower(SelectedUnit, ActiveParser);
                    SelectedUnit.UnitStat.AttackUpgrades.Value -= AttackUpgradeCount;

                    g.DrawStringRightAligned(fntFinlanderFont, CurrentPower.ToString(), new Vector2(400, YStart + i * YStep), Color.White);
                    g.DrawString(fntFinlanderFont, "-> " + NextPower.ToString(), new Vector2(410, YStart + i * YStep), Color.White);
                    g.DrawStringRightAligned(fntFinlanderFont, "(+" +(NextPower - CurrentPower).ToString() + ")", new Vector2(620, YStart + i * YStep), Color.White);
                }
            }

            Y = 360 + (int)UpgradeChoice * fntFinlanderFont.LineSpacing;
            DrawRectangle(g, new Vector2(15, Y), new Vector2(452, Y + fntFinlanderFont.LineSpacing), Color.Yellow);

            Y = 350;
            g.DrawString(fntFinlanderFont, "Money", new Vector2(460, Y += 5), Color.White);
            g.DrawStringRightAligned(fntFinlanderFont, Constants.Money.ToString(), new Vector2(625, Y), Color.White);

            g.DrawString(fntFinlanderFont, "Cost", new Vector2(460, Y += 40), Color.White);
            g.DrawStringRightAligned(fntFinlanderFont, UpgradeTotalCost.ToString(), new Vector2(625, Y), Color.White);

            g.DrawString(fntFinlanderFont, "Result", new Vector2(460, Y += 40), Color.White);
            g.DrawStringRightAligned(fntFinlanderFont, (Constants.Money - UpgradeTotalCost).ToString(), new Vector2(625, Y), Color.White);
        }

        private void DrawUpgradeBar(CustomSpriteBatch g, int Y, int UpgradeCount, int UpgradesToBuy)
        {
            int UpgradeWidth = 15;
            int BoxWidth = UpgradeWidth * 10;
            int BoxHeight = UpgradeWidth;
            int BoxX = 300;
            int BoxY = Y + 8;

            g.DrawLine(sprPixel, new Vector2(BoxX, BoxY), new Vector2(BoxX - 1 + BoxWidth, BoxY), Color.White);
            g.DrawLine(sprPixel, new Vector2(BoxX, BoxY + BoxHeight), new Vector2(BoxX - 1 + BoxWidth, BoxY + BoxHeight), Color.White);

            g.Draw(sprPixel, new Rectangle(BoxX, BoxY + 1, UpgradeCount * UpgradeWidth, BoxHeight - 1), Color.Red);
            g.Draw(sprPixel, new Rectangle(BoxX + UpgradeCount * UpgradeWidth, BoxY + 1, UpgradesToBuy * UpgradeWidth, BoxHeight - 1), Color.Cyan);

            for (int i = 0; i <= 10; ++i)
            {
                g.DrawLine(sprPixel, new Vector2(BoxX + i * UpgradeWidth, BoxY), new Vector2(BoxX + i * UpgradeWidth, BoxY + BoxHeight), Color.White);
            }
        }

        private void DrawQuad(CustomSpriteBatch g, Texture2D Sprite, bool FlipX, float CenterX, float Top, float BottomSkewValue, float ScaleX)
        {
            basicEffect.Texture = Sprite;

            Vector2 TopLeft = new Vector2(CenterX - Sprite.Width * ScaleX, Top);
            Vector2 TopRight = new Vector2(CenterX + Sprite.Width * ScaleX, Top);
            Vector2 BottomLeft = new Vector2(CenterX + BottomSkewValue - Sprite.Width * ScaleX, Top + Sprite.Height * 2);
            Vector2 BottomRight = new Vector2(CenterX + BottomSkewValue + Sprite.Width * ScaleX, Top + Sprite.Height * 2);

            if (FlipX)
            {
                quad[0].TextureCoordinate = new Vector2(1f, 0f);
                quad[1].TextureCoordinate = new Vector2(0f, 1f);
                quad[2].TextureCoordinate = new Vector2(1f, 1f);

                quad[3].TextureCoordinate = new Vector2(0f, 1f);
                quad[4].TextureCoordinate = new Vector2(1f, 0f);
                quad[5].TextureCoordinate = new Vector2(0f, 0f);
            }
            else
            {
                quad[0].TextureCoordinate = new Vector2(0f, 0f);
                quad[1].TextureCoordinate = new Vector2(1f, 1f);
                quad[2].TextureCoordinate = new Vector2(0f, 1f);

                quad[3].TextureCoordinate = new Vector2(1f, 1f);
                quad[4].TextureCoordinate = new Vector2(0f, 0f);
                quad[5].TextureCoordinate = new Vector2(1f, 0f);
            }

            quad[0].Position = new Vector3(TopLeft, 0f);
            quad[1].Position = new Vector3(BottomRight, 0f);
            quad[2].Position = new Vector3(BottomLeft, 0f);

            quad[3].Position = new Vector3(BottomRight, 0f);
            quad[4].Position = new Vector3(TopLeft, 0f);
            quad[5].Position = new Vector3(TopRight, 0f);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                g.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList, quad, 0, 2);
            }
        }

        private void DrawCarousel(CustomSpriteBatch g)
        {
            if (HangarPosition > 480)
            {
                HangarPosition -= 480;
            }

            g.End();

            Vector2 CenterUnitPosition = new Vector2(Constants.Width / 2, 270);

            if (PreviousUnitIndex >= 0)
            {
                DrawHangar(g, HangarPosition - 480, PreviousUnitIndex);
            }

            if (NextUnitIndex >= 0)
            {
                DrawHangar(g, HangarPosition + 480, NextUnitIndex);
            }

            DrawHangar(g, HangarPosition, SelectedUnitIndex);

            DrawQuad(g, sprFacingWall, false, HangarPosition - 240, CenterUnitPosition.Y - 190, 0, 1f);
            DrawQuad(g, sprFacingWall, false, HangarPosition + 240, CenterUnitPosition.Y - 190, 0, 1f);

            DrawQuad(g, sprOverhang, false, HangarPosition, 0, 0, 1f);
            DrawQuad(g, sprOverhang, false, HangarPosition + sprOverhang.Width * 2 - 74, 0, 0, 1f);
            DrawQuad(g, sprOverhang, false, HangarPosition - sprOverhang.Width * 2 + 74, 0, 0, 1f);

            g.Begin();
        }

        private void DrawHangar(CustomSpriteBatch g, float RealHangarPosition, int UnitIndex)
        {
            Vector2 CenterUnitPosition = new Vector2(Constants.Width / 2, 270);
            float FloorSkewMax = 100;

            float Hangar1Scale = 1 - (CenterUnitPosition.X - RealHangarPosition) / 320;
            float HangarPositionDrawn = RealHangarPosition + (1 - Hangar1Scale) * 120;

            float Hangar1RightWallCenter = RealHangarPosition + 152;
            float Hangar1RightWallScale = 1 - (472 - Hangar1RightWallCenter) / 320;
            float Hangar1RightWallWidth = (sprInnerWall.Width - 3) * Hangar1RightWallScale;
            float Hangar1RightWallX = HangarPositionDrawn + 90;
            Hangar1RightWallCenter = Hangar1RightWallX + Hangar1RightWallWidth + (1 - Hangar1RightWallScale) * 4;

            float Hangar1LeftWallCenter = RealHangarPosition - 152;
            float Hangar1LeftWallScale = 1 - (Hangar1LeftWallCenter - 168) / 320;
            float Hangar1LeftWallWidth = (sprInnerWall.Width - 3) * Hangar1LeftWallScale;
            float Hangar1LeftWallX = HangarPositionDrawn - 90;
            Hangar1LeftWallCenter = Hangar1LeftWallX - Hangar1LeftWallWidth - (1 - Hangar1LeftWallScale) * 4;

            DrawQuad(g, sprBackWall, false, HangarPositionDrawn, CenterUnitPosition.Y - 188, 0f, 1f);
            DrawQuad(g, sprFloor, false, HangarPositionDrawn, CenterUnitPosition.Y, -(1 - Hangar1Scale) * FloorSkewMax, 1f);
            DrawQuad(g, sprInnerWall, true, Hangar1LeftWallCenter, CenterUnitPosition.Y - 186, 0, Hangar1LeftWallScale);
            DrawQuad(g, sprInnerWall, false, Hangar1RightWallCenter, CenterUnitPosition.Y - 186, 0, Hangar1RightWallScale);

            DrawQuad(g, ListPresentUnit[UnitIndex].SpriteUnit, true, RealHangarPosition + 50 * (1 - Hangar1Scale), CenterUnitPosition.Y - ListPresentUnit[UnitIndex].SpriteUnit.Height * 2 + 50, 0, 1f);
        }
    }
}
