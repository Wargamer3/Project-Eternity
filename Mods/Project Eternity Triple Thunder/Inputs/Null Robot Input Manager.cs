using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class NullRobotInputManager : RobotInputManager
    {
        public RobotInput GetRobotInput(GameplayTypes GameplayType, RobotAnimation Owner, Rectangle CameraBounds)
        {
            return new NullRobotInput();
        }
    }
}
