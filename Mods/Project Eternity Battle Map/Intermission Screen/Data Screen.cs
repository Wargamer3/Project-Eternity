using System;
using System.IO;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DataScreen : GameScreen
    {
        private Texture2D sprMapMenuBackground;

        private SpriteFont fntArial12;
        private SpriteFont fntFinlanderFont;

        private FMODSound sndConfirm;
        private FMODSound sndSelection;
        private FMODSound sndDeny;
        private FMODSound sndCancel;

        private readonly Roster PlayerRoster;

        private int CursorIndex;

        public Dictionary<string, Unit> DicUnitType;
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        public DataScreen(Roster PlayerRoster)
        {
            this.PlayerRoster = PlayerRoster;
        }

        public override void Load()
        {
            sprMapMenuBackground = Content.Load<Texture2D>("Status Screen/Background Black");

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");

            DicUnitType = Unit.LoadAllUnits();
            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
            DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                --CursorIndex;

                if (CursorIndex < 0)
                    CursorIndex = 2;

                sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                ++CursorIndex;

                if (CursorIndex >= 3)
                    CursorIndex = 0;

                sndSelection.Play();
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndex == 0)
                {
                    SaveProgression();
                }
                else if (CursorIndex == 1)
                {
                    LoadProgression(PlayerRoster, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                }
                else if (CursorIndex == 2)
                {
                    RemoveScreen(this);
                    sndCancel.Play();
                }

                sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
                sndCancel.Play();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            g.Draw(sprMapMenuBackground, new Vector2(0, 0), Color.White);
            g.DrawString(fntFinlanderFont, "Data", new Vector2(120, 10), Color.White);
            DrawBox(g, new Vector2(10, 45), 420, 300, Color.White);
            g.DrawString(fntFinlanderFont, "Save", new Vector2(20, 50), Color.White);
            g.DrawString(fntFinlanderFont, "Load", new Vector2(20, 80), Color.White);
            g.DrawString(fntFinlanderFont, "Exit", new Vector2(20, 110), Color.White);

            g.Draw(sprPixel, new Rectangle(17, 52 + CursorIndex * 30, 100, 30), Color.FromNonPremultiplied(255, 255, 255, 127));
        }

        private void SaveProgression()
        {
            //Create the Part file.
            FileStream FS = new FileStream("User Data/Saves/SRWE Save.bin", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SaveProgression(BW, PlayerRoster);

            FS.Close();
            BW.Close();
        }

        public static void SaveProgression(BinaryWriter BW, Roster PlayerRoster)
        {
            BW.Write(BattleMap.NextMapType);
            BW.Write(BattleMap.NextMapPath);
            BW.Write(BattleMap.ClearedStages);
            BW.Write(Constants.Money);

            BW.Write(BattleMap.DicGlobalVariables.Count);
            foreach (KeyValuePair<string, string> GlobalVariable in BattleMap.DicGlobalVariables)
            {
                BW.Write(GlobalVariable.Key);
                BW.Write(GlobalVariable.Value);
            }

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
        }

        public static void LoadProgression(Roster PlayerRoster, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            FileStream FS = new FileStream("User Data/Saves/SRWE Save.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            LoadProgression(BR, PlayerRoster, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            FS.Close();
            BR.Close();
        }

        public static void LoadProgression(BinaryReader BR, Roster PlayerRoster, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            BattleMap.DicGlobalVariables.Clear();
            BattleMap.DicRouteChoices.Clear();

            BattleMap.NextMapType = BR.ReadString();
            BattleMap.NextMapPath = BR.ReadString();
            BattleMap.ClearedStages = BR.ReadInt32();
            Constants.Money = BR.ReadInt32();

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

            SystemList.ListPart.Clear();
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
        }
    }
}
