using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class GameOverMenu : GameScreen
    {
        #region Ressources

        #endregion

        #region Variables

        private SimpleAnimation GameOverAnimation;

        #endregion

        public GameOverMenu()
        {
        }

        public override void Load()
        {
            GameOverAnimation = new SimpleAnimation();
            GameOverAnimation.IsAnimated = true;
            GameOverAnimation.Path = "Game Over";
            GameOverAnimation.Load(Content, string.Empty);
        }

        public override void Update(GameTime gameTime)
        {
            if (GameOverAnimation.ActiveAnimation.ActiveKeyFrame + 1 < GameOverAnimation.ActiveAnimation.LoopEnd)
            {
                GameOverAnimation.Update(gameTime);
            }

            if (InputHelper.InputConfirmPressed())
            {
                RemoveAllScreens();
                PushScreen(new MainMenu());
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            GameOverAnimation.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameOverAnimation.Draw(g, new Vector2(0, 0));
        }
    }
}
