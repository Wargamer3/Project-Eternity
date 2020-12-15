using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public readonly string CardType;
        public string Name;
        public string Description;
        public CardRarities Rarity;
        public int MagicCost;
        public Texture2D sprCard;
        public EffectHolder Effects;
        public List<BaseAutomaticSkill> ListSkill;

        public Card()
            : this("None")
        {
        }

        protected Card(string CardType)
        {
            this.CardType = CardType;
            Effects = new EffectHolder();
            ListSkill = new List<BaseAutomaticSkill>();
        }

        public static Card LoadCard(string Path, ContentManager Content)
        {
            string[] UnitInfo = Path.Split(new[] { "/" }, StringSplitOptions.None);

            return FromType(UnitInfo[0], Path.Remove(0, UnitInfo[0].Length + 1), Content);
        }

        private static Card FromType(string CardType, string Path, ContentManager Content)
        {
            switch(CardType)
            {
                case "Creature Cards":
                    return new CreatureCard(Path, Content);

                case SpellCard.SpellCardType:
                    return new SpellCard(Path, Content);
            }

            throw new Exception("Unkown card type: " + CardType);
        }

        public void ActivateSkill(string RequirementName)
        {
            for (int E = 0; E < ListSkill.Count; ++E)
            {
                ListSkill[E].AddSkillEffectsToTarget(RequirementName);
            }
        }

        public void DrawCard(CustomSpriteBatch g)
        {
            //Draw Card on left
            g.Draw(sprCard, new Vector2(Constants.Width / 4, Constants.Height / 10), new Rectangle(0, 0, sprCard.Width, sprCard.Height), Color.White,
                0f, new Vector2(sprCard.Width / 2, 0), new Vector2(0.6f, 0.6f), SpriteEffects.None, 0f);
        }

        public virtual void DrawCardInfo(CustomSpriteBatch g, SpriteFont fntCardInfo)
        {
            float InfoBoxX = Constants.Width / 1.8f;
            float InfoBoxY = Constants.Height / 10;
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);

            GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY + 5;
            g.DrawString(fntCardInfo, CardType + " Card", new Vector2(CurrentX, CurrentY), Color.White);

            CurrentY += 20;

            g.DrawString(fntCardInfo, Name, new Vector2(CurrentX, CurrentY), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)InfoBoxX + BoxWidth - 30, (int)CurrentY, 18, 18), Color.White);

            CurrentY += 20;

            g.DrawString(fntCardInfo, CardType, new Vector2(CurrentX, CurrentY), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)InfoBoxX + BoxWidth - 30, (int)CurrentY, 18, 18), Color.White);

            CurrentY += 20;

            if (CardType == CreatureCard.CreatureCardType)
            {
                CreatureCard ActiveCard = (CreatureCard)this;
                g.Draw(GameScreen.sprPixel, new Rectangle((int)CurrentX, (int)CurrentY, 18, 18), Color.White);
                g.DrawString(fntCardInfo, ActiveCard.MaxST.ToString(), new Vector2(CurrentX + 20, CurrentY), Color.White);

                g.Draw(GameScreen.sprPixel, new Rectangle((int)CurrentX + 50, (int)CurrentY, 18, 18), Color.White);
                g.DrawString(fntCardInfo, ActiveCard.CurrentHP.ToString(), new Vector2(CurrentX + 70, CurrentY), Color.White);

                g.Draw(GameScreen.sprPixel, new Rectangle((int)CurrentX + 100, (int)CurrentY, 18, 18), Color.White);
                g.DrawString(fntCardInfo, ActiveCard.MaxHP.ToString(), new Vector2(CurrentX + 120, CurrentY), Color.White);
            }

            CurrentY += 20;

            g.Draw(GameScreen.sprPixel, new Rectangle((int)CurrentX, (int)CurrentY, 18, 18), Color.White);
            g.DrawString(fntCardInfo, MagicCost.ToString(), new Vector2(CurrentX + 20, CurrentY), Color.White);

            CurrentY += 20;

            g.DrawString(fntCardInfo, Description.ToString(), new Vector2(CurrentX, CurrentY), Color.White);
        }

        public static void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCardFront, Texture2D sprCardBack, Color CardFrontColor,
            float X, float Y, float ScaleX, float ScaleY, float RealRotationTimer)
        {
            if (RealRotationTimer < MathHelper.Pi)
            {
                g.Draw(sprCardBack, new Vector2(X, Y), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height), Color.Black,
                    0f, new Vector2(sprCardFront.Width / 2, 0), new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0f);
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
                g.Draw(sprCardBack, new Vector2(X, Y), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height), Color.Black,
                    0f, new Vector2(sprCardFront.Width / 2, sprCardFront.Height / 2), new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0f);
            }
            else
            {
                g.Draw(sprCardFront, new Vector2(X, Y), new Rectangle(0, 0, sprCardFront.Width, sprCardFront.Height), CardFrontColor,
                    0f, new Vector2(sprCardFront.Width / 2, sprCardFront.Height / 2), new Vector2(-ScaleX, ScaleY), SpriteEffects.None, 0f);
            }
        }

        public virtual ActionPanelSorcererStreet ActivateOnMap(SorcererStreetMap Map, Player ActivePlayer)
        {
            return null;
        }

        public virtual ActionPanelSorcererStreet ActivateInBattle(SorcererStreetMap Map, Player ActivePlayer)
        {
            return null;
        }
    }

    public class CreatureCard : Card
    {
        public enum ElementalAffinity { Neutral, Fire, Water, Earth, Air }

        public const string CreatureCardType = "Creature";

        public readonly int MaxHP;
        public readonly int MaxST;

        public ElementalAffinity[] ArrayAffinity;
        public ElementalAffinity[] ArrayLandLimit;
        public ItemCard.ItemTypes[] ArrayItemLimit;

        public Dictionary<ElementalAffinity, int> DicTerrainRequiement;//Number of owned terrain of a certain type

        public bool SupportCreature;
        public bool ItemCreature;
        public int DiscardCost;

        public string AttackAnimationPath;

        public int CurrentHP;
        public int CurrentST;

        public bool BonusAttackFirst;//Overrides natural ability
        public bool BonusAttackLast;//Overrides natural ability

        public CardAbilities Abilities;

        public CreatureCard()
            : base(CreatureCardType)
        {
            Name = string.Empty;
            Description = string.Empty;
            AttackAnimationPath = "Sorcerer Street/New Item";
        }

        public CreatureCard(string Path, ContentManager Content)
            : this()
        {
            if (Content != null)
            {
                sprCard = Content.Load<Texture2D>("Sorcerer Street/Creature Cards/fighter");
            }

            FileStream FS = new FileStream("Content/Sorcerer Street/Creature Cards/" + Path + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            MagicCost = BR.ReadInt32();
            Rarity = (CardRarities)BR.ReadByte();

            CurrentHP = MaxHP = BR.ReadInt32();
            CurrentST = MaxST = BR.ReadInt32();

            int ArrayAffinityLength = BR.ReadInt32();
            ArrayAffinity = new ElementalAffinity[ArrayAffinityLength];
            for (int A = 0; A < ArrayAffinityLength; ++A)
            {
                ArrayAffinity[A] = (ElementalAffinity)BR.ReadByte();
            }

            int ArrayLandLimitLength = BR.ReadInt32();
            ArrayLandLimit = new ElementalAffinity[ArrayLandLimitLength];
            for (int L = 0; L < ArrayLandLimitLength; ++L)
            {
                ArrayLandLimit[L] = (ElementalAffinity)BR.ReadByte();
            }

            int ArrayItemLimitLength = BR.ReadInt32();
            ArrayItemLimit = new ItemCard.ItemTypes[ArrayItemLimitLength];
            for (int I = 0; I < ArrayItemLimitLength; ++I)
            {
                ArrayItemLimit[I] = (ItemCard.ItemTypes)BR.ReadByte();
            }

            int DicTerrainRequiementCount = BR.ReadInt32();
            DicTerrainRequiement = new Dictionary<ElementalAffinity, int>(DicTerrainRequiementCount);
            for (int I = 0; I < DicTerrainRequiementCount; ++I)
            {
                DicTerrainRequiement.Add((ElementalAffinity)BR.ReadByte(), BR.ReadInt32());
            }

            BR.Close();
            FS.Close();
        }

        public CreatureCard(int MaxHP, int MaxST)
            : this()
        {
            this.MaxHP = MaxHP;
            this.MaxST = MaxST;

            CurrentHP = MaxHP;
            CurrentST = MaxST;

            DicTerrainRequiement = new Dictionary<ElementalAffinity, int>();
            ArrayAffinity = new ElementalAffinity[0];
            ArrayLandLimit = new ElementalAffinity[0];
            ArrayItemLimit = new ItemCard.ItemTypes[0];
            Abilities = new CardAbilities();
        }

        public void ResetBonuses()
        {
            BonusAttackFirst = false;
            BonusAttackLast = false;
        }

        public override ActionPanelSorcererStreet ActivateOnMap(SorcererStreetMap Map, Player ActivePlayer)
        {
            return new ActionPanelConfirmCreatureSummon(Map, ActivePlayer, this);
        }
    }

    public class ItemCard : Card
    {
        public enum ItemTypes { Weapon, Armor, Tools, Scrolls }

        public string AttackAnimationPath;
    }

    public class SpellCard : Card
    {
        public enum SpellTypes { SingleFlash, MultiFlash, SingleEnchant, MultiEnchant, World, Secret }

        public const string SpellCardType = "Spell";

        public SpellCard()
            : base(SpellCardType)
        {

        }

        public SpellCard(string Path, ContentManager Content)
            : this()
        {

        }
    }
}
