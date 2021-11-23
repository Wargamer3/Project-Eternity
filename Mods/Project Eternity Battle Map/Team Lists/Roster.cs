using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Roster
    {
        public TeamListSquad TeamSquads;
        public TeamListUnit TeamUnits;
        public TeamListCharacter TeamCharacters;
        public Dictionary<string, RosterCharacter> DicRosterCharacter;
        public Dictionary<string, RosterUnit> DicRosterUnit;

        public Roster()
        {
            TeamSquads = new TeamListSquad();
            TeamUnits = new TeamListUnit();
            TeamCharacters = new TeamListCharacter();
            DicRosterCharacter = new Dictionary<string, RosterCharacter>();
            DicRosterUnit = new Dictionary<string, RosterUnit>();
        }

        public Character AddCharacterFromRoster(string NewCharacterName, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            RosterCharacter RosterCharacter;
            DicRosterCharacter.TryGetValue(NewCharacterName, out RosterCharacter);

            if (RosterCharacter != null)
            {
                Character NewCharacter = new Character(RosterCharacter.FilePath, GameScreen.ContentFallback, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                NewCharacter.DicCharacterLink = new Dictionary<string, Character.CharacterLinkTypes>(RosterCharacter.DicCharacterLink);
                NewCharacter.TeamTags.AddTag("Present");
                NewCharacter.Level = 1;
                if (NewCharacter.Slave != null)
                {
                    NewCharacter.Slave.Level = 1;
                }
                TeamCharacters.Add(NewCharacter);
                CreateCharacterRosterDependencies();
                return NewCharacter;
            }

            return null;
        }

        public void AddUnitFromRoster(string NewUnitName, string[] ArrayNewUnitCharacter, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            RosterUnit RosterUnit;
            DicRosterUnit.TryGetValue(NewUnitName, out RosterUnit);

            if (RosterUnit != null)
            {
                Unit SpawnUnit = Unit.FromType(RosterUnit.UnitTypeName, RosterUnit.FilePath, GameScreen.ContentFallback, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                SpawnUnit.UnitStat.DicUnitLink = new Dictionary<string, UnitStats.UnitLinkTypes>(RosterUnit.DicUnitLink);
                SpawnUnit.TeamEventID = RosterUnit.EventID;
                SpawnUnit.TeamTags.AddTag("Present");
                TeamUnits.Add(SpawnUnit);

                List<Character> ListCharacter = new List<Character>(ArrayNewUnitCharacter.Length);

                for (int C = 0; C < ArrayNewUnitCharacter.Length; ++C)
                {
                    Character NewCharacter = AddCharacterFromRoster(ArrayNewUnitCharacter[C], DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                    ListCharacter.Add(NewCharacter);
                    if (NewCharacter.Slave != null)
                    {
                        ListCharacter.Add(NewCharacter.Slave);
                    }
                }

                SpawnUnit.ArrayCharacterActive = ListCharacter.ToArray();

                CreateUnitRosterDependencies();
            }
        }

        public void SaveTeam(BinaryWriter BW)
        {
            TeamCharacters.Save(BW);
            TeamUnits.Save(BW);
            TeamSquads.Save(BW);
        }

        public void LoadRoster()
        {
            if (!File.Exists("Content/Roster.bin"))
                return;

            FileStream FS = new FileStream("Content/Roster.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            int DicCharacterCount = BR.ReadInt32();
            DicRosterCharacter = new Dictionary<string, RosterCharacter>(DicCharacterCount);

            for (int C = 0; C < DicCharacterCount; C++)
            {
                RosterCharacter NewCharacter = new RosterCharacter(BR.ReadString());

                DicRosterCharacter.Add(NewCharacter.FilePath, NewCharacter);

                int DicCharacterLinkCount = BR.ReadInt32();
                for (int L = 0; L < DicCharacterLinkCount; L++)
                {
                    string Key = BR.ReadString();
                    Character.CharacterLinkTypes CharacterLinkType = (Character.CharacterLinkTypes)BR.ReadByte();
                    NewCharacter.DicCharacterLink.Add(Key, CharacterLinkType);
                }
            }

            int DicUnitCount = BR.ReadInt32();
            DicRosterUnit = new Dictionary<string, RosterUnit>(DicUnitCount);

            for (int U = 0; U < DicUnitCount; U++)
            {
                string UnitTypeName = BR.ReadString();
                string UnitName = BR.ReadString();
                string EventID = BR.ReadString();
                RosterUnit NewUnit = new RosterUnit(UnitTypeName, UnitName, EventID);

                DicRosterUnit.Add(EventID, NewUnit);

                int DicUnitLinkCount = BR.ReadInt32();
                for (int L = 0; L < DicUnitLinkCount; L++)
                {
                    string Key = BR.ReadString();
                    UnitStats.UnitLinkTypes UnitLinkType = (UnitStats.UnitLinkTypes)BR.ReadByte();
                    NewUnit.DicUnitLink.Add(Key, UnitLinkType);
                }
            }

            BR.Close();
            FS.Close();

            CreateCharacterRosterDependencies();
            CreateUnitRosterDependencies();
        }

        public void LoadTeam(BinaryReader BR, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            TeamCharacters.Load(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            TeamUnits.Load(BR, TeamCharacters.ListAll, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            TeamSquads.Load(BR, TeamCharacters.ListAll, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            CreateCharacterRosterDependencies();
            CreateUnitRosterDependencies();
        }

        private void CreateCharacterRosterDependencies()
        {
            foreach (Character ActiveCharacter in TeamCharacters.ListAll)
            {
                foreach (KeyValuePair<string, Character.CharacterLinkTypes> CharacterLink in ActiveCharacter.DicCharacterLink)
                {
                    foreach (Character ActiveLinkCharacter in TeamCharacters.ListAll)
                    {
                        if (ActiveLinkCharacter.FullName == CharacterLink.Key)
                        {
                            ActiveCharacter.ShareStats(ActiveLinkCharacter, CharacterLink.Value);
                            break;
                        }
                    }
                }
            }
        }

        private void CreateUnitRosterDependencies()
        {
            foreach (Unit ActiveUnit in TeamUnits.ListAll)
            {
                foreach (KeyValuePair<string, UnitStats.UnitLinkTypes> UnitLink in ActiveUnit.UnitStat.DicUnitLink)
                {
                    foreach (Unit ActiveLinkUnit in TeamUnits.ListAll)
                    {
                        if (ActiveLinkUnit.TeamEventID == UnitLink.Key)
                        {
                            ActiveUnit.ShareStats(ActiveLinkUnit, UnitLink.Value);
                            break;
                        }
                    }
                }
            }
        }

        public void LoadCharactersLoadout()
        {
            FileStream FS = new FileStream("Content/New game characters loadout.txt", FileMode.Open, FileAccess.ReadWrite);
            StreamReader SR = new StreamReader(FS);

            Dictionary<string, BaseSkillRequirement> DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            Dictionary<string, BaseEffect> DicEffect = BaseEffect.LoadAllEffects();
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();

            //Read everything
            while (!SR.EndOfStream)
            {
                string PilotName = SR.ReadLine();

                Character NewCharacter = new Character(PilotName, GameScreen.ContentFallback, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                NewCharacter.Level = 1;
                TeamCharacters.ListAll.Add(NewCharacter);
            }
            FS.Close();
        }

        public void LoadUnitsLoadout()
        {
            Unit.LoadAllUnits();

            FileStream FS = new FileStream("Content/New game units loadout.txt", FileMode.Open, FileAccess.ReadWrite);
            StreamReader SR = new StreamReader(FS);

            Dictionary<string, Unit> DicUnitType = Unit.LoadAllUnits();
            Dictionary<string, BaseSkillRequirement> DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            Dictionary<string, BaseEffect> DicEffect = BaseEffect.LoadAllEffects();
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();

            //Read everything
            while (!SR.EndOfStream)
            {
                string UnitName = SR.ReadLine();

                string[] UnitInfo = UnitName.Split(new[] { "\\", "/" }, StringSplitOptions.None);
                Unit _SpawnUnit = Unit.FromType(UnitInfo[0], UnitName.Remove(0, UnitInfo[0].Length + 1), GameScreen.ContentFallback, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                TeamUnits.ListAll.Add(_SpawnUnit);
            }
            FS.Close();
        }
    }
}
