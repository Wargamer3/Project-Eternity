using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class DynamicText
    {
        public string OriginalText;
        public double EllapsedTime;
        public float TextMaxWidthInPixel;
        public float LineHeight;

        public DynamicTextProcessor DefaultProcessor;
        public List<DynamicTextProcessor> ListProcessor;

        private bool UpdatePositions;

        public DynamicTextPart Root;
        public Dictionary<string, DynamicTextPart> DicTextPartByID;
        public List<Rectangle> ListObstacle;

        public DynamicText()
        {
            ListProcessor = new List<DynamicTextProcessor>();

            DicTextPartByID = new Dictionary<string, DynamicTextPart>();
            ListObstacle = new List<Rectangle>();
        }

        public void Load(ContentManager Content)
        {
            foreach (DynamicTextProcessor ActiveProcessor in ListProcessor)
            {
                ActiveProcessor.Load(Content);
            }
        }

        public void SetDefaultProcessor(DynamicTextProcessor DefaultProcessor)
        {
            this.DefaultProcessor = DefaultProcessor;
            Root = DefaultProcessor.ParseText(string.Empty);
        }

        public void ParseText(string TextToParse)
        {
            int i = 0;
            Root.ProcessText(TextToParse, ref i);

            UpdateTextPositions();
        }

        public void Update(GameTime gameTime)
        {
            Root.Update(gameTime);

            if (UpdatePositions)
            {
                UpdatePositions = false;
                UpdateTextPositions();
            }
        }

        public void AskUpdateTextPositions()
        {
            UpdatePositions = true;
        }

        private void UpdateTextPositions()
        {
            ListObstacle.Clear();

            Vector2 CurrentPosition = Vector2.Zero;

            Root.UpdatePosition();
        }

        public void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            Root.Draw(g, Offset);
        }
    }
}
