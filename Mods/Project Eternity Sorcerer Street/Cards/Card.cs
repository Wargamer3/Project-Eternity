using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public struct CardAbilities
    {
        public bool AttackFirst;
        public bool AttackLast;
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

            NewCardSymbols.sprElementAir = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Air");
            NewCardSymbols.sprElementEarth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Earth");
            NewCardSymbols.sprElementFire = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Fire");
            NewCardSymbols.sprElementWater = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Water");
            NewCardSymbols.sprElementMulti = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Multi");
            NewCardSymbols.sprElementNeutral = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Neutral");

            NewCardSymbols.sprMenuG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/G");
            NewCardSymbols.sprMenuTG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/TG");
            NewCardSymbols.sprMenuST = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/ST");
            NewCardSymbols.sprMenuHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/HP");
            NewCardSymbols.sprMenuMHP = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/MHP");

            NewCardSymbols.sprRarityE = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity E");
            NewCardSymbols.sprRarityN = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity N");
            NewCardSymbols.sprRarityR = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity R");
            NewCardSymbols.sprRarityS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Rarity/Rarity S");

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

                case SpellCard.SpellCardType:
                    return new SpellCard(Path, Content);
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

        public bool CanActivateSkill(string RequirementName)
        {
            for (int E = 0; E < ListActiveSkill.Count; ++E)
            {
                if (ListActiveSkill[E].CanAddSkillEffectsToTarget(RequirementName))
                {
                    return true;
                }
            }

            return false;
        }

        public void ActivateSkill(string RequirementName)
        {
            for (int E = 0; E < ListActiveSkill.Count; ++E)
            {
                ListActiveSkill[E].AddSkillEffectsToTarget(RequirementName);
            }
        }

        public void DrawCard(CustomSpriteBatch g)
        {
            //Draw Card on left
            g.Draw(sprCard, new Vector2(Constants.Width / 4, Constants.Height / 10), new Rectangle(0, 0, sprCard.Width, sprCard.Height), Color.White,
                0f, new Vector2(sprCard.Width / 2, 0), new Vector2(0.6f, 0.6f), SpriteEffects.None, 0f);
        }

        public virtual void DrawCardInfo(CustomSpriteBatch g, CardSymbols Symbols, SpriteFont fntCardInfo, float OffsetX, float OffsetY)
        {
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth + OffsetX;
            float InfoBoxY = Constants.Height / 10 + OffsetY;

            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
            g.DrawString(fntCardInfo, CardType + " Card", new Vector2(InfoBoxX + 10, InfoBoxY - 20), Color.White);
            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY - 10;

            CurrentY += 20;

            g.DrawString(fntCardInfo, Name, new Vector2(CurrentX, CurrentY), Color.White);
            g.Draw(Symbols.sprRarityE, new Vector2((int)InfoBoxX + BoxWidth - 30, (int)CurrentY), Color.White);

            CurrentY += 20;

            g.DrawString(fntCardInfo, CardType, new Vector2(CurrentX, CurrentY), Color.White);

            CurrentY += 20;

            CurrentY += 24;

            g.Draw(Symbols.sprMenuG, new Vector2((int)CurrentX - 5, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            g.DrawString(fntCardInfo, MagicCost.ToString(), new Vector2(CurrentX + 15, CurrentY), Color.White);

            CurrentY += 25;

            List<string> ListLine = TextHelper.FitToWidth(fntCardInfo, Description, BoxWidth - 20);
            TextHelper.DrawTextMultiline(g, fntCardInfo, ListLine, TextHelper.TextAligns.Left, CurrentX + BoxWidth / 2, CurrentY, BoxWidth);
        }

        public static void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCardFront, Texture2D sprCardBack, Color CardFrontColor,
            float X, float Y, float ScaleX, float ScaleY, float RealRotationTimer)
        {
            if (RealRotationTimer < MathHelper.Pi)
            {
                g.Draw(sprCardBack, new Vector2(X, Y), new Rectangle(0, 0, sprCardBack.Width, sprCardBack.Height), Color.White,
                    0f, new Vector2(sprCardBack.Width / 2, 0), new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0f);
            }
            else
            {
                g.Draw(sprCardFront, new Vector2(X, Y), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height), CardFrontColor,
                    0f, new Vector2(sprCardFront.Width / 2, 0), new Vector2(-ScaleX, ScaleY), SpriteEffects.None, 0f);
            }
        }

        public static void DrawCardMiniatureCentered(CustomSpriteBatch g, Texture2D sprCardFront, Texture2D sprCardBack, Color CardFrontColor,
            float X, float Y, float ScaleX, float ScaleY, float RealRotationTimer)
        {
            if (RealRotationTimer < MathHelper.Pi)
            {
                g.Draw(sprCardBack, new Vector2(X, Y), new Rectangle(0, 0, sprCardBack.Width, sprCardBack.Height), Color.White,
                    0f, new Vector2(sprCardBack.Width / 2, sprCardBack.Height / 2), new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0f);
            }
            else
            {
                g.Draw(sprCardFront, new Vector2(X, Y), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height), CardFrontColor,
                    0f, new Vector2(sprCardFront.Width / 2, sprCardFront.Height / 2), new Vector2(-ScaleX, ScaleY), SpriteEffects.None, 0f);
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
