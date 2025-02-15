using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CardBook
    {
        private static CardBook AllCardsBook;

        public string BookName;
        public string BookModel;
        public DateTime LastModification;
        public string Tags;

        public int Wins;
        public int Matches;

        public List<Card> ListCard;
        public Dictionary<string, Dictionary<string, Card>> DicCardsByType;

        public int UniqueCreaturesNeutral;
        public int UniqueCreaturesFire;
        public int UniqueCreaturesWater;

        public int UniqueCreaturesEarth;
        public int UniqueCreaturesAir;
        public int UniqueCreaturesMulti;

        public int UniqueItemsWeapon;
        public int UniqueItemsArmor;
        public int UniqueItemsTool;
        public int UniqueItemsScroll;

        public int UniqueSpellsSingle;
        public int UniqueSpellsMultiple;

        public int UniqueEnchantSingle;
        public int UniqueEnchantMultiple;

        public int TotalCreaturesNeutral;
        public int TotalCreaturesFire;
        public int TotalCreaturesWater;
        public int TotalCreaturesEarth;
        public int TotalCreaturesAir;
        public int TotalCreaturesMulti;

        public int TotalItemsWeapon;
        public int TotalItemsArmor;
        public int TotalItemsTool;
        public int TotalItemsScroll;

        public int TotalSpellsSingle;
        public int TotalSpellsMultiple;

        public int TotalEnchantSingle;
        public int TotalEnchantMultiple;

        public int TotalCards;

        public CardBook()
            : this("New Book")
        {
        }

        public CardBook(string BookName)
        {
            this.BookName = BookName;
            BookModel = string.Empty;
            Tags = string.Empty;
            ListCard = new List<Card>();
            DicCardsByType = new Dictionary<string, Dictionary<string, Card>>();
        }

        public CardBook(BinaryReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Tags = string.Empty;
            DicCardsByType = new Dictionary<string, Dictionary<string, Card>>();

            BookName = BR.ReadString();
            BookModel = BR.ReadString();
            LastModification = DateTime.FromFileTimeUtc(BR.ReadInt64());

            Wins = BR.ReadInt32();
            Matches = BR.ReadInt32();

            int ListCardCount = BR.ReadInt32();
            ListCard = new List<Card>(ListCardCount);
            for (int C = 0; C < ListCardCount; ++C)
            {
                Card LoadedCard = Card.FromType(BR.ReadString(), BR.ReadString(), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCard.QuantityOwned = BR.ReadInt32();
                AddCard(LoadedCard);
            }
        }

        public CardBook(BinaryReader BR, CardBook GlobalBook, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Tags = string.Empty;
            DicCardsByType = new Dictionary<string, Dictionary<string, Card>>();

            BookName = BR.ReadString();
            BookModel = BR.ReadString();
            LastModification = DateTime.FromFileTimeUtc(BR.ReadInt64());

            Wins = BR.ReadInt32();
            Matches = BR.ReadInt32();

            int ListCardCount = BR.ReadInt32();
            ListCard = new List<Card>(ListCardCount);
            for (int C = 0; C < ListCardCount; ++C)
            {
                string CardType = BR.ReadString();
                string CardPath = BR.ReadString();
                int QuantityOwned = BR.ReadInt32();
                Card CopyCard = GlobalBook.DicCardsByType[CardType][CardPath].Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                CopyCard.QuantityOwned = QuantityOwned;
                AddCard(CopyCard);
            }
        }

        public CardBook(ByteReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Tags = string.Empty;
            DicCardsByType = new Dictionary<string, Dictionary<string, Card>>();

            BookName = BR.ReadString();
            BookModel = BR.ReadString();
            LastModification = DateTime.FromFileTimeUtc(BR.ReadInt64());

            Wins = BR.ReadInt32();
            Matches = BR.ReadInt32();

            int ListCardCount = BR.ReadInt32();
            ListCard = new List<Card>(ListCardCount);
            for (int C = 0; C < ListCardCount; ++C)
            {
                Card LoadedCard = Card.FromType(BR.ReadString(), BR.ReadString(), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                LoadedCard.QuantityOwned = BR.ReadInt32();
                AddCard(LoadedCard);
            }
        }

        public CardBook(ByteReader BR, CardBook GlobalBook, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Tags = string.Empty;
            DicCardsByType = new Dictionary<string, Dictionary<string, Card>>();

            BookName = BR.ReadString();
            BookModel = BR.ReadString();
            LastModification = DateTime.FromFileTimeUtc(BR.ReadInt64());

            Wins = BR.ReadInt32();
            Matches = BR.ReadInt32();

            int ListCardCount = BR.ReadInt32();
            ListCard = new List<Card>(ListCardCount);
            for (int C = 0; C < ListCardCount; ++C)
            {
                string CardType = BR.ReadString();
                string CardPath = BR.ReadString();
                int QuantityOwned = BR.ReadInt32();
                Card CopyCard = GlobalBook.DicCardsByType[CardType][CardPath].Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                CopyCard.QuantityOwned = QuantityOwned;
                AddCard(CopyCard);
            }
        }

        public static CardBook GetCardBook(string BookPath, CardBook GlobalBook, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            FileStream FS = new FileStream("Content/Sorcerer Street/Books/" + BookPath + ".peb", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            CardBook NewBook = new CardBook(BR, GlobalBook, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            BR.Close();
            FS.Close();

            return NewBook;
        }

        public static CardBook LoadGlobalBook()
        {
            if (AllCardsBook == null)
            {
                AllCardsBook = new CardBook("Global Book");

                foreach (string ActiveCardsFolder in Directory.EnumerateDirectories(GameScreen.ContentFallback.RootDirectory + "/Sorcerer Street/", "* Cards"))
                {
                    foreach (string ActiveRootFolder in Directory.EnumerateDirectories(ActiveCardsFolder, "*", SearchOption.AllDirectories))
                    {
                        foreach (string ActiveGameFolder in Directory.EnumerateDirectories(ActiveRootFolder, "*", SearchOption.AllDirectories))
                        {
                            foreach (string ActiveFile in Directory.EnumerateFiles(ActiveGameFolder, "*.pec", SearchOption.AllDirectories))
                            {
                                Card LoadedCard = Card.LoadCard(ActiveFile.Remove(ActiveFile.Length - 4, 4).Remove(0, 24).Replace('\\', '/'), GameScreen.ContentFallback,
                                    SorcererStreetBattleParams.DicParams[string.Empty].DicRequirement, SorcererStreetBattleParams.DicParams[string.Empty].DicEffect, SorcererStreetBattleParams.DicParams[string.Empty].DicAutomaticSkillTarget, SorcererStreetBattleParams.DicParams[string.Empty].DicManualSkillTarget);
                                LoadedCard.QuantityOwned = 1;

                                AllCardsBook.AddCard(LoadedCard);
                            }
                        }
                    }
                }
            }

            return AllCardsBook;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(BookName);
            BW.Write(BookModel);
            BW.Write(LastModification.Ticks);
            BW.Write(Wins);
            BW.Write(Matches);

            BW.Write(ListCard.Count);
            for (int C = 0; C < ListCard.Count; ++C)
            {
                BW.Write(ListCard[C].CardType);
                BW.Write(ListCard[C].Path);
                BW.Write(ListCard[C].QuantityOwned);
            }
        }

        public void AddCard(Card CardToAdd)
        {
            TotalCards += CardToAdd.QuantityOwned;
            ListCard.Add(CardToAdd);

            if (!DicCardsByType.ContainsKey(CardToAdd.CardType))
                DicCardsByType.Add(CardToAdd.CardType, new Dictionary<string, Card>());

            DicCardsByType[CardToAdd.CardType].Add(CardToAdd.Path, CardToAdd);

            CreatureCard CreatureCardToAdd = CardToAdd as CreatureCard;
            ItemCard ItemCardToAdd = CardToAdd as ItemCard;
            SpellCard SpellCardToAdd = CardToAdd as SpellCard;
            if (CreatureCardToAdd != null)
            {
                CardAbilities Abilities = CreatureCardToAdd.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None);
                if (Abilities.ArrayElementAffinity.Length > 1)
                {
                    ++UniqueCreaturesMulti;
                    TotalCreaturesMulti += CardToAdd.QuantityOwned;
                }
                else if (Abilities.ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Neutral)
                {
                    ++UniqueCreaturesNeutral;
                    TotalCreaturesNeutral += CardToAdd.QuantityOwned;
                }
                else if (Abilities.ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Fire)
                {
                    ++UniqueCreaturesFire;
                    TotalCreaturesFire += CardToAdd.QuantityOwned;
                }
                else if (Abilities.ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Water)
                {
                    ++UniqueCreaturesWater;
                    TotalCreaturesWater += CardToAdd.QuantityOwned;
                }
                else if (Abilities.ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Earth)
                {
                    ++UniqueCreaturesEarth;
                    TotalCreaturesEarth += CardToAdd.QuantityOwned;
                }
                else if (Abilities.ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Air)
                {
                    ++UniqueCreaturesAir;
                    TotalCreaturesAir += CardToAdd.QuantityOwned;
                }
                else
                {
                }
            }
            else if (ItemCardToAdd != null)
            {
                if (ItemCardToAdd.ItemType == ItemCard.ItemTypes.Weapon)
                {
                    ++UniqueItemsWeapon;
                    TotalItemsWeapon += CardToAdd.QuantityOwned;
                }
                else if (ItemCardToAdd.ItemType == ItemCard.ItemTypes.Armor)
                {
                    ++UniqueItemsArmor;
                    TotalItemsArmor += CardToAdd.QuantityOwned;
                }
                else if (ItemCardToAdd.ItemType == ItemCard.ItemTypes.Tools)
                {
                    ++UniqueItemsTool;
                    TotalItemsTool += CardToAdd.QuantityOwned;
                }
                else if (ItemCardToAdd.ItemType == ItemCard.ItemTypes.Scrolls)
                {
                    ++UniqueItemsScroll;
                    TotalItemsScroll += CardToAdd.QuantityOwned;
                }
                else
                {
                }
            }
            else
            {
                if (SpellCardToAdd.SpellType == SpellCard.SpellTypes.SingleFlash)
                {
                    ++UniqueSpellsSingle;
                    TotalSpellsSingle += CardToAdd.QuantityOwned;
                }
                else if (SpellCardToAdd.SpellType == SpellCard.SpellTypes.MultiFlash)
                {
                    ++UniqueSpellsMultiple;
                    TotalSpellsMultiple += CardToAdd.QuantityOwned;
                }
                else if (SpellCardToAdd.SpellType == SpellCard.SpellTypes.SingleEnchant)
                {
                    ++UniqueEnchantSingle;
                    TotalEnchantSingle += CardToAdd.QuantityOwned;
                }
                else if (SpellCardToAdd.SpellType == SpellCard.SpellTypes.MultiEnchant)
                {
                    ++UniqueEnchantMultiple;
                    TotalEnchantMultiple += CardToAdd.QuantityOwned;
                }
                else
                {
                }
            }
        }
    }
}
