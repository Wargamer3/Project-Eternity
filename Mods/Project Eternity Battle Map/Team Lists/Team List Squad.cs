using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TeamListSquad
    {
        public readonly List<Squad> ListAll = new List<Squad>();
        public readonly List<Squad> ListAvailable = new List<Squad>();

        public void Clear()
        {
            ListAll.Clear();
            ListAvailable.Clear();
        }

        public List<Squad> GetAll()
        {
            return new List<Squad>(ListAvailable);
        }

        public List<Squad> GetPresent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Present"));
        }

        public List<Squad> GetEvent()
        {
            return ListAvailable.FindAll(S => S.TeamTags.ContainsTag("Event"));
        }

        public void Add(Squad SquadToAdd)
        {
            ListAll.Add(SquadToAdd);
            ListAvailable.Add(SquadToAdd);
        }

        public void MakeAllAvailableAndPresent()
        {
            ListAvailable.Clear();
            ListAvailable.AddRange(ListAll);
            foreach (Squad ActiveSquad in ListAll)
            {
                ActiveSquad.TeamTags.AddTag("Present");
            }
        }

        public void Remove(Squad SquadToRemove)
        {
            ListAvailable.Remove(SquadToRemove);
        }

        public void RemoveAll(string SquadName)
        {
            ListAll.RemoveAll(C => C.SquadName == SquadName);
            ListAvailable.RemoveAll(C => C.SquadName == SquadName);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListAll.Count);
            for (int S = 0; S < ListAll.Count; S++)
            {
                BW.Write(ListAll[S].TeamTags.ContainsTag("Present"));
                BW.Write(ListAll[S].TeamTags.ContainsTag("Event"));
                ListAll[S].Save(BW);
            }
        }

        public void Load(BinaryReader BR, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            Clear();

            int ListSquadCount = BR.ReadInt32();
            for (int C = 0; C < ListSquadCount; C++)
            {
                bool IsPresent = BR.ReadBoolean();
                bool IsEvent = BR.ReadBoolean();
                Squad LoadedSquad = Squad.LoadSquadWithProgression(BR, GameScreen.ContentFallback, DicUnitType, DicRequirement, DicEffect);

                if (IsPresent)
                {
                    LoadedSquad.TeamTags.AddTag("Present");
                }
                if (IsEvent)
                {
                    LoadedSquad.TeamTags.AddTag("Event");
                }

                Add(LoadedSquad);
            }
        }
    }
}
