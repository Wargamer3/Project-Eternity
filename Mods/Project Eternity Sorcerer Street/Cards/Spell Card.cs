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
    public class SpellCard : Card
    {
        public enum SpellTypes
        {
            SingleFlash,//Offsensive attack for a single target
            MultiFlash,
            SingleEnchant,//Enchant a creature or a Player
            MultiEnchant,
            World,//??
            Secret//??
        }

        public const string SpellCardType = "Spell";

        public SpellTypes SpellType;

        public bool Doublecast;
        public int DiscardCost;

        public SpellCard(string Path, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
            : base(Path, SpellCardType)
        {
            FileStream FS = new FileStream("Content/Sorcerer Street/Spell Cards/" + Path + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();
            FlavorText = BR.ReadString();
            Tags = BR.ReadString();
            Doublecast = BR.ReadBoolean();

            MagicCost = BR.ReadInt32();
            DiscardCost = BR.ReadByte();
            Rarity = (CardRarities)BR.ReadByte();
            SpellType = (SpellTypes)BR.ReadByte();

            byte ListSpellCount = BR.ReadByte();
            ListSpell = new List<ManualSkill>(ListSpellCount);
            ListSpellName = new List<string>(ListSpellCount);
            ListSpellActivationAnimationPath = new List<string>(ListSpellCount);
            for (int S = 0; S < ListSpellCount; ++S)
            {
                string SpellName = BR.ReadString();
                string SpellActivationAnimationPath = BR.ReadString();

                if (!string.IsNullOrEmpty(SpellName))
                {
                    ListSpell.Add(new ManualSkill("Content/Sorcerer Street/Spells/" + SpellName + ".pes", DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget));
                    ListSpellName.Add(SpellName);
                    ListSpellActivationAnimationPath.Add(SpellActivationAnimationPath);
                }
            }

            ListActiveSkill = new List<BaseAutomaticSkill>();

            BR.Close();
            FS.Close();

            if (Content != null)
            {
                sprCard = Content.Load<Texture2D>("Sorcerer Street/Spell Cards/" + Path);
            }
        }

        public SpellCard(SpellCard Clone, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
            : base(Clone.Path, SpellCardType)
        {
            Name = Clone.Name;
            Description = Clone.Description;
            FlavorText = Clone.FlavorText;
            SpellType = Clone.SpellType;

            SkillChainName = Clone.SkillChainName;
            ListSpell = new List<ManualSkill>(Clone.ListSpell.Count);
            ListSpellName = new List<string>(Clone.ListSpell.Count);
            ListSpellActivationAnimationPath = new List<string>(Clone.ListSpell.Count);

            for (int S = 0; S < Clone.ListSpell.Count; ++S)
            {
                ManualSkill Spell = new ManualSkill(Clone.ListSpell[S]);
                Spell.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                ListSpell.Add(Spell);
                ListSpellName.Add(Clone.ListSpellName[S]);
                ListSpellActivationAnimationPath.Add(Clone.ListSpellActivationAnimationPath[S]);
            }

            sprCard = Clone.sprCard;
        }

        public override Card DoCopy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            return new SpellCard(this, DicRequirement, DicEffects, DicAutomaticSkillTarget, DicManualSkillTarget);
        }

        public override void DrawCardInfo(CustomSpriteBatch g, CardSymbols Symbols, SpriteFont fntCardInfo, Player ActivePlayer, float OffsetX, float OffsetY)
        {
            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth + OffsetX;
            float InfoBoxY = Constants.Height / 10 + OffsetY;

            float CurrentX = InfoBoxX + 10;
            float CurrentY = InfoBoxY + Constants.Height / 24;

            base.DrawCardInfo(g, Symbols, fntCardInfo, ActivePlayer, OffsetX, OffsetY);

            switch (SpellType)
            {
                case SpellTypes.SingleEnchant:
                    g.Draw(Symbols.sprEnchantSingle, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case SpellTypes.MultiEnchant:
                    g.Draw(Symbols.sprEnchantMultiple, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case SpellTypes.SingleFlash:
                    g.Draw(Symbols.sprSpellsSingle, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;

                case SpellTypes.MultiFlash:
                    g.Draw(Symbols.sprSpellsMultiple, new Vector2((int)InfoBoxX + BoxWidth - Constants.Width / 38, (int)CurrentY), Color.White);
                    break;
            }
        }

        public override ActionPanelSorcererStreet ActivateOnMap(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            Player ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            Map.GlobalPlayerContext.SetActiveCard(ActivePlayer.ListCardInHand.IndexOf(this), this);
            ListSpell[0].ActiveSkillFromMenu();
            return null;
        }

        public override List<Texture2D> GetIcons(CardSymbols Symbols)
        {
            switch (SpellType)
            {
                case SpellTypes.SingleEnchant:
                    return new List<Texture2D>() { Symbols.sprEnchantSingle };

                case SpellTypes.MultiEnchant:
                    return new List<Texture2D>() { Symbols.sprEnchantMultiple };

                case SpellTypes.SingleFlash:
                    return new List<Texture2D>() { Symbols.sprSpellsSingle };

                case SpellTypes.MultiFlash:
                    return new List<Texture2D>() { Symbols.sprSpellsMultiple };
            }

            return null;
        }
    }
}
