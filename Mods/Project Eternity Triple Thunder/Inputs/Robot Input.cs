using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public enum GameplayTypes { None, MouseAndKeyboard, Controller1, Controller2, Controller3, Controller4 }

    public interface RobotInput
    {
        Rectangle CameraBounds { get; }

        void Update(GameTime gameTime);

        void ResetCameraBounds(Rectangle CameraBounds);
    }
}
