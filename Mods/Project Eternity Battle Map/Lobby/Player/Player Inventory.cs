using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PlayerInventory
    {
        public List<SquadLoadout> ListSquadLoadout;
        public List<Squad> ListOwnedSquad;
        public List<Character> ListOwnedCharacter;

        public SquadLoadout ActiveLoadout;

        public PlayerInventory()
        {
            ListSquadLoadout = new List<SquadLoadout>();
            ListOwnedSquad = new List<Squad>();
            ListOwnedCharacter = new List<Character>();

            ActiveLoadout = new SquadLoadout();
            ListSquadLoadout.Add(ActiveLoadout);
        }

        public void Load(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, Dictionary<string, Unit> DicUnitType,
            Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            int ListOwnedSquadCount = BR.ReadInt32();
            ListOwnedSquad = new List<Squad>(ListOwnedSquadCount);
            for (int S = 0; S < ListOwnedSquadCount; ++S)
            {
                string RelativePath = BR.ReadString();
                string UnitTypeName = BR.ReadString();

                Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                string CharacterFullName = BR.ReadString();
                Character LoadedCharacter = new Character(CharacterFullName, GameScreen.ContentFallback, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCharacter.Level = 1;

                LoadedUnit.ArrayCharacterActive[0] = LoadedCharacter;

                Squad NewSquad = new Squad("Squad", LoadedUnit);
                NewSquad.IsPlayerControlled = true;

                ListOwnedSquad.Add(NewSquad);
            }

            int ListOwnedCharacterCount = BR.ReadInt32();
            ListOwnedCharacter = new List<Character>(ListOwnedCharacterCount);
            for (int C = 0; C < ListOwnedCharacterCount; ++C)
            {
                string CharacterFullName = BR.ReadString();
                Character LoadedCharacter = new Character(CharacterFullName, GameScreen.ContentFallback, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCharacter.Level = 1;

                ListOwnedCharacter.Add(LoadedCharacter);
            }

            int ListSquadLoadoutCount = BR.ReadInt32();
            ListSquadLoadout = new List<SquadLoadout>(ListSquadLoadoutCount);
            for (int L = 0; L < ListSquadLoadoutCount; ++L)
            {
                SquadLoadout NewSquadLoadout = new SquadLoadout();
                ListSquadLoadout.Add(NewSquadLoadout);

                int NewSquadLoadoutSquadCount = BR.ReadInt32();
                NewSquadLoadout.ListSquad = new List<Squad>(NewSquadLoadoutSquadCount);
                for (int S = 0; S < NewSquadLoadoutSquadCount; ++S)
                {
                    string RelativePath = BR.ReadString();

                    if (string.IsNullOrEmpty(RelativePath))
                    {
                        NewSquadLoadout.ListSquad.Add(null);
                        continue;
                    }

                    string UnitTypeName = BR.ReadString();
                    string CharacterFullName = BR.ReadString();

                    Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                    Character LoadedCharacter = new Character(CharacterFullName, GameScreen.ContentFallback, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                    LoadedCharacter.Level = 1;

                    LoadedUnit.ArrayCharacterActive[0] = LoadedCharacter;

                    Squad NewSquad = new Squad("Squad", LoadedUnit);
                    NewSquad.IsPlayerControlled = true;

                    NewSquadLoadout.ListSquad.Add(NewSquad);
                }
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListOwnedSquad.Count);
            for (int S = 0; S < ListOwnedSquad.Count; ++S)
            {
                BW.Write(ListOwnedSquad[S].CurrentLeader.RelativePath);
                BW.Write(ListOwnedSquad[S].CurrentLeader.UnitTypeName);
                BW.Write(ListOwnedSquad[S].CurrentLeader.Pilot.FullName);
            }

            BW.Write(ListOwnedCharacter.Count);
            for (int C = 0; C < ListOwnedCharacter.Count; ++C)
            {
                BW.Write(ListOwnedCharacter[C].FullName);
            }

            BW.Write(ListSquadLoadout.Count);
            for (int L = 0; L < ListSquadLoadout.Count; ++L)
            {
                BW.Write(ListSquadLoadout[L].ListSquad.Count);
                for (int S = 0; S < ListSquadLoadout[L].ListSquad.Count; ++S)
                {
                    if (ListSquadLoadout[L].ListSquad[S] == null)
                    {
                        BW.Write(string.Empty);
                    }
                    else
                    {
                        BW.Write(ListSquadLoadout[L].ListSquad[S].CurrentLeader.RelativePath);
                        BW.Write(ListSquadLoadout[L].ListSquad[S].CurrentLeader.UnitTypeName);
                        BW.Write(ListSquadLoadout[L].ListSquad[S].CurrentLeader.Pilot.FullName);
                    }
                }
            }
        }
    }

    public class SquadLoadout
    {
        public List<Squad> ListSquad;

        public SquadLoadout()
        {
            ListSquad = new List<Squad>();
        }
    }
}
