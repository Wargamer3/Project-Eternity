using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CreatureCard : Card
    {
        public enum ElementalAffinity { Neutral, Fire, Water, Earth, Air }

        public const string CreatureCardType = "Creature";

        public readonly int MaxHP;
        public readonly int MaxST;
        public readonly byte DiscardCardRequired;

        public ElementalAffinity[] ArrayAffinity;
        public ElementalAffinity[] ArrayLandLimit;
        public ItemCard.ItemTypes[] ArrayItemLimit;

        public Dictionary<ElementalAffinity, int> DicTerrainRequiement;//Number of owned terrain of a certain type

        public bool SupportCreature;
        public bool ItemCreature;
        public bool Immediate;//Allow all territory command after taking a land (either vacant or after a battle)
        public int DiscardCost;

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

        public void ResetBonuses()
        {
            BonusAttackFirst = false;
            BonusAttackLast = false;
        }

        public override ActionPanelSorcererStreet ActivateOnMap(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            return new ActionPanelConfirmCreatureSummon(Map, ActivePlayerIndex, this);
        }
    }
}
