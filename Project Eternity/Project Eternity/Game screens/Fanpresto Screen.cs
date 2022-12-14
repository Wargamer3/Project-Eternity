using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity
{
    public sealed class FanprestoScreen : GameScreen
    {
        Texture2D sprFanpresto;
        Texture2D sprDisclaimer;

        FMODSound sndFanpresto;

        int Timer;
        float FadeIn;
        float FadeIn2;
        float FadeOut;

        public FanprestoScreen()
            : base()
        {
            RequireDrawFocus = true;
            RequireFocus = true;

            Timer = 0;
            FadeIn = 0;
            FadeIn2 = 0;
            FadeOut = 100;
        }

        public override void Load()
        {
            sprFanpresto = Content.Load<Texture2D>("Menus/Title Screen/Fanpresto");
            sprDisclaimer = Content.Load<Texture2D>("Menus/Title Screen/Disclaimer");
            sndFanpresto = new FMODSound(FMODSystem, "Content/[Original] Fanpresto.mp3");
        }

        public override void Update(GameTime gameTime)
        {
            if (Timer < 100)
                Timer++;
            else
            {
                if (FadeIn == 0)
                {
                    sndFanpresto.PlayAsBGM();
                }
                if (FadeIn < 300)
                    FadeIn++;
                else if (FadeIn == 300 && FadeIn2 < 300)
                    FadeIn2++;
                else if (FadeIn2 == 300)
                {
                    FadeOut--;
                    if (FadeOut == 0)
                    {
                        SoundSystem.ReleaseSound(sndFanpresto);
                        PushScreen(new MainMenu());
                        RemoveScreen(this);
                    }
                }
            }
            if (InputHelper.InputConfirmPressed() || InputHelper.InputCancelPressed() || InputHelper.InputSkipPressed())
            {
                SoundSystem.ReleaseSound(sndFanpresto);

                PushScreen(new MainMenu());
                RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (FadeOut == 100)
            {
                if (Timer == 100)
                    g.Draw(sprFanpresto, Vector2.Zero, Color.FromNonPremultiplied(255, 255, 255, (int)((FadeIn / 100) * 255)));
                if (FadeIn == 300)
                    g.Draw(sprDisclaimer, Vector2.Zero, Color.FromNonPremultiplied(255, 255, 255, (int)((FadeIn2 / 150) * 255)));
            }
            else
                g.Draw(sprDisclaimer, Vector2.Zero, Color.FromNonPremultiplied(255, 255, 255, (int)((FadeOut / 100) * 255)));
        }
    }
}
