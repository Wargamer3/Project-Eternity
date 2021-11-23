using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FMOD;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Units;

namespace ProjectEternity.Core.Characters
{
    public class Character
    {
        public enum CharacterLinkTypes : byte { None = 0x00, EXP = 0x01, Level = 0x02, MEL = 0x04, RNG = 0x08, DEF = 0x10, SKL = 0x20, EVA = 0x40, HIT = 0x80 };

        public struct TerrainGrades
        {
            public char TerrainGradeAir;
            public char TerrainGradeLand;
            public char TerrainGradeSea;
            public char TerrainGradeSpace;

            public TerrainGrades(char TerrainGradeAir, char TerrainGradeLand, char TerrainGradeSea, char TerrainGradeSpace)
            {
                this.TerrainGradeAir = TerrainGradeAir;
                this.TerrainGradeLand = TerrainGradeLand;
                this.TerrainGradeSea = TerrainGradeSea;
                this.TerrainGradeSpace = TerrainGradeSpace;
            }

            public TerrainGrades(int TerrainGradeAir, int TerrainGradeLand, int TerrainGradeSea, int TerrainGradeSpace)
            {
                char[] Grades = new char[5] { 'S', 'A', 'B', 'C', 'D' };
                this.TerrainGradeAir = Grades[TerrainGradeAir];
                this.TerrainGradeLand = Grades[TerrainGradeLand];
                this.TerrainGradeSea = Grades[TerrainGradeSea];
                this.TerrainGradeSpace = Grades[TerrainGradeSpace];
            }

            public char GetTerrain(string TerrainType)
            {
                switch (TerrainType)
                {
                    case UnitStats.TerrainAir:
                        return TerrainGradeAir;
                    case UnitStats.TerrainLand:
                        return TerrainGradeLand;
                    case UnitStats.TerrainSea:
                        return TerrainGradeSea;
                    case UnitStats.TerrainSpace:
                        return TerrainGradeSpace;
                }

                return 'B';
            }
        };

        public class QuoteSet
        {
            public List<string> ListQuote;
            public List<string> ListQuoteVersus;
            public string PortraitPath;

            public QuoteSet()
            {
                ListQuote = new List<string>();
                ListQuoteVersus = new List<string>();
                PortraitPath = "";
            }
        }

        public class SkillLevels
        {
            public Dictionary<int, int> DicSkillLevelPerCharacterLevel;

            public SkillLevels()
            {
                DicSkillLevelPerCharacterLevel = new Dictionary<int, int>();
            }

            public SkillLevels(BinaryReader BR, int LevelsCount)
            {
                DicSkillLevelPerCharacterLevel = new Dictionary<int, int>(LevelsCount);
                for (int L = 0; L < LevelsCount; ++L)
                {
                    DicSkillLevelPerCharacterLevel.Add(BR.ReadInt32(), BR.ReadInt32());
                }
            }
        }

        public struct CharacterPersonality
        {
            public int WillGainHitEnemy;
            public int WillGainMissedEnemy;
            public int WillGainDestroyedEnemy;
            public int WillGainGotHit;
            public int WillGainEvaded;
            public int WillGainAlliedUnitDestroyed;
            public string Name;

            public CharacterPersonality(string PersonalityName)
            {
                Name = PersonalityName;

                if (File.Exists("Content/Characters/Personalities/" + PersonalityName + ".pecp"))
                {
                    FileStream FS = new FileStream("Content/Characters/Personalities/" + PersonalityName + ".pecp", FileMode.Open, FileAccess.Read);
                    BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                    BR.BaseStream.Seek(0, SeekOrigin.Begin);

                    WillGainHitEnemy = BR.ReadInt32();
                    WillGainMissedEnemy = BR.ReadInt32();
                    WillGainDestroyedEnemy = BR.ReadInt32();
                    WillGainGotHit = BR.ReadInt32();
                    WillGainEvaded = BR.ReadInt32();
                    WillGainAlliedUnitDestroyed = BR.ReadInt32();

                    FS.Close();
                    BR.Close();
                }
                else
                {
                    WillGainHitEnemy = 0;
                    WillGainMissedEnemy = 0;
                    WillGainDestroyedEnemy = 0;
                    WillGainGotHit = 0;
                    WillGainEvaded = 0;
                    WillGainAlliedUnitDestroyed = 0;
                }
            }
        }

        #region Variables

        public ManualSkill[] ArrayPilotSpirit;
        public BaseAutomaticSkill[] ArrayPilotSkill;
        public BaseAutomaticSkill[] ArrayRelationshipBonus;
        public bool[] ArrayPilotSkillLocked;
        public SkillLevels[] ArrayPilotSkillLevels;
        public string Tags;
        public TagSystem TeamTags;
        public EffectHolder Effects;

        public static Dictionary<string, FMODSound> DicBattleTheme = new Dictionary<string, FMODSound>();
        public string BattleThemeName;
        public ManualSkill AceBonus;

        public string Name;
        public string FullName;
        public string PortraitPath;
        public string[] ArrayPortraitBustPath;
        public string[] ArrayPortraitBoxPath;
        public int EXPValue;
        public bool CanPilot;
        public TerrainGrades TerrainGrade;
        private SharableInt32 _EXP;
        public int EXP { private set { _EXP.Value = value; } get { return _EXP.Value; } }

        public int NextEXP { get { return 500; } }// How long until a character levels up.
        private SharableInt32 _Level;
        public int Level { set { _Level.Value = value; } get { return _Level.Value; } }// The characters current level. MEL, RNG, DEF, SKL, EVA, HIT, and SP all improve slightly with every level up.

        public int MaxLevel;

        private int _Will;
        public int Will { get { return Math.Min(MaxWill, _Will + WillBonus); } set { _Will = value < MaxWill ? value : MaxWill; } }// Starts at 100 for all pilots, can be improved via items, pilot skills, and Ace Bonus. Morale is used in the damage formula.
        public int WillBonus;
        public int MaxWill;
        public CharacterPersonality Personality;
        public Character Slave;

        public int SP;// Spirit Points are used to cast spirits, the game's equivelant of magic.
        public int MaxSP { get { return ArrayLevelMaxSP[Level - 1] + BonusMaxSP; } }

        public int BonusMaxSP;
        public int Kills;// The number of enemy pilots the pilot has shot down over the course of the game. Ace Bonuses are unlocked at certain amount of kills.
        public int PilotPoints;// Pilot Points. They are gained by successfully completing objectives and defeating enemy units. They are used to upgrade Pilot Stats.
        public Dictionary<string, CharacterLinkTypes> DicCharacterLink;//List which Characters it can link to and how.

        public Dictionary<string, QuoteSet> DicAttackQuoteSet;

        public List<string> ListQuoteSetVersusName;

        public int MEL { get { return BaseMEL.Value + BonusMEL; } }// Melee. Used for damage calculations involving Melee Attacks. (Swords, Axes, Punches, etc.)

        public int RNG { get { return BaseRNG.Value + BonusRNG; } }// Ranged. Used for damage calculations involving Ranged Attacks. (Rifles, Bazookas, Machineguns, etc.)

        public int DEF { get { return BaseDEF.Value + BonusDEF; } }// Defense. Used for calculations involving receiving damage from enemy attack.

        public int SKL { get { return BaseSKL.Value + BonusSKL; } }// Skill. Used for calculations to determine critical rate percentage.

        public int EVA { get { return BaseEVA.Value + BonusEVA; } }// Evasion. Used for calculations to determine whether or not the pilot's unit will dodge an attack.

        public int HIT { get { return BaseHIT.Value + BonusHIT; } }// Hit. Used for calculations to determine whether or not the pilot's attack will hit an enemy.

        private SharableInt32 BaseMEL;
        private SharableInt32 BaseRNG;
        private SharableInt32 BaseDEF;
        private SharableInt32 BaseSKL;
        private SharableInt32 BaseEVA;
        private SharableInt32 BaseHIT;

        public int[] ArrayLevelMEL;
        public int[] ArrayLevelRNG;
        public int[] ArrayLevelDEF;
        public int[] ArrayLevelSKL;
        public int[] ArrayLevelEVA;
        public int[] ArrayLevelHIT;
        public int[] ArrayLevelMaxSP;

        public int BonusMEL;
        public int BonusRNG;
        public int BonusDEF;
        public int BonusSKL;
        public int BonusEVA;
        public int BonusHIT;

        #endregion

        #region Ressources

        // This is shown in places like the status screen.
        public Texture2D sprPortrait;

        public QuoteSet QuoteSetBattleStart { get { return ArrayBaseQuoteSet[0]; } }

        public QuoteSet QuoteSetDodge { get { return ArrayBaseQuoteSet[1]; } }

        public QuoteSet QuoteSetDamaged { get { return ArrayBaseQuoteSet[2]; } }

        public QuoteSet QuoteSetDestroyed { get { return ArrayBaseQuoteSet[3]; } }

        public QuoteSet QuoteSetSupportAttack { get { return ArrayBaseQuoteSet[4]; } }

        public QuoteSet QuoteSetSupportDefend { get { return ArrayBaseQuoteSet[5]; } }

        public QuoteSet[] ArrayBaseQuoteSet = new QuoteSet[6];

        #endregion

        public Character()
        {
            _EXP = new SharableInt32();
            _Level = new SharableInt32();
            BaseMEL = new SharableInt32();
            BaseRNG = new SharableInt32();
            BaseDEF = new SharableInt32();
            BaseSKL = new SharableInt32();
            BaseEVA = new SharableInt32();
            BaseHIT = new SharableInt32();

            _EXP.Value = 0;
            _Level.Value = 0;
            BaseMEL.Value = 0;
            BaseRNG.Value = 0;
            BaseDEF.Value = 0;
            BaseSKL.Value = 0;
            BaseEVA.Value = 0;
            BaseHIT.Value = 0;

            ArrayPilotSkill = new BaseAutomaticSkill[0];
            ArrayRelationshipBonus = new BaseAutomaticSkill[0];
            ArrayPilotSpirit = new ManualSkill[0];

            TerrainGrade = new TerrainGrades(0, 0, 0, 0);

            MaxWill = 150;
            Will = 100;
            TeamTags = new TagSystem();
            Effects = new EffectHolder();
            DicCharacterLink = new Dictionary<string, CharacterLinkTypes>();
            ListQuoteSetVersusName = new List<string>();
            DicAttackQuoteSet = new Dictionary<string, QuoteSet>();
        }

        public Character(string CharacterPath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
            : this()
        {
            FileStream FS = new FileStream("Content/Characters/" + CharacterPath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Init variables.
            Name = BR.ReadString();
            PortraitPath = BR.ReadString();

            ArrayPortraitBustPath = new string[BR.ReadInt32()];
            for (int B = 0; B < ArrayPortraitBustPath.Length; ++B)
            {
                ArrayPortraitBustPath[B] = BR.ReadString();
            }

            ArrayPortraitBoxPath = new string[BR.ReadInt32()];
            for (int B = 0; B < ArrayPortraitBoxPath.Length; ++B)
            {
                ArrayPortraitBoxPath[B] = BR.ReadString();
            }

            Tags = BR.ReadString();
            FullName = CharacterPath;
            EXPValue = BR.ReadInt32();
            CanPilot = BR.ReadBoolean();
            BattleThemeName = BR.ReadString();
            string AceBonus = BR.ReadString();
            string PersonalityName = BR.ReadString();
            string SlaveName = BR.ReadString();

            Personality = new CharacterPersonality(PersonalityName);

            if (!string.IsNullOrWhiteSpace(SlaveName) && SlaveName != "None")
            {
                Slave = new Character(SlaveName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            }

            Int32 SpiritListCount = BR.ReadInt32();
            ArrayPilotSpirit = new ManualSkill[SpiritListCount];

            for (int S = 0; S < SpiritListCount; ++S)
            {
                ArrayPilotSpirit[S] = new ManualSkill("Content/Characters/Spirits/" + BR.ReadString() + ".pecs", DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                ArrayPilotSpirit[S].SPCost = BR.ReadInt32();
                ArrayPilotSpirit[S].LevelRequirement = BR.ReadInt32();
            }

            Int32 SkillListCount = BR.ReadInt32();
            ArrayPilotSkill = new BaseAutomaticSkill[SkillListCount];
            ArrayPilotSkillLocked = new bool[SkillListCount];
            ArrayPilotSkillLevels = new SkillLevels[SkillListCount];

            for (int S = 0; S < SkillListCount; ++S)
            {
                string RelativePath = BR.ReadString();
                ArrayPilotSkill[S] = new BaseAutomaticSkill("Content/Characters/Skills/" + RelativePath + ".pecs", RelativePath, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                ArrayPilotSkillLocked[S] = BR.ReadBoolean();
                Int32 SkillLevelsCount = BR.ReadInt32();
                ArrayPilotSkillLevels[S] = new SkillLevels(BR, SkillLevelsCount);
            }

            Int32 RelationshipBonusCount = BR.ReadInt32();
            ArrayRelationshipBonus = new BaseAutomaticSkill[RelationshipBonusCount];

            for (int S = 0; S < RelationshipBonusCount; ++S)
            {
                string RelationshipBonusName = BR.ReadString();
                int RelationshipLevel = BR.ReadInt32();
                ArrayRelationshipBonus[S] = new BaseAutomaticSkill("Content/Characters/Relationships/" + RelationshipBonusName + ".pecr", RelationshipBonusName, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                ArrayRelationshipBonus[S].CurrentLevel = RelationshipLevel;

                for (int L = 0; L < ArrayRelationshipBonus[S].ListSkillLevel.Count; ++L)
                {
                    BaseSkillRequirement NewSkillRequirement = BaseSkillRequirement.LoadCopy(BR, DicRequirement);
                    ArrayRelationshipBonus[S].ListSkillLevel[L].ListActivation[0].ListRequirement.Add(NewSkillRequirement);
                }
            }

            //If it's a pilot, read its stats.
            if (CanPilot)
            {
                MaxLevel = BR.ReadInt32();
                ArrayLevelMEL = new int[MaxLevel];
                ArrayLevelRNG = new int[MaxLevel];
                ArrayLevelDEF = new int[MaxLevel];
                ArrayLevelSKL = new int[MaxLevel];
                ArrayLevelEVA = new int[MaxLevel];
                ArrayLevelHIT = new int[MaxLevel];
                ArrayLevelMaxSP = new int[MaxLevel];

                for (int L = 0; L < MaxLevel; ++L)
                {
                    ArrayLevelMEL[L] = BR.ReadInt32();
                    ArrayLevelRNG[L] = BR.ReadInt32();
                    ArrayLevelDEF[L] = BR.ReadInt32();
                    ArrayLevelSKL[L] = BR.ReadInt32();
                    ArrayLevelEVA[L] = BR.ReadInt32();
                    ArrayLevelHIT[L] = BR.ReadInt32();
                    ArrayLevelMaxSP[L] = BR.ReadInt32();
                }

                int TerrainGradeAir = BR.ReadInt32();
                int TerrainGradeLand = BR.ReadInt32();
                int TerrainGradeSea = BR.ReadInt32();
                int TerrainGradeSpace = BR.ReadInt32();

                TerrainGrade = new TerrainGrades(TerrainGradeAir, TerrainGradeLand, TerrainGradeSea, TerrainGradeSpace);
            }

            int ListQuoteSetVersusNameCount = BR.ReadInt32();
            for (int Q = 0; Q < ListQuoteSetVersusNameCount; Q++)
                ListQuoteSetVersusName.Add(BR.ReadString());

            //Base quotes
            for (int I = 0; I < 6; I++)
            {
                ArrayBaseQuoteSet[I] = new QuoteSet();

                int ListQuoteCount = BR.ReadInt32();
                for (int Q = 0; Q < ListQuoteCount; Q++)
                    ArrayBaseQuoteSet[I].ListQuote.Add(BR.ReadString());

                //Versus quotes.
                int ListQuoteVersusCount = BR.ReadInt32();
                for (int Q = 0; Q < ListQuoteVersusCount; Q++)
                    ArrayBaseQuoteSet[I].ListQuoteVersus.Add(BR.ReadString());

                ArrayBaseQuoteSet[I].PortraitPath = BR.ReadString();
            }

            int DicAttackQuoteSetCount = BR.ReadInt32();
            for (int i = 0; i < DicAttackQuoteSetCount; i++)
            {
                QuoteSet NewQuoteSet = new QuoteSet();

                string QuoteSetName = BR.ReadString();

                int ListQuoteCount = BR.ReadInt32();
                for (int Q = 0; Q < ListQuoteCount; Q++)
                    NewQuoteSet.ListQuote.Add(BR.ReadString());

                int ListQuoteVersusCount = BR.ReadInt32();
                for (int Q = 0; Q < ListQuoteVersusCount; Q++)
                    NewQuoteSet.ListQuoteVersus.Add(BR.ReadString());

                NewQuoteSet.PortraitPath = BR.ReadString();

                DicAttackQuoteSet.Add(QuoteSetName, NewQuoteSet);
            }

            FS.Close();
            BR.Close();

            if (Content != null)
            {
                string SpritePath = Path.GetFileNameWithoutExtension(CharacterPath);
                if (!string.IsNullOrEmpty(PortraitPath))
                {
                    SpritePath = PortraitPath;
                }

                if (File.Exists("Content\\Visual Novels\\Portraits\\" + SpritePath + ".xnb"))
                    this.sprPortrait = Content.Load<Texture2D>("Visual Novels\\Portraits\\" + SpritePath);
                else
                    this.sprPortrait = Content.Load<Texture2D>("Characters\\Portraits\\Default");
            }
        }

        public void Init()
        {
            InitStats();
        }

        public void IncreaseEXP(int EXPGained)
        {
            EXP += EXPGained;
        }

        public void LevelUpOnce()
        {
            EXP -= NextEXP;
            Level++;
            Level = Math.Min(Level, ArrayLevelMEL.Length);

            InitStats();
        }

        private void InitStats()
        {
            BaseMEL.Value = ArrayLevelMEL[Level - 1];
            BaseRNG.Value = ArrayLevelRNG[Level - 1];
            BaseDEF.Value = ArrayLevelDEF[Level - 1];
            BaseSKL.Value = ArrayLevelSKL[Level - 1];
            BaseEVA.Value = ArrayLevelEVA[Level - 1];
            BaseHIT.Value = ArrayLevelHIT[Level - 1];

            for (int S = ArrayPilotSkill.Length - 1; S >= 0; --S)
            {
                int SkillLevel;
                if (ArrayPilotSkillLevels[S].DicSkillLevelPerCharacterLevel.TryGetValue(Level, out SkillLevel))
                {
                    ArrayPilotSkill[S].CurrentLevel = SkillLevel;
                }
            }

            for (int S = ArrayPilotSpirit.Length - 1; S >= 0; --S)
            {
                if (Level >= ArrayPilotSpirit[S].LevelRequirement)
                {
                    ArrayPilotSpirit[S].IsUnlocked = true;
                }
            }
        }

        public void ShareStats(Character CharacterToShareFrom, CharacterLinkTypes CharacterLinkType)
        {
            if ((CharacterLinkType & CharacterLinkTypes.EXP) == CharacterLinkTypes.EXP)
            {
                _EXP.Pointer = CharacterToShareFrom._EXP;
            }
            if ((CharacterLinkType & CharacterLinkTypes.Level) == CharacterLinkTypes.Level)
            {
                _Level.Pointer = CharacterToShareFrom._Level;
            }
            if ((CharacterLinkType & CharacterLinkTypes.MEL) == CharacterLinkTypes.MEL)
            {
                BaseMEL.Pointer = CharacterToShareFrom.BaseMEL;
            }
            if ((CharacterLinkType & CharacterLinkTypes.RNG) == CharacterLinkTypes.RNG)
            {
                BaseRNG.Pointer = CharacterToShareFrom.BaseRNG;
            }
            if ((CharacterLinkType & CharacterLinkTypes.DEF) == CharacterLinkTypes.DEF)
            {
                BaseDEF.Pointer = CharacterToShareFrom.BaseDEF;
            }
            if ((CharacterLinkType & CharacterLinkTypes.SKL) == CharacterLinkTypes.SKL)
            {
                BaseSKL.Pointer = CharacterToShareFrom.BaseSKL;
            }
            if ((CharacterLinkType & CharacterLinkTypes.EVA) == CharacterLinkTypes.EVA)
            {
                BaseEVA.Pointer = CharacterToShareFrom.BaseEVA;
            }
            if ((CharacterLinkType & CharacterLinkTypes.HIT) == CharacterLinkTypes.HIT)
            {
                BaseHIT.Pointer = CharacterToShareFrom.BaseHIT;
            }
        }

        public void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            for (int S = 0; S < ArrayPilotSpirit.Length; ++S)
            {
                ArrayPilotSpirit[S].ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            }

            for (int S = 0; S < ArrayPilotSkill.Length; ++S)
            {
                ArrayPilotSkill[S].ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }

            for (int S = 0; S < ArrayRelationshipBonus.Length; ++S)
            {
                ArrayRelationshipBonus[S].ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
        }

        public void LoadProgression(BinaryReader BR)
        {
            Level = BR.ReadInt32();
            EXP = BR.ReadInt32();
            Kills = BR.ReadInt32();
            PilotPoints = BR.ReadInt32();
            InitStats();
        }

        public void QuickSave(BinaryWriter BW)
        {
            BW.Write(EXP);
            BW.Write(Kills);
            BW.Write(Level);
            BW.Write(Will);
            BW.Write(PilotPoints);
            BW.Write(SP);
        }

        public void QuickLoad(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            EXP = BR.ReadInt32();
            Kills = BR.ReadInt32();
            Level = BR.ReadInt32();
            Will = BR.ReadInt32();
            PilotPoints = BR.ReadInt32();
            SP = BR.ReadInt32();
        }

        public static Character QuickLoadFromFile(BinaryReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Character NewCharacter = new Character(BR.ReadString(), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            NewCharacter.QuickLoad(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            NewCharacter.InitStats();
            return NewCharacter;
        }

        public void SaveProgression(BinaryWriter BW)
        {
            BW.Write(FullName);
            BW.Write(Level);
            BW.Write(EXP);
            BW.Write(Kills);
            BW.Write(PilotPoints);
        }
    }
}
