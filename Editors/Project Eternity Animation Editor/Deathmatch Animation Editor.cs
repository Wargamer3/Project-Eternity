using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationEditor
{
    public class DeathmatchAnimationEditor : AnimationClassEditor
    {
        private SpriteFont fntFinlanderFont;

        private Texture2D sprBarExtraLargeBackground;
        private Texture2D sprBarExtraLargeEN;
        private Texture2D sprBarExtraLargeHP;
        private Texture2D sprInfinity;

        public DeathmatchAnimationEditor(string AnimationPath)
            : base(AnimationPath)
        {
        }

        public override void Load()
        {
            base.Load();

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            sprBarExtraLargeBackground = Content.Load<Texture2D>("Battle/Bars/Extra Long Bar");
            sprBarExtraLargeEN = Content.Load<Texture2D>("Battle/Bars/Extra Long Energy");
            sprBarExtraLargeHP = Content.Load<Texture2D>("Battle/Bars/Extra Long Health");
            sprInfinity = Content.Load<Texture2D>("Battle/Infinity");
        }

        public override AnimationClass Copy()
        {
            DeathmatchAnimationEditor NewAnimationClass = new DeathmatchAnimationEditor(AnimationPath);

            NewAnimationClass.UpdateFrom(this);

            return NewAnimationClass;
        }

        protected void UpdateFrom(DeathmatchAnimationEditor Other)
        {
            base.UpdateFrom(Other);

            fntFinlanderFont = Other.fntFinlanderFont;

            sprBarExtraLargeBackground = Other.sprBarExtraLargeBackground;
            sprBarExtraLargeEN = Other.sprBarExtraLargeEN;
            sprBarExtraLargeHP = Other.sprBarExtraLargeHP;
            sprInfinity = Other.sprInfinity;
        }

        public override void DrawEditor(CustomSpriteBatch g, int ScreenWidth, int ScreenHeight, bool IsInEditMode, bool ShowBorderBoxes, bool ShowNextPositions, bool ShowUI)
        {
            base.DrawEditor(g, ScreenWidth, ScreenHeight, IsInEditMode, ShowBorderBoxes, ShowNextPositions, ShowUI);

            if (ShowUI)
            {
                DrawHUD(g, IsInEditMode, ShowBorderBoxes, ShowNextPositions);
            }
        }

        public void DrawHUD(CustomSpriteBatch g, bool IsInEditMode, bool ShowBorderBoxes, bool ShowNextPositions)
        {
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            DrawBox(g, new Vector2(0, 0), Width / 2, 84, Color.Red);
            int PosX = 0;
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(PosX + 75, 30), 100, 100);
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(PosX + 75, 50), 100, 100);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(PosX + 40, 20), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "5000/5000", new Vector2(PosX + 242, 17), Color.White);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(PosX + 40, 40), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "200/200", new Vector2(PosX + 242, 37), Color.White);
            g.Draw(sprPixel, new Rectangle(PosX + 7, 30, 32, 32), Color.White);

            PosX = Width / 2 + 68;
            DrawBox(g, new Vector2(Width / 2, 0), Width / 2, 84, Color.Blue);
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(PosX + 75, 30), 100, 100);
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(PosX + 75, 50), 100, 100);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(PosX + 40, 20), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "5000/5000", new Vector2(PosX + 242, 17), Color.White);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(PosX + 40, 40), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "200/200", new Vector2(PosX + 242, 37), Color.White);
            g.Draw(sprPixel, new Rectangle(PosX + 7, 30, 32, 32), Color.White);
            g.Draw(sprInfinity, new Vector2((Width - sprInfinity.Width) / 2, 15), Color.White);

            DrawBox(g, new Vector2(0, Height - VNBoxHeight), Width, VNBoxHeight, Color.White);

            g.End();
        }
    }
}
