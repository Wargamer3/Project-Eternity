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
        public Color TextColor = Color.White;
        public bool IsInit = false;

        public DynamicTextProcessor DefaultProcessor;
        public List<DynamicTextProcessor> ListProcessor;

        public static double AnimationProgression;
        public static int CurrentDrawnCharacterIndex;
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
            DefaultProcessor.Load(Content);

            foreach (DynamicTextProcessor ActiveProcessor in ListProcessor)
            {
                ActiveProcessor.Load(Content);
            }

            Root = DefaultProcessor.ParseText(string.Empty);
        }

        public void SetDefaultProcessor(DynamicTextProcessor DefaultProcessor)
        {
            this.DefaultProcessor = DefaultProcessor;
        }

        public void ParseText(string TextToParse)
        {
            int i = 0;
            Root.ProcessText(TextToParse, ref i);

            UpdateTextPositions();
            IsInit = true;
        }

        public void Update(GameTime gameTime)
        {
            AnimationProgression += gameTime.ElapsedGameTime.TotalSeconds;

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

        public DynamicText Copy()
        {
            DynamicText NewText = new DynamicText();
            NewText.TextMaxWidthInPixel = TextMaxWidthInPixel;
            NewText.LineHeight = LineHeight;
            foreach (DynamicTextProcessor ActiveProcessor in ListProcessor)
            {
                NewText.ListProcessor.Add(ActiveProcessor);
            }

            NewText.SetDefaultProcessor(DefaultProcessor);

            NewText.Root = NewText.DefaultProcessor.ParseText(string.Empty);

            return NewText;
        }

        public void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            CurrentDrawnCharacterIndex = 0;
            Root.Draw(g, Offset);
        }
    }
}
