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
    }
}
