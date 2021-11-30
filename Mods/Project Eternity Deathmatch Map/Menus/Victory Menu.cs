using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class VictoryMenu : GameScreen
    {
        #region Ressources

        #endregion

        #region Variables

        private SimpleAnimation VictoryAnimation;

        #endregion

        public VictoryMenu()
        {
        }

        public override void Load()
        {
            VictoryAnimation = new SimpleAnimation();
            VictoryAnimation.IsAnimated = true;
            VictoryAnimation.Path = "Game End";
            VictoryAnimation.Load(Content, string.Empty);
        }

        public override void Update(GameTime gameTime)
        {
            if (VictoryAnimation.ActiveAnimation.ActiveKeyFrame + 1 < VictoryAnimation.ActiveAnimation.LoopEnd)
            {
                VictoryAnimation.Update(gameTime);
            }

            if (InputHelper.InputConfirmPressed())
            {
                RemoveAllScreens();
                PushScreen(new MainMenu());
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            VictoryAnimation.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            VictoryAnimation.Draw(g, new Vector2(0, 0));
        }
    }
}
