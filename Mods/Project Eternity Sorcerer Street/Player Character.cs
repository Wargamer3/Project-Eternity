﻿using System;
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

    public class PlayerCharacterSkin
    {
        public string SkinPath;
        public bool Locked;
        public PlayerCharacter Skin;

        public PlayerCharacterSkin(string SkinPath, bool Locked)
        {
            this.SkinPath = SkinPath;
            this.Locked = Locked;
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
        public List<PlayerCharacterSkin> ListSkin;
        public List<string> ListAIBook;

        public ManualSkill[] ArraySpell;
        public BaseAutomaticSkill[] ArraySkill;
        public BaseAutomaticSkill[] ArrayRelationshipBonus;

        public QuoteSet[] ArrayBaseQuoteSet = new QuoteSet[6];
        public Dictionary<string, QuoteSet> DicAttackQuoteSet;
        public List<string> ListQuoteSetVersusName;

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

            byte ListLockedSkinCount = BR.ReadByte();
            ListSkin = new List<PlayerCharacterSkin>(ListLockedSkinCount);
            for (int S = 0; S < ListLockedSkinCount; ++S)
            {
                ListSkin.Add(new PlayerCharacterSkin(BR.ReadString(), BR.ReadBoolean()));
            }

            byte ListAIBookCount = BR.ReadByte();
            ListAIBook = new List<string>(ListAIBookCount);
            for (int S = 0; S < ListAIBookCount; ++S)
            {
                ListAIBook.Add(BR.ReadString());
            }

            int RelationshipBonusCount = BR.ReadInt32();
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

            #region Quotes

            int ListQuoteSetVersusNameCount = BR.ReadInt32();
            ListQuoteSetVersusName = new List<string>(ListQuoteSetVersusNameCount);
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
            DicAttackQuoteSet = new Dictionary<string, QuoteSet>(DicAttackQuoteSetCount);
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

            #endregion

            FS.Close();
            BR.Close();

            if (Content != null)
            {
                string FinalSpriteMapPath = "\\Map Sprites\\" + CharacterPath;
                if (!string.IsNullOrEmpty(SpriteMapPath))
                    FinalSpriteMapPath = "\\Map Sprites\\" + SpriteMapPath;

                string FinalSpriteUnitPath = "\\Shop Sprites\\" + CharacterPath;
                if (!string.IsNullOrEmpty(SpriteShopPath))
                    FinalSpriteUnitPath = "\\Shop Sprites\\" + SpriteShopPath;

                string UnitDirectory = Path.GetDirectoryName("Content\\Sorcerer Street\\");
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + FinalSpriteMapPath + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + FinalSpriteMapPath);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                if (File.Exists(UnitDirectory + FinalSpriteUnitPath + ".xnb"))
                    SpriteShop = Content.Load<Texture2D>(XNADirectory + FinalSpriteUnitPath);
                else
                    SpriteShop = Content.Load<Texture2D>("Units/Default");

                if (!string.IsNullOrEmpty(Model3DPath))
                {
                    string[] ArrayModelFolder = Model3DPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    string ModelFolder = Model3DPath.Substring(0, Model3DPath.Length - ArrayModelFolder[ArrayModelFolder.Length - 1].Length);
                    Unit3DModel = new AnimatedModel("Sorcerer Street/Models/Characters/" + Model3DPath);
                    Unit3DModel.LoadContent(Content);

                    Unit3DModel.AddAnimation("Sorcerer Street/Models/Characters/" + ModelFolder + "Walking", "Walking", Content);
                    Unit3DModel.AddAnimation("Sorcerer Street/Models/Characters/" + ModelFolder + "Idle", "Idle", Content);
                }
            }
        }
    }
}
