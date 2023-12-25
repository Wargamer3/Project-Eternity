using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class UIText : IUIElement
    {
        DynamicText Text;
        Vector2 Position;

        public UIText(Microsoft.Xna.Framework.Content.ContentManager Content, string TextToDisplay, Vector2 Position, int MaxWidth)
        {
            this.Position = Position;

            Text = new DynamicText();
            Text.TextMaxWidthInPixel = MaxWidth;
            Text.LineHeight = 20;
            Text.ListProcessor.Add(new RegularTextProcessor(Text));
            Text.ListProcessor.Add(new IconProcessor(Text));
            Text.ListProcessor.Add(new DefaultTextProcessor(Text, Content.Load<SpriteFont>("Fonts/Arial10")));
            Text.SetDefaultProcessor(new DefaultTextProcessor(Text, Content.Load<SpriteFont>("Fonts/Arial10")));

            Text.Load(Content);
            Text.ParseText(TextToDisplay);
        }


        public void Update(GameTime gameTime)
        {
            Text.Update(gameTime);
        }

        public void Draw(CustomSpriteBatch g)
        {
            Text.Draw(g, Position);
        }

        public void Select()
        {
            throw new System.NotImplementedException();
        }

        public void Unselect()
        {
            throw new System.NotImplementedException();
        }

        public void Enable()
        {
            throw new System.NotImplementedException();
        }

        public void Disable()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return Text.OriginalText;
        }
    }
}
