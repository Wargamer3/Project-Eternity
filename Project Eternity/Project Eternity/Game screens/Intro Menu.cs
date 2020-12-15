using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens;

namespace ProjectEternity
{
    public class IntroMenu : GameScreen
    {
        private string[] Text;
        private float TextAdvancement;
        private float TextEnd;
        private int ScreenSize;
        public SpriteFont fntArial;

        public IntroMenu()
            : base()
        {
        }

        public override void Load()
        {
            fntArial = Content.Load<SpriteFont>("Fonts/Arial");

            Text = new string[] {"A word with very little meaning to you, the average person",
                "living his life in our world, but one with great weight and tangible",
                "meaning in the realm of fiction.",
                "While we buzz about our world, doing the things we please,",
                "DIMENSION rules over the denizens of our entertainment.",
                "",
                "In a lavish estate, a district attorney stares down a watch as it counts",
                "down the life of an accused. As it reaches its destination, so does the",
                "sentenced man reach the chamber, and soon the two are swapped.",
                "",
                "In a foreign land, a brave warrior-maiden makes her journey to the far",
                "north. Her goal is to slay a false god and the cull the reigns of the",
                "evil cult he has spawned. As she cuts down the last of them, however,",
                "she finds herself losing every shred of her humanity.",
                "",
                "While a young woman finds herself awakening to the nightmares she",
                "thought she had once escaped a younger man slowly realizes that he",
                "is becoming the nightmares that stalk the minds of men.",
                "",
                "What you see here is no mere myth; it is merely the power of dimension,",
                "made manifest.",
                "For, dimension, that simple, yet toyetic word is what determines the fate",
                "of fictional man and those he holds dear."};
            TextAdvancement = 0;
            TextEnd = Text.Length * fntArial.LineSpacing;
            ScreenSize = Constants.Height / fntArial.LineSpacing;
        }

        public override void Update(GameTime gameTime)
        {
            //Skip the text
            if (InputHelper.InputSkipPressed())
                TextAdvancement = (int)TextEnd + Constants.Height - 1;
            //If a confirm choice key is down, accelerate the scrolling speed.
            if (InputHelper.InputConfirmHold())
                TextAdvancement += 4;
            //Normal scrolling of the text.
            if (TextAdvancement < TextEnd + Constants.Height)
                TextAdvancement+= 0.25f;
            else
            {//End of text. Create the visual novel page.
                //GameScreen.PushScreen(new LoadScreen(new GameScreen[] { new IntermissionScreen(), new VisualNovel("Tuto.pevn") }, null));
                RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int i;
            //Make sure you only draw the right amount of text lines.
            int TextPosition = (int)Math.Min(Text.Length, TextAdvancement / fntArial.LineSpacing);
            //Start at the right height so you won't draw out of screen text.
            if (TextPosition > ScreenSize)
                i = TextPosition - ScreenSize;
            else
                i = 0;
            while (i < TextPosition)
            {
                g.DrawStringMiddleAligned(fntArial, Text[i], new Vector2(Constants.Width / 2, -TextAdvancement + Constants.Height + i * fntArial.LineSpacing), Color.White);
                i++;
            }
        }
    }
}
