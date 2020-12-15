using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectEternity.Core.ControlHelper
{
    public class ControllerHelper
    {
        private GamePadState LasGamePadState = new GamePadState();
        private GamePadState CurrentGamePadState = new GamePadState();
        private PlayerIndex ControllerIndex;

        public ControllerHelper(PlayerIndex ControllerIndex)
        {
            this.ControllerIndex = ControllerIndex;
        }

        public void Update()
        {
            // store off the last state before reading the new one:
            LasGamePadState = CurrentGamePadState;
            CurrentGamePadState = GamePad.GetState(ControllerIndex);
        }

        public bool IsButtonPressed(Buttons ButtonToCheck)
        {
            return CurrentGamePadState.IsButtonDown(ButtonToCheck) && LasGamePadState.IsButtonUp(ButtonToCheck);
        }

        public bool IsButtonReleased(Buttons ButtonToCheck)
        {
            return CurrentGamePadState.IsButtonUp(ButtonToCheck) && LasGamePadState.IsButtonDown(ButtonToCheck);
        }

        public bool IsButtonDown(Buttons ButtonToCheck)
        {
            return CurrentGamePadState.IsButtonDown(ButtonToCheck);
        }

        public Vector2 RightStickPosition()
        {
            return CurrentGamePadState.ThumbSticks.Right;
        }
    }
}
