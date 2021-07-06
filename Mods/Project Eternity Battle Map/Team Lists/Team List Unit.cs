using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TeamListUnit
    {
        public readonly List<Unit> ListAll = new List<Unit>();
        public readonly List<Unit> ListAvailable = new List<Unit>();

        public void Clear()
        {
            ListAll.Clear();
            ListAvailable.Clear();
        }

        public List<Unit> GetAll()
        {
            return new List<Unit>(ListAvailable);
        }

        public List<Unit> GetPresent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Present"));
        }

        public List<Unit> GetEvent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Event"));
        }

        public void Add(Unit UnitToAdd)
        {
            ListAll.Add(UnitToAdd);
            ListAvailable.Add(UnitToAdd);
        }

        public void MakeAllAvailableAndPresent()
        {
            ListAvailable.Clear();
            ListAvailable.AddRange(ListAll);
            foreach (Unit ActiveUnit in ListAll)
            {
                ActiveUnit.TeamTags.AddTag("Present");
            }
        }

        public void Remove(Unit UnitToRemove)
        {
            ListAvailable.Remove(UnitToRemove);
        }

        public void RemoveAll(string UnitName)
        {
            ListAll.RemoveAll(C => C.RelativePath == UnitName);
            ListAvailable.RemoveAll(C => C.RelativePath == UnitName);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListAll.Count);
            for (int U = 0; U < ListAll.Count; U++)
            {
                BW.Write(ListAll[U].TeamTags.ContainsTag("Present"));
                BW.Write(ListAll[U].TeamTags.ContainsTag("Event"));
                ListAll[U].Save(BW);
            }
        }

        public void Load(BinaryReader BR, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Clear();

            int ListUnitCount = BR.ReadInt32();
            for (int C = 0; C < ListUnitCount; C++)
            {
                bool IsPresent = BR.ReadBoolean();
                bool IsEvent = BR.ReadBoolean();

                Unit LoadedUnit = Unit.LoadUnitWithProgress(BR, GameScreen.ContentFallback, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                if (IsPresent)
                {
                    LoadedUnit.TeamTags.AddTag("Present");
                }
                if (IsEvent)
                {
                    LoadedUnit.TeamTags.AddTag("Event");
                }

                Add(LoadedUnit);
            }
        }
    }
}
