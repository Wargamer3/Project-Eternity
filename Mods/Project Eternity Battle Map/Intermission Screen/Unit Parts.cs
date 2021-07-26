using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Roslyn;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{/*http://i.imgur.com/aupA0IO.png
    http://pastebin.com/4jQgZKY4
    With parts getting the skill editor without levels, and consumables getting the spirit editor with SP cost swapped out for uses*/
    public sealed class UnitPartsScreen : UnitListScreen
    {
        public class PartInfo
        {
            public UnitPart ActivePart;
            public List<Unit> ListUnit;

            public PartInfo(UnitPart ActivePart)
            {
                this.ActivePart = ActivePart;
                ListUnit = new List<Unit>();
            }
        }

        private Texture2D sprBackground;
        private SpriteFont fntArial8;
        private SpriteFont fntArial12;

        private int CurrentMaxHP;
        private int CurrentMaxEN;
        private int CurrentMaxArmor;
        private int CurrentMaxMobility;
        private int CurrentMaxMV;
        private List<string> ListTerrainChoices;
        private Dictionary<string, char> DicTerrainLetterAttribute;

        private int CursorIndexUnitPart;
        private int CursorIndexListPart;
        private int CursorIndexListPartUnits;
        private int LineSpacing = 18;
        private int CurrentPagePart;
        private int PageMaxPart;
        private const int MaxPerPagePart = 10;

        private List<PartInfo> ListPartInfo;

        private List<Unit> ListPresentUnit;

        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        private BattleContext GlobalBattleContext;
        private UnitQuickLoadEffectContext GlobalQuickLoadContext;

        public UnitPartsScreen(Roster PlayerRoster)
            : base(PlayerRoster)
        {
            CursorIndexUnitPart = 0;
            Stage = -1;
            ListTerrainChoices = new List<string>();
            DicTerrainLetterAttribute = new Dictionary<string, char>();
            CurrentPagePart = 1;
            PageMaxPart = (int)Math.Ceiling(SystemList.ListPart.Count / (double)MaxPerPagePart);

            DicRequirement = new Dictionary<string, BaseSkillRequirement>();
            DicEffect = new Dictionary<string, BaseEffect>();
            DicAutomaticSkillTarget = new Dictionary<string, AutomaticSkillTargetType>();
            DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();
            GlobalBattleContext = new BattleContext();
            GlobalQuickLoadContext = new UnitQuickLoadEffectContext();
        }

        public override void Load()
        {
            base.Load();

            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
            sprBackground = Content.Load<Texture2D>("Intermission Screens/Pilot Selection");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            LoadEffects();
            LoadSkillRequirements();
            LoadAutomaticSkillActivation();
            LoadManualSkillActivation();

            ListPartInfo = new List<PartInfo>();
            foreach(var ActivePart in SystemList.ListPart)
            {
                ListPartInfo.Add(new PartInfo(ActivePart.Value));
            }
            //Link Units to Parts
            for (int U = 0; U < ListPresentUnit.Count; U++)
            {
                for (int P = 0; P < ListPresentUnit[U].ArrayParts.Length; P++)
                {
                    for (int i = 1; i < ListPartInfo.Count; i++)
                    {
                        if (ListPresentUnit[U].ArrayParts[P] != null && ListPartInfo[i].ActivePart == ListPresentUnit[U].ArrayParts[P])
                        {
                            ListPartInfo[i].ListUnit.Add(ListPresentUnit[U]);
                            break;
                        }
                    }
                }
            }
        }

        private void LoadEffects()
        {
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(GlobalBattleContext, GlobalQuickLoadContext)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadAllEffects())
            {
                if (!DicEffect.ContainsKey(ActiveEffect.Key))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        private void LoadSkillRequirements()
        {
            DicRequirement.Add(BaseSkillRequirement.OnCreatedRequirementName, new OnCreatedRequirement());

            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(UnitSkillRequirement), GlobalBattleContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }
            Dictionary<string, BaseSkillRequirement> DicRequirementBattleMap = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(UnitSkillRequirement), GlobalBattleContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMap)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), GlobalBattleContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementBattleMapAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), GlobalBattleContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMapAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in BaseSkillRequirement.LoadAllRequirements())
            {
                if (!DicRequirement.ContainsKey(ActiveRequirement.Key))
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        private void LoadAutomaticSkillActivation()
        {
            DicAutomaticSkillTarget.Clear();
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(AutomaticSkillTargetType)))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(AutomaticSkillTargetType), GlobalBattleContext))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType)))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType), GlobalBattleContext))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadAllTargetTypes())
            {
                if (!DicAutomaticSkillTarget.ContainsKey(ActiveAutomaticSkill.Key))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }

        private void LoadManualSkillActivation()
        {
            DicManualSkillTarget.Clear();
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll")))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll")))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }

            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadAllTargetTypes())
            {
                if (!DicManualSkillTarget.ContainsKey(ActiveManualSkill.Key))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch (Stage)
            {
                case -1:
                    UnitSelectionMenu.Update(gameTime);
                    if (InputHelper.InputConfirmPressed() && ListPresentUnit.Count > 0)
                    {
                        CursorIndexUnitPart = 0;
                        ListTerrainChoices.AddRange(SelectedUnit.ListTerrainChoices);
                        GoToPartChange();
                    }
                    break;

                case 0:
                    UpdatePartSelectionPart1();
                    break;

                case 1:
                    UpdatePartSelectionPart2();
                    break;

                case 2:
                    SelectPartLinkedUnit();
                    break;
            }
            if (InputHelper.InputCancelPressed())
            {
                if (Stage > -1)
                    Stage--;
                else
                    RemoveScreen(this);
            }
        }

        private void UpdatePartsEffects()
        {
            SelectedUnit.ResetBoosts();
            SelectedUnit.ActivePassiveBuffs();
            if (CursorIndexListPart > 0)
            {
                GlobalBattleContext.SetContext(null, SelectedUnit, SelectedUnit.Pilot, null, SelectedUnit, SelectedUnit.Pilot);
                UnitPart ActivePart = SystemList.ListPart.ElementAt(CursorIndexListPart - 1).Value;
                ActivePart.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                ActivePart.ActivatePassiveBuffs();
            }
        }

        private void GoToPartChange()
        {
            CurrentMaxHP = SelectedUnit.MaxHP;
            CurrentMaxEN = SelectedUnit.MaxEN;
            CurrentMaxArmor = SelectedUnit.Armor;
            CurrentMaxMobility = SelectedUnit.Mobility;
            CurrentMaxMV = SelectedUnit.MaxMovement;

            SelectedUnit.ListTerrainChoices.Clear();
            SelectedUnit.ListTerrainChoices.AddRange(ListTerrainChoices);

            DicTerrainLetterAttribute.Clear();
            DicTerrainLetterAttribute.Add("Air", SelectedUnit.TerrainLetterAttribute("Air"));
            DicTerrainLetterAttribute.Add("Land", SelectedUnit.TerrainLetterAttribute("Land"));
            DicTerrainLetterAttribute.Add("Sea", SelectedUnit.TerrainLetterAttribute("Sea"));
            DicTerrainLetterAttribute.Add("Space", SelectedUnit.TerrainLetterAttribute("Space"));

            Stage = 0;
        }

        private void UpdatePartSelectionPart1()
        {
            if (InputHelper.InputUpPressed())
            {
                if (CursorIndexUnitPart > 0)
                    CursorIndexUnitPart--;
                else
                    CursorIndexUnitPart = SelectedUnit.ArrayParts.Length - 1;
            }
            else if (InputHelper.InputDownPressed())
            {
                if (CursorIndexUnitPart + 1 < SelectedUnit.ArrayParts.Length)
                    CursorIndexUnitPart++;
                else
                    CursorIndexUnitPart = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                CursorIndexListPart = 0;
                Stage = 1;
                UpdatePartsEffects();
            }
        }

        private void UpdatePartSelectionPart2()
        {
            if (InputHelper.InputUpPressed())
            {
                if (CursorIndexListPart > 0)
                    CursorIndexListPart--;
                else
                    CursorIndexListPart = ListPartInfo.Count;

                UpdatePartsEffects();
            }
            else if (InputHelper.InputDownPressed())
            {
                if (CursorIndexListPart + 1 < ListPartInfo.Count + 1)
                    CursorIndexListPart++;
                else
                    CursorIndexListPart = 0;

                UpdatePartsEffects();
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndexListPart == 0)
                {
                    //Remove link.
                    foreach (PartInfo ActivePartInfo in ListPartInfo)
                    {
                        if (ActivePartInfo.ActivePart == SelectedUnit.ArrayParts[CursorIndexUnitPart])
                        {
                            ActivePartInfo.ListUnit.Remove(SelectedUnit);
                        }
                    }
                    SelectedUnit.ArrayParts[CursorIndexUnitPart] = null;
                    GoToPartChange();
                }
                else
                {
                    PartInfo ActivePartInfo = ListPartInfo[CursorIndexListPart - 1];
                    if (ActivePartInfo.ActivePart.Quantity > 0)
                    {
                        if (ActivePartInfo.ListUnit.Count == ActivePartInfo.ActivePart.Quantity && !SelectedUnit.ArrayParts.Contains(ActivePartInfo.ActivePart))
                        {
                            Stage = 2;
                        }
                        else
                        {
                            SelectedUnit.ArrayParts[CursorIndexUnitPart] = ActivePartInfo.ActivePart;
                            ActivePartInfo.ListUnit.Add(SelectedUnit);
                            GoToPartChange();
                        }
                    }
                }
            }
        }

        private void SelectPartLinkedUnit()
        {
            PartInfo ActivePartInfo = ListPartInfo[CursorIndexListPart - 1];

            if (InputHelper.InputUpPressed())
            {
                if (CursorIndexListPartUnits > 0)
                    CursorIndexListPartUnits--;
                else
                    CursorIndexListPartUnits = ActivePartInfo.ListUnit.Count;

                UpdatePartsEffects();
            }
            else if (InputHelper.InputDownPressed())
            {
                if (CursorIndexListPartUnits + 1 < ActivePartInfo.ListUnit.Count + 1)
                    CursorIndexListPartUnits++;
                else
                    CursorIndexListPartUnits = 0;

                UpdatePartsEffects();
            }
            else if (InputHelper.InputConfirmPressed())
            {
                Unit LinkedUnit = ActivePartInfo.ListUnit[CursorIndexListPartUnits];
                for (int P = 0; P < LinkedUnit.ArrayParts.Length; ++P)
                {
                    if (LinkedUnit.ArrayParts[P] == ActivePartInfo.ActivePart)
                    {
                        LinkedUnit.ArrayParts[P] = null;
                    }
                }

                SelectedUnit.ArrayParts[CursorIndexUnitPart] = ActivePartInfo.ActivePart;
                ActivePartInfo.ListUnit.Add(SelectedUnit);
                ActivePartInfo.ListUnit.Remove(LinkedUnit);

                SelectedUnit.ResetBoosts();
                SelectedUnit.ActivePassiveBuffs();
                LinkedUnit.ResetBoosts();
                LinkedUnit.ActivePassiveBuffs();
                GoToPartChange();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            g.DrawString(fntFinlanderFont, "Unit Parts", new Vector2(10, 10), Color.White);

            switch (Stage)
            {
                case -1:
                    DrawMenu(g);
                    break;

                case 0:
                    DrawPartMenu(g, false);
                    break;

                case 1:
                    DrawPartMenu(g, true);
                    break;

                case 2:
                    DrawPartLinkedUnits(g);
                    break;
            }
        }

        private void DrawUnitInfo(CustomSpriteBatch g, int StartX)
        {
            g.DrawString(fntFinlanderFont, SelectedUnit.RelativePath, new Vector2(StartX + 15, 40), Color.White);
            g.Draw(SelectedUnit.SpriteUnit, new Vector2(StartX + 30, 100), Color.White);

            int Y = 155;
            DrawBox(g, new Vector2(StartX + 10, Y + LineSpacing), 310, 85, Color.White);
            for (int P = 0; P < SelectedUnit.ArrayParts.Length; ++P)
            {
                if (SelectedUnit.ArrayParts[P] != null)
                {
                    g.DrawString(fntFinlanderFont, SelectedUnit.ArrayParts[P].Name, new Vector2(StartX + 15, Y += LineSpacing), Color.White);
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "None", new Vector2(StartX + 15, Y += LineSpacing), Color.White);
                }
            }
            g.Draw(sprPixel, new Rectangle(StartX + 15, 180 + CursorIndexUnitPart * LineSpacing, 300, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));

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
            if (ListTerrainChoices.Contains(Core.Units.UnitStats.TerrainAir))
                g.Draw(sprSky, new Vector2(StartX + 150, Y + 7), Color.White);
            else
                g.Draw(sprLand, new Vector2(StartX + 150, Y + 7), Color.White);

            if (SelectedUnit.ListTerrainChoices.Contains(Core.Units.UnitStats.TerrainAir))
                g.Draw(sprSky, new Vector2(StartX + 230, Y + 7), Color.White);
            else
                g.Draw(sprLand, new Vector2(StartX + 230, Y + 7), Color.White);

            g.DrawString(fntFinlanderFont, "Terrain", new Vector2(StartX + 15, Y += LineSpacing), Color.White);

            DrawTerrainChange(g, Core.Units.UnitStats.TerrainAir, sprSky, StartX + 40, Y + 28);
            DrawTerrainChange(g, Core.Units.UnitStats.TerrainLand, sprLand, StartX + 90, Y + 28);
            DrawTerrainChange(g, Core.Units.UnitStats.TerrainSea, sprSea, StartX + 140, Y + 28);
            DrawTerrainChange(g, Core.Units.UnitStats.TerrainSpace, sprSpace, StartX + 190, Y + 28);
        }

        private void DrawPartMenu(CustomSpriteBatch g, bool ShowListPartCursor)
        {
            Unit ActiveUnit = SelectedUnit;

            DrawUnitInfo(g, 0);

            int Y = 5;
            DrawBox(g, new Vector2(325, Y), 310, 315, Color.White);
            g.DrawString(fntFinlanderFont, "REMOVE PART", new Vector2(330, Y), Color.Red);
            g.DrawStringRightAligned(fntFinlanderFont, CurrentPagePart + "/" + PageMaxPart, new Vector2(625, Y), Color.Yellow);
            foreach (PartInfo ActivePart in ListPartInfo)
            {
                if (ActivePart.ActivePart.Quantity > 0)
                {
                    g.DrawString(fntFinlanderFont, ActivePart.ActivePart.Name, new Vector2(330, Y += LineSpacing), Color.White);
                    g.DrawStringRightAligned(fntFinlanderFont, ActivePart.ListUnit.Count + "/" + ActivePart.ActivePart.Quantity, new Vector2(625, Y), Color.White);
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "--------", new Vector2(330, Y += LineSpacing), Color.White);
                    g.DrawStringRightAligned(fntFinlanderFont, "-/-", new Vector2(625, Y), Color.White);
                }
            }
            Y = 305;
            DrawBox(g, new Vector2(325, Y + 20), 310, 150, Color.White);

            if (ShowListPartCursor)
            {
                g.Draw(sprPixel, new Rectangle(330, 10 + CursorIndexListPart * LineSpacing, 300, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
            }
        }

        private void DrawPartLinkedUnits(CustomSpriteBatch g)
        {
            DrawUnitInfo(g, 315);

            DrawBox(g, new Vector2(5, 50), 315, 425, Color.White);
            PartInfo ActivePartInfo = ListPartInfo[CursorIndexListPart - 1];

            for (int U = 0; U < ActivePartInfo.ListUnit.Count; ++U)
            {
                g.DrawString(fntFinlanderFont, ActivePartInfo.ListUnit[U].RelativePath, new Vector2(10, 55 + LineSpacing * U), Color.White);
            }

            g.Draw(sprPixel, new Rectangle(10, 61 + CursorIndexListPartUnits * LineSpacing, 305, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
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

        private void DrawTerrainChange(CustomSpriteBatch g, string Terrain, Texture2D Sprite, int X, int Y)
        {
            g.Draw(Sprite, new Vector2(X, Y), Color.White);
            g.DrawString(fntFinlanderFont, DicTerrainLetterAttribute[Terrain].ToString(), new Vector2(X + 25, Y + 4), Color.White);

            g.Draw(Sprite, new Vector2(X, Y + 46), Color.White);
            g.DrawString(fntFinlanderFont, SelectedUnit.TerrainLetterAttribute(Terrain).ToString(), new Vector2(X + 25, Y + 50), Color.White);
        }
    }
}
