using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TeamListCharacter
    {
        public readonly List<Character> ListAll = new List<Character>();
        public readonly List<Character> ListAvailable = new List<Character>();

        public void Clear()
        {
            ListAll.Clear();
            ListAvailable.Clear();
        }

        public List<Character> GetAll()
        {
            return new List<Character>(ListAvailable);
        }

        public List<Character> GetPresent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Present"));
        }

        public List<Character> GetEvent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Event"));
        }

        public void Add(Character CharacterToAdd)
        {
            ListAll.Add(CharacterToAdd);
            ListAvailable.Add(CharacterToAdd);
        }

        public void MakeAllAvailableAndPresent()
        {
            ListAvailable.Clear();
            ListAvailable.AddRange(ListAll);
            foreach (Character ActiveCharacter in ListAll)
            {
                ActiveCharacter.TeamTags.AddTag("Present");
            }
        }

        public void Remove(Character CharacterToRemove)
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
                Character LoadedCharacter = new Character(CharacterFullName, GameScreen.ContentFallback, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                LoadedCharacter.LoadProgression(BR);

                if (IsPresent)
                {
                    LoadedCharacter.TeamTags.AddTag("Present");
                }
                if (IsEvent)
                {
                    LoadedCharacter.TeamTags.AddTag("Event");
                }

                Add(LoadedCharacter);
            }
        }
    }
}
