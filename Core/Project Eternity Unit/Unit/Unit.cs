using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using FMOD;
using Roslyn;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Core.Units
{
    public abstract partial class Unit : ShopItem
    {
        public static readonly Dictionary<string, Unit> DicDefaultUnitType = new Dictionary<string, Unit>();//When you just need a placeholder outside of a game.

        public static char[] Grades = new char[6] { '-', 'S', 'A', 'B', 'C', 'D' };

        public static readonly List<char> ListRank = new List<char>() { '-', 'S', 'A', 'B', 'C', 'D' };

        /// <summary>
        /// List of Unit path refered by their Name.
        /// </summary>

        public enum AnimationTypes { EnemyNeutral, AttackPhase1, AttackPhase2, AttackPhase3, AttackMiss, EnemyHit, EnemyDodge1, EnemyDodge2, EnemyRecover, EnemyDamage };

        public enum BattleDefenseChoices { Attack = 0, Defend = 1, Evade = 2, DoNothing = 3 };

        #region Variables

        public FMODSound BattleTheme { get { return string.IsNullOrEmpty(Pilot.BattleThemeName) ? null : Character.DicBattleTheme[Pilot.BattleThemeName]; } }

        public string BattleThemeName { get { return Pilot.BattleThemeName; } }

        public abstract string UnitTypeName { get; }//Used to reconstruct Units from files.

        protected UnitStats _UnitStat;
        public UnitStats UnitStat { get { return _UnitStat; } }

        public TagSystem TeamTags;//Used to make units unavailable
        public string UnitTags;//Used to categorize units
        public string ID;//Unique ID

        public string SpriteUnitPath;
        public Texture2D SpriteUnit;

        public string SpriteMapPath;
        public Texture2D SpriteMap;
        public UnitMap3D Unit3DSprite;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;

        protected int _HP;
        protected int _EN;
        public int HP { get { return _HP; } }
        public int EN { get { return _EN; } }
        public byte SizeIndex { get { return _UnitStat.SizeIndex; } }

        #region Stats getters

        public int MaxHP { get { return _UnitStat.MaxHP; } set { _UnitStat.MaxHP = value; } }

        public int MaxEN { get { return _UnitStat.MaxEN; } set { _UnitStat.MaxEN = value; } }

        public int RegenEN { get { return _UnitStat.RegenEN; } set { _UnitStat.RegenEN = value; } }

        public int Armor { get { return _UnitStat.Armor; } set { _UnitStat.Armor = value; } }

        public int Mobility { get { return _UnitStat.Mobility; } set { _UnitStat.Mobility = value; } }

        public int MaxMovement { get { return _UnitStat.MaxMovement; } set { _UnitStat.MaxMovement = value; } }

        #endregion

        #region Pilot getters and setters

        public Character Pilot
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0];
                else
                    return null;
            }
        }

        public string PilotName
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].Name;
                else
                    return "";
            }
        }

        public int PilotLevel
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].Level;
                else
                    return 0;
            }
        }

        public int PilotEXP
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].EXP;
                else
                    return 0;
            }
        }

        public int PilotNextEXP
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].NextEXP;
                else
                    return 0;
            }
        }

        public int PilotMorale
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].Will;
                else
                    return 0;
            }
            set
            {
                if (ArrayCharacterActive.Length > 0)
                    ArrayCharacterActive[0].Will = value;
            }
        }

        public int PilotSP
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].SP;
                else
                    return 0;
            }
            set
            {
                if (ArrayCharacterActive.Length > 0)
                    ArrayCharacterActive[0].SP = value;
            }
        }

        public int PilotMaxSP
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].MaxSP;
                else
                    return 0;
            }
        }

        public int PilotPilotPoints
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].PilotPoints;
                else
                    return 0;
            }
            set
            {
                if (ArrayCharacterActive.Length > 0)
                    ArrayCharacterActive[0].PilotPoints = value;
            }
        }

        public int PilotMEL
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].MEL;
                else
                    return 0;
            }
        }

        public int PilotRNG
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].RNG;
                else
                    return 0;
            }
        }

        public int PilotDEF
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].DEF;
                else
                    return 0;
            }
        }

        public int PilotSKL
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].SKL;
                else
                    return 0;
            }
        }

        public int PilotEVA
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].EVA;
                else
                    return 0;
            }
        }

        public int PilotHIT
        {
            get
            {
                if (ArrayCharacterActive.Length > 0)
                    return ArrayCharacterActive[0].HIT;
                else
                    return 0;
            }
        }

        #endregion

        public BattleDefenseChoices BattleDefenseChoice;

        public int MaxCharacter = 5;
        public Character[] ArrayCharacterActive;
        public UnitPart[] ArrayParts;
        public BaseAutomaticSkill[] ArrayUnitAbility { get { return _UnitStat.ArrayUnitAbility; } }

        public UnitAnimations Animations { get { return _UnitStat.Animations; } }

        public Dictionary<byte, byte> DicRankByMovement { get { return _UnitStat.DicRankByMovement; } }

        public List<string> ListIgnoreSkill;//List used to ignore other skills.
        public List<string> ListCharacterIDWhitelist { get { return _UnitStat.ListCharacterIDWhitelist; } }

        private Attack ChargedAttack;
        public bool CanAttack { get { foreach (Attack ActiveAttack in ListAttack) { if (ActiveAttack.CanAttack) return true; }; return false; } }//If the character is in range to attack.

        private List<Attack> _ListAttack;
        private List<TemporaryAttackPickup> ListAttackTemporary;//Picked up weapons and others.
        public List<Attack> ListAttack { get { return _ListAttack; } }

        public Attack PLAAttack { get { return _UnitStat.PLAAttack; } }

        public Attack CurrentAttack { get { return _UnitStat.CurrentAttack; }
            set { _UnitStat.CurrentAttack = value; } }

        public string AttackAccuracy;
        public List<string> ListMAPAttackAccuracy;

        public StatsBoosts Boosts { get { return _UnitStat.Boosts; } }

        #endregion

        protected Unit()
            : this(null)
        {
        }

        protected Unit(string RelativePath)
            : base(RelativePath)
        {
            _ListAttack = new List<Attack>();
            ListAttackTemporary = new List<TemporaryAttackPickup>();
            ListIgnoreSkill = new List<string>();
            AttackAccuracy = "";
            ListMAPAttackAccuracy = new List<string>();
            _UnitStat = new UnitStats(new bool[1, 1] { { true } });
            ArrayCharacterActive = new Character[0];
            ArrayParts = new UnitPart[0];
            TeamTags = new TagSystem();
            ID = "";
        }

        public void ShareStats(Unit UnitToShareFrom, UnitStats.UnitLinkTypes UnitLinkType)
        {
            _UnitStat.ShareStats(UnitToShareFrom._UnitStat, UnitLinkType);
        }

        public static Dictionary<string, Unit> LoadAllUnits()
        {
            Dictionary<string, Unit> DicUnitType = new Dictionary<string, Unit>();

            foreach (KeyValuePair<string, Unit> ActiveUnit in LoadFromAssemblyFiles(Directory.GetFiles("Units", "*.dll", SearchOption.AllDirectories)))
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Units", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Unit> ActiveUnit in LoadFromAssembly(ActiveAssembly, typeof(Unit)))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }

            return DicUnitType;
        }

        public static Dictionary<string, Unit> LoadFromAssembly(Assembly ActiveAssembly, params object[] Args)
        {
            Dictionary<string, Unit> DicUnitType = new Dictionary<string, Unit>();
            List<Unit> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<Unit>(typeof(Unit), ActiveAssembly.GetTypes(), Args);

            foreach (Unit Instance in ListSkillEffect)
            {
                DicUnitType.Add(Instance.UnitTypeName, Instance);
            }

            return DicUnitType;
        }

        public static Dictionary<string, Unit> LoadFromAssemblyFiles(string[] ArrayFilePath, params object[] Args)
        {
            Dictionary<string, Unit> DicUnitType = new Dictionary<string, Unit>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, Unit> ActiveUnit in LoadFromAssembly(ActiveAssembly, Args))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }

            return DicUnitType;
        }

        public static Unit FromFullName(string Name, Microsoft.Xna.Framework.Content.ContentManager Content,
            Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            string[] UnitInfo = Name.Split(new[] { "/" }, StringSplitOptions.None);
            return FromType(UnitInfo[0], Name.Remove(0, UnitInfo[0].Length + 1), Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        public static Unit FromType(string UnitType, string RelativePath, Microsoft.Xna.Framework.Content.ContentManager Content,
            Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return DicUnitType[UnitType].FromFile(RelativePath, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        public abstract Unit FromFile(string RelativePath, Microsoft.Xna.Framework.Content.ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget);

        public void QuickSave(BinaryWriter BW)
        {
            BW.Write(UnitTypeName);
            BW.Write(RelativePath);
            BW.Write(ID);

            BW.Write(HP);
            BW.Write(EN);
            BW.Write(_UnitStat.HPUpgrades.Value);
            BW.Write(_UnitStat.ENUpgrades.Value);
            BW.Write(_UnitStat.ArmorUpgrades.Value);
            BW.Write(_UnitStat.MobilityUpgrades.Value);
            BW.Write(_UnitStat.AttackUpgrades.Value);

            for (int P = 0; P < ArrayParts.Length; ++P)
            {
                if (ArrayParts[P] != null)
                {
                    BW.Write(ArrayParts[P].Name);
                }
                else
                {
                    BW.Write("");
                }
            }

            if (string.IsNullOrEmpty(ID))
            {
                BW.Write(ArrayCharacterActive.Length);
                for (int C = 0; C < ArrayCharacterActive.Length; ++C)
                {
                    BW.Write(ArrayCharacterActive[C].FullName);
                    ArrayCharacterActive[C].QuickSave(BW);
                }
            }
            else
            {
                for (int C = 0; C < ArrayCharacterActive.Length; ++C)
                {
                    ArrayCharacterActive[C].QuickSave(BW);
                }
            }

            Boosts.QuickSave(BW);

            DoQuickSave(BW);
        }

        protected abstract void DoQuickSave(BinaryWriter BW);

        public void QuickLoad(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            _HP = BR.ReadInt32();
            _EN = BR.ReadInt32();
            _UnitStat.HPUpgrades.Value = BR.ReadInt32();
            _UnitStat.ENUpgrades.Value = BR.ReadInt32();
            _UnitStat.ArmorUpgrades.Value = BR.ReadInt32();
            _UnitStat.MobilityUpgrades.Value = BR.ReadInt32();
            _UnitStat.AttackUpgrades.Value = BR.ReadInt32();

            for (int P = 0; P < ArrayParts.Length; ++P)
            {
                string PartName = BR.ReadString();
            }

            if (string.IsNullOrEmpty(ID))
            {
                int ArrayCharacterActiveLength = BR.ReadInt32();
                ArrayCharacterActive = new Character[ArrayCharacterActiveLength];
                for (int C = 0; C < ArrayCharacterActive.Length; ++C)
                {
                    ArrayCharacterActive[C] = Character.QuickLoadFromFile(BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                }
            }
            else
            {
                for (int C = 0; C < ArrayCharacterActive.Length; ++C)
                {
                    ArrayCharacterActive[C].QuickLoad(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                }
            }

            Boosts.QuickLoad(BR);

            DoQuickLoad(BR, Content);
        }

        protected abstract void DoQuickLoad(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content);

        public static Unit LoadUnitWithProgress(BinaryReader BR, List<Character> ListTeamCharacter, Microsoft.Xna.Framework.Content.ContentManager Content, Dictionary<string, Unit> DicUnitType,
            Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            string UnitFullName = BR.ReadString();
            string UnitTypeName = BR.ReadString();
            string TeamEventID = BR.ReadString();
            Unit NewUnit = Unit.FromType(UnitTypeName, UnitFullName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            NewUnit.ID = TeamEventID;
            NewUnit.UnitStat.HPUpgrades.Value = BR.ReadInt32();
            NewUnit.UnitStat.ENUpgrades.Value = BR.ReadInt32();
            NewUnit.UnitStat.ArmorUpgrades.Value = BR.ReadInt32();
            NewUnit.UnitStat.MobilityUpgrades.Value = BR.ReadInt32();
            NewUnit.UnitStat.AttackUpgrades.Value = BR.ReadInt32();

            for (int P = 0; P < NewUnit.ArrayParts.Length; ++P)
            {
                string PartName = BR.ReadString();
            }

            int ArrayCharacterActiveLength = BR.ReadInt32();
            NewUnit.ArrayCharacterActive = new Character[ArrayCharacterActiveLength];
            for (int C = 0; C < ArrayCharacterActiveLength; C++)
            {
                string CharacterFullName = BR.ReadString();
                Character CorrespondingCharacter = null;
                foreach (Character ActiveCharacter in ListTeamCharacter)
                {
                    if (ActiveCharacter.FullName == CharacterFullName)
                    {
                        CorrespondingCharacter = ActiveCharacter;
                        break;
                    }
                }

                if (CorrespondingCharacter != null)
                {
                    CorrespondingCharacter.LoadProgression(BR);
                    NewUnit.ArrayCharacterActive[C] = CorrespondingCharacter;

                }
                else
                {
                    NewUnit.ArrayCharacterActive[C] = new Character(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                }
            }

            return NewUnit;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(RelativePath);
            BW.Write(UnitTypeName);
            BW.Write(ID);
            BW.Write(_UnitStat.HPUpgrades.Value);
            BW.Write(_UnitStat.ENUpgrades.Value);
            BW.Write(_UnitStat.ArmorUpgrades.Value);
            BW.Write(_UnitStat.MobilityUpgrades.Value);
            BW.Write(_UnitStat.AttackUpgrades.Value);

            for (int P = 0; P < ArrayParts.Length; ++P)
            {
                if (ArrayParts[P] != null)
                {
                    BW.Write(ArrayParts[P].Name);
                }
                else
                {
                    BW.Write("");
                }
            }

            BW.Write(ArrayCharacterActive.Length);
            for (int C = 0; C < ArrayCharacterActive.Length; C++)
            {
                ArrayCharacterActive[C].SaveProgression(BW);
            }
        }

        public void Init()
        {
            //Initialise the Unit stats.
            _HP = MaxHP;
            _EN = MaxEN;
            foreach (Character ActiveCharacter in ArrayCharacterActive)
            {
                ActiveCharacter.Init();
            }

            _UnitStat.Init();
            DoInit();
            UpdateUsableAttacks();
        }

        public abstract void DoInit();

        public TemporaryAttackPickup OnDeath()
        {
            TemporaryAttackPickup AttackToDrop = null;

            foreach (TemporaryAttackPickup ActiveAttack in ListAttackTemporary)
            {
                if (ActiveAttack.Owner.MaxAmmo > 0 && ActiveAttack.Owner.Ammo > 0)
                {
                    AttackToDrop = ActiveAttack;
                    AttackToDrop.Ammo = ActiveAttack.Owner.Ammo;
                    AttackToDrop.Owner = null;
                    break;
                }
            }

            ListAttackTemporary.Clear();

            UpdateUsableAttacks();

            return AttackToDrop;
        }

        public void UpdateAllAttacks(Vector3 StartPosition, int UnitTeam, Vector3 TargetPosition, int TargetTeam, bool[,] ArrayTargetMapSize, Point TerrainSize, byte TargetMovementType, bool CanMove)
        {
            foreach (Attack ActiveAttack in ListAttack)
            {
                if (UnitStat.RegularAttackDisabled && _UnitStat.ListAttack.Contains(ActiveAttack))
                {
                    ActiveAttack.DisableAttack();
                }
                else if (!ActiveAttack.CanAttack)
                {
                    ActiveAttack.UpdateAttack(this, StartPosition, UnitTeam, TargetPosition, TargetTeam, ArrayTargetMapSize, TerrainSize, TargetMovementType, CanMove);
                }
            }
        }

        public void UpdateNonMAPAttacks(Vector3 StartPosition, int UnitTeam, Vector3 TargetPosition, int TargetTeam, bool[,] ArrayTargetMapSize, Point TerrainSize, byte TargetMovementType, bool CanMove)
        {
            foreach (Attack ActiveAttack in ListAttack)
            {
                if (ActiveAttack.Pri != WeaponPrimaryProperty.MAP)
                {
                    ActiveAttack.UpdateAttack(this, StartPosition, UnitTeam, TargetPosition, TargetTeam, ArrayTargetMapSize, TerrainSize, TargetMovementType, CanMove);
                }
                else
                {
                    ActiveAttack.DisableAttack();
                }
            }
        }

        public void DisableAllAttacks()
        {
            foreach (Attack ActiveAttack in ListAttack)
            {
                ActiveAttack.DisableAttack();
            }
        }

        public void AddTemporaryAttack(string NewAttackName, string SpritePath, Texture2D sprWeapon, Effect Effect3D, byte Ammo, Microsoft.Xna.Framework.Content.ContentManager Content,
            Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            TemporaryAttackPickup ExistingAttack = null;
            foreach (TemporaryAttackPickup ActiveAttack in ListAttackTemporary)
            {
                if (ActiveAttack.Owner.RelativePath == NewAttackName)
                {
                    ExistingAttack = ActiveAttack;
                    break;
                }
            }

            if (ExistingAttack == null)
            {
                ExistingAttack = new TemporaryAttackPickup(new Attack(NewAttackName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget), SpritePath, sprWeapon, Effect3D);
                ListAttackTemporary.Add(ExistingAttack);
            }

            ExistingAttack.Owner.IncreaseAmmo(Ammo);

            UpdateUsableAttacks();
        }

        public void AddTemporaryAttack(TemporaryAttackPickup AttackPickup, Microsoft.Xna.Framework.Content.ContentManager Content,
            Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            TemporaryAttackPickup ExistingAttack = null;
            foreach (TemporaryAttackPickup ActiveAttack in ListAttackTemporary)
            {
                if (ActiveAttack.Owner.RelativePath == AttackPickup.AttackName)
                {
                    ExistingAttack = ActiveAttack;
                    break;
                }
            }

            if (ExistingAttack == null)
            {
                ExistingAttack = AttackPickup;
                ExistingAttack.Owner = new Attack(AttackPickup.AttackName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                ListAttackTemporary.Add(ExistingAttack);
            }

            ExistingAttack.Owner.IncreaseAmmo(ExistingAttack.Ammo);

            UpdateUsableAttacks();
        }

        public void ChargeAttack()
        {
            ChargedAttack = CurrentAttack;
            ChargedAttack.IsChargeable = false;
            UpdateUsableAttacks();
        }

        public void UseChargeAttack()
        {
            ChargedAttack.IsChargeable = true;
            ChargedAttack = null;
            UpdateUsableAttacks();
        }

        public void UpdateUsableAttacks()
        {
            _ListAttack.Clear();

            if (ChargedAttack != null)
            {
                _ListAttack.Add(ChargedAttack);
                foreach (Attack ActiveChargedAttack in ChargedAttack.ListChargedAttack)
                {
                    ActiveChargedAttack.IsChargeable = true;
                    _ListAttack.Add(ActiveChargedAttack);
                }
                return;
            }

            foreach (Attack ActiveAttack in _UnitStat.ListAttack)
            {
                if (ActiveAttack.ListChargedAttack.Count == 0 || ActiveAttack.PowerFormula != "0")
                {
                    _ListAttack.Add(ActiveAttack);
                }

                foreach (Attack ActiveSecondaryAttack in ActiveAttack.ListSecondaryAttack)
                {
                    _ListAttack.Add(ActiveSecondaryAttack);
                }
                foreach (Attack ActiveChargedAttack in ActiveAttack.ListChargedAttack)
                {
                    ActiveChargedAttack.IsChargeable = true;
                    _ListAttack.Add(ActiveChargedAttack);
                }
            }

            foreach (TemporaryAttackPickup ActiveAttack in ListAttackTemporary)
            {
                if (ActiveAttack.Owner.ListChargedAttack.Count == 0 || ActiveAttack.Owner.PowerFormula != "0")
                {
                    _ListAttack.Add(ActiveAttack.Owner);
                }

                foreach (Attack ActiveSecondaryAttack in ActiveAttack.Owner.ListSecondaryAttack)
                {
                    _ListAttack.Add(ActiveSecondaryAttack);
                }
                foreach (Attack ActiveChargedAttack in ActiveAttack.Owner.ListChargedAttack)
                {
                    ActiveChargedAttack.IsChargeable = true;
                    _ListAttack.Add(ActiveChargedAttack);
                }
            }
        }

        public byte GetPostMovementLevel()
        {
            return (byte)((_UnitStat.PostMVLevel + Pilot.PostMVLevel) / 2);
        }

        public virtual void ReloadSkills(Unit Copy, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            for (int P = 0; P < ArrayParts.Length; ++P)
            {
                if (ArrayParts[P] != null)
                {
                    ArrayParts[P].ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                }
            }

            for (int A = 0; A < ArrayUnitAbility.Length; ++A)
            {
                if (ArrayUnitAbility[A] != null)
                {
                    ArrayUnitAbility[A].ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                }
            }

            for (int C = 0; C < ArrayCharacterActive.Length; ++C)
            {
                ArrayCharacterActive[C].ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            }

            for (int A = 0; A < _UnitStat.ListAttack.Count; ++A)
            {
                if (_UnitStat.ListAttack[A].PERAttributes.ListActiveSkill != null)
                {
                    foreach (BaseAutomaticSkill ActivePERAttackSkill in _UnitStat.ListAttack[A].PERAttributes.ListActiveSkill)
                    {
                        ActivePERAttackSkill.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                    }
                }

                foreach (BaseAutomaticSkill ActiveAttackSkill in _UnitStat.ListAttack[A].ArrayAttackAttributes)
                {
                    ActiveAttackSkill.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                }
            }
        }

        public abstract void ReinitializeMembers(Unit InitializedUnitBase);

        public void KillUnit()
        {
            _HP = 0;
        }

        public void DamageUnit(int Damage)
        {
            int NewHP = ComputeRemainingHPAfterDamage(Damage);
            if (_HP <= 0)
            {
                _HP = NewHP;
            }
            else
            {
                _HP = NewHP;
            }
        }

        public int ComputeRemainingHPAfterDamage(int Damage)
        {
            return Math.Max(Boosts.HPMinModifier, HP - Damage);
        }

        public void HealUnit(int HealValue)
        {
            _HP = Math.Min(_HP + HealValue, MaxHP);
        }

        public void ConsumeEN(int Amount)
        {
            _EN = Math.Max(_EN - Amount, 0);
        }

        public void RefillEN(int Amount)
        {
            _EN = Math.Min(_EN + Amount, MaxEN);
        }

        public void RefillSP(int Amount)
        {
            Pilot.SP = Math.Min(Pilot.SP + Amount, Pilot.MaxSP);
        }

        public void EmptySP()
        {
            Pilot.SP = 0;
        }

        public void RefillAmmo(byte Amount, float PercentAmount)
        {
            foreach (Attack ActiveAttack in ListAttack)
            {
                ActiveAttack.IncreaseAmmo((byte)(Amount + ActiveAttack.MaxAmmo * PercentAmount * 0.01f));
            }
        }

        public void ResetBoosts()
        {
            Boosts.Reset();
            ListIgnoreSkill.Clear();
            if (ArrayCharacterActive == null)
                return;

            for (int C = ArrayCharacterActive.Length - 1; C >= 0; --C)
            {
                if (ArrayCharacterActive[C] == null)
                {
                    continue;
                }
                ArrayCharacterActive[C].BonusMEL = 0;
                ArrayCharacterActive[C].BonusRNG = 0;
                ArrayCharacterActive[C].BonusDEF = 0;
                ArrayCharacterActive[C].BonusSKL = 0;
                ArrayCharacterActive[C].BonusEVA = 0;
                ArrayCharacterActive[C].BonusHIT = 0;
                ArrayCharacterActive[C].BonusMaxSP = 0;
                ArrayCharacterActive[C].MaxWill = 150;
                ArrayCharacterActive[C].WillBonus = 0;
            }
        }

        public void ActivePassiveBuffs()
        {
            for (int P = 0; P < ArrayParts.Length; ++P)
            {
                if (ArrayParts[P] != null)
                {
                    ArrayParts[P].ActivatePassiveBuffs();
                }
            }
        }

        public int SizeValue
        {
            get
            {
                switch (UnitStats.ListUnitSize[SizeIndex])
                {
                    case UnitStats.UnitSizeSS:
                        return -20;

                    case UnitStats.UnitSizeS:
                        return -10;

                    case UnitStats.UnitSizeM:
                        return 0;

                    case UnitStats.UnitSizeL:
                        return 10;

                    case UnitStats.UnitSizeLL:
                        return 20;

                    case UnitStats.UnitSizeLLL:
                        return 30;
                }
                return 0;
            }
        }

        public string QualityRank
        {
            get
            {
                if (_UnitStat.SpawnCost >= 1000)
                {
                    return "S";
                }
                else if (_UnitStat.SpawnCost >= 700)
                {
                    return "A";
                }
                else if (_UnitStat.SpawnCost >= 550)
                {
                    return "B";
                }
                else if (_UnitStat.SpawnCost >= 400)
                {
                    return "C";
                }
                else if (_UnitStat.SpawnCost >= 200)
                {
                    return "D";
                }
                else if (_UnitStat.SpawnCost >= 150)
                {
                    return "E";
                }
                else
                {
                    return "F";
                }
            }
        }
        public void ReactivateEffects()
        {
            ResetBoosts();
            for (int C = ArrayCharacterActive.Length - 1; C >= 0; --C)
            {
                ArrayCharacterActive[C].Effects.ReactivateEffects(ListIgnoreSkill);
            }
        }

        public void RemoveEffects()
        {
            ResetBoosts();
            for (int C = ArrayCharacterActive.Length - 1; C >= 0; --C)
            {
                ArrayCharacterActive[C].Effects.RemoveEffects();
            }
        }

        public void UpdateSkillsLifetime(string LifetimeType)
        {
            for (int C = ArrayCharacterActive.Length - 1; C >= 0; --C)
            {
                ArrayCharacterActive[C].Effects.UpdateAllEffectsLifetime(LifetimeType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActiveSquad">Can be null if not using a Squad</param>
        public virtual void OnPlayerPhaseStart(int ActivePlayerIndex, Squad ActiveSquad)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActiveSquad">Can be null if not using a Squad</param>
        public virtual void OnTurnEnd(int ActivePlayerIndex, Squad ActiveSquad)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActiveSquad">Can be null if not using a Squad</param>
        /// <param name="ListActionMenuChoice">Action Panel Holder</param>
        public virtual List<ActionPanel> OnInputPressed(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        { return new List<ActionPanel>(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActiveSquad">Can be null if not using a Squad</param>
        /// <param name="ListActionMenuChoice">Action Panel Holder</param>
        public virtual List<ActionPanel> OnMenuMovement(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        { return new List<ActionPanel>(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActiveSquad">Can be null if not using a Squad</param>
        /// <param name="ListActionMenuChoice">Action Panel Holder</param>
        public virtual List<ActionPanel> OnMenuSelect(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        { return new List<ActionPanel>(); }

        public abstract GameScreens.GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser);

        #region TerrainAttributes

        public char TerrainLetterAttribute(byte MovementTypeIndex)
        {
            return Grades[Math.Min(Grades.Length - 1, _UnitStat.TerrainAttributeValue(MovementTypeIndex))];
        }

        public int TerrainMobilityAttribute(byte MovementTypeIndex)
        {
            switch (TerrainLetterAttribute(MovementTypeIndex))
            {
                case 'S':
                    return 20;

                case 'A':
                    return 10;

                case 'B':
                    return 0;

                case 'C':
                    return -15;

                case 'D':
                    return -30;
            }
            return 0;
        }

        public int TerrainArmorAttribute(byte MovementTypeIndex)
        {
            switch (TerrainLetterAttribute(MovementTypeIndex))
            {
                case 'S':
                    return 20;

                case 'A':
                    return 10;

                case 'B':
                    return 0;

                case 'C':
                    return -10;

                case 'D':
                    return -20;
            }
            return 0;
        }

        #endregion
    }
}
