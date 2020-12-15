using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class NullRobotInput : RobotInput
    {
        public Rectangle CameraBounds { get; private set; }

        public void Update(GameTime gameTime)
        {
        }

        public void ResetCameraBounds(Rectangle CameraBounds)
        {
            this.CameraBounds = CameraBounds;
        }
    }
}
