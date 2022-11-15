using System;
using System.IO;
using System.Text;
using ProjectEternity.Core.Item;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ItemCard : Card
    {
        public const string ItemCardType = "Item";

        public enum ItemTypes { Weapon, Armor, Tools, Scrolls }

        public ItemTypes ItemType;

        public string ItemActivationAnimationPath;

        public ItemCard(string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            :base(Path, ItemCardType)
        {
            FileStream FS = new FileStream("Content/Sorcerer Street/Item Cards/" + Path + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            MagicCost = BR.ReadInt32();
            Rarity = (CardRarities)BR.ReadByte();
            ItemType = (ItemTypes)BR.ReadByte();

            SkillChainName = BR.ReadString();
            ItemActivationAnimationPath = BR.ReadString();

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
                sprCard = Content.Load<Texture2D>("Sorcerer Street/Item Cards/" + Path);
            }
        }

        public ItemCard(ItemCard Clone, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Clone.Path, ItemCardType)
        {
            Name = Clone.Name;
            Description = Clone.Description;
            ItemType = Clone.ItemType;

            SkillChainName = Clone.SkillChainName;

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
        }


        public override Card DoCopy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return new ItemCard(this, DicRequirement, DicEffects, DicAutomaticSkillTarget);
        }
    }
}
