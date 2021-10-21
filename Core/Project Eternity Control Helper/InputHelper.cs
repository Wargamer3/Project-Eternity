namespace ProjectEternity.Core.ControlHelper
{
    public class InputHelper
    {
        #region Key press

        public static bool InputLeftPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputLeftStatus);
        }
        public static bool InputRightPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputRightStatus);
        }
        public static bool InputUpPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputUpStatus);
        }
        public static bool InputDownPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputDownStatus);
        }
        public static bool InputConfirmPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputConfirmStatus) || MouseHelper.InputLeftButtonPressed();
        }
        public static bool InputCancelPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputCancelStatus) || MouseHelper.InputRightButtonPressed();
        }
        public static bool InputCommand1Pressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputCommand1Status);
        }
        public static bool InputCommand2Pressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputCommand2Status);
        }
        public static bool InputLButtonPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputLButtonStatus);
        }
        public static bool InputRButtonPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputRButtonStatus);
        }
        public static bool InputSkipPressed()
        {
            return KeyboardHelper.InputPressed(KeyboardHelper.InputSkipStatus);
        }

        #endregion

        #region Key release

        public static bool InputLeftReleased()
        {
            for (int K = 0; K < KeyboardHelper.MoveLeft.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.MoveLeft[K]))
                    return true;

            return false;
        }
        public static bool InputRightReleased()
        {
            for (int K = 0; K < KeyboardHelper.MoveRight.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.MoveRight[K]))
                    return true;

            return false;
        }
        public static bool InputUpReleased()
        {
            for (int K = 0; K < KeyboardHelper.MoveUp.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.MoveUp[K]))
                    return true;

            return false;
        }
        public static bool InputDownReleased()
        {
            for (int K = 0; K < KeyboardHelper.MoveDown.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.MoveDown[K]))
                    return true;

            return false;
        }
        public static bool InputConfirmReleased()
        {
            for (int K = 0; K < KeyboardHelper.ConfirmChoice.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.ConfirmChoice[K]))
                    return true;

            return MouseHelper.InputLeftButtonPressed();
        }
        public static bool InputCancelReleased()
        {
            for (int K = 0; K < KeyboardHelper.CancelChoice.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.CancelChoice[K]))
                    return true;

            return MouseHelper.InputRightButtonPressed();
        }
        public static bool InputCommand1Released()
        {
            for (int K = 0; K < KeyboardHelper.Command1.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.Command1[K]))
                    return true;

            return false;
        }
        public static bool InputCommand2Released()
        {
            for (int K = 0; K < KeyboardHelper.Command2.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.Command2[K]))
                    return true;

            return false;
        }
        public static bool InputLButtonReleased()
        {
            for (int K = 0; K < KeyboardHelper.LButton.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.LButton[K]))
                    return true;

            return false;
        }
        public static bool InputRButtonReleased()
        {
            for (int K = 0; K < KeyboardHelper.RButton.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.RButton[K]))
                    return true;

            return false;
        }
        public static bool InputSkipReleased()
        {
            for (int K = 0; K < KeyboardHelper.Skip.Length; K++)
                if (KeyboardHelper.KeyReleased(KeyboardHelper.Skip[K]))
                    return true;

            return false;
        }

        #endregion

        #region Key hold

        public static bool InputLeftHold()
        {
            return KeyboardHelper.InputLeftStatus > 0;
        }
        public static bool InputRightHold()
        {
            return KeyboardHelper.InputRightStatus > 0;
        }
        public static bool InputUpHold()
        {
            return KeyboardHelper.InputUpStatus > 0;
        }
        public static bool InputDownHold()
        {
            return KeyboardHelper.InputDownStatus > 0;
        }
        public static bool InputConfirmHold()
        {
            return KeyboardHelper.InputConfirmStatus > 0 || MouseHelper.InputLeftButtonHold();
        }
        public static bool InputCancelHold()
        {
            return KeyboardHelper.InputCancelStatus > 0 || MouseHelper.InputRightButtonHold();
        }
        public static bool InputCommand1Hold()
        {
            return KeyboardHelper.InputCommand1Status > 0;
        }
        public static bool InputCommand2Hold()
        {
            return KeyboardHelper.InputCommand2Status > 0;
        }
        public static bool InputLButtonHold()
        {
            return KeyboardHelper.InputLButtonStatus > 0;
        }
        public static bool InputRButtonHold()
        {
            return KeyboardHelper.InputRButtonStatus > 0;
        }
        public static bool InputSkipHold()
        {
            return KeyboardHelper.InputSkipStatus > 0;
        }

        #endregion

        public static void ResetState()
        {
            KeyboardHelper.ResetState();
            MouseHelper.ResetState();
        }
    }
}
