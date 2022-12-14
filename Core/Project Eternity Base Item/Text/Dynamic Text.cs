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

        public List<DynamicTextProcessor> ListProcessor;

        private bool UpdatePositions;

        public List<DynamicTextPart> ListTextSection;
        public List<Rectangle> ListObstacle;

        public DynamicText()
        {
            ListTextSection = new List<DynamicTextPart>();
            ListObstacle = new List<Rectangle>();
            ListProcessor = new List<DynamicTextProcessor>();
        }

        public void Load(ContentManager Content)
        {
            foreach (DynamicTextProcessor ActiveProcessor in ListProcessor)
            {
                ActiveProcessor.Load(Content);
            }
        }

        public void ParseText(string TextToParse)
        {
            OriginalText = TextToParse;
            string[] SplitText = TextToParse.Split(new string[] { "{{", "}}" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string ActiveText in SplitText)
            {
                foreach (DynamicTextProcessor ActiveProcessor in ListProcessor)
                {
                    DynamicTextPart AvailableTextPart = ActiveProcessor.ParseText(ActiveText);

                    if (AvailableTextPart != null)
                    {
                        ListTextSection.Add(AvailableTextPart);
                        break;
                    }
                }
            }

            UpdateTextPositions();
        }

        public void Update(GameTime gameTime)
        {
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

            foreach (DynamicTextPart ActiveText in ListTextSection)
            {
                ActiveText.Position = CurrentPosition;
                CurrentPosition = ActiveText.UpdatePosition();
            }
        }

        public void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            foreach (DynamicTextPart ActiveText in ListTextSection)
            {
                ActiveText.Draw(g, Offset);
            }
        }
    }
}
