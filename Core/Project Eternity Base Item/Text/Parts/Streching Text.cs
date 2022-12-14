using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class StretchingText : RegularText
    {
        public float Scale;

        public StretchingText(DynamicText Owner, FontsHolder Fonts, string OriginalText, string Prefix)
            : base(Owner, Fonts, OriginalText, Prefix)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Owner.AskUpdateTextPositions();
        }

        public override void Draw(CustomSpriteBatch g, Vector2 Offset)
        {

        }
    }
}
