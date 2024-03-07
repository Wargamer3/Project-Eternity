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
        public string PortraitPath;

        public QuoteSetMap()
        {
            ListQuoteVersus = new List<QuoteSetVersus>();
            PortraitPath = "";
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

    public class PlayerCharacterSkin
    {
        public string SkinPath;
        public bool Locked;
        public PlayerCharacter Skin;

        public PlayerCharacterSkin(PlayerCharacter Clone)
        {
            SkinPath = Clone.CharacterPath;
            Locked = false;
            Skin = new PlayerCharacter(Clone);
        }

        public PlayerCharacterSkin(string SkinPath)
        {
            this.SkinPath = SkinPath;
            Locked = false;
        }

        public PlayerCharacterSkin(string SkinPath, bool Locked)
        {
            this.SkinPath = SkinPath;
            this.Locked = Locked;
        }

        public override string ToString()
        {
            return SkinPath;
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

        public QuoteSet[] ArrayBaseQuoteSet;
        public List<string> ListQuoteSetMapName;
        public List<string> ListQuoteSetVersusName;

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
            this.ListSkin = new List<PlayerCharacterSkin>(Clone.ListSkin);
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

            byte ListLockedSkinCount = BR.ReadByte();
            ListSkin = new List<PlayerCharacterSkin>(ListLockedSkinCount + 1);
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

            ListSkin.Insert(0, new PlayerCharacterSkin(this));
        }
    }
}
