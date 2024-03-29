﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.MultiForm;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class NewIntermissionScreen : GameScreen
    {
        private struct PartMenu
        {
            public string Name;
            public string[] Categories;
            public bool Open;
            public bool[] IsAvailable;

            public PartMenu(string Name, string[] Categories)
            {
                this.Name = Name;
                this.Categories = Categories;
                this.Open = false;
                this.IsAvailable = new bool[Categories.Length];
            }
        };

        private enum MenuChoice { PilotStatus, PilotTraining, PilotSwap, UnitStatus, UnitUpgrade, UnitParts, UnitEquipment, Shop, Commander, ChangeBGM, Options, Records, Data, NextStage };

        private readonly BattleMapPlayer ActivePlayer;
        private readonly Roster PlayerRoster;

        private PartMenu[] Menu;
        private int SelectedChoice = 0;
        int MenuElements;
        private SpriteFont fntArial15;
        private SpriteFont fntArial26;
        private bool UnitEquipmentAvailable;
        private List<Unit> ListPresentUnit;
        private List<Character> ListPresentCharacter;

        FormulaParser ActiveParser;

        public NewIntermissionScreen(BattleMapPlayer ActivePlayer, Roster PlayerRoster)
            : base()
        {
            this.ActivePlayer = ActivePlayer;
            this.PlayerRoster = PlayerRoster;
            ActiveParser = new IntermissionScreenFormulaParser();

            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
            Menu = new PartMenu[] { new PartMenu("Pilot", new string[] { "Pilot Status", "Pilot Training", "Pilot Swap" }),
                                    new PartMenu("Unit", new string[] { "Unit Status", "Unit Upgrade", "Unit Parts", "Unit Equipment" }),
                                    new PartMenu("Misc", new string[] { "Shop", "Commander", "Change BGM", "Options", "Records", "Data", "MOVE OUT!" }) };
            Menu[0].Open = true;
            Menu[1].Open = true;
            Menu[2].Open = true;

            Menu[0].IsAvailable[0] = true;
            Menu[0].IsAvailable[1] = true;
            Menu[0].IsAvailable[2] = false;

            Menu[1].IsAvailable[0] = true;
            Menu[1].IsAvailable[1] = true;
            Menu[1].IsAvailable[2] = true;
            Menu[1].IsAvailable[3] = true;

            Menu[2].IsAvailable[0] = false;
            Menu[2].IsAvailable[1] = true;
            Menu[2].IsAvailable[2] = false;
            Menu[2].IsAvailable[3] = true;
            Menu[2].IsAvailable[4] = true;
            Menu[2].IsAvailable[5] = true;
            Menu[2].IsAvailable[6] = true;

            MenuElements = Menu[0].Categories.Length + Menu[1].Categories.Length + Menu[2].Categories.Length;

            fntArial15 = Content.Load<SpriteFont>("Fonts/Arial15");
            fntArial26 = Content.Load<SpriteFont>("Fonts/Arial26");

            UnitEquipmentAvailable = false;

            ListPresentCharacter = PlayerRoster.TeamCharacters.GetPresent();
            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
            ListPresentCharacter = ListPresentCharacter.OrderByDescending(C => ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills[C.ID]).ToList();

            for (int U = 0; U < ListPresentUnit.Count; ++U)
            {
                UnitMultiForm ActiveUnit = ListPresentUnit[U] as UnitMultiForm;
                if (ActiveUnit != null)
                {
                    UnitEquipmentAvailable = true;
                }

                ListPresentUnit[U].Init();
            }
            Menu[1].IsAvailable[3] = UnitEquipmentAvailable;
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                if (--SelectedChoice < 0)
                    SelectedChoice = MenuElements - 1;
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++SelectedChoice > MenuElements - 1)
                    SelectedChoice = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                KeyboardHelper.PlayerStateLast = KeyboardHelper.PlayerState;
                switch ((MenuChoice)SelectedChoice)
                {
                    case MenuChoice.PilotStatus:
                        PushScreen(new PilotListScreen(ActivePlayer, PlayerRoster));
                        break;

                    case MenuChoice.PilotTraining:
                        PushScreen(new PilotTrainingScreen(ActivePlayer, PlayerRoster));
                        break;

                    case MenuChoice.PilotSwap:
                        if (Menu[0].IsAvailable[2])
                        {
                            PushScreen(new PilotSwapScreen(ActivePlayer, PlayerRoster, ActiveParser));
                        }
                        break;

                    case MenuChoice.UnitStatus:
                        PushScreen(new UnitListScreen(PlayerRoster, ActiveParser));
                        break;

                    case MenuChoice.UnitUpgrade:
                        PushScreen(new UnitUpgradesScreen(ActivePlayer, PlayerRoster, ActiveParser));
                        break;

                    case MenuChoice.UnitParts:
                        PushScreen(new UnitPartsScreen(PlayerRoster, ActiveParser));
                        break;

                    case MenuChoice.UnitEquipment:
                        if (UnitEquipmentAvailable)
                        {
                            PushScreen(new UnitEquipmentScreen(PlayerRoster));
                        }
                        break;

                    case MenuChoice.Shop:
                        if (Menu[2].IsAvailable[0])
                        {
                            PushScreen(new Shop(ActivePlayer));
                        }
                        break;

                    case MenuChoice.Commander:
                        if (Menu[2].IsAvailable[1])
                        {
                            PushScreen(new CommanderScreen(PlayerRoster));
                        }
                        break;

                    case MenuChoice.Records:
                        PushScreen(new RecordsScreen(ActivePlayer, PlayerRoster));
                        break;

                    case MenuChoice.Data:
                        PushScreen(new DataScreen(ActivePlayer, PlayerRoster));
                        break;

                    case MenuChoice.NextStage:
                        //PushScreen(new CommanderLoadoutScreen(PlayerRoster));
                        PushScreen(new LoadoutScreen(ActivePlayer, PlayerRoster, new List<Commander>()));
                        //LoadoutScreen.LoadMap(ListGameScreen, PlayerRoster);
                        //LoadoutScreen.StartMap(this, gameTime);
                        break;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            g.DrawString(fntArial26, "INTERMISSION", new Vector2(0, 0), Color.White);
            int X = (int)fntArial26.MeasureString("INTERMISSION").X;
            int Y = fntArial26.LineSpacing - 1;

            int CursorCounter = 0;
            for (int i = 0; i < Menu.Count(); i++)
            {
                DrawBox(g, new Vector2(0, Y + fntArial15.LineSpacing), X, fntArial15.LineSpacing * Menu[i].Categories.Count() + 6, Color.White);
                g.DrawStringRightAligned(fntArial15, Menu[i].Name, new Vector2(X, Y), Color.White);
                Y += 3;
                for (int j = 0; j < Menu[i].Categories.Count(); j++)
                {
                    if (Menu[i].IsAvailable[j])
                        g.DrawString(fntArial15, Menu[i].Categories[j], new Vector2(10, Y += fntArial15.LineSpacing), Color.White);
                    else
                        g.DrawString(fntArial15, Menu[i].Categories[j], new Vector2(10, Y += fntArial15.LineSpacing), Color.Gray);

                    if (SelectedChoice == CursorCounter)
                    {
                        g.Draw(sprPixel, new Rectangle(5, Y, X - 10, fntArial15.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    ++CursorCounter;
                }
                Y += fntArial15.LineSpacing + 3;
            }
            Y += 3;
            DrawBox(g, new Vector2(0, Y), Constants.Width, fntArial15.LineSpacing * 3 + 6, Color.White);
            g.DrawString(fntArial15, "Money", new Vector2(10, Y + 3), Color.White);
            g.DrawStringRightAligned(fntArial15, ActivePlayer.Records.CurrentMoney.ToString(), new Vector2(340, Y + 3), Color.White);
            g.DrawString(fntArial15, "Next Stage", new Vector2(350, Y + 3), Color.White);
            g.DrawStringRightAligned(fntArial15, BattleMap.NextMapPath, new Vector2(630, Y + 3), Color.White);
            Y += fntArial15.LineSpacing;
            g.DrawString(fntArial15, "Skill Points", new Vector2(10, Y + 3), Color.White);
            g.DrawStringRightAligned(fntArial15, "0", new Vector2(340, Y + 3), Color.White);
            g.DrawString(fntArial15, "Deploy 0 units", new Vector2(350, Y + 3), Color.White);
            Y += fntArial15.LineSpacing;
            g.DrawString(fntArial15, "Cleared Stages", new Vector2(10, Y + 3), Color.White);
            g.DrawStringRightAligned(fntArial15, BattleMap.ClearedStages.ToString(), new Vector2(340, Y + 3), Color.White);
            g.DrawString(fntArial15, "Turns", new Vector2(350, Y + 3), Color.White);

            Y = fntArial26.LineSpacing - 1;

            if (ListPresentCharacter.Count > 0)
            {
                Character MainCharacter = ListPresentCharacter[0];

                DrawBox(g, new Vector2(X + 10, fntArial26.LineSpacing - 1), 120, 120, Color.White);
                g.Draw(sprPixel, new Rectangle(X + 30, Y + 20, 80, 80), Color.White);
                DrawRectangle(g, new Vector2(X + 30, Y + 20), new Vector2(X + 30 + 80, Y + 20 + 80), Color.Black);
                g.Draw(MainCharacter.sprPortrait, new Vector2(X + 30, Y + 20), Color.White);
                g.DrawString(fntArial26, "#1 ACE", new Vector2(X + 140, Y), Color.White);
                g.DrawString(fntArial15, MainCharacter.Name, new Vector2(X + 140, Y += fntArial26.LineSpacing), Color.White);
                g.DrawString(fntArial15, "Level:", new Vector2(X + 140, Y += fntArial15.LineSpacing), Color.White);
                g.DrawStringRightAligned(fntArial15, MainCharacter.Level.ToString(), new Vector2(X + 240, Y + 3), Color.White);
                g.DrawString(fntArial15, "Kills:", new Vector2(X + 140, Y += fntArial15.LineSpacing), Color.White);
                g.DrawStringRightAligned(fntArial15, ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills[MainCharacter.ID].ToString(), new Vector2(X + 240, Y + 3), Color.White);

                Y = 170;
                for (int i = 2; i <= 7 && i - 1 < ListPresentCharacter.Count; ++i)
                {
                    Character ActiveCharacter = ListPresentCharacter[i - 1];
                    g.DrawString(fntArial26, "#" + i, new Vector2(X + 15, Y), Color.White);
                    g.DrawStringRightAligned(fntArial26, ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills[ActiveCharacter.ID].ToString(), new Vector2(X + 130, Y), Color.White);
                    g.DrawString(fntArial26, ActiveCharacter.Name, new Vector2(X + 150, Y), Color.White);
                    Y += fntArial26.LineSpacing;
                }
            }
        }
    }
}
