using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.Core.Operators;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;
using static ProjectEternity.GameScreens.SorcererStreetScreen.ActionPanelBattleAttackPhase;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public struct CardAbilities
    {
        public bool AttackFirst;
        public bool AttackLast;
        public bool AttackTwice;
        public bool CriticalHit;
        public bool IsDefensive;//Can't move
        public bool SupportCreature;//Can use other creatures as items
        public bool ItemCreature;//Can be used as an item
        public bool Immediate;//Allow all territory command after taking a land (either vacant or after a battle)
        public bool Regenerate;//Regain max HP after battle
        public bool ItemProtection;//Immune to Destroy Item and Steal Item effects.
        public bool TargetProtection;//Cannot be targeted by spells or territory abilities.
        public bool HPProtection;//HP & MHP cannot be altered by spells or territory abilities.
        public bool Recycle;

        public ElementalAffinity[] ArrayAffinity;

        public ElementalAffinity[] ArrayPenetrateAffinity;//HP from Land Bonus is negated, attack with creature ST

        public bool ScrollAttack;//HP from Land Bonus is negated, attack with scroll, can't be reflected or negated
        public string ScrollValue;

        public List<AttackTypes> ListNeutralizeType;
        public NumberTypes NeutralizeSignOperator;
        public string NeutralizeValue;

        public List<AttackTypes> ListReflectType;
        public NumberTypes ReflectSignOperator;
        public string ReflectValue;

        public CardAbilities(CardAbilities Other)
        {
            AttackFirst = Other.AttackFirst;
            AttackLast = Other.AttackLast;
            AttackTwice = Other.AttackTwice;
            CriticalHit = Other.CriticalHit;
            IsDefensive = Other.IsDefensive;
            SupportCreature = Other.SupportCreature;
            ItemCreature = Other.ItemCreature;
            Immediate = Other.Immediate;
            Regenerate = Other.Regenerate;
            ItemProtection = Other.ItemProtection;
            TargetProtection = Other.TargetProtection;
            HPProtection = Other.HPProtection;
            Recycle = Other.Recycle;

            ArrayPenetrateAffinity = Other.ArrayPenetrateAffinity;

            ArrayAffinity = Other.ArrayAffinity;

            ScrollAttack = Other.ScrollAttack;
            ScrollValue = null;

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

        public static CardSymbols Load(ContentManager Content)
        {
            CardSymbols NewCardSymbols = new CardSymbols();

            NewCardSymbols.sprElementAir = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Air");
            NewCardSymbols.sprElementEarth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Earth");
            NewCardSymbols.sprElementFire = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Fire");
            NewCardSymbols.sprElementWater = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Water");
            NewCardSymbols.sprElementNeutral = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Neutral");
            NewCardSymbols.sprElementMulti = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Elements/Multi");

            NewCardSymbols.sprMenuG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/G");
            NewCardSymbols.sprMenuTG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/TG");
            NewCardSymbols.sprMenuST = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/ST");
            NewCardSymbols.sprMenuHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/HP");
            NewCardSymbols.sprMenuMHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Text/MHP");

            NewCardSymbols.sprRarityE = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Rare");
            NewCardSymbols.sprRarityN = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Normal");
            NewCardSymbols.sprRarityR = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Rare");
            NewCardSymbols.sprRarityS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Rarity/Strange");

            NewCardSymbols.sprItemsWeapon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Weapon");
            NewCardSymbols.sprItemsArmor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Armor");
            NewCardSymbols.sprItemsTool = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Tool");
            NewCardSymbols.sprItemsScroll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Scroll");

            NewCardSymbols.sprSpellsSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Spell single");
            NewCardSymbols.sprSpellsMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Spell Multiple");

            NewCardSymbols.sprEnchantSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Enchant Single");
            NewCardSymbols.sprEnchantMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Card Icons/Enchant Multiple");

            return NewCardSymbols;
        }
    }

    public abstract class Card
    {
        public enum CardRarities { Normal, Strange, Rare, Extra }

        public Texture2D sprCard;

        public readonly string Path;
        public readonly string CardType;
        public string Name;
        public string Description;
        public CardRarities Rarity;
        public int MagicCost;
        public int QuantityOwned;

        public string Tags;
        public TagSystem TeamTags;
        public EffectHolder Effects;
        public string SkillChainName;
        public List<BaseAutomaticSkill> ListActiveSkill;

        public Card()
            : this("None", "None")
        {
        }

        protected Card(string Path, string CardType)
        {
            this.Path = Path;
            this.CardType = CardType;
            QuantityOwned = 1;
            TeamTags = new TagSystem();
            Effects = new EffectHolder();
            ListActiveSkill = new List<BaseAutomaticSkill>();
        }

        public static Card LoadCard(string Path)
        {
            string[] UnitInfo = Path.Split(new[] { "/", "\\" }, StringSplitOptions.None);

            return FromType(UnitInfo[0], Path.Remove(0, UnitInfo[0].Length + 1), null, null, null, null);
        }

        public static Card LoadCard(string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            string[] UnitInfo = Path.Split(new[] { "/", "\\" }, StringSplitOptions.None);

            return FromType(UnitInfo[0], Path.Remove(0, UnitInfo[0].Length + 1), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        public static Card FromType(string CardType, string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            switch(CardType)
            {
                case "Creature":
                case "Creature Cards":
                    return new CreatureCard(Path, Content, DicRequirement, DicEffects, DicAutomaticSkillTarget);

                case "Item":
                case "Item Cards":
                    return new ItemCard(Path, Content, DicRequirement, DicEffects, DicAutomaticSkillTarget);

                case "Spell Cards":
                case SpellCard.SpellCardType:
                    return new SpellCard(Path, Content, DicRequirement, DicEffects, DicAutomaticSkillTarget);
            }

            throw new Exception("Unkown card type: " + CardType);
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

        public Card Copy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            Card NewCopy = DoCopy(DicRequirement, DicEffects, DicAutomaticSkillTarget);

            NewCopy.Description = Description;
            NewCopy.Rarity = Rarity;
            NewCopy.MagicCost = MagicCost;
            NewCopy.QuantityOwned = QuantityOwned;

            return NewCopy;
        }

        public abstract Card DoCopy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget);

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
            //Draw Card on left
            g.Draw(sprCard, new Vector2(Constants.Width / 4, Constants.Height / 10), new Rectangle(0, 0, sprCard.Width, sprCard.Height), Color.White,
                0f, new Vector2(sprCard.Width / 2, 0), new Vector2(1f, 1f), SpriteEffects.None, 0f);
        }

        public abstract List<Texture2D> GetIcons(CardSymbols Symbols);

        public virtual void DrawCardInfo(CustomSpriteBatch g, CardSymbols Symbols, SpriteFont fntCardInfo, float OffsetX, float OffsetY)
        {
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth + OffsetX;
            float InfoBoxY = Constants.Height / 10 + OffsetY;
            int IconWidth = Constants.Width / 112;
            int IconHeight = Constants.Width / 60;

            MenuHelper.DrawNamedBox(g, CardType, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY - 10;

            CurrentY += 20;

            g.DrawString(fntCardInfo, Name, new Vector2(CurrentX, CurrentY), Color.White);
            switch (Rarity)
            {
                case CardRarities.Normal:
                    g.Draw(Symbols.sprRarityN, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case CardRarities.Strange:
                    g.Draw(Symbols.sprRarityS, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case CardRarities.Rare:
                    g.Draw(Symbols.sprRarityR, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case CardRarities.Extra:
                    g.Draw(Symbols.sprRarityE, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;
            }

            CurrentY += Constants.Height / 24;

            g.DrawString(fntCardInfo, CardType, new Vector2(CurrentX, CurrentY), Color.White);

            CurrentY += Constants.Height / 24;

            CurrentY += 24;
            
            g.Draw(Symbols.sprMenuG, new Rectangle((int)CurrentX - 5, (int)CurrentY, IconWidth, IconHeight), Color.White);
            g.DrawString(fntCardInfo, MagicCost.ToString(), new Vector2(CurrentX + 15, CurrentY), Color.White);

            CurrentY += Constants.Height / 24;

            List<string> ListLine = TextHelper.FitToWidth(fntCardInfo, Description, BoxWidth - 20);
            TextHelper.DrawTextMultiline(g, fntCardInfo, ListLine, TextHelper.TextAligns.Left, CurrentX + BoxWidth / 2, CurrentY, BoxWidth);
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
