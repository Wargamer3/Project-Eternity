using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class PlayerNamePart : RegularText
    {
        SorcererStreetMap Map;

        public PlayerNamePart(DynamicText Owner, FontsHolder Fonts, SorcererStreetMap Map, string OriginalText)
             : base(Owner, Fonts, OriginalText, "Player:")
        {
            this.Map = Map;
        }

        public override void OnTextRead(string TextRead)
        {
            if (TextRead == "Self")
            {
                OriginalText = "Success";
            }
            else
            {
                OriginalText = TextRead;
            }
        }
    }

    public class PlayerNameProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private FontsHolder Fonts;
        private SpriteFont DefaultFont;
        private readonly SorcererStreetMap Map;

        public PlayerNameProcessor(DynamicText Owner, SorcererStreetMap Map)
        {
            this.Owner = Owner;
            this.Map = Map;
        }

        public PlayerNameProcessor(DynamicText Owner, SpriteFont DefaultFont, SorcererStreetMap Map)
        {
            this.Owner = Owner;
            this.DefaultFont = DefaultFont;
            this.Map = Map;
        }

        public override void Load(ContentManager Content)
        {
            Fonts = new FontsHolder(Content);
            if (DefaultFont != null)
            {
                Fonts.fntDefaultFont = DefaultFont;
            }
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            if (Prefix.StartsWith("Player:"))
            {
                return new PlayerNamePart(Owner, Fonts, Map, string.Empty);
            }

            return null;
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("Text:"))
            {
                return new PlayerNamePart(Owner, Fonts, Map, Text);
            }

            return null;
        }
    }
}
