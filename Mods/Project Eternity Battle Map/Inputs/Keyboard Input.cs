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
            if (!IsInZone(0, 0, Constants.Width, Constants.Height))
            {
                return false;
            }

            return KeyboardHelper.InputLeftPressed();
        }

        public bool InputRightPressed()
        {
            if (!IsInZone(0, 0, Constants.Width, Constants.Height))
            {
                return false;
            }

            return KeyboardHelper.InputRightPressed();
        }

        public bool InputUpPressed()
        {
            if (!IsInZone(0, 0, Constants.Width, Constants.Height))
            {
                return false;
            }

            return KeyboardHelper.InputUpPressed();
        }

        public bool InputDownPressed()
        {
            if (!IsInZone(0, 0, Constants.Width, Constants.Height))
            {
                return false;
            }

            return KeyboardHelper.InputDownPressed();
        }

        public bool InputMovePressed()
        {
            if (!IsInZone(0, 0, Constants.Width, Constants.Height))
            {
                return false;
            }

            return KeyboardHelper.InputLeftPressed() || KeyboardHelper.InputRightPressed() || KeyboardHelper.InputUpPressed() || KeyboardHelper.InputDownPressed()
                || MouseHelper.MouseMoved();
        }

        public bool InputConfirmPressed()
        {
            if (!IsInZone(0, 0, Constants.Width, Constants.Height))
            {
                return false;
            }

            return KeyboardHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonPressed();
        }

        public bool InputCancelPressed()
        {
            if (!IsInZone(0, 0, Constants.Width, Constants.Height))
            {
                return false;
            }

            return KeyboardHelper.InputCancelPressed() || MouseHelper.InputRightButtonPressed();
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

        public bool IsInZone(float MinX, float MinY, float MaxX, float MaxY)
        {
            return MouseHelper.MouseStateCurrent.X >= MinX && MouseHelper.MouseStateCurrent.X <= MaxX
                && MouseHelper.MouseStateCurrent.Y >= MinY && MouseHelper.MouseStateCurrent.Y <= MaxY;
        }
    }
}
