using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SpellCard : Card
    {
        public enum SpellTypes { SingleFlash, MultiFlash, SingleEnchant, MultiEnchant, World, Secret }

        public const string SpellCardType = "Spell";

        public SpellCard(string Path)
            : base(Path, SpellCardType)
        {

        }

        public SpellCard(string Path, ContentManager Content)
            : this(Path)
        {

        }

        public override Card DoCopy(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            throw new System.NotImplementedException();
        }
    }
}
