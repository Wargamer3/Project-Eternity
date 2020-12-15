using Microsoft.Xna.Framework.Input;

namespace ProjectEternity.Core.ControlHelper
{
    public class MouseHelper
    {
        public static MouseState MouseStateCurrent;
        public static MouseState MouseStateLast;

        public static bool InputLeftButtonHold()
        {
            if (IsMouseOutOfBound())
            {
                return false;
            }
            else
            {
                return MouseStateCurrent.LeftButton == ButtonState.Pressed;
            }
        }

        public static bool InputRightButtonHold()
        {
            if (IsMouseOutOfBound())
            {
                return false;
            }
            else
            {
                return MouseStateCurrent.RightButton == ButtonState.Pressed;
            }
        }

        public static bool InputLeftButtonPressed()
        {
            if (IsMouseOutOfBound())
            {
                return false;
            }
            else
            {
                return MouseStateCurrent.LeftButton == ButtonState.Pressed && MouseStateLast.LeftButton == ButtonState.Released;
            }
        }

        public static bool InputRightButtonPressed()
        {
            if (IsMouseOutOfBound())
            {
                return false;
            }
            else
            {
                return MouseStateCurrent.RightButton == ButtonState.Pressed && MouseStateLast.RightButton == ButtonState.Released;
            }
        }

        public static bool InputLeftButtonReleased()
        {
            if (IsMouseOutOfBound())
            {
                return false;
            }
            else
            {
                return MouseStateCurrent.LeftButton == ButtonState.Released && MouseStateLast.LeftButton == ButtonState.Pressed;
            }
        }

        public static bool InputRightButtonReleased()
        {
            if (IsMouseOutOfBound())
            {
                return false;
            }
            else
            {
                return MouseStateCurrent.RightButton == ButtonState.Released && MouseStateLast.RightButton == ButtonState.Pressed;
            }
        }

        public static bool MouseMoved()
        {
            if (IsMouseOutOfBound())
            {
                return false;
            }
            else
            {
                return MouseStateCurrent.X != MouseStateLast.X || MouseStateCurrent.Y != MouseStateLast.Y;
            }
        }

        private static bool IsMouseOutOfBound()
        {
            return MouseStateCurrent.X < 0 || MouseStateCurrent.X > Constants.Width ||
                MouseStateCurrent.Y < 0 || MouseStateCurrent.Y > Constants.Height;
        }
    }
}
