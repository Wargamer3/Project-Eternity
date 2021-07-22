using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.VisualNovelScreen
{
    public class ExtraMenu : GameScreen
    {
        private int SummaryStartIndex;
        private int CurosorIndex;

        private readonly VisualNovel Owner;
        private readonly SpriteFont fntFinlanderFont;
        private readonly int TimelineIndexMax;

        private List<string> ListDialogSummary;

        public ExtraMenu(VisualNovel Owner, SpriteFont fntFinlanderFont, int TimelineIndexMax)
        {
            this.Owner = Owner;
            this.fntFinlanderFont = fntFinlanderFont;
            this.TimelineIndexMax = TimelineIndexMax;

            ListDialogSummary = new List<string>();
        }

        public override void Load()
        {
            for (int i = 0; i <= TimelineIndexMax; ++i)
            {
                //Crop the text before drawing it.
                string TextBuffer = Owner.Timeline[i].Text.Replace("\r", "").Replace("\n", "");
                TextBuffer = TextHelper.FitToWidth(fntFinlanderFont, TextBuffer, Constants.Width - 100)[0];
                ListDialogSummary.Add(TextBuffer);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                SummaryStartIndex -= SummaryStartIndex > 0 ? 1 : 0;
                Owner.CurrentDialog = Owner.Timeline[SummaryStartIndex];
            }
            else if (InputHelper.InputDownPressed())
            {
                SummaryStartIndex += SummaryStartIndex <= TimelineIndexMax - 1 ? 1 : 0;
                Owner.CurrentDialog = Owner.Timeline[SummaryStartIndex];
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(50, 50), Constants.Width - 100, Constants.Height - 200, Color.White);

            for (int i = SummaryStartIndex, CurrentText = 0; i < ListDialogSummary.Count && CurrentText <= 5; ++i, ++CurrentText)
            {
                if (i == CurosorIndex)
                {
                    g.Draw(sprPixel,
                        new Rectangle(55,
                        55 + (CurrentText * fntFinlanderFont.LineSpacing + 2),
                        Constants.Width - 110,
                        fntFinlanderFont.LineSpacing + 2), 
                        Color.FromNonPremultiplied(255, 255, 255, 100));
                }

                g.DrawString(fntFinlanderFont, ListDialogSummary[i],
                    new Vector2(55,
                    50 + CurrentText * (fntFinlanderFont.LineSpacing + 2)),
                    Color.White);
            }
        }
    }
}
