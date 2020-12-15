using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Item;

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

        public void Load(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            Clear();

            int ListCharacterCount = BR.ReadInt32();
            for (int C = 0; C < ListCharacterCount; C++)
            {
                bool IsPresent = BR.ReadBoolean();
                bool IsEvent = BR.ReadBoolean();

                Character LoadedCharacter = Character.LoadCharacterWithProgression(BR, GameScreen.ContentFallback, DicRequirement, DicEffect);

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
