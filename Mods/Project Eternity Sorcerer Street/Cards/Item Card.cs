using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Graphics;

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

        public override Card DoCopy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            return new ItemCard(this, DicRequirement, DicEffects, DicAutomaticSkillTarget);
        }

        public override void DrawCardInfo(CustomSpriteBatch g, CardSymbols Symbols, SpriteFont fntCardInfo, float OffsetX, float OffsetY)
        {
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth + OffsetX;
            float InfoBoxY = Constants.Height / 10 + OffsetY;

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY + Constants.Height / 24;

            base.DrawCardInfo(g, Symbols, fntCardInfo, OffsetX, OffsetY);

            switch (ItemType)
            {
                case ItemTypes.Weapon:
                    g.Draw(Symbols.sprItemsWeapon, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case ItemTypes.Armor:
                    g.Draw(Symbols.sprItemsArmor, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case ItemTypes.Tools:
                    g.Draw(Symbols.sprItemsTool, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case ItemTypes.Scrolls:
                    g.Draw(Symbols.sprItemsArmor, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;
            }
        }

        public override List<Texture2D> GetIcons(CardSymbols Symbols)
        {
            switch (ItemType)
            {
                case ItemTypes.Weapon:
                    return new List<Texture2D>() { Symbols.sprItemsWeapon };

                case ItemTypes.Armor:
                    return new List<Texture2D>() { Symbols.sprItemsArmor };

                case ItemTypes.Tools:
                    return new List<Texture2D>() { Symbols.sprItemsTool };

                case ItemTypes.Scrolls:
                    return new List<Texture2D>() { Symbols.sprItemsScroll };
            }

            return null;
        }
    }
}
