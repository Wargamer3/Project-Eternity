using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class EndScreen : GameScreen
    {
        Texture2D sprRect;
        Texture2D sprActiveBackground;

        FMODSound sndExecuteProgram;

        float FadeIn;
        float FadeInMessage;

        public EndScreen()
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            FadeIn = 0;
            FadeInMessage = 0;
        }

        public override void Load()
        {
            sprRect = Content.Load<Texture2D>("Rectangle");
            sprActiveBackground = Content.Load<Texture2D>("Message");
            if (SoundSystem.AudioFound)
                sndExecuteProgram = new FMODSound(FMODSystem, "Content/[Original] Execute Program Savior!.mp3");
        }

        public override void Update(GameTime gameTime)
        {
            if (FadeIn < 150)
                FadeIn++;
            else
            {
                if (FadeInMessage == 0)
                {
                    sndExecuteProgram.SetLoop(true);
                    sndExecuteProgram.PlayAsBGM();
                }
                if (FadeInMessage < 150)
                    FadeInMessage++;
            }
            if (InputHelper.InputConfirmPressed() || InputHelper.InputCancelPressed() || InputHelper.InputSkipPressed())
            {
                SoundSystem.ReleaseSound(sndExecuteProgram);
                RemoveAllScreens();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprRect, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.FromNonPremultiplied(255, 255, 255, (int)((FadeIn / 150) * 255)));
            g.Draw(sprActiveBackground, Vector2.Zero, Color.FromNonPremultiplied(255, 255, 255, (int)((FadeInMessage / 150) * 255)));
        }
    }
}