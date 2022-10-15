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

    public class Card
    {
        public enum CardRarities { Normal, Strange, Rare, Extra }

        public readonly string Path;
        public readonly string CardType;
        public string Name;
        public string Description;
        public CardRarities Rarity;
        public int MagicCost;
        public Texture2D sprCard;
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
            Effects = new EffectHolder();
            ListActiveSkill = new List<BaseAutomaticSkill>();
        }

        public static Card LoadCard(string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            string[] UnitInfo = Path.Split(new[] { "/" }, StringSplitOptions.None);

            return FromType(UnitInfo[0], Path.Remove(0, UnitInfo[0].Length + 1), Content, DicRequirement, DicEffects, DicAutomaticSkillTarget);
        }

        private static Card FromType(string CardType, string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            switch(CardType)
            {
                case "Creature Cards":
                    return new CreatureCard(Path, Content, DicRequirement, DicEffects, DicAutomaticSkillTarget);

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

        public virtual void DrawCardInfo(CustomSpriteBatch g, SorcererStreetMap Map, SpriteFont fntCardInfo)
        {
            float InfoBoxX = Constants.Width / 1.8f;
            float InfoBoxY = Constants.Height / 10;
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);

            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
            g.DrawString(fntCardInfo, CardType + " Card", new Vector2(InfoBoxX + 10, InfoBoxY - 20), Color.White);
            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY - 10;

            CurrentY += 20;

            g.DrawString(fntCardInfo, Name, new Vector2(CurrentX, CurrentY), Color.White);
            g.Draw(Map.sprRarityE, new Vector2((int)InfoBoxX + BoxWidth - 30, (int)CurrentY), Color.White);

            CurrentY += 20;

            g.DrawString(fntCardInfo, CardType, new Vector2(CurrentX, CurrentY), Color.White);
            g.Draw(Map.sprElementFire, new Vector2((int)InfoBoxX + BoxWidth - 30, (int)CurrentY), Color.White);

            CurrentY += 20;

            if (CardType == CreatureCard.CreatureCardType)
            {
                CreatureCard ActiveCard = (CreatureCard)this;
                g.Draw(Map.sprMenuST, new Vector2((int)CurrentX, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                g.DrawString(fntCardInfo, ActiveCard.MaxST.ToString(), new Vector2(CurrentX + 20, CurrentY), Color.White);

                g.Draw(Map.sprMenuHP, new Vector2((int)CurrentX + 50, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                g.DrawString(fntCardInfo, ActiveCard.CurrentHP.ToString(), new Vector2(CurrentX + 70, CurrentY), Color.White);

                g.Draw(Map.sprMenuMHP, new Vector2((int)CurrentX + 100, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                g.DrawString(fntCardInfo, ActiveCard.MaxHP.ToString(), new Vector2(CurrentX + 120, CurrentY), Color.White);
            }

            CurrentY += 24;

            g.Draw(Map.sprMenuG, new Vector2((int)CurrentX, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            g.DrawString(fntCardInfo, MagicCost.ToString(), new Vector2(CurrentX + 20, CurrentY), Color.White);

            CurrentY += 20;

            g.DrawString(fntCardInfo, Description.ToString(), new Vector2(CurrentX, CurrentY), Color.White);
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
                g.Draw(sprCardBack, new Vector2(X, Y), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height), Color.White,
                    0f, new Vector2(sprCardFront.Width / 2, sprCardFront.Height / 2), new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0f);
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
