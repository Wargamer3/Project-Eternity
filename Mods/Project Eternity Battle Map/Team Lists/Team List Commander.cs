using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TeamListCommander
    {
        public readonly List<Commander> ListAll = new List<Commander>();
        public readonly List<Commander> ListAvailable = new List<Commander>();

        public void Clear()
        {
            ListAll.Clear();
            ListAvailable.Clear();
        }

        public List<Commander> GetAll()
        {
            return new List<Commander>(ListAvailable);
        }

        public List<Commander> GetPresent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Present"));
        }

        public List<Commander> GetEvent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Event"));
        }

        public void Add(Commander CharacterToAdd)
        {
            ListAll.Add(CharacterToAdd);
            ListAvailable.Add(CharacterToAdd);
        }

        public void MakeAllAvailableAndPresent()
        {
            ListAvailable.Clear();
            ListAvailable.AddRange(ListAll);
            foreach (Commander ActiveCharacter in ListAll)
            {
                ActiveCharacter.TeamTags.AddTag("Present");
            }
        }

        public void Remove(Commander CharacterToRemove)
        {
            ListAvailable.Remove(CharacterToRemove);
        }

        public void RemoveAll(string CharacterName)
        {
            ListAll.RemoveAll(C => C.Name == CharacterName);
            ListAvailable.RemoveAll(C => C.Name == CharacterName);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListAll.Count);
            for (int C = 0; C < ListAll.Count; C++)
            {
                BW.Write(ListAll[C].TeamTags.ContainsTag("Present"));
                BW.Write(ListAll[C].TeamTags.ContainsTag("Event"));
                ListAll[C].SaveProgression(BW);
            }
        }

        public void Load(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
             Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Clear();

            int ListCharacterCount = BR.ReadInt32();
            for (int C = 0; C < ListCharacterCount; C++)
            {
                bool IsPresent = BR.ReadBoolean();
                bool IsEvent = BR.ReadBoolean();

                string CharacterFullName = BR.ReadString();
                Commander LoadedCommander = new Commander(CharacterFullName);
                LoadedCommander.LoadProgression(BR);

                if (IsPresent)
                {
                    LoadedCommander.TeamTags.AddTag("Present");
                }
                if (IsEvent)
                {
                    LoadedCommander.TeamTags.AddTag("Event");
                }

                Add(LoadedCommander);
            }
        }
    }
}
