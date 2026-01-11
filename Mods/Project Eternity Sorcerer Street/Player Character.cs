using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class QuoteSet
    {
        public List<QuoteSetMap> ListMapQuote;

        public QuoteSet()
        {
            ListMapQuote = new List<QuoteSetMap>();
        }

        public QuoteSet(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListMapQuote = new List<QuoteSetMap>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListMapQuote.Add(new QuoteSetMap(BR));
            }
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListMapQuote.Count);
            for (int I = 0; I < ListMapQuote.Count; I++)
            {
                ListMapQuote[I].Write(BW);
            }
        }
    }

    public class QuoteSetMap
    {
        public List<QuoteSetVersus> ListQuoteVersus;

        public QuoteSetMap()
        {
            ListQuoteVersus = new List<QuoteSetVersus>();
        }

        public QuoteSetMap(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListQuoteVersus = new List<QuoteSetVersus>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListQuoteVersus.Add(new QuoteSetVersus(BR));
            }
        }

        internal void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListQuoteVersus.Count);
            for (int I = 0; I < ListQuoteVersus.Count; I++)
            {
                ListQuoteVersus[I].Write(BW);
            }
        }
    }

    public class QuoteSetVersus
    {
        public List<string> ListQuote;
        public List<string> ListPortraitPath;

        public QuoteSetVersus()
        {
            ListQuote = new List<string>();
            ListPortraitPath = new List<string>();
        }

        public QuoteSetVersus(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListQuote = new List<string>(ListQuoteCount);
            ListPortraitPath = new List<string>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListQuote.Add(BR.ReadString());
                ListPortraitPath.Add(BR.ReadString());
            }
        }

        internal void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListQuote.Count);
            for (int I = 0; I < ListQuote.Count; I++)
            {
                BW.Write(ListQuote[I]);
                BW.Write(ListPortraitPath[I]);
            }
        }
    }

    public class PlayerCharacterInfo
    {
        public PlayerCharacter Character;
        public List<PlayerCharacterSkin> ListOwnedUnitSkin;
        public List<PlayerCharacterSkin> ListOwnedUnitAlt;

        public PlayerCharacterInfo(PlayerCharacter Pilot)
        {
            this.Character = Pilot;
            ListOwnedUnitSkin = new List<PlayerCharacterSkin>();
            ListOwnedUnitAlt = new List<PlayerCharacterSkin>();
        }
    }

    public class PlayerCharacterSkin
    {
        public string CharacterRelativePath;
        public string SkinRelativePath;
        public PlayerCharacter CharacterSkin;

        public PlayerCharacterSkin(string CharacterRelativePath, string SkinRelativePath, PlayerCharacter CharacterSkin)
        {
            this.CharacterRelativePath = CharacterRelativePath;
            this.SkinRelativePath = SkinRelativePath;
            this.CharacterSkin = CharacterSkin;
        }

        public override string ToString()
        {
            return SkinRelativePath;
        }
    }

    public class PlayerCharacterAIParameters
    {
        public byte InvasionAggressiveness;//Aggressive 1 - 9 Careful
        public byte InvasionElement;//Anywhere 1 - 9 Elemental-focused
        public byte DefenceElement;//Anywhere 1 - 9 Elemental-focused
        public byte SummonElement;//Anywhere 1 - 9 Elemental-focused
        public byte CreatureSummonCost;//Anywhere 1 - 9 Elemental-focused

        public byte RemainingMagic;//Don't worry 1 - 9 Worry
        public byte SpellEffectiveness;//Don't worry 1 - 9 Worry
        public byte SpellDamage;//Don't worry 1 - 9 Worry
        public byte CardsInHand;//Don't worry 1 - 6 Worry
        public byte SpellsOnCepters;//Emotional 1 - 3 Calm
        public byte Symbols;//Don't worry 1 - 9 Worry
        public byte SymbolChainBuy;//Little 1 - 9 A lot
        public byte SymbolBuy;//Just a little 1 - 9 Lots
        public byte SymbolSell;//Just a little 1 - 9 Lots

        public byte CreatureCardsImportance;//Unimportant 1 - 9 Important
        public byte ItemCardsImportance;//Unimportant 1 - 9 Important
        public byte SpellCardsImportance;//Unimportant 1 - 9 Important
        public byte LevelingUpLand;//Just a little 1 - 9 Lots
        public byte LandLevelUpCommand;//Just a little 1 - 9 Lots
        public byte CreatureExchangeCommand;//Sometimes 1 - 9 All the time
        public byte CreatureMovement;//Sometimes 1 - 9 All the time
        public byte CreatureAbility;//Sometimes 1 - 9 All the time
        public byte ElementToStopOn;//Anywhere 1 - 9 Element-focused
        public byte AvoidExpensiveLand;//Don't worry 1 - 9 Avoid
        public byte PlayerAlliances;//Ignore 1 - 9 Take good care of

        public PlayerCharacterAIParameters(BinaryReader BR)
        {
            InvasionAggressiveness = BR.ReadByte();
            InvasionElement = BR.ReadByte();
            DefenceElement = BR.ReadByte();
            SummonElement = BR.ReadByte();
            CreatureSummonCost = BR.ReadByte();

            RemainingMagic = BR.ReadByte();
            SpellEffectiveness = BR.ReadByte();
            SpellDamage = BR.ReadByte();
            CardsInHand = BR.ReadByte();
            SpellsOnCepters = BR.ReadByte();
            Symbols = BR.ReadByte();
            SymbolChainBuy = BR.ReadByte();
            SymbolBuy = BR.ReadByte();
            SymbolSell = BR.ReadByte();

            CreatureCardsImportance = BR.ReadByte();
            ItemCardsImportance = BR.ReadByte();
            SpellCardsImportance = BR.ReadByte();
            LevelingUpLand = BR.ReadByte();
            LandLevelUpCommand = BR.ReadByte();
            CreatureExchangeCommand = BR.ReadByte();
            CreatureMovement = BR.ReadByte();
            CreatureAbility = BR.ReadByte();
            ElementToStopOn = BR.ReadByte();
            AvoidExpensiveLand = BR.ReadByte();
            PlayerAlliances = BR.ReadByte();
        }
    }

    public class PlayerCharacter
    {
        public string SpriteMapPath;
        public Texture2D SpriteMap;
        public string SpriteShopPath;
        public Texture2D SpriteShop;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;

        public string Tags;//Used to categorize Characters

        public string Name;
        public string CharacterPath;
        public string Description;
        public int Price;

        public List<string> ListWhitelist;
        public List<string> ListBlacklist;
        public List<string> ListAIBook;

        public PlayerCharacterAIParameters PlayerCharacterAIParameter;

        public ManualSkill[] ArraySpell;
        public BaseAutomaticSkill[] ArraySkill;
        public BaseAutomaticSkill[] ArrayRelationshipBonus;

        #region Quotes

        public QuoteSet[] ArrayBaseQuoteSet;
        public List<string> ListQuoteSetMapName;
        public List<string> ListQuoteSetVersusName;

        public QuoteSet Introduction => ArrayBaseQuoteSet[0];
        public QuoteSet AllianceIntroduction => ArrayBaseQuoteSet[1];
        public QuoteSet Banter => ArrayBaseQuoteSet[2];
        public QuoteSet AllianceBanter => ArrayBaseQuoteSet[3];
        public QuoteSet WinningBanter => ArrayBaseQuoteSet[4];
        public QuoteSet WinningAllianceBanter => ArrayBaseQuoteSet[5];
        public QuoteSet LosingBanter => ArrayBaseQuoteSet[6];
        public QuoteSet MajorLosingBanter => ArrayBaseQuoteSet[7];
        public QuoteSet LosingAllianceBanter => ArrayBaseQuoteSet[8];
        public QuoteSet TerritoryClaim => ArrayBaseQuoteSet[9];
        public QuoteSet ChainSmall => ArrayBaseQuoteSet[10];
        public QuoteSet ChainBig => ArrayBaseQuoteSet[11];
        public QuoteSet TerritoryLevelUp => ArrayBaseQuoteSet[12];
        public QuoteSet TerritoryLevelUpBig => ArrayBaseQuoteSet[13];
        public QuoteSet SuccessfulInvasion => ArrayBaseQuoteSet[14];
        public QuoteSet FailedInvasion => ArrayBaseQuoteSet[15];
        public QuoteSet SuccessfulDefense => ArrayBaseQuoteSet[16];
        public QuoteSet FailedDefense => ArrayBaseQuoteSet[17];
        public QuoteSet SmallMoneyLoss => ArrayBaseQuoteSet[18];
        public QuoteSet MediumMoneyLoss => ArrayBaseQuoteSet[19];
        public QuoteSet LargeMoneyLoss => ArrayBaseQuoteSet[20];
        public QuoteSet SmallMoneyGains => ArrayBaseQuoteSet[21];
        public QuoteSet BigMoneyGains => ArrayBaseQuoteSet[22];
        public QuoteSet OpponentAchieveObjective => ArrayBaseQuoteSet[23];
        public QuoteSet OpponentAchieveObjectiveAlliance => ArrayBaseQuoteSet[24];
        public QuoteSet AchieveObjective => ArrayBaseQuoteSet[25];
        public QuoteSet AchieveObjectiveAlliance => ArrayBaseQuoteSet[26];
        public QuoteSet WonMatch => ArrayBaseQuoteSet[27];
        public QuoteSet WonAllianceMatch => ArrayBaseQuoteSet[28];

        #endregion

        public PlayerCharacter(PlayerCharacter Clone)
        {
            this.SpriteMapPath = Clone.SpriteMapPath;
            this.SpriteMap = Clone.SpriteMap;
            this.SpriteShopPath = Clone.SpriteShopPath;
            this.SpriteShop = Clone.SpriteShop;
            this.Model3DPath = Clone.Model3DPath;
            this.Unit3DModel = Clone.Unit3DModel;

            this.Tags = Clone.Tags;

            this.Name = Clone.Name;
            this.CharacterPath = Clone.CharacterPath;
            this.Description = Clone.Description;
            this.Price = Clone.Price;

            this.ListWhitelist = new List<string>(Clone.ListWhitelist);
            this.ListBlacklist = new List<string>(Clone.ListBlacklist);
            this.ListAIBook = new List<string>(Clone.ListAIBook);

            this.ArraySpell = (ManualSkill[])Clone.ArraySpell.Clone();
            this.ArraySkill = (BaseAutomaticSkill[])Clone.ArraySkill.Clone();
            this.ArrayRelationshipBonus = (BaseAutomaticSkill[])Clone.ArrayRelationshipBonus.Clone();

            this.ArrayBaseQuoteSet = (QuoteSet[])Clone.ArrayBaseQuoteSet.Clone();

            this.ListQuoteSetVersusName = new List<string>(Clone.ListQuoteSetVersusName);
        }

        public PlayerCharacter(string CharacterPath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            this.CharacterPath = CharacterPath;

            FileStream FS = new FileStream("Content/Sorcerer Street/Characters/" + CharacterPath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Init variables.
            Name = BR.ReadString();
            Description = BR.ReadString();
            Price = BR.ReadInt32();

            SpriteShopPath = BR.ReadString();
            SpriteMapPath = BR.ReadString();
            Model3DPath = BR.ReadString();
            Tags = BR.ReadString();

            byte ArraySpellCount = BR.ReadByte();
            ArraySpell = new ManualSkill[ArraySpellCount];

            for (int S = 0; S < ArraySpellCount; ++S)
            {
                ArraySpell[S] = new ManualSkill("Content/Characters/Spirits/" + BR.ReadString() + ".pecs", DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                ArraySpell[S].ActivationCost = BR.ReadInt32();
            }

            byte ArraySkillCount = BR.ReadByte();
            ArraySkill = new BaseAutomaticSkill[ArraySkillCount];

            for (int S = 0; S < ArraySkillCount; ++S)
            {
                string RelativePath = BR.ReadString();
                ArraySkill[S] = new BaseAutomaticSkill("Content/Characters/Skills/" + RelativePath + ".pecs", RelativePath, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }

            byte ListWhitelistCount = BR.ReadByte();
            ListWhitelist = new List<string>(ListWhitelistCount);
            for (int S = 0; S < ListWhitelistCount; ++S)
            {
                ListWhitelist.Add(BR.ReadString());
            }

            byte ListBlacklistCount = BR.ReadByte();
            ListBlacklist = new List<string>(ListBlacklistCount);
            for (int S = 0; S < ListBlacklistCount; ++S)
            {
                ListBlacklist.Add(BR.ReadString());
            }

            byte ListAIBookCount = BR.ReadByte();
            ListAIBook = new List<string>(ListAIBookCount);
            for (int S = 0; S < ListAIBookCount; ++S)
            {
                ListAIBook.Add(BR.ReadString());
            }

            byte RelationshipBonusCount = BR.ReadByte();
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

            PlayerCharacterAIParameter = new PlayerCharacterAIParameters(BR);

            #region Quotes

            int ListQuoteSetMapNameCount = BR.ReadInt32();
            ListQuoteSetMapName = new List<string>(ListQuoteSetMapNameCount);
            for (int Q = 0; Q < ListQuoteSetMapNameCount; Q++)
                ListQuoteSetMapName.Add(BR.ReadString());

            int ListQuoteSetVersusNameCount = BR.ReadInt32();
            ListQuoteSetVersusName = new List<string>(ListQuoteSetVersusNameCount);
            for (int Q = 0; Q < ListQuoteSetVersusNameCount; Q++)
                ListQuoteSetVersusName.Add(BR.ReadString());

            byte ArrayBaseQuoteSetCount = BR.ReadByte();
            ArrayBaseQuoteSet = new QuoteSet[ArrayBaseQuoteSetCount];
            //Base quotes
            for (int I = 0; I < ArrayBaseQuoteSetCount; I++)
            {
                ArrayBaseQuoteSet[I] = new QuoteSet(BR);
            }

            #endregion

            FS.Close();
            BR.Close();

            if (Content != null)
            {
                string FinalSpriteMapPath = "/Map Sprites/" + CharacterPath;
                if (!string.IsNullOrEmpty(SpriteMapPath))
                    FinalSpriteMapPath = "/Map Sprites/" + SpriteMapPath;

                string FinalSpriteUnitPath = "/Shop Sprites/Characters/" + CharacterPath;
                if (!string.IsNullOrEmpty(SpriteShopPath))
                    FinalSpriteUnitPath = "/Shop Sprites/Characters/" + SpriteShopPath;

                string UnitDirectory = Path.GetDirectoryName("Content/Sorcerer Street/");
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + FinalSpriteMapPath + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + FinalSpriteMapPath);
                else
                    SpriteMap = Content.Load<Texture2D>("Deathmatch/Units/Default");

                if (File.Exists(UnitDirectory + FinalSpriteUnitPath + ".xnb"))
                    SpriteShop = Content.Load<Texture2D>(XNADirectory + FinalSpriteUnitPath);
                else
                    SpriteShop = Content.Load<Texture2D>("Deathmatch/Units/Default");

                if (!string.IsNullOrEmpty(Model3DPath))
                {
                    string[] ArrayModelFolder = Model3DPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    string ModelFolder = Model3DPath.Substring(0, Model3DPath.Length - ArrayModelFolder[ArrayModelFolder.Length - 1].Length);
                    Unit3DModel = new AnimatedModel("Sorcerer Street/Models/Characters/" + Model3DPath);
                    Unit3DModel.LoadContent(Content);

                    Unit3DModel.AddAnimation("Sorcerer Street/Models/Characters/" + ModelFolder + "Walking", "Walking", Content);
                    Unit3DModel.AddAnimation("Sorcerer Street/Models/Characters/" + ModelFolder + "Idle", "Idle", Content);
                    Unit3DModel.DisableLights();
                }
            }
        }
    }
}
