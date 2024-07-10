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
    public struct UnitInventoryContainer
    {
        public Dictionary<string, UnitInventoryContainer> DicFolder;
        public List<UnitInventoryContainer> ListFolder;//Share the same folders as the dictionnary
        public List<UnitInfo> ListUnit;
        public UnitInfo IconUnit;

        public string Name;

        public UnitInventoryContainer(string Name)
        {
            this.Name = Name;

            DicFolder = new Dictionary<string, UnitInventoryContainer>();
            ListFolder = new List<UnitInventoryContainer>();
            ListUnit = new List<UnitInfo>();
            IconUnit = null;
        }

        public void AddUnit(UnitInfo UnitToAdd)
        {
            if (ListUnit.Count == 0)
            {
                IconUnit = UnitToAdd;
            }

            if (!ListUnit.Contains(UnitToAdd))
            {
                ListUnit.Add(UnitToAdd);
            }
        }
    }

    public struct CharacterInventoryContainer
    {
        public Dictionary<string, CharacterInventoryContainer> DicFolder;
        public List<CharacterInventoryContainer> ListFolder;//Share the same folders as the dictionnary
        public List<CharacterInfo> ListCharacter;
        public CharacterInfo IconUnit;

        public string Name;

        public CharacterInventoryContainer(string Name)
        {
            this.Name = Name;

            DicFolder = new Dictionary<string, CharacterInventoryContainer>();
            ListFolder = new List<CharacterInventoryContainer>();
            ListCharacter = new List<CharacterInfo>();
            IconUnit = null;
        }

        public void AddCharacter(CharacterInfo UnitToAdd)
        {
            if (ListCharacter.Count == 0)
            {
                IconUnit = UnitToAdd;
            }

            if (!ListCharacter.Contains(UnitToAdd))
            {
                ListCharacter.Add(UnitToAdd);
            }
        }
    }

    public class UnitInfo
    {
        public Unit Leader;
        public byte QuantityOwned;
        public List<UnitSkinInfo> ListOwnedUnitSkin;
        public List<UnitSkinInfo> ListOwnedUnitAlt;

        public UnitInfo(Unit Leader, byte QuantityOwned)
        {
            this.Leader = Leader;
            this.QuantityOwned = QuantityOwned;
        }
    }

    public class UnitSkinInfo
    {
        public string UnitTypeAndRelativePath;
        public string SkinTypeAndRelativePath;
        public Unit Leader;

        public UnitSkinInfo(string UnitTypeAndRelativePath, string SkinTypeAndRelativePath, Unit Leader)
        {
            this.UnitTypeAndRelativePath = UnitTypeAndRelativePath;
            this.SkinTypeAndRelativePath = SkinTypeAndRelativePath;
            this.Leader = Leader;
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

    public class PlayerLoadout
    {
        public string Name;
        public List<Squad> ListSpawnSquad;
        public List<Commander> ListSpawnCommander;

        public PlayerLoadout()
        {
            Name = "Loadout";
            ListSpawnSquad = new List<Squad>();
            ListSpawnCommander = new List<Commander>();
        }
    }

    public class BattleMapPlayerInventory
    {
        public UnitInventoryContainer RootUnitContainer;
        public CharacterInventoryContainer RootCharacterContainer;

        public Dictionary<string, UnitInfo> DicOwnedUnit;//UnitTypeAndPath
        public Dictionary<string, UnitSkinInfo> DicOwnedUnitSkin;//Skins for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, UnitSkinInfo> DicOwnedUnitAlt;//Alts for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, CharacterInfo> DicOwnedCharacter;
        public Dictionary<string, MissionInfo> DicOwnedMission;
        public List<PlayerLoadout> ListSquadLoadout;

        public PlayerLoadout ActiveLoadout;

        public BattleMapPlayerInventory()
        {
            RootUnitContainer = new UnitInventoryContainer("ALL");
            RootCharacterContainer = new CharacterInventoryContainer("ALL");

            ListSquadLoadout = new List<PlayerLoadout>();
            DicOwnedUnit = new Dictionary<string, UnitInfo>();
            DicOwnedCharacter = new Dictionary<string, CharacterInfo>();
            DicOwnedUnitSkin = new Dictionary<string, UnitSkinInfo>();
            DicOwnedUnitAlt = new Dictionary<string, UnitSkinInfo>();
            DicOwnedMission = new Dictionary<string, MissionInfo>();

            ActiveLoadout = new PlayerLoadout();
            ListSquadLoadout.Add(ActiveLoadout);
        }

        public void Load(BinaryReader BR, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            int DicOwnedUnitCount = BR.ReadInt32();
            DicOwnedUnit = new Dictionary<string, UnitInfo>(DicOwnedUnitCount);
            for (int U = 0; U < DicOwnedUnitCount; ++U)
            {
                string RelativePath = BR.ReadString();
                string UnitTypeName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();
                byte UnitSkinsQuantityOwned = BR.ReadByte();
                byte UnitAltsQuantityOwned = BR.ReadByte();

                Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                LoadedUnit.ID = LoadedUnit.ItemName;
                UnitInfo LoadedUnitInfo = new UnitInfo(LoadedUnit, QuantityOwned);

                for (int S = UnitSkinsQuantityOwned - 1; S >= 0; --S)
                {
                    string SkinRelativePath = BR.ReadString();
                    string SkinUnitTypeName = BR.ReadString();

                    Unit LoadedSkin = Unit.FromType(SkinUnitTypeName, SkinRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedSkin.ID = LoadedSkin.ItemName;
                    LoadedUnit.ListSkin.Add(LoadedSkin);
                }
                for (int A = UnitAltsQuantityOwned - 1; A >= 0; --A)
                {
                    string AltRelativePath = BR.ReadString();
                    string AltUnitTypeName = BR.ReadString();

                    Unit LoadedAlt = Unit.FromType(AltUnitTypeName, AltRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedAlt.ID = LoadedAlt.ItemName;
                    LoadedUnit.ListAlt.Add(LoadedAlt);
                }

                DicOwnedUnit.Add(UnitTypeName + "/" + RelativePath, LoadedUnitInfo);
                AddUnit(LoadedUnitInfo);
            }

            int DicOwnedCharacterCount = BR.ReadInt32();
            DicOwnedCharacter = new Dictionary<string, CharacterInfo>(DicOwnedCharacterCount);
            for (int C = 0; C < DicOwnedCharacterCount; ++C)
            {
                string CharacterFullName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                Character LoadedCharacter = new Character(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCharacter.Level = 1;
                LoadedCharacter.ID = LoadedCharacter.Name;

                CharacterInfo LoadedCharacterInfo = new CharacterInfo(LoadedCharacter, QuantityOwned);

                DicOwnedCharacter.Add(CharacterFullName, LoadedCharacterInfo);
                AddCharacter(LoadedCharacterInfo);
            }

            int DicOwnedMissionCount = BR.ReadInt32();
            DicOwnedMission = new Dictionary<string, MissionInfo>(DicOwnedMissionCount);
            for (int C = 0; C < DicOwnedMissionCount; ++C)
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

                NewSquadLoadout.Name = BR.ReadString();

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

            byte SkinsQuantityOwned = BR.ReadByte();
            byte AltsQuantityOwned = BR.ReadByte();

            for (int S = SkinsQuantityOwned - 1; S >= 0; --S)
            {
                string OwnerUnitTypeAndRelativePath = BR.ReadString();
                string SkinTypeAndRelativePath = BR.ReadString();

                if (DicOwnedUnit.ContainsKey(OwnerUnitTypeAndRelativePath))
                {
                    Unit LoadedSkin = Unit.FromFullName(SkinTypeAndRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedSkin.ID = LoadedSkin.ItemName;
                    DicOwnedUnit[OwnerUnitTypeAndRelativePath].Leader.ListSkin.Add(LoadedSkin);
                }
                else
                {
                    DicOwnedUnitSkin.Add(OwnerUnitTypeAndRelativePath, new UnitSkinInfo(OwnerUnitTypeAndRelativePath, SkinTypeAndRelativePath, null));
                }
            }

            for (int A = 0; A < AltsQuantityOwned; ++A)
            {
                string UnitTypeAndRelativePath = BR.ReadString();
                string AltTypeAndRelativePath = BR.ReadString();

                if (DicOwnedUnit.ContainsKey(UnitTypeAndRelativePath))
                {
                    Unit LoadedAlt = Unit.FromFullName(AltTypeAndRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedAlt.ID = LoadedAlt.ItemName;
                    DicOwnedUnit[UnitTypeAndRelativePath].Leader.ListAlt.Add(LoadedAlt);
                }
                else
                {
                    DicOwnedUnitAlt.Add(UnitTypeAndRelativePath, new UnitSkinInfo(UnitTypeAndRelativePath, AltTypeAndRelativePath, null));
                }
            }

            ActiveLoadout = ListSquadLoadout[0];
        }

        public void Load(ByteReader BR, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            int DicOwnedUnitCount = BR.ReadInt32();
            DicOwnedUnit = new Dictionary<string, UnitInfo>(DicOwnedUnitCount);
            for (int U = 0; U < DicOwnedUnitCount; ++U)
            {
                string RelativePath = BR.ReadString();
                string UnitTypeName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();
                byte UnitSkinsQuantityOwned = BR.ReadByte();
                byte UnitAltsQuantityOwned = BR.ReadByte();

                Unit LoadedUnit = Unit.FromType(UnitTypeName, RelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                LoadedUnit.ID = LoadedUnit.ItemName;
                UnitInfo LoadedUnitInfo = new UnitInfo(LoadedUnit, QuantityOwned);

                for (int S = UnitSkinsQuantityOwned - 1; S >= 0; --S)
                {
                    string SkinRelativePath = BR.ReadString();
                    string SkinUnitTypeName = BR.ReadString();

                    Unit LoadedSkin = Unit.FromType(SkinUnitTypeName, SkinRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedSkin.ID = LoadedSkin.ItemName;
                    LoadedUnit.ListSkin.Add(LoadedSkin);
                }
                for (int A = UnitAltsQuantityOwned - 1; A >= 0; --A)
                {
                    string AltRelativePath = BR.ReadString();
                    string AltUnitTypeName = BR.ReadString();

                    Unit LoadedAlt = Unit.FromType(AltUnitTypeName, AltRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedAlt.ID = LoadedAlt.ItemName;
                    LoadedUnit.ListAlt.Add(LoadedAlt);
                }

                DicOwnedUnit.Add(UnitTypeName + "/" + RelativePath, LoadedUnitInfo);
                AddUnit(LoadedUnitInfo);
            }

            int DicOwnedCharacterCount = BR.ReadInt32();
            DicOwnedCharacter = new Dictionary<string, CharacterInfo>(DicOwnedCharacterCount);
            for (int C = 0; C < DicOwnedCharacterCount; ++C)
            {
                string CharacterFullName = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                Character LoadedCharacter = new Character(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCharacter.Level = 1;
                LoadedCharacter.ID = LoadedCharacter.Name;

                CharacterInfo LoadedCharacterInfo = new CharacterInfo(LoadedCharacter, QuantityOwned);

                DicOwnedCharacter.Add(CharacterFullName, LoadedCharacterInfo);
                AddCharacter(LoadedCharacterInfo);
            }

            int DicOwnedMissionCount = BR.ReadInt32();
            DicOwnedMission = new Dictionary<string, MissionInfo>(DicOwnedMissionCount);
            for (int C = 0; C < DicOwnedMissionCount; ++C)
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

                NewSquadLoadout.Name = BR.ReadString();

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

            byte SkinsQuantityOwned = BR.ReadByte();
            byte AltsQuantityOwned = BR.ReadByte();

            for (int S = SkinsQuantityOwned - 1; S >= 0; --S)
            {
                string UnitTypeAndRelativePath = BR.ReadString();
                string SkinTypeAndRelativePath = BR.ReadString();

                if (DicOwnedUnit.ContainsKey(UnitTypeAndRelativePath))
                {
                    Unit LoadedSkin = Unit.FromFullName(SkinTypeAndRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedSkin.ID = LoadedSkin.ItemName;
                    DicOwnedUnit[UnitTypeAndRelativePath].Leader.ListSkin.Add(LoadedSkin);
                }
                else
                {
                    DicOwnedUnitSkin.Add(UnitTypeAndRelativePath, new UnitSkinInfo(UnitTypeAndRelativePath, SkinTypeAndRelativePath, null));
                }
            }

            for (int A = 0; A < AltsQuantityOwned; ++A)
            {
                string UnitTypeAndRelativePath = BR.ReadString();
                string AltTypeAndRelativePath = BR.ReadString();

                if (DicOwnedUnit.ContainsKey(UnitTypeAndRelativePath))
                {
                    Unit LoadedAlt = Unit.FromFullName(AltTypeAndRelativePath, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    LoadedAlt.ID = LoadedAlt.ItemName;
                    DicOwnedUnit[UnitTypeAndRelativePath].Leader.ListAlt.Add(LoadedAlt);
                }
                else
                {
                    DicOwnedUnitAlt.Add(UnitTypeAndRelativePath, new UnitSkinInfo(UnitTypeAndRelativePath, AltTypeAndRelativePath, null));
                }
            }

            ActiveLoadout = ListSquadLoadout[0];
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(DicOwnedUnit.Count);
            foreach (UnitInfo ActiveSquad in DicOwnedUnit.Values)
            {
                BW.Write(ActiveSquad.Leader.RelativePath);
                BW.Write(ActiveSquad.Leader.UnitTypeName);
                BW.Write((byte)ActiveSquad.QuantityOwned);
                BW.Write((byte)ActiveSquad.Leader.ListSkin.Count);
                foreach (Unit ActiveSkin in ActiveSquad.Leader.ListSkin)
                {
                    BW.Write(ActiveSkin.RelativePath);
                    BW.Write(ActiveSkin.UnitTypeName);
                }
                BW.Write((byte)ActiveSquad.Leader.ListAlt.Count);
                foreach (var ActiveAlt in ActiveSquad.Leader.ListAlt)
                {
                    BW.Write(ActiveAlt.RelativePath);
                    BW.Write(ActiveAlt.UnitTypeName);
                }
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
                BW.Write(ListSquadLoadout[L].Name);

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

            BW.Write(DicOwnedUnitSkin.Count);
            foreach (UnitSkinInfo ActiveSkin in DicOwnedUnitSkin.Values)
            {
                BW.Write(ActiveSkin.UnitTypeAndRelativePath);
                BW.Write(ActiveSkin.SkinTypeAndRelativePath);
            }
            BW.Write(DicOwnedUnitAlt.Count);
            foreach (UnitSkinInfo ActiveSkin in DicOwnedUnitAlt.Values)
            {
                BW.Write(ActiveSkin.UnitTypeAndRelativePath);
                BW.Write(ActiveSkin.SkinTypeAndRelativePath);
            }
        }

        public void AddUnit(UnitInfo UnitToAdd)
        {
            UnitInventoryContainer CurrentUnitContainer = RootUnitContainer;

            CurrentUnitContainer.AddUnit(UnitToAdd);

            string[] Tags = UnitToAdd.Leader.UnitTags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ActiveTag in Tags)
            {
                CurrentUnitContainer = RootUnitContainer;

                CurrentUnitContainer.AddUnit(UnitToAdd);

                string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveFolder in SubFolders)
                {
                    UnitInventoryContainer NewContainer;
                    if (!CurrentUnitContainer.DicFolder.TryGetValue(ActiveFolder, out NewContainer))
                    {
                        NewContainer = new UnitInventoryContainer(ActiveFolder);
                        CurrentUnitContainer.DicFolder.Add(ActiveFolder, NewContainer);
                        CurrentUnitContainer.ListFolder.Add(NewContainer);
                    }

                    CurrentUnitContainer = NewContainer;

                    CurrentUnitContainer.AddUnit(UnitToAdd);
                }
            }
        }

        public void AddCharacter(CharacterInfo CharacterToAdd)
        {
            CharacterInventoryContainer CurrentCharacterContainer = RootCharacterContainer;

            CurrentCharacterContainer.AddCharacter(CharacterToAdd);

            string[] Tags = CharacterToAdd.Pilot.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ActiveTag in Tags)
            {
                CurrentCharacterContainer = RootCharacterContainer;

                CurrentCharacterContainer.AddCharacter(CharacterToAdd);

                string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveFolder in SubFolders)
                {
                    CharacterInventoryContainer NewContainer;
                    if (!CurrentCharacterContainer.DicFolder.TryGetValue(ActiveFolder, out NewContainer))
                    {
                        NewContainer = new CharacterInventoryContainer(ActiveFolder);
                        CurrentCharacterContainer.DicFolder.Add(ActiveFolder, NewContainer);
                        CurrentCharacterContainer.ListFolder.Add(NewContainer);
                    }

                    CurrentCharacterContainer = NewContainer;

                    CurrentCharacterContainer.AddCharacter(CharacterToAdd);
                }
            }
        }
    }
}
