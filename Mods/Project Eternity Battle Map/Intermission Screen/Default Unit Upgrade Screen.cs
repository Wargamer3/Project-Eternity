using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using FMOD;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    //http://imgur.com/a/2kw7R
    public sealed class DefaultUnitUpgradesScreen : GameScreen
    {
        private enum UpgradeChoices { HP, EN, Armor, Mobility, Attacks }

        private Texture2D sprMapMenuBackground;

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

        private Unit SelectedUnit;
        private FormulaParser ActiveParser;

        public DefaultUnitUpgradesScreen(Unit SelectedUnit, FormulaParser ActiveParser)
            : base()
        {
            this.SelectedUnit = SelectedUnit;
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
            sprMapMenuBackground = Content.Load<Texture2D>("Status Screen/Background Black");

            AttackPicker = new AttacksMenu(ActiveParser);
            AttackPicker.Load();

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputCancelPressed())
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
            DrawUpgradeMenu(g);
        }
        
        private void DrawUpgradeMenu(CustomSpriteBatch g)
        {
            g.Draw(sprMapMenuBackground, new Vector2(0, 0), Color.White);
            g.DrawString(fntFinlanderFont, "UNIT UPGRADE", new Vector2(10, 10), Color.White);

            g.Draw(SelectedUnit.SpriteMap, new Vector2(20, 50), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.RelativePath, new Vector2(60, 50), Color.White);
            g.Draw(sprPixel, new Rectangle(60, 75, (int)fntFinlanderFont.MeasureString(SelectedUnit.RelativePath).X, 1), Color.FromNonPremultiplied(173, 216, 230, 190));
            g.Draw(SelectedUnit.SpriteUnit, new Vector2(250 - SelectedUnit.SpriteUnit.Width, 280 - SelectedUnit.SpriteUnit.Height), Color.White);

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
                AttackPicker.DrawTopAttackPanel(g, fntFinlanderFont, SelectedUnit, 0, false);

                int YStart = 115;
                int YStep = 25;

                g.DrawString(fntFinlanderFont, "Power", new Vector2(350, YStart - 25), Color.Yellow);
                for (int i = 0; i < SelectedUnit.ListAttack.Count && i <= 8; i++)
                {
                    int CurrentPower = SelectedUnit.ListAttack[i].GetPower(SelectedUnit, ActiveParser);
                    SelectedUnit.UnitStat.AttackUpgrades.Value += AttackUpgradeCount;
                    int NextPower = SelectedUnit.ListAttack[i].GetPower(SelectedUnit, ActiveParser);
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
    }
}
