using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.Core.Operators;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;
using static ProjectEternity.GameScreens.SorcererStreetScreen.ActionPanelBattleAttackPhase;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CardAbilities
    {
        public bool AttackFirst;
        public bool AttackLast;
        public bool AttackTwice;
        public bool CriticalHit;
        public float DamageMultiplier;
        public int DamageModifier;

        public bool IsDefensive;//Can't move
        public bool FreeTravel;//Target creature can travel to any vacant land within the area when moving.

        public bool SupportCreature;//Can use other creatures as items
        public bool ItemCreature;//Can be used as an item
        public bool Immediate;//Allow all territory command after taking a land (either vacant or after a battle)
        public bool ForceStop;//Forces any Player that steps onto target territory to stop.
        public bool Regenerate;//Regain max HP after battle

        public bool ItemProtection;//Immune to Destroy Item and Steal Item effects.
        public bool TargetProtection;//Cannot be targeted by spells or territory abilities.
        public bool InvasionProtection;//Target territory cannot be invaded.

        public bool HPProtection;//HP & MHP cannot be altered by spells or territory abilities.
        public bool Recycle;//Return to hand.
        public bool LapRegenerationLimit;//Disable Lap Regeneration

        public int TollOverride;

        public bool Paralysis;//Creature is unable to use items and abilities. Note: Creature does still receive Boost and Global Ability effects from other creatures.
        public bool EffectLimit;//No effects or abilities can be used during battle
        public bool LandEffectLimit;//Target creature cannot receive land effect.
        public bool LandEffectNoLimit;//Target creature always receive land effect.
        public bool LandLevelLock;//Cannot change the level of a land.
        public bool LandLevelDowngradeLock;//Territory level cannot be lowered.

        public ElementalAffinity[] ArrayElementAffinity;
        public ElementalAffinity[] ArrayPenetrateAffinity;//HP from Land Bonus is negated, attack with creature ST

        public bool ScrollAttack;//HP from Land Bonus is negated, attack with scroll, can't be reflected or negated
        public bool ScrollCriticalHit;
        public byte ScrollValue;

        public List<AttackTypes> ListNeutralizeType;
        public NumberTypes NeutralizeSignOperator;
        public string NeutralizeValue;

        public List<AttackTypes> ListReflectType;
        public NumberTypes ReflectSignOperator;
        public string ReflectValue;

        public CardAbilities()
        {
            AttackFirst = false;
            AttackLast = false;
            AttackTwice = false;
            CriticalHit = false;
            DamageMultiplier = 1f;
            DamageModifier = -1;

            IsDefensive = false;
            FreeTravel = false;

            SupportCreature = false;
            ItemCreature = false;
            Immediate = false;
            ForceStop = false;
            Regenerate = false;

            ItemProtection = false;
            TargetProtection = false;
            InvasionProtection = false;

            HPProtection = false;
            Recycle = false;
            LapRegenerationLimit = false;

            TollOverride = -1;

            Paralysis = false;
            EffectLimit = false;
            LandEffectLimit = false;
            LandEffectNoLimit = false;
            LandLevelLock = false;
            LandLevelDowngradeLock = false;

            ArrayElementAffinity = new ElementalAffinity[0];
            ArrayPenetrateAffinity = new ElementalAffinity[0];

            ScrollAttack = false;
            ScrollCriticalHit = false;
            ScrollValue = 0;

            ListNeutralizeType = new List<AttackTypes>();
            NeutralizeSignOperator = NumberTypes.Absolute;
            NeutralizeValue = string.Empty;

            ListReflectType = new List<AttackTypes>();
            ReflectSignOperator = NumberTypes.Absolute;
            ReflectValue = string.Empty;
        }

        public CardAbilities(CardAbilities Other)
        {
            AttackFirst = Other.AttackFirst;
            AttackLast = Other.AttackLast;
            AttackTwice = Other.AttackTwice;
            CriticalHit = Other.CriticalHit;
            DamageMultiplier = Other.DamageMultiplier;
            DamageModifier = Other.DamageModifier;

            IsDefensive = Other.IsDefensive;
            FreeTravel = Other.FreeTravel;

            SupportCreature = Other.SupportCreature;
            ItemCreature = Other.ItemCreature;
            Immediate = Other.Immediate;
            ForceStop = Other.ForceStop;
            Regenerate = Other.Regenerate;

            ItemProtection = Other.ItemProtection;
            TargetProtection = Other.TargetProtection;
            InvasionProtection = Other.InvasionProtection;

            HPProtection = Other.HPProtection;
            Recycle = Other.Recycle;
            LapRegenerationLimit = Other.LapRegenerationLimit;

            TollOverride = Other.TollOverride;

            Paralysis = Other.Paralysis;
            EffectLimit = Other.EffectLimit;
            LandEffectLimit = Other.LandEffectLimit;
            LandEffectNoLimit = Other.LandEffectNoLimit;
            LandLevelLock = Other.LandLevelLock;
            LandLevelDowngradeLock = Other.LandLevelDowngradeLock;

            ArrayPenetrateAffinity = Other.ArrayPenetrateAffinity;
            ArrayElementAffinity = Other.ArrayElementAffinity;

            ScrollAttack = Other.ScrollAttack;
            ScrollCriticalHit = Other.ScrollCriticalHit;
            ScrollValue = Other.ScrollValue;

            ListNeutralizeType = new List<AttackTypes>();
            NeutralizeSignOperator = NumberTypes.Relative;
            NeutralizeValue = null;

            ListReflectType = new List<AttackTypes>();
            ReflectSignOperator = NumberTypes.Relative;
            ReflectValue = null;
        }
    }

    public class CardSymbols
    {
        public Texture2D sprRarityE;
        public Texture2D sprRarityN;
        public Texture2D sprRarityR;
        public Texture2D sprRarityS;

        public Texture2D sprMenuG;
        public Texture2D sprMenuTG;
        public Texture2D sprMenuST;
        public Texture2D sprMenuHP;
        public Texture2D sprMenuMHP;

        public Texture2D sprElementAir;
        public Texture2D sprElementEarth;
        public Texture2D sprElementFire;
        public Texture2D sprElementWater;
        public Texture2D sprElementNeutral;
        public Texture2D sprElementMulti;

        public Texture2D sprItemsWeapon;
        public Texture2D sprItemsArmor;
        public Texture2D sprItemsTool;
        public Texture2D sprItemsScroll;

        public Texture2D sprSpellsSingle;
        public Texture2D sprSpellsMultiple;

        public Texture2D sprEnchantSingle;
        public Texture2D sprEnchantMultiple;

        public Texture2D sprCreature;

        public static CardSymbols Symbols;

        public static void Load(ContentManager Content)
        {
            Symbols = new CardSymbols();

            Symbols.sprElementAir = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Air");
            Symbols.sprElementEarth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Earth");
            Symbols.sprElementFire = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Fire");
            Symbols.sprElementWater = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Water");
            Symbols.sprElementNeutral = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Neutral");
            Symbols.sprElementMulti = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Multi");

            Symbols.sprMenuG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/G");
            Symbols.sprMenuTG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/TG");
            Symbols.sprMenuST = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/ST");
            Symbols.sprMenuHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/HP");
            Symbols.sprMenuMHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/MHP");

            Symbols.sprRarityE = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Rare");
            Symbols.sprRarityN = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Normal");
            Symbols.sprRarityR = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Rare");
            Symbols.sprRarityS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Strange");

            Symbols.sprItemsWeapon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Weapon");
            Symbols.sprItemsArmor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Armor");
            Symbols.sprItemsTool = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Tool");
            Symbols.sprItemsScroll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Scroll");

            Symbols.sprSpellsSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Spell single");
            Symbols.sprSpellsMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Spell Multiple");

            Symbols.sprEnchantSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Enchant Single");
            Symbols.sprEnchantMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Enchant Multiple");

            Symbols.sprCreature = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Creature");
        }
    }

    public class CardInfo
    {
        public Card Card;
        public Card CardSkin;
        public byte QuantityOwned;
        public int SelectedSkinIndex;//if higher than ListOwnedCardSkin, use ListOwnedCardAlt
        public List<CardSkinInfo> ListOwnedCardSkin;
        public List<CardAltInfo> ListOwnedCardAlt;//Alts have different stats

        public CardInfo(Card Card, byte QuantityOwned)
        {
            this.Card = Card;
            this.CardSkin = Card;
            this.QuantityOwned = QuantityOwned;
            ListOwnedCardSkin = new List<CardSkinInfo>();
            ListOwnedCardAlt = new List<CardAltInfo>();
            SelectedSkinIndex = -1;
        }

        public CardInfo(BinaryReader BR, Card LoadedCard)
        {
            QuantityOwned = BR.ReadByte();

            byte ListOwnedCardSkinCount = BR.ReadByte();
            ListOwnedCardSkin = new List<CardSkinInfo>(ListOwnedCardSkinCount);
            for (int S = 0; S < ListOwnedCardSkinCount; S++)
            {
                string ReplacementName = BR.ReadString();
                string TextureRelativePath = BR.ReadString();
                string ModelRelativePath = BR.ReadString();
                bool UserCreated = BR.ReadBoolean();
                ListOwnedCardSkin.Add(new CardSkinInfo(ReplacementName, TextureRelativePath, ModelRelativePath, null, UserCreated));
            }

            byte ListOwnedCardAltCount = BR.ReadByte();
            ListOwnedCardAlt = new List<CardAltInfo>(ListOwnedCardAltCount);
            for (int A = 0; A < ListOwnedCardAltCount; A++)
            {
                string CardRelativePath = BR.ReadString();
                string SkinRelativePath = BR.ReadString();
                ListOwnedCardAlt.Add(new CardAltInfo(CardRelativePath, SkinRelativePath, null));
            }

            Card = LoadedCard;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Card.CardType);
            BW.Write(Card.Path);
            BW.Write(QuantityOwned);

            BW.Write((byte)ListOwnedCardSkin.Count);
            foreach (CardSkinInfo ActiveSkin in ListOwnedCardSkin)
            {
                BW.Write(ActiveSkin.ReplacementName);
                BW.Write(ActiveSkin.TextureRelativePath);
                BW.Write(ActiveSkin.ModelRelativePath);
                BW.Write(ActiveSkin.UserCreated);
            }

            BW.Write((byte)ListOwnedCardAlt.Count);
            foreach (CardAltInfo ActiveAlt in ListOwnedCardAlt)
            {
                BW.Write(ActiveAlt.CardRelativePath);
                BW.Write(ActiveAlt.SkinRelativePath);
            }
        }
    }

    public class CardSkinInfo
    {
        public string ReplacementName;
        public string TextureRelativePath;
        public string ModelRelativePath;
        public readonly bool UserCreated;//Allow deletion of user created content
        public Card CardSkin;

        public CardSkinInfo(string ReplacementName, string TextureRelativePath, string ModelRelativePath, Card CardSkin, bool UserCreated)
        {
            this.ReplacementName = ReplacementName;
            this.TextureRelativePath = TextureRelativePath;
            this.ModelRelativePath = ModelRelativePath;
            this.UserCreated = UserCreated;
            this.CardSkin = CardSkin;
        }

        public override string ToString()
        {
            return ReplacementName;
        }
    }

    public class CardAltInfo
    {
        public string CardRelativePath;
        public string SkinRelativePath;
        public Card CardSkin;

        public CardAltInfo(string CardRelativePath, string SkinRelativePath, Card CardSkin)
        {
            this.CardRelativePath = CardRelativePath;
            this.SkinRelativePath = SkinRelativePath;
            this.CardSkin = CardSkin;
        }

        public override string ToString()
        {
            return SkinRelativePath;
        }
    }

    public abstract class Card
    {
        private static readonly Dictionary<string, Dictionary<string, Card>> DicCardsByType = new Dictionary<string, Dictionary<string, Card>>();

        public enum CardRarities { Normal, Strange, Rare, Extra }

        public Texture2D sprCard;

        public readonly string Path;
        public readonly string CardType;
        public string Name;
        public string Description;
        public static DynamicText TextParser;
        public DynamicText DescriptionText;
        public string FlavorText = string.Empty;
        public int ShopPrice;
        public CardRarities Rarity;
        public int MagicCost;

        public string Tags;//Used to categorize Characters
        public TagSystem TeamTags;
        public EffectHolder Effects;
        public string SkillChainName;
        public List<BaseAutomaticSkill> ListActiveSkill;//Passive abilities
        public List<string> ListSpellName;
        public List<string> ListSpellActivationAnimationPath;
        public List<ManualSkill> ListSpell;

        public const int BoxWidth = 620;

        public Card()
            : this("None", "None")
        {
        }

        protected Card(string Path, string CardType)
        {
            this.Path = Path;
            this.CardType = CardType;
            TeamTags = new TagSystem();
            Effects = new EffectHolder();
            ListActiveSkill = new List<BaseAutomaticSkill>();
        }

        public static void Init()
        {
            DicCardsByType.Add(CreatureCard.CreatureCardType, new Dictionary<string, Card>());
            DicCardsByType.Add(ItemCard.ItemCardType, new Dictionary<string, Card>());
            DicCardsByType.Add(SpellCard.SpellCardType, new Dictionary<string, Card>());
        }

        public static Card LoadCard(string Path)
        {
            return LoadCard(Path, null, null, null, null, null);
        }

        public static Card LoadCard(string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            string[] UnitInfo = Path.Split(new[] { "/", "\\" }, StringSplitOptions.None);

            return FromType(UnitInfo[0], Path.Remove(0, UnitInfo[0].Length + 1), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
        }

        public static Card FromType(string CardType, string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Card LoadedCard = null;

            switch (CardType)
            {
                case "Creature":
                case "Creature Cards":
                    if (!DicCardsByType[CreatureCard.CreatureCardType].TryGetValue(Path, out LoadedCard))
                    {
                        LoadedCard = new CreatureCard(Path, Content, DicRequirement, DicEffects, DicAutomaticSkillTarget, DicManualSkillTarget);
                        DicCardsByType[CreatureCard.CreatureCardType].Add(Path, LoadedCard);
                    }
                    break;

                case "Item":
                case "Item Cards":
                    if (!DicCardsByType[ItemCard.ItemCardType].TryGetValue(Path, out LoadedCard))
                    {
                        LoadedCard = new ItemCard(Path, Content, DicRequirement, DicEffects, DicAutomaticSkillTarget, DicManualSkillTarget);
                        DicCardsByType[ItemCard.ItemCardType].Add(Path, LoadedCard);
                    }
                    break;

                case "Spell Cards":
                case SpellCard.SpellCardType:
                    if (!DicCardsByType[SpellCard.SpellCardType].TryGetValue(Path, out LoadedCard))
                    {
                        LoadedCard = new SpellCard(Path, Content, DicRequirement, DicEffects, DicAutomaticSkillTarget, DicManualSkillTarget);
                        DicCardsByType[SpellCard.SpellCardType].Add(Path, LoadedCard);
                    }
                    break;
            }

            return LoadedCard.Copy(DicRequirement, DicEffects, DicAutomaticSkillTarget, DicManualSkillTarget);
        }

        public void InitSkillChainTarget(BaseAutomaticSkill ActiveSkill, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            foreach (BaseSkillLevel ActiveSkillLevel in ActiveSkill.ListSkillLevel)
            {
                foreach (BaseSkillActivation ActiveSkillActivation in ActiveSkillLevel.ListActivation)
                {
                    for (int E = 0; E < ActiveSkillActivation.ListEffect.Count; ++E)
                    {
                        if (ActiveSkillActivation.ListEffect[E] is SorcererStreetEffect)
                        {
                            ActiveSkillActivation.ListEffectTarget[E].Add("Sorcerer Street Self");
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(DicAutomaticSkillTarget["Sorcerer Street Self"]);
                        }

                        foreach (BaseAutomaticSkill ActiveFollowingSkill in ActiveSkillActivation.ListEffect[E].ListFollowingSkill)
                        {
                            InitSkillChainTarget(ActiveFollowingSkill, DicAutomaticSkillTarget);
                        }
                    }
                }
            }
        }

        public static void InitTextParser(SorcererStreetMap Map, ContentManager Content, CardSymbols Symbols, SpriteFont fntMenuText, Color TextColor)
        {
            Card.TextParser = new DynamicText();
            Card.TextParser.TextMaxWidthInPixel = Card.BoxWidth - 100;
            Card.TextParser.LineHeight = fntMenuText.LineSpacing;
            Card.TextParser.TextColor = TextColor;
            Card.TextParser.ListProcessor.Add(new RegularTextProcessor(Card.TextParser, fntMenuText));
            IconProcessor IconParser = new IconProcessor(Card.TextParser);
            Card.TextParser.ListProcessor.Add(IconParser);
            Card.TextParser.ListProcessor.Add(new PlayerNameProcessor(Card.TextParser, fntMenuText, Map));
            Card.TextParser.ListProcessor.Add(new DefaultTextProcessor(Card.TextParser, fntMenuText));
            Card.TextParser.SetDefaultProcessor(new DefaultTextProcessor(Card.TextParser, fntMenuText));
            Card.TextParser.Load(Content);

            IconParser.PreloadImage("rarityE", Symbols.sprRarityE);
            IconParser.PreloadImage("rarityN", Symbols.sprRarityN);
            IconParser.PreloadImage("rarityR", Symbols.sprRarityR);
            IconParser.PreloadImage("rarityS", Symbols.sprRarityS);

            IconParser.PreloadImage("MenuG", Symbols.sprMenuG);
            IconParser.PreloadImage("MenuTG", Symbols.sprMenuTG);
            IconParser.PreloadImage("MenuST", Symbols.sprMenuST);
            IconParser.PreloadImage("MenuHP", Symbols.sprMenuHP);
            IconParser.PreloadImage("MenuMHP", Symbols.sprMenuMHP);

            IconParser.PreloadImage("water", Symbols.sprElementWater);
            IconParser.PreloadImage("fire", Symbols.sprElementFire);
            IconParser.PreloadImage("earth", Symbols.sprElementEarth);
            IconParser.PreloadImage("air", Symbols.sprElementAir);
            IconParser.PreloadImage("neutral", Symbols.sprElementNeutral);
            IconParser.PreloadImage("multi", Symbols.sprElementMulti);

            IconParser.PreloadImage("weapon", Symbols.sprItemsWeapon);
            IconParser.PreloadImage("armor", Symbols.sprItemsArmor);
            IconParser.PreloadImage("tool", Symbols.sprItemsTool);
            IconParser.PreloadImage("scroll", Symbols.sprItemsScroll);

            IconParser.PreloadImage("spellsingle", Symbols.sprSpellsSingle);
            IconParser.PreloadImage("spellmultiple", Symbols.sprSpellsMultiple);

            IconParser.PreloadImage("enchantsingle", Symbols.sprEnchantSingle);
            IconParser.PreloadImage("enchantmultiple", Symbols.sprEnchantMultiple);

            IconParser.PreloadImage("creature", Symbols.sprCreature);
        }

        public Card Copy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Card NewCopy = DoCopy(DicRequirement, DicEffects, DicAutomaticSkillTarget, DicManualSkillTarget);

            NewCopy.Description = Description;
            NewCopy.Rarity = Rarity;
            NewCopy.MagicCost = MagicCost;

            if (TextParser != null)
            {
                NewCopy.DescriptionText = TextParser.Copy();
            }

            return NewCopy;
        }

        public abstract Card DoCopy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget);

        public Dictionary<BaseAutomaticSkill, List<BaseSkillActivation>> GetAvailableActivation(string RequirementName)
        {
            Dictionary<BaseAutomaticSkill, List<BaseSkillActivation>> DicSkillActivation = new Dictionary<BaseAutomaticSkill, List<BaseSkillActivation>>();

            foreach (BaseAutomaticSkill ActiveSkill in ListActiveSkill)
            {
                List<BaseSkillActivation> ListAvailableActivation = ActiveSkill.GetAvailableActivation(RequirementName);
                if (ListAvailableActivation != null && ListAvailableActivation.Count > 0)
                {
                    DicSkillActivation.Add(ActiveSkill, ListAvailableActivation);
                }
            }

            return DicSkillActivation;
        }

        public void DrawCard(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 720f;
            //Draw Card on left
            g.Draw(sprCard, new Vector2(450, 70), new Rectangle(0, 0, sprCard.Width, sprCard.Height), Color.White,
                0f, new Vector2(sprCard.Width / 2, 0), Ratio, SpriteEffects.None, 0f);
        }

        public abstract List<Texture2D> GetIcons(CardSymbols Symbols);

        public virtual void DrawCardInfo(CustomSpriteBatch g, CardSymbols Symbols, SpriteFont fntCardInfo, Player ActivePlayer, float OffsetX, float OffsetY)
        {
            int BoxHeight = 610;
            float InfoBoxX = Constants.Width - BoxWidth - 50 + OffsetX;
            float InfoBoxY = 106 + OffsetY;
            int IconWidth = 17;
            int IconHeight = 32;

            MenuHelper.DrawNamedBox(g, CardType, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight);

            float CurrentX = InfoBoxX + 50;
            float CurrentY = InfoBoxY + 10;

            g.DrawString(fntCardInfo, Name, new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);
            switch (Rarity)
            {
                case CardRarities.Normal:
                    g.Draw(Symbols.sprRarityN, new Vector2((int)InfoBoxX + BoxWidth - 60, (int)CurrentY), Color.White);
                    break;

                case CardRarities.Strange:
                    g.Draw(Symbols.sprRarityS, new Vector2((int)InfoBoxX + BoxWidth - 60, (int)CurrentY), Color.White);
                    break;

                case CardRarities.Rare:
                    g.Draw(Symbols.sprRarityR, new Vector2((int)InfoBoxX + BoxWidth - 60, (int)CurrentY), Color.White);
                    break;

                case CardRarities.Extra:
                    g.Draw(Symbols.sprRarityE, new Vector2((int)InfoBoxX + BoxWidth - 60, (int)CurrentY), Color.White);
                    break;
            }

            CurrentY += 40;

            g.DrawString(fntCardInfo, CardType, new Vector2(CurrentX, CurrentY), SorcererStreetMap.TextColor);

            CurrentY += Constants.Height / 24;

            CurrentY += 40;
            
            g.Draw(Symbols.sprMenuG, new Rectangle((int)CurrentX, (int)CurrentY, IconWidth, IconHeight), Color.White);
            g.DrawStringVerticallyAligned(fntCardInfo, ActivePlayer.GetFinalCardCost(this).ToString(), new Vector2(CurrentX + 20, CurrentY + IconHeight / 2 + 2), SorcererStreetMap.TextColor);

            CurrentY += Constants.Height / 24;

            if (!DescriptionText.IsInit)
            {
                DescriptionText.ParseText(Description);
            }

            DescriptionText.Draw(g, new Vector2(CurrentX, CurrentY));
        }

        public static void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCardFront, Texture2D sprCardBack, Color CardFrontColor,
            float X, float Y, float ScaleX, float ScaleY, bool IsFaceDown)
        {
            int FinalCardWidth = (int)(Constants.Width / 4.70588255);
            int FinalCardHeight = (int)(Constants.Height / 2.11764717);

            if (IsFaceDown)
            {
                g.Draw(sprCardBack, new Rectangle((int)X, (int)Y, (int)(FinalCardWidth * ScaleX), (int)(FinalCardHeight * ScaleY)), new Rectangle(0, 0, sprCardBack.Width, sprCardBack.Height),
                    CardFrontColor, 0, new Vector2(sprCardBack.Width / 2, 0), SpriteEffects.None, 0f);
            }
            else
            {
                g.Draw(sprCardFront, new Rectangle((int)X, (int)Y, (int)(FinalCardWidth * -ScaleX), (int)(FinalCardHeight * ScaleY)), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height),
                    CardFrontColor, 0, new Vector2(sprCardFront.Width / 2, 0), SpriteEffects.None, 0f);
            }
        }

        public static void DrawCardMiniatureCentered(CustomSpriteBatch g, Texture2D sprCardFront, Texture2D sprCardBack, Color CardFrontColor,
            float X, float Y, float ScaleX, float ScaleY, bool IsFaceDown)
        {
            int FinalCardWidth = (int)(Constants.Width / 4.70588255);
            int FinalCardHeight = (int)(Constants.Height / 2.11764717);

            if (IsFaceDown)
            {
                g.Draw(sprCardBack, new Rectangle((int)X, (int)Y, (int)(FinalCardWidth * ScaleX), (int)(FinalCardHeight * ScaleY)), new Rectangle(0, 0, sprCardBack.Width, sprCardBack.Height),
                    CardFrontColor, 0, new Vector2(sprCardBack.Width / 2, sprCardBack.Height / 2), SpriteEffects.None, 0f);
            }
            else
            {
                g.Draw(sprCardFront, new Rectangle((int)X, (int)Y, (int)(FinalCardWidth * -ScaleX), (int)(FinalCardHeight * ScaleY)), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height),
                    CardFrontColor, 0, new Vector2(sprCardFront.Width / 2, sprCardFront.Height / 2), SpriteEffects.None, 0f);
            }
        }

        public virtual ActionPanelSorcererStreet ActivateOnMap(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            return null;
        }

        public virtual ActionPanelSorcererStreet ActivateInBattle(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            return null;
        }
    }
}
