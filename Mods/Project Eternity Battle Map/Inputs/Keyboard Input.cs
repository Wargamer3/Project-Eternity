using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class KeyboardInput : PlayerInput
    {
        public bool InputLeftPressed()
        {
            return KeyboardHelper.InputLeftPressed();
        }

        public bool InputRightPressed()
        {
            return KeyboardHelper.InputRightPressed();
        }

        public bool InputUpPressed()
        {
            return KeyboardHelper.InputUpPressed();
        }

        public bool InputDownPressed()
        {
            return KeyboardHelper.InputDownPressed();
        }

        public bool InputMovePressed()
        {
            return IsInZone(0, 0, Constants.Width, Constants.Height) && MouseHelper.MouseMoved();
        }

        public bool InputConfirmPressed()
        {
            return KeyboardHelper.InputConfirmPressed() || (IsInZone(0, 0, Constants.Width, Constants.Height) && MouseHelper.InputLeftButtonPressed());
        }

        public bool InputConfirmPressed(float MinX, float MinY, float MaxX, float MaxY)
        {
            return KeyboardHelper.InputConfirmPressed() || (IsInZone(MinX, MinY, MaxX, MaxY) && MouseHelper.InputLeftButtonPressed());
        }

        public bool InputCancelPressed()
        {
            return KeyboardHelper.InputCancelPressed() || (IsInZone(0, 0, Constants.Width, Constants.Height) && MouseHelper.InputRightButtonPressed());
        }

        public bool InputCommand1Pressed()
        {
            return KeyboardHelper.InputCommand1Pressed();
        }

        public bool InputCommand2Pressed()
        {
            return KeyboardHelper.InputCommand2Pressed();
        }

        public bool InputSkipPressed()
        {
            return KeyboardHelper.InputSkipPressed();
        }

        public bool InputLeftHold()
        {
            return InputHelper.InputLeftHold();
        }

        public bool InputRightHold()
        {
            return InputHelper.InputRightHold();
        }

        public bool InputUpHold()
        {
            return InputHelper.InputUpHold();
        }

        public bool InputDownHold()
        {
            return InputHelper.InputDownHold();
        }

        public bool IsInZone(float MinX, float MinY, float MaxX, float MaxY)
        {
            return MouseHelper.MouseStateCurrent.X >= MinX && MouseHelper.MouseStateCurrent.X <= MaxX
                && MouseHelper.MouseStateCurrent.Y >= MinY && MouseHelper.MouseStateCurrent.Y <= MaxY;
        }
    }
}
