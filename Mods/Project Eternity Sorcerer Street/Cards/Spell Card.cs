using Microsoft.Xna.Framework.Content;

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
    }
}
