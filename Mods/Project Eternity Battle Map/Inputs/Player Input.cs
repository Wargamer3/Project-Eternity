using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public enum GameplayTypes { None, Keyboard, MouseAndKeyboard, Controller1, Controller2, Controller3, Controller4 }

    public interface PlayerInput
    {
        bool InputDownPressed();
        bool InputLeftPressed();
        bool InputRightPressed();
        bool InputUpPressed();
        bool InputMovePressed();
        bool InputConfirmPressed();
        bool InputCancelPressed();
        bool InputCommand1Pressed();
        bool InputCommand2Pressed();
        bool InputSkipPressed();
        bool IsInZone(float MinX, float MinY, float MaxX, float MaxY);
    }
}
