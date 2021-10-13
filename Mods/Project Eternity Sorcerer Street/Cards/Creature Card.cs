using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
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

        public CreatureCard(string Path)
            : base(Path, CreatureCardType)
        {
            Name = string.Empty;
            Description = string.Empty;
            AttackAnimationPath = "Sorcerer Street/New Item";
        }

        public CreatureCard(string Path, ContentManager Content)
            : this(Path)
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
