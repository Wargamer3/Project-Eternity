using Microsoft.Xna.Framework.Input;

namespace ProjectEternity.Core.ControlHelper
{
    public class KeyboardHelper
    {
        private enum InputStatus { FirstPress = 1, SecondPress = 50, ThirdPress = 80, FourthPress = 110, LastPress = 140 };

        public static KeyboardState PlayerState;
        public static KeyboardState PlayerStateLast;

        public static Keys[] MoveLeft = new Keys[] { Keys.Left };
        public static Keys[] MoveRight = new Keys[] { Keys.Right };
        public static Keys[] MoveUp = new Keys[] { Keys.Up };
        public static Keys[] MoveDown = new Keys[] { Keys.Down };
        public static Keys[] ConfirmChoice = new Keys[] { Keys.X };
        public static Keys[] CancelChoice = new Keys[] { Keys.Z };
        public static Keys[] Command1 = new Keys[] { Keys.A };
        public static Keys[] Command2 = new Keys[] { Keys.S };
        public static Keys[] LButton = new Keys[] { Keys.D };
        public static Keys[] RButton = new Keys[] { Keys.C };
        public static Keys[] Skip = new Keys[] { Keys.Escape, Keys.Enter };

        internal static int InputLeftStatus = 0;
        internal static int InputRightStatus = 0;
        internal static int InputUpStatus = 0;
        internal static int InputDownStatus = 0;
        internal static int InputConfirmStatus = 0;
        internal static int InputCancelStatus = 0;
        internal static int InputCommand1Status = 0;
        internal static int InputCommand2Status = 0;
        internal static int InputLButtonStatus = 0;
        internal static int InputRButtonStatus = 0;
        internal static int InputSkipStatus = 0;

        internal static bool InputPressed(int StatusValue)
        {
            if (StatusValue == 0)
                return false;
            else if (StatusValue == (int)InputStatus.FirstPress)
                return true;
            else if (StatusValue <= (int)InputStatus.SecondPress)
            {
                if (StatusValue % 50 == 0)
                    return true;
            }
            else if (StatusValue <= (int)InputStatus.ThirdPress)
            {
                if (StatusValue % 10 == 0)
                    return true;
            }
            else if (StatusValue <= (int)InputStatus.FourthPress)
            {
                if (StatusValue % 5 == 0)
                    return true;
            }
            else if (StatusValue <= (int)InputStatus.LastPress)
            {
                if (StatusValue % 2 == 0)
                    return true;
            }
            else
                return true;
            return false;
        }
        
        public static void UpdateKeyboardStatus()
        {
            bool KeyPressed;

            #region Left

            KeyPressed = false;

            for (int K = 0; K < MoveLeft.Length; K++)
                if (PlayerState.IsKeyDown(MoveLeft[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputLeftStatus++;
            else
                InputLeftStatus = 0;

            #endregion

            #region Right

            KeyPressed = false;

            for (int K = 0; K < MoveRight.Length; K++)
                if (PlayerState.IsKeyDown(MoveRight[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputRightStatus++;
            else
                InputRightStatus = 0;

            #endregion

            #region Up

            KeyPressed = false;

            for (int K = 0; K < MoveUp.Length; K++)
                if (PlayerState.IsKeyDown(MoveUp[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputUpStatus++;
            else
                InputUpStatus = 0;

            #endregion

            #region Down

            KeyPressed = false;

            for (int K = 0; K < MoveDown.Length; K++)
                if (PlayerState.IsKeyDown(MoveDown[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputDownStatus++;
            else
                InputDownStatus = 0;

            #endregion

            #region Confirm

            KeyPressed = false;

            for (int K = 0; K < ConfirmChoice.Length; K++)
                if (PlayerState.IsKeyDown(ConfirmChoice[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputConfirmStatus++;
            else
                InputConfirmStatus = 0;

            #endregion

            #region Cancel

            KeyPressed = false;

            for (int K = 0; K < CancelChoice.Length; K++)
                if (PlayerState.IsKeyDown(CancelChoice[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputCancelStatus++;
            else
                InputCancelStatus = 0;

            #endregion

            #region Command 1

            KeyPressed = false;

            for (int K = 0; K < Command1.Length; K++)
                if (PlayerState.IsKeyDown(Command1[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputCommand1Status++;
            else
                InputCommand1Status = 0;

            #endregion

            #region Command 2

            KeyPressed = false;

            for (int K = 0; K < Command2.Length; K++)
                if (PlayerState.IsKeyDown(Command2[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputCommand2Status++;
            else
                InputCommand2Status = 0;

            #endregion

            #region L Button

            KeyPressed = false;

            for (int K = 0; K < LButton.Length; K++)
                if (PlayerState.IsKeyDown(LButton[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputLButtonStatus++;
            else
                InputLButtonStatus = 0;

            #endregion

            #region R Button

            KeyPressed = false;

            for (int K = 0; K < RButton.Length; K++)
                if (PlayerState.IsKeyDown(RButton[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputRButtonStatus++;
            else
                InputRButtonStatus = 0;

            #endregion

            #region Skip

            KeyPressed = false;

            for (int K = 0; K < Skip.Length; K++)
                if (PlayerState.IsKeyDown(Skip[K]))
                    KeyPressed = true;

            if (KeyPressed)
                InputSkipStatus++;
            else
                InputSkipStatus = 0;

            #endregion

            KeyboardHelper.PlayerState = Keyboard.GetState();
        }

        /// <summary>
        /// Determine if a key is pressed.
        /// </summary>
        /// <param name="K">The key to use for the test.</param>
        /// <returns>True if the key was found; else, false.</returns>
        public static bool KeyPressed(Keys K)
        {//If the key is currently pressed and was not already pressed before
            return PlayerState.IsKeyDown(K) && PlayerStateLast.IsKeyUp(K);
        }

        /// <summary>
        /// Determine if a key is released.
        /// </summary>
        /// <param name="K">The key to use for the test.</param>
        /// <returns>True if the key was found; else, false.</returns>
        public static bool KeyReleased(Keys K)
        {//If the key is currently pressed and was not already pressed before
            return PlayerStateLast.IsKeyDown(K) && PlayerState.IsKeyUp(K);
        }

        public static Keys[] KeyPressed()
        {
            return Keyboard.GetState().GetPressedKeys();
        }

        /// <summary>
        /// Determine if a key is holden.
        /// </summary>
        /// <param name="K">The key to use for the test.</param>
        /// <returns>True if the key was found; else, false.</returns>
        public static bool KeyHold(Keys K)
        {
            return PlayerState.IsKeyDown(K);
        }
    }
}
