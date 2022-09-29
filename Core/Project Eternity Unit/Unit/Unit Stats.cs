using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.Core.Units
{
    public class UnitStats
    {
        public const byte TerrainAirIndex = 0;
        public const byte TerrainLandIndex = 1;
        public const byte TerrainSeaIndex = 2;
        public const byte TerrainSpaceIndex = 3;
        public const byte TerrainWallIndex = 4;
        public const byte TerrainVoidIndex = 5;
        public const byte TerrainUnderwaterIndex = 6;
        public const byte TerrainUndergroundIndex = 7;

        public static readonly List<string> ListUnitSize = new List<string>() { UnitSizeLLL, UnitSizeLL, UnitSizeL, UnitSizeM, UnitSizeS, UnitSizeSS };

        public const string UnitSizeLLL = "LLL";
        public const string UnitSizeLL = "LL";
        public const string UnitSizeL = "L";
        public const string UnitSizeM = "M";
        public const string UnitSizeS = "S";
        public const string UnitSizeSS = "SS";

        [Flags]
        public enum UnitLinkTypes : int
        {
            None = 0x00,
            MaxHP = 1 << 1,
            MaxEN = 1 << 2,
            RegenEN = 1 << 4,
            Armor = 1 << 5,
            Mobility = 1 << 6,
            MaxMovement = 1 << 7,
            HPUpgrades = 1 << 8,
            ENUpgrades = 1 << 9,
            ArmorUpgrades = 1 << 10,
            MobilityUpgrades = 1 << 11,
            AttackUpgrades = 1 << 12,
        };

        public enum AttackUpgradesSpeeds { VerySlow, Slow, Normal, Fast };

        public enum AttackUpgradesCosts { Cheapest, Cheap, Normal, Expensive };

        #region Variables

        public string Name;
        public int EXPValue;
        public SharableInt32 _MaxHP;
        public SharableInt32 _MaxEN;
        public SharableInt32 _RegenEN;
        public SharableInt32 _Armor;
        public SharableInt32 _Mobility;
        public SharableInt32 _MaxMovement;

        public SharableInt32 HPUpgrades { get { return ArrayUpgrade[0]; } set { ArrayUpgrade[0] = value; } }
        public SharableInt32 ENUpgrades { get { return ArrayUpgrade[1]; } set { ArrayUpgrade[1] = value; } }
        public SharableInt32 ArmorUpgrades { get { return ArrayUpgrade[2]; } set { ArrayUpgrade[2] = value; } }
        public SharableInt32 MobilityUpgrades { get { return ArrayUpgrade[3]; } set { ArrayUpgrade[3] = value; } }
        public SharableInt32 AttackUpgrades { get { return ArrayUpgrade[4]; } set { ArrayUpgrade[4] = value; } }

        public SharableInt32[] ArrayUpgrade;

        public AttackUpgradesSpeeds AttackUpgradesSpeed;
        public AttackUpgradesCosts AttackUpgradesCost;

        public readonly StatsBoosts Boosts;
        public Dictionary<string, UnitLinkTypes> DicUnitLink;//List which Units it can link to and how.

        public byte SizeIndex;
        public bool[,] ArrayMapSize;//Custom mask for actual place a Unit is taking.

        public List<Attack> ListAttack;
        public Attack PLAAttack;
        public Attack CurrentAttack;

        public byte MaxClimb;
        public byte PostMVLevel;
        public byte ReMoveLevel;
        public byte ChargedAttackCancelLevel;

        public Dictionary<byte, byte> DicRankByMovement;
        public byte UnitTypeIndex;
        public BaseAutomaticSkill[] ArrayUnitAbility;

        public UnitAnimations Animations;
        public List<string> ListCharacterIDWhitelist;

        public int MaxHP { get { return (int)((_MaxHP.Value + (HPUpgrades.Value * 200)) * Boosts.HPMaxMultiplier) + Boosts.HPMaxModifier; } set { _MaxHP.Value = value; } }

        public int MaxEN { get { return (int)((_MaxEN.Value + (ENUpgrades.Value * 10)) * Boosts.ENMaxMultiplier) + Boosts.ENMaxModifier; } set { _MaxEN.Value = value; } }

        public int RegenEN { get { return _RegenEN.Value; } set { _RegenEN.Value = value; } }

        public int Armor { get { return (int)((_Armor.Value + (ArmorUpgrades.Value * 60)) * Boosts.ArmorMultiplier) + Boosts.ArmorModifier; } set { _Armor.Value = value; } }

        public int Mobility { get { return (int)((_Mobility.Value + (MobilityUpgrades.Value * 5)) * Boosts.MobilityMultiplier) + Boosts.MobilityModifier; } set { _Mobility.Value = value; } }

        public int MaxMovement { get { return (int)((_MaxMovement.Value * Boosts.MVMaxMultiplier) + Boosts.MVMaxModifier); } set { _MaxMovement.Value = value; } }

        #endregion

        private UnitStats()
        {
            _MaxHP = new SharableInt32();
            _MaxEN = new SharableInt32();
            _RegenEN = new SharableInt32();
            _Armor = new SharableInt32();
            _Mobility = new SharableInt32();
            _MaxMovement = new SharableInt32();

            ArrayUpgrade = new SharableInt32[5] { new SharableInt32(), new SharableInt32(), new SharableInt32(), new SharableInt32(), new SharableInt32() };
            HPUpgrades = new SharableInt32();
            ENUpgrades = new SharableInt32();
            ArmorUpgrades = new SharableInt32();
            MobilityUpgrades = new SharableInt32();
            AttackUpgrades = new SharableInt32();

            HPUpgrades.Value = 0;
            ENUpgrades.Value = 0;
            ArmorUpgrades.Value = 0;
            MobilityUpgrades.Value = 0;
            AttackUpgrades.Value = 0;

            DicUnitLink = new Dictionary<string, UnitLinkTypes>();
            Boosts = new StatsBoosts();
            Boosts.DicTerrainLetterAttributeModifier.Add(UnitStats.TerrainAirIndex, 0);
            Boosts.DicTerrainLetterAttributeModifier.Add(UnitStats.TerrainLandIndex, 0);
            Boosts.DicTerrainLetterAttributeModifier.Add(UnitStats.TerrainSeaIndex, 0);
            Boosts.DicTerrainLetterAttributeModifier.Add(UnitStats.TerrainSpaceIndex, 0);

            ArrayUnitAbility = new BaseAutomaticSkill[0];
            ListAttack = new List<Attack>();
            DicRankByMovement = new Dictionary<byte, byte>();
            ListCharacterIDWhitelist = new List<string>();

            Animations = new UnitAnimations();
        }

        public UnitStats(bool[,] ArrayMapSize)
            : this()
        {
            this.ArrayMapSize = ArrayMapSize;
        }

        public UnitStats(string Name, BinaryReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this()
        {
            this.Name = Name;
            EXPValue = BR.ReadInt32();
            _MaxHP.Value = BR.ReadInt32();
            _MaxEN.Value = BR.ReadInt32();
            _Armor.Value = BR.ReadInt32();
            _Mobility.Value = BR.ReadInt32();
            _MaxMovement.Value = (int)BR.ReadSingle();
            MaxClimb = BR.ReadByte();
            PostMVLevel = BR.ReadByte();
            ReMoveLevel = BR.ReadByte();
            ChargedAttackCancelLevel = BR.ReadByte();

            AttackUpgradesSpeed = (AttackUpgradesSpeeds)BR.ReadByte();
            AttackUpgradesCost = (AttackUpgradesCosts)BR.ReadByte();

            int TerrainGradeCount = BR.ReadInt32();
            DicRankByMovement = new Dictionary<byte, byte>(TerrainGradeCount);
            for (int G = 0; G < TerrainGradeCount; ++G)
            {
                DicRankByMovement.Add(BR.ReadByte(), BR.ReadByte());
            }

            UnitTypeIndex = BR.ReadByte();

            SizeIndex = BR.ReadByte();

            int SizeWidth = BR.ReadInt32();
            int SizeHeight = BR.ReadInt32();
            ArrayMapSize = new bool[SizeWidth, SizeHeight];
            for (int X = 0; X < SizeWidth; X++)
            {
                for (int Y = 0; Y < SizeHeight; Y++)
                    ArrayMapSize[X, Y] = BR.ReadBoolean();
            }

            //Read Pilots whitelist.
            ListCharacterIDWhitelist = new List<string>();
            Int32 ListPilotCount = BR.ReadInt32();
            for (int P = 0; P < ListPilotCount; P++)
            {
                string CharacterName = BR.ReadString();

                ListCharacterIDWhitelist.Add(CharacterName);
            }

            Int32 ListAttackCount = BR.ReadInt32();
            ListAttack = new List<Attack>(ListAttackCount);
            for (int A = 0; A < ListAttackCount; ++A)
            {
                Attack NewAttack;
                bool IsExternal = BR.ReadBoolean();
                string AttackName = BR.ReadString();

                if (IsExternal)
                {
                    NewAttack = new Attack(AttackName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                }
                else
                {
                    NewAttack = new Attack(BR, Content, AttackName, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                }

                NewAttack.RefillAmmo();
                if (NewAttack.Pri == WeaponPrimaryProperty.PLA)
                {
                    PLAAttack = NewAttack;
                }

                Int32 ListAttackContextCount = BR.ReadInt32();
                for (int AC = 0; AC < ListAttackContextCount; ++AC)
                {
                    NewAttack.Animations.Add(new AttackContext(BR));
                }

                ListAttack.Add(NewAttack);
            }

            Animations = new UnitAnimations(BR);

            Int32 ListAbilityCount = BR.ReadInt32();
            ArrayUnitAbility = new BaseAutomaticSkill[ListAbilityCount];
            for (int A = ListAbilityCount - 1; A >= 0; --A)
            {
                string RelativePath = BR.ReadString();
                ArrayUnitAbility[A] = new BaseAutomaticSkill("Content/Units/Abilities/" + RelativePath + ".pes", RelativePath, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
        }

        public UnitStats(string Name, ContentManager Content, IniFile UnitFile, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this()
        {
            this.Name = Name;
            EXPValue = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "EXP"));
            _MaxHP.Value = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "BaseHP"));
            _MaxEN.Value = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "BaseEN"));
            _Armor.Value = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "BaseArmor"));
            _Mobility.Value = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "BaseMobility"));
            _MaxMovement.Value = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "BaseMovement"));
            AttackUpgradesSpeed = (AttackUpgradesSpeeds)Convert.ToInt32(UnitFile.ReadField("Unit Stats", "AttackUpgradesValueIndex"));
            AttackUpgradesCost = (AttackUpgradesCosts)Convert.ToInt32(UnitFile.ReadField("Unit Stats", "AttackUpgradesCostIndex"));

            DicRankByMovement = new Dictionary<byte, byte>();
            foreach (KeyValuePair<string, string> ActiveField in UnitFile.ReadHeader("Unit Terrain"))
            {
                DicRankByMovement.Add(Convert.ToByte(ActiveField.Key), Convert.ToByte(ActiveField.Value));
            }
            
            SizeIndex = (byte)UnitStats.ListUnitSize.IndexOf(UnitFile.ReadField("Unit Stats", "Size"));

            int SizeWidth = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "Size Mask Width"));
            int SizeHeight = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "Size Mask Height"));
            ArrayMapSize = new bool[SizeWidth, SizeHeight];

            foreach (KeyValuePair<string, string> ActiveField in UnitFile.ReadHeader("Size Mask"))
            {
                int IndexOfX = ActiveField.Key.IndexOf('X') + 1;
                int IndexOfY = ActiveField.Key.IndexOf('Y');
                int X = Convert.ToInt32(ActiveField.Key.Substring(IndexOfX, IndexOfY - IndexOfX));
                int Y = Convert.ToInt32(ActiveField.Key.Substring(IndexOfY + 1));
                ArrayMapSize[X, Y] = Convert.ToBoolean(ActiveField.Value);
            }

            //Read Pilots whitelist.
            ListCharacterIDWhitelist = new List<string>();
            foreach (KeyValuePair<string, string> ActiveField in UnitFile.ReadHeader("Pilot Whitelist"))
            {
                string CharacterName = ActiveField.Value;

                ListCharacterIDWhitelist.Add(CharacterName);
            }

            Dictionary<string, string> DicAttackAnimations = UnitFile.ReadHeader("Attack Animations");
            
            ListAttack = new List<Attack>();
            foreach (KeyValuePair<string, string> ActiveField in UnitFile.ReadHeader("Attacks"))
            {
                Attack NewAttack = new Attack(ActiveField.Value, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                NewAttack.RefillAmmo();
                if (NewAttack.Pri == WeaponPrimaryProperty.PLA)
                    PLAAttack = NewAttack;
                
                //Load Animation paths.
                foreach (KeyValuePair<string, string> ActiveAnimationField in DicAttackAnimations)
                {
                    if (ActiveAnimationField.Key.StartsWith(ActiveField.Key))
                    {
                        int An = Convert.ToInt32(ActiveAnimationField.Key.Substring(ActiveField.Key.Length + 5));
                        NewAttack.Animations[0].Animations[An] = new AnimationInfo(ActiveField.Key);
                    }
                }
                ListAttack.Add(NewAttack);
            }

            foreach (KeyValuePair<string, string> ActiveField in UnitFile.ReadHeader("Animations"))
            {
                int A = Convert.ToInt32(ActiveField.Key.Substring(5));
                Animations[A] = ActiveField.Value;
            }
            
            List<BaseAutomaticSkill> ListAbility = new List<BaseAutomaticSkill>();
            foreach (KeyValuePair<string, string> ActiveField in UnitFile.ReadHeader("Abilities"))
            {
                ListAbility.Add(new BaseAutomaticSkill("Content/Units/Abilities/" + ActiveField.Value + ".pes", ActiveField.Value, DicRequirement, DicEffect, DicAutomaticSkillTarget));
            }

            ArrayUnitAbility = ListAbility.ToArray();
        }

        public void Init()
        {
            PLAAttack = null;

            //Add the weapon stocked in the Unit's parts to a generic weapon list.
            for (int i = 0; i < ListAttack.Count; i++)
            {
                ListAttack[i].RefillAmmo();
                if (ListAttack[i].Pri == WeaponPrimaryProperty.PLA)
                    PLAAttack = ListAttack[i];
            }
        }

        public void ShareStats(UnitStats UnitToShareFrom, UnitLinkTypes UnitLinkType)
        {
            if ((UnitLinkType & UnitLinkTypes.MaxHP) == UnitLinkTypes.MaxHP)
            {
                _MaxHP.Pointer = UnitToShareFrom._MaxHP;
            }
            if ((UnitLinkType & UnitLinkTypes.MaxEN) == UnitLinkTypes.MaxEN)
            {
                _MaxEN.Pointer = UnitToShareFrom._MaxEN;
            }
            if ((UnitLinkType & UnitLinkTypes.RegenEN) == UnitLinkTypes.RegenEN)
            {
                _RegenEN.Pointer = UnitToShareFrom._RegenEN;
            }
            if ((UnitLinkType & UnitLinkTypes.Armor) == UnitLinkTypes.Armor)
            {
                _Armor.Pointer = UnitToShareFrom._Armor;
            }
            if ((UnitLinkType & UnitLinkTypes.Mobility) == UnitLinkTypes.Mobility)
            {
                _Mobility.Pointer = UnitToShareFrom._Mobility;
            }
            if ((UnitLinkType & UnitLinkTypes.MaxMovement) == UnitLinkTypes.MaxMovement)
            {
                _MaxMovement.Pointer = UnitToShareFrom._MaxMovement;
            }

            if ((UnitLinkType & UnitLinkTypes.HPUpgrades) == UnitLinkTypes.HPUpgrades)
            {
                HPUpgrades.Pointer = UnitToShareFrom.HPUpgrades;
            }
            if ((UnitLinkType & UnitLinkTypes.ENUpgrades) == UnitLinkTypes.ENUpgrades)
            {
                ENUpgrades.Pointer = UnitToShareFrom.ENUpgrades;
            }
            if ((UnitLinkType & UnitLinkTypes.ArmorUpgrades) == UnitLinkTypes.ArmorUpgrades)
            {
                ArmorUpgrades.Pointer = UnitToShareFrom.ArmorUpgrades;
            }
            if ((UnitLinkType & UnitLinkTypes.MobilityUpgrades) == UnitLinkTypes.MobilityUpgrades)
            {
                MobilityUpgrades.Pointer = UnitToShareFrom.MobilityUpgrades;
            }
            if ((UnitLinkType & UnitLinkTypes.AttackUpgrades) == UnitLinkTypes.AttackUpgrades)
            {
                AttackUpgrades.Pointer = UnitToShareFrom.AttackUpgrades;
            }
        }

        public bool IsUnitAtPosition(Vector3 Position, Vector3 PositionToCheck)
        {
            int FinalX = (int)(PositionToCheck.X - Position.X);
            int FinalY = (int)(PositionToCheck.Y - Position.Y);

            if (FinalX < 0 || FinalY < 0 || FinalX >= ArrayMapSize.GetLength(0) || FinalY >= ArrayMapSize.GetLength(1) || (int)Position.Z != (int)PositionToCheck.Z)
                return false;

            return ArrayMapSize[FinalX, FinalY];
        }

        public int TerrainAttributeValue(byte MovementTypeIndex)
        {
            return DicRankByMovement[MovementTypeIndex] + Boosts.DicTerrainLetterAttributeModifier[MovementTypeIndex];
        }

        public float GetClearance()
        {
            if (UnitStats.ListUnitSize[SizeIndex] == UnitStats.UnitSizeLLL)
                return 3;
            else if (UnitStats.ListUnitSize[SizeIndex] == UnitStats.UnitSizeLL)
                return 2;
            else if (UnitStats.ListUnitSize[SizeIndex] == UnitStats.UnitSizeL)
                return 1;
            else if (UnitStats.ListUnitSize[SizeIndex] == UnitStats.UnitSizeM)
                return 1;
            else if (UnitStats.ListUnitSize[SizeIndex] == UnitStats.UnitSizeS)
                return 1;
            else if (UnitStats.ListUnitSize[SizeIndex] == UnitStats.UnitSizeSS)
                return 0.75f;

            return 1;
        }
    }
}
