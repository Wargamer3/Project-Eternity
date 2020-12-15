using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public interface RobotInputManager
    {
        RobotInput GetRobotInput(GameplayTypes GameplayType, RobotAnimation Owner, Rectangle CameraBounds);
    }
}
