using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CreatureCard : Card
    {
        public enum ElementalAffinity { Neutral, Fire, Water, Earth, Air }

        public const string CreatureCardType = "Creature";

        public readonly int MaxHP;
        public readonly int MaxST;
        public string Subtype;
        public readonly byte DiscardCardRequired;

        public ElementalAffinity[] ArrayAffinity;
        public ElementalAffinity[] ArrayLandLimit;
        public ItemCard.ItemTypes[] ArrayItemLimit;

        public Dictionary<ElementalAffinity, int> DicTerrainRequiement;//Number of owned terrain of a certain type

        public bool SupportCreature;
        public bool ItemCreature;
        public bool Immediate;//Allow all territory command after taking a land (either vacant or after a battle)
        public int DiscardCost;
        public float TollMultiplier = 1;

        public string AttackAnimationPath;
        public AnimatedModel Map3DModel;

        public int CurrentHP;
        public int CurrentST;

        public bool BonusAttackFirst;//Overrides natural ability
        public bool BonusAttackLast;//Overrides natural ability

        public CardAbilities Abilities;

        public CreatureCard(string Path)
            : base(Path, CreatureCardType)
        {
            Name = string.Empty;
            Description = string.Empty;
            AttackAnimationPath = "Sorcerer Street/New Item";
        }

        public CreatureCard(string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(Path)
        {
            FileStream FS = new FileStream("Content/Sorcerer Street/Creature Cards/" + Path + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            MagicCost = BR.ReadInt32();
            DiscardCardRequired = BR.ReadByte();
            Rarity = (CardRarities)BR.ReadByte();

            AttackAnimationPath = BR.ReadString();

            CurrentHP = MaxHP = BR.ReadInt32();
            CurrentST = MaxST = BR.ReadInt32();
            Subtype = BR.ReadString();
            SkillChainName = BR.ReadString();

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
            if (!string.IsNullOrWhiteSpace(SkillChainName) && DicRequirement != null)
            {
                FileStream FSSkillChain = new FileStream("Content/Sorcerer Street/Skill Chains/" + SkillChainName + ".pesc", FileMode.Open, FileAccess.Read);
                BinaryReader BRSkillChain = new BinaryReader(FSSkillChain, Encoding.UTF8);
                BRSkillChain.BaseStream.Seek(0, SeekOrigin.Begin);

                int tvSkillsNodesCount = BRSkillChain.ReadInt32();
                ListActiveSkill = new List<BaseAutomaticSkill>(tvSkillsNodesCount);
                for (int N = 0; N < tvSkillsNodesCount; ++N)
                {
                    BaseAutomaticSkill ActiveSkill = new BaseAutomaticSkill(BRSkillChain, DicRequirement, DicEffects, DicAutomaticSkillTarget);

                    InitSkillChainTarget(ActiveSkill, DicAutomaticSkillTarget);

                    ListActiveSkill.Add(ActiveSkill);
                }

                BRSkillChain.Close();
                FSSkillChain.Close();
            }
            else
            {
                ListActiveSkill = new List<BaseAutomaticSkill>();
            }

            BR.Close();
            FS.Close();

            if (Content != null)
            {
                sprCard = Content.Load<Texture2D>("Sorcerer Street/Creature Cards/" + Path);

                Map3DModel = new AnimatedModel("Sorcerer Street/Models/Bad Guy");
                Map3DModel.LoadContent(Content);
            }
        }

        public CreatureCard(int MaxHP, int MaxST)
            : this("")
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

        public CreatureCard(CreatureCard Clone, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Clone.Path, CreatureCardType)
        {
            Name = Clone.Name;
            Description = Clone.Description;

            CurrentHP = MaxHP = Clone.MaxHP;
            CurrentST = MaxST = Clone.MaxST;
            DiscardCardRequired = Clone.DiscardCardRequired;

            AttackAnimationPath = Clone.AttackAnimationPath;

            SkillChainName = Clone.SkillChainName;

            ArrayAffinity = new ElementalAffinity[Clone.ArrayAffinity.Length];
            for (int A = 0; A < Clone.ArrayAffinity.Length; ++A)
            {
                ArrayAffinity[A] = Clone.ArrayAffinity[A];
            }

            ArrayLandLimit = new ElementalAffinity[Clone.ArrayLandLimit.Length];
            for (int L = 0; L < Clone.ArrayLandLimit.Length; ++L)
            {
                ArrayLandLimit[L] = Clone.ArrayLandLimit[L];
            }

            ArrayItemLimit = new ItemCard.ItemTypes[Clone.ArrayItemLimit.Length];
            for (int I = 0; I < Clone.ArrayItemLimit.Length; ++I)
            {
                ArrayItemLimit[I] = Clone.ArrayItemLimit[I];
            }

            DicTerrainRequiement = new Dictionary<ElementalAffinity, int>(Clone.DicTerrainRequiement.Count);
            foreach (KeyValuePair<ElementalAffinity, int> ActiveRequirement in Clone.DicTerrainRequiement)
            {
                DicTerrainRequiement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            if (!string.IsNullOrWhiteSpace(SkillChainName) && DicRequirement != null)
            {
                ListActiveSkill = new List<BaseAutomaticSkill>(Clone.ListActiveSkill.Count);

                for (int N = 0; N < Clone.ListActiveSkill.Count; ++N)
                {
                    ListActiveSkill.Add(new BaseAutomaticSkill(Clone.ListActiveSkill[N]));
                }
            }
            else
            {
                ListActiveSkill = new List<BaseAutomaticSkill>();
            }

            sprCard = Clone.sprCard;
            if (Map3DModel != null)
            {
                Map3DModel = Map3DModel.Clone();
            }
        }

        public override Card DoCopy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return new CreatureCard(this, DicRequirement, DicEffects, DicAutomaticSkillTarget);
        }

        public void ResetBonuses()
        {
            BonusAttackFirst = false;
            BonusAttackLast = false;
        }

        public override ActionPanelSorcererStreet ActivateOnMap(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            return new ActionPanelConfirmCreatureSummon(Map, ActivePlayerIndex, this);
        }

        public override void DrawCardInfo(CustomSpriteBatch g, CardSymbols Symbols, SpriteFont fntCardInfo, float OffsetX, float OffsetY)
        {
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth + OffsetX;
            float InfoBoxY = Constants.Height / 10 + OffsetY;

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY + 30;

            base.DrawCardInfo(g, Symbols, fntCardInfo, OffsetX, OffsetY);

            for (int A = 0; A < ArrayAffinity.Length; A++)
            {
                ElementalAffinity ActiveAffinity = ArrayAffinity[A];
                switch (ActiveAffinity)
                {
                    case ElementalAffinity.Neutral:
                        g.Draw(Symbols.sprElementNeutral, new Vector2((int)InfoBoxX + BoxWidth - 30 - 20 * A, (int)CurrentY), Color.White);
                        break;
                    case ElementalAffinity.Fire:
                        g.Draw(Symbols.sprElementFire, new Vector2((int)InfoBoxX + BoxWidth - 30 - 20 * A, (int)CurrentY), Color.White);
                        break;
                    case ElementalAffinity.Air:
                        g.Draw(Symbols.sprElementAir, new Vector2((int)InfoBoxX + BoxWidth - 30 - 20 * A, (int)CurrentY), Color.White);
                        break;
                    case ElementalAffinity.Earth:
                        g.Draw(Symbols.sprElementEarth, new Vector2((int)InfoBoxX + BoxWidth - 30 - 20 * A, (int)CurrentY), Color.White);
                        break;
                    case ElementalAffinity.Water:
                        g.Draw(Symbols.sprElementWater, new Vector2((int)InfoBoxX + BoxWidth - 30 - 20 * A, (int)CurrentY), Color.White);
                        break;
                }
            }

            CurrentY += 20;

            g.Draw(Symbols.sprMenuST, new Vector2((int)CurrentX - 5, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            g.DrawString(fntCardInfo, MaxST.ToString(), new Vector2(CurrentX + 15, CurrentY), Color.White);

            g.Draw(Symbols.sprMenuHP, new Vector2((int)CurrentX + 45, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            g.DrawString(fntCardInfo, CurrentHP.ToString(), new Vector2(CurrentX + 65, CurrentY), Color.White);

            g.Draw(Symbols.sprMenuMHP, new Vector2((int)CurrentX + 95, (int)CurrentY), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            g.DrawString(fntCardInfo, MaxHP.ToString(), new Vector2(CurrentX + 115, CurrentY), Color.White);
        }

    }
}
