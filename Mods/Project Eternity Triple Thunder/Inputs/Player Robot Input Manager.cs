using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class PlayerRobotInputManager : RobotInputManager
    {
        public RobotInput GetRobotInput(GameplayTypes GameplayType, RobotAnimation Owner, Rectangle CameraBounds)
        {
            switch (GameplayType)
            {
                case GameplayTypes.MouseAndKeyboard:
                    return new KeyboardAndMouseRobotInput(Owner, CameraBounds);

                case GameplayTypes.Controller1:
                    return new ControllerRobotInput(Owner, CameraBounds, PlayerIndex.One);

                case GameplayTypes.Controller2:
                    return new ControllerRobotInput(Owner, CameraBounds, PlayerIndex.Two);

                case GameplayTypes.Controller3:
                    return new ControllerRobotInput(Owner, CameraBounds, PlayerIndex.Three);

                case GameplayTypes.Controller4:
                    return new ControllerRobotInput(Owner, CameraBounds, PlayerIndex.Four);
            }

            return new NullRobotInput();
        }
    }
}
