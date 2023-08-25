using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnitInfo
    {
        public Unit Leader;
        public byte QuantityOwned;

        public UnitInfo(Unit Leader, byte QuantityOwned)
        {
            this.Leader = Leader;
            this.QuantityOwned = QuantityOwned;
        }
    }

    public class CharacterInfo
    {
        public Character Pilot;
        public byte QuantityOwned;

        public CharacterInfo(Character Pilot, byte QuantityOwned)
        {
            this.Pilot = Pilot;
            this.QuantityOwned = QuantityOwned;
        }
    }

    public struct MissionInfo
    {
        public string MapPath;
        public byte QuantityOwned;
        public int SortOrder;

        public MissionInfo(string MapPath, byte QuantityOwned)
        {
            this.MapPath = MapPath;
            this.QuantityOwned = QuantityOwned;

            SortOrder = 0;
        }
    }

    public class BattleMapPlayerInventory
    {
        public Dictionary<string, UnitInfo> DicOwnedSquad;
        public Dictionary<string, CharacterInfo> DicOwnedCharacter;
        public Dictionary<string, MissionInfo> DicOwnedMission;
        public List<PlayerLoadout> ListSquadLoadout;

        public PlayerLoadout ActiveLoadout;

        public BattleMapPlayerInventory()
        {
            ListSquadLoadout = new List<PlayerLoadout>();
            DicOwnedSquad = new Dictionary<string, UnitInfo>();
            DicOwnedCharacter = new Dictionary<string, CharacterInfo>();
            DicOwnedMission = new Dictionary<string, MissionInfo>();

            ActiveLoadout = new PlayerLoadout();
            ListSquadLoadout.Add(ActiveLoadout);
        }

        public void Load(BinaryReader BR, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            int ListOwnedSquadCount = BR.ReadInt32();
            DicOwnedSquad = new Dictionary<string, UnitInfo>(ListOwnedSquadCount);
            for (int S = 0; S < ListOwnedSquadCount; ++S)
            {
                string RelativePath = BR.ReadString();
                string UnitTypeName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                LoadedUnit.ID = LoadedUnit.ItemName;

                DicOwnedSquad.Add(RelativePath, new UnitInfo(LoadedUnit, QuantityOwned));
            }

            int ListOwnedCharacterCount = BR.ReadInt32();
            DicOwnedCharacter = new Dictionary<string, CharacterInfo>(ListOwnedCharacterCount);
            for (int C = 0; C < ListOwnedCharacterCount; ++C)
            {
                string CharacterFullName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                Character LoadedCharacter = new Character(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCharacter.Level = 1;
                LoadedCharacter.ID = LoadedCharacter.Name;

                DicOwnedCharacter.Add(CharacterFullName, new CharacterInfo(LoadedCharacter, QuantityOwned));
            }

            int ListOwnedMissionCount = BR.ReadInt32();
            DicOwnedMission = new Dictionary<string, MissionInfo>();
            for (int C = 0; C < ListOwnedMissionCount; ++C)
            {
                string MissionPath = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                MissionInfo LoadedMission = new MissionInfo(MissionPath, QuantityOwned);

                DicOwnedMission.Add(MissionPath, LoadedMission);
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

                    Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedUnit.ID = LoadedUnit.ItemName;

                    if (!string.IsNullOrEmpty(CharacterFullName))
                    {
                        Character LoadedCharacter = new Character(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                        LoadedCharacter.Level = 1;
                        LoadedCharacter.ID = LoadedCharacter.Name;

                        LoadedUnit.ArrayCharacterActive[0] = LoadedCharacter;
                    }

                    Squad NewSquad = new Squad("Squad", LoadedUnit);
                    NewSquad.IsPlayerControlled = true;

                    NewSquadLoadout.ListSpawnSquad.Add(NewSquad);
                }
            }

            ActiveLoadout = ListSquadLoadout[0];
        }

        public void Load(ByteReader BR, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            int ListOwnedSquadCount = BR.ReadInt32();
            DicOwnedSquad = new Dictionary<string, UnitInfo>(ListOwnedSquadCount);
            for (int S = 0; S < ListOwnedSquadCount; ++S)
            {
                string RelativePath = BR.ReadString();
                string UnitTypeName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                LoadedUnit.ID = LoadedUnit.ItemName;

                DicOwnedSquad.Add(RelativePath, new UnitInfo(LoadedUnit, QuantityOwned));
            }

            int ListOwnedCharacterCount = BR.ReadInt32();
            DicOwnedCharacter = new Dictionary<string, CharacterInfo>(ListOwnedCharacterCount);
            for (int C = 0; C < ListOwnedCharacterCount; ++C)
            {
                string CharacterFullName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                Character LoadedCharacter = new Character(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCharacter.Level = 1;
                LoadedCharacter.ID = LoadedCharacter.Name;

                DicOwnedCharacter.Add(CharacterFullName, new CharacterInfo(LoadedCharacter, QuantityOwned));
            }

            int ListOwnedMissionCount = BR.ReadInt32();
            DicOwnedMission = new Dictionary<string, MissionInfo>();
            for (int C = 0; C < ListOwnedMissionCount; ++C)
            {
                string MissionPath = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                MissionInfo LoadedMission = new MissionInfo(MissionPath, QuantityOwned);

                DicOwnedMission.Add(MissionPath, LoadedMission);
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

                    Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedUnit.ID = LoadedUnit.ItemName;

                    if (!string.IsNullOrEmpty(CharacterFullName))
                    {
                        Character LoadedCharacter = new Character(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                        LoadedCharacter.Level = 1;
                        LoadedCharacter.ID = LoadedCharacter.Name;

                        LoadedUnit.ArrayCharacterActive[0] = LoadedCharacter;
                    }

                    Squad NewSquad = new Squad("Squad", LoadedUnit);
                    NewSquad.IsPlayerControlled = true;

                    NewSquadLoadout.ListSpawnSquad.Add(NewSquad);
                }
            }

            ActiveLoadout = ListSquadLoadout[0];
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(DicOwnedSquad.Count);
            foreach (UnitInfo ActiveSquad in DicOwnedSquad.Values)
            {
                BW.Write(ActiveSquad.Leader.RelativePath);
                BW.Write(ActiveSquad.Leader.UnitTypeName);
                BW.Write(ActiveSquad.QuantityOwned);
            }

            BW.Write(DicOwnedCharacter.Count);
            foreach (CharacterInfo ActiveCharacter in DicOwnedCharacter.Values)
            {
                BW.Write(ActiveCharacter.Pilot.FullName);
                BW.Write(ActiveCharacter.QuantityOwned);
            }

            BW.Write(DicOwnedMission.Count);
            foreach (MissionInfo ActiveMission in DicOwnedMission.Values)
            {
                BW.Write(ActiveMission.MapPath);
                BW.Write(ActiveMission.QuantityOwned);
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
                        if (ListSquadLoadout[L].ListSpawnSquad[S].CurrentLeader.Pilot != null)
                        {
                            BW.Write(ListSquadLoadout[L].ListSpawnSquad[S].CurrentLeader.Pilot.FullName);
                        }
                        else
                        {
                            BW.Write(string.Empty);
                        }
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
