using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct MissionInfo
    {
        public string MapPath;
        public int QuantityOwned;
        public int SortOrder;

        public MissionInfo(string MapPath, int QuantityOwned)
        {
            this.MapPath = MapPath;
            this.QuantityOwned = QuantityOwned;

            SortOrder = 0;
        }
    }

    public class BattleMapPlayerInventory
    {
        public List<Squad> ListOwnedSquad;
        public List<Character> ListOwnedCharacter;
        public List<MissionInfo> ListOwnedMission;
        public List<PlayerLoadout> ListSquadLoadout;

        public PlayerLoadout ActiveLoadout;

        public BattleMapPlayerInventory()
        {
            ListSquadLoadout = new List<PlayerLoadout>();
            ListOwnedSquad = new List<Squad>();
            ListOwnedCharacter = new List<Character>();
            ListOwnedMission = new List<MissionInfo>();

            ActiveLoadout = new PlayerLoadout();
            ListSquadLoadout.Add(ActiveLoadout);
        }

        public void Load(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            int ListOwnedSquadCount = BR.ReadInt32();
            ListOwnedSquad = new List<Squad>(ListOwnedSquadCount);
            for (int S = 0; S < ListOwnedSquadCount; ++S)
            {
                string RelativePath = BR.ReadString();
                string UnitTypeName = BR.ReadString();

                Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);

                string CharacterFullName = BR.ReadString();
                Character LoadedCharacter = new Character(CharacterFullName, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
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
                Character LoadedCharacter = new Character(CharacterFullName, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                LoadedCharacter.Level = 1;

                ListOwnedCharacter.Add(LoadedCharacter);
            }

            int ListOwnedMissionCount = BR.ReadInt32();
            ListOwnedMission = new List<MissionInfo>(ListOwnedMission);
            for (int C = 0; C < ListOwnedMissionCount; ++C)
            {
                string MissionPath = BR.ReadString();
                int QuantityOwned = BR.ReadInt32();

                MissionInfo LoadedMission = new MissionInfo(MissionPath, QuantityOwned);

                ListOwnedMission.Add(LoadedMission);
            }

            int ListSquadLoadoutCount = BR.ReadInt32();
            ListSquadLoadout = new List<PlayerLoadout>(ListSquadLoadoutCount);
            for (int L = 0; L < ListSquadLoadoutCount; ++L)
            {
                PlayerLoadout NewSquadLoadout = new PlayerLoadout();
                ListSquadLoadout.Add(NewSquadLoadout);

                int NewSquadLoadoutSquadCount = BR.ReadInt32();
                NewSquadLoadout.ListSpawnSquad = new List<Squad>(NewSquadLoadoutSquadCount);
                for (int S = 0; S < NewSquadLoadoutSquadCount; ++S)
                {
                    string RelativePath = BR.ReadString();

                    if (string.IsNullOrEmpty(RelativePath))
                    {
                        NewSquadLoadout.ListSpawnSquad.Add(null);
                        continue;
                    }

                    string UnitTypeName = BR.ReadString();
                    string CharacterFullName = BR.ReadString();

                    Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);

                    Character LoadedCharacter = new Character(CharacterFullName, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                    LoadedCharacter.Level = 1;

                    LoadedUnit.ArrayCharacterActive[0] = LoadedCharacter;

                    Squad NewSquad = new Squad("Squad", LoadedUnit);
                    NewSquad.IsPlayerControlled = true;

                    NewSquadLoadout.ListSpawnSquad.Add(NewSquad);
                }
            }

            ActiveLoadout = ListSquadLoadout[0];
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

            BW.Write(ListOwnedMission.Count);
            for (int M = 0; M < ListOwnedMission.Count; ++M)
            {
                BW.Write(ListOwnedMission[M].MapPath);
            }

            BW.Write(ListSquadLoadout.Count);
            for (int L = 0; L < ListSquadLoadout.Count; ++L)
            {
                BW.Write(ListSquadLoadout[L].ListSpawnSquad.Count);
                for (int S = 0; S < ListSquadLoadout[L].ListSpawnSquad.Count; ++S)
                {
                    if (ListSquadLoadout[L].ListSpawnSquad[S] == null)
                    {
                        BW.Write(string.Empty);
                    }
                    else
                    {
                        BW.Write(ListSquadLoadout[L].ListSpawnSquad[S].CurrentLeader.RelativePath);
                        BW.Write(ListSquadLoadout[L].ListSpawnSquad[S].CurrentLeader.UnitTypeName);
                        BW.Write(ListSquadLoadout[L].ListSpawnSquad[S].CurrentLeader.Pilot.FullName);
                    }
                }
            }
        }
    }

    public class PlayerLoadout
    {
        public List<Squad> ListSpawnSquad;
        public List<Commander> ListSpawnCommander;

        public PlayerLoadout()
        {
            ListSpawnSquad = new List<Squad>();
            ListSpawnCommander = new List<Commander>();
        }
    }
}
