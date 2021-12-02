using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.MultiForm;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class IntermissionScreen : GameScreen
    {
        private struct PartMenu
        {
            public string Name;
            public string[] Categories;
            public bool Open;
            public PartMenu(string Name, string[] Categories)
            {
                this.Name = Name;
                this.Categories = Categories;
                this.Open = false;
            }
        };

        private enum MenuChoice { StartBattle = 0, View = 1, Customize = 2, Data = 3, Multiplayer = 4, Exit = 5 };

        private Roster PlayerRoster;

        private PartMenu[] Menu;
        private int SelectedChoice = 0;
        private int SubMenu = -1;
        private int SelectedAlpha = 150;
        private bool SelectedAlphaAppearing = true;
        private bool UnitEquipmentAvailable;
        private SpriteFont fntArial12;
        public Dictionary<string, Unit> DicUnitType;
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        public IntermissionScreen()
            : base()
        {
            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
            FormulaParser.ActiveParser = new IntermissionScreenFormulaParser();

            Menu = new PartMenu[] { new PartMenu("Start battle", new string[] { }),
                                    new PartMenu("View", new string[] {"Pilot View", "Unit View", "Parts View" }),
                                    new PartMenu("Customize", new string[] { "Squad Customize", "Unit Customize", "Unit Equipment", "Pilot Selection", "Parts Equip", "Shop", "Forge"}),
                                    new PartMenu("Data", new string[] { "Save", "Load", "Options"}),
                                    new PartMenu("VR training", new string[] { }),
                                    new PartMenu("Exit", new string[] { }) };
            Menu[2].Open = true;

            DicUnitType = Unit.LoadAllUnits();
            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
            DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            UnitEquipmentAvailable = false;

            PlayerRoster = new Roster();
            PlayerRoster.LoadRoster();
            foreach (RosterUnit ActiveUnit in PlayerRoster.DicRosterUnit.Values)
            {
                Unit NewUnit = Unit.FromType(ActiveUnit.UnitTypeName, ActiveUnit.FilePath, GameScreen.ContentFallback, DicUnitType, DicRequirement,
                    DicEffect, DicAutomaticSkillTarget);

                NewUnit.TeamTags.AddTag("Present");
                NewUnit.TeamTags.AddTag("Event");
                PlayerRoster.TeamUnits.Add(NewUnit);
            }

            foreach (RosterCharacter ActiveCharacter in PlayerRoster.DicRosterCharacter.Values)
            {
                Character NewCharacter = new Character(ActiveCharacter.FilePath, GameScreen.ContentFallback, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                NewCharacter.TeamTags.AddTag("Present");
                NewCharacter.TeamTags.AddTag("Event");

                PlayerRoster.TeamCharacters.Add(NewCharacter);
            }

            List<Unit> ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

            for (int U = 0; U < ListPresentUnit.Count; ++U)
            {
                var ActiveUnit = ListPresentUnit[U] as UnitMultiForm;
                if (ActiveUnit != null)
                {
                    UnitEquipmentAvailable = true;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (SelectedAlphaAppearing)
            {//Increment SelectedAlpha before comparing it to 200
                if (++SelectedAlpha >= 200)
                    SelectedAlphaAppearing = false;
            }
            else
            {//Decrement SelectedAlpha before comparing it to 55
                if (--SelectedAlpha <= 55)
                    SelectedAlphaAppearing = true;
            }
            if (InputHelper.InputUpPressed())
            {
                if (Menu[SelectedChoice].Open)
                {
                    if (SubMenu > -1)
                        SubMenu--;
                    else
                    {
                        //Decrement SelectedChoice before comparing it to 0
                        if (--SelectedChoice < 0)
                            SelectedChoice = Menu.Count() - 1;
                        SubMenu = -1;
                    }
                }
                else
                {
                    SelectedChoice -= SelectedChoice > 0 ? 1 : 0;
                    if (Menu[SelectedChoice].Open)
                        SubMenu = Menu[SelectedChoice].Categories.Count() - 1;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (Menu[SelectedChoice].Open)
                {
                    if (SubMenu < Menu[SelectedChoice].Categories.Count() - 1)
                        SubMenu++;
                    else
                    {
                        SelectedChoice++;
                        SubMenu = -1;
                    }
                }
                else
                    //Increment SelectedChoice before comparing it to maximum value.
                    if (++SelectedChoice > Menu.Count() - 1)
                        SelectedChoice = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (SubMenu == -1 && Menu[SelectedChoice].Categories.Length > 0)
                {
                    Menu[SelectedChoice].Open = !Menu[SelectedChoice].Open;
                }
                else
                {
                    switch ((MenuChoice)SelectedChoice)
                    {
                        case MenuChoice.StartBattle:
                            PushScreen(new LoadoutScreen(PlayerRoster));
                            break;

                        case MenuChoice.View:
                            if (SubMenu == 0)//Pilot
                            {
                            }
                            else if (SubMenu == 1)//Units
                            {
                            }
                            else if (SubMenu == 2)//Parts
                            {
                            }
                            PushScreen(new Shop());
                            break;

                        case MenuChoice.Customize:
                            if (SubMenu == 0)//Squad
                            {
                                PushScreen(new SquadSelection(PlayerRoster));
                            }
                            else if (SubMenu == 1)//Unit
                            {
                                PushScreen(new UnitUpgradesScreen(PlayerRoster));
                            }
                            else if (SubMenu == 2)//Unit
                            {
                                if (UnitEquipmentAvailable)
                                {
                                    PushScreen(new UnitEquipmentScreen(PlayerRoster));
                                }
                            }
                            else if (SubMenu == 3)//Pilot
                            {
                                PushScreen(new PilotSwapScreen(PlayerRoster));
                            }
                            else if (SubMenu == 4)//Parts
                            {
                                PushScreen(new UnitPartsScreen(PlayerRoster));
                            }
                            else if (SubMenu == 5)//Shop
                            {
                                PushScreen(new Shop());
                            }
                            else if (SubMenu == 6)//Forge
                            {
                                PushScreen(new Forge());
                            }
                            break;

                        case MenuChoice.Data:
                            if (SubMenu == 0)//Save
                            {
                                SaveProgression();
                            }
                            break;

                        case MenuChoice.Multiplayer:
                            PushScreen(new MultiplayerModeSelectionScreen());
                            break;

                        case MenuChoice.Exit:
                            RemoveScreen(this);
                            break;
                    }
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            int X = Constants.Width / 2 - 60;
            int Y = Constants.Height / 2 - fntArial12.LineSpacing / 2;
            int BaseY = Constants.Height / 2 - fntArial12.LineSpacing / 2;
            if (SubMenu != -1)
                Y -= fntArial12.LineSpacing * (SubMenu + 1);
            for (int i = SelectedChoice; i < Menu.Count(); i++)
            {
                g.DrawString(fntArial12, Menu[i].Name, new Vector2(X, Y), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((Y - BaseY) / 10.0) * 10));
                if (Menu[i].Open)
                    for (int j = 0; j < Menu[i].Categories.Count(); j++)
                        g.DrawString(fntArial12, Menu[i].Categories[j], new Vector2(X + 10, Y += fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((Y - BaseY) / 10.0) * 10));
                Y += fntArial12.LineSpacing  * 3;
            }
            Y = BaseY - fntArial12.LineSpacing * 3;
            if (SubMenu != -1)
                Y -= fntArial12.LineSpacing * (SubMenu + 1);
            for (int i = SelectedChoice - 1; i >= 0; i--)
            {
                if (Menu[i].Open)
                {
                    for (int j = 0; j < Menu[i].Categories.Count(); j++)
                    {
                        g.DrawString(fntArial12, Menu[i].Categories[j], new Vector2(X + 10, Y), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((BaseY - Y) / 10.0) * 10));
                        Y -= fntArial12.LineSpacing;
                    }
                    Y -= fntArial12.LineSpacing;
                }
                g.DrawString(fntArial12, Menu[i].Name, new Vector2(X, Y), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((BaseY - Y) / 10.0) * 10));

                Y -= fntArial12.LineSpacing * 3;
            }
            //Draw cursor.
            g.Draw(sprPixel, new Rectangle(0, BaseY, Constants.Width, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, SelectedAlpha));
        }

        private void SaveProgression()
        {
            //Create the Part file.
            FileStream FS = new FileStream("SRWE Save.bin", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(BattleMap.NextMapType);
            BW.Write(BattleMap.NextMapPath);

            BW.Write(BattleMap.DicRouteChoices.Count);
            foreach (KeyValuePair<string, int> RouteChoice in BattleMap.DicRouteChoices)
            {
                BW.Write(RouteChoice.Key);
                BW.Write(RouteChoice.Value);
            }

            PlayerRoster.SaveTeam(BW);

            BW.Write(SystemList.ListPart.Count);
            foreach (string ActivePart in SystemList.ListPart.Keys)
            {
                BW.Write(ActivePart);
            }

            FS.Close();
            BW.Close();
        }

        public void LoadProgression()
        {
            FileStream FS = new FileStream("SRWE Save.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            BattleMap.DicRouteChoices.Clear();

            PlayerRoster.TeamCharacters.Clear();
            PlayerRoster.TeamUnits.Clear();
            PlayerRoster.TeamSquads.Clear();

            BattleMap.NextMapType = BR.ReadString();
            BattleMap.NextMapPath = BR.ReadString();

            int DicGlobalVariablesCount = BR.ReadInt32();
            for (int R = 0; R < DicGlobalVariablesCount; R++)
            {
                string RouteKey = BR.ReadString();
                string RouteValue = BR.ReadString();
                BattleMap.DicGlobalVariables.Add(RouteKey, RouteValue);
            }

            int DicRouteChoicesCount = BR.ReadInt32();
            for (int R = 0; R < DicRouteChoicesCount; R++)
            {
                string RouteKey = BR.ReadString();
                int RouteValue = BR.ReadInt32();
                BattleMap.DicRouteChoices.Add(RouteKey, RouteValue);
            }

            PlayerRoster.LoadTeam(BR, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            int ListPartCount = BR.ReadInt32();
            for (int P = 0; P < ListPartCount; P++)
            {
                string LoadedPartPath = BR.ReadString();
                string[] PartByType = LoadedPartPath.Split('/');
                if (PartByType[0] == "Standard Parts")
                {
                    SystemList.ListPart.Add(LoadedPartPath, new UnitStandardPart("Content/Units/" + LoadedPartPath + ".pep", DicRequirement, DicEffect, DicAutomaticSkillTarget));
                }
                else if (PartByType[0] == "Consumable Parts")
                {
                    SystemList.ListPart.Add(LoadedPartPath, new UnitConsumablePart("Content/Units/" + LoadedPartPath + ".pep", DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget));
                }
            }

            FS.Close();
            BR.Close();
        }
    }
}
