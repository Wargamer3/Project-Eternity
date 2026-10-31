using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class GamepadInput : PlayerInput
    {
        const float Dead = 0.5f;

        readonly ControllerHelper Pad;
        bool WasLeft;
        bool WasRight;
        bool WasUp;
        bool WasDown;

        public GamepadInput(ControllerHelper Pad)
        {
            this.Pad = Pad;
        }

        public bool PreviousPressed()
        {
            return Pad != null && Pad.IsButtonPressed(Buttons.LeftTrigger);
        }

        public bool NextPressed()
        {
            return Pad != null && Pad.IsButtonPressed(Buttons.RightTrigger);
        }

        public bool JumpPressed()
        {
            return Pad != null && Pad.IsButtonPressed(Buttons.A);
        }

        public bool BuildPressed()
        {
            return Pad != null && Pad.IsButtonPressed(Buttons.Start);
        }

        public void Remember()
        {
            WasLeft = InputLeftHold();
            WasRight = InputRightHold();
            WasUp = InputUpHold();
            WasDown = InputDownHold();
        }

        public bool InputLeftPressed()
        {
            return Pressed(Buttons.DPadLeft, InputLeftHold(), WasLeft);
        }

        public bool InputRightPressed()
        {
            return Pressed(Buttons.DPadRight, InputRightHold(), WasRight);
        }

        public bool InputUpPressed()
        {
            return Pressed(Buttons.DPadUp, InputUpHold(), WasUp);
        }

        public bool InputDownPressed()
        {
            return Pressed(Buttons.DPadDown, InputDownHold(), WasDown);
        }

        public bool InputMovePressed()
        {
            return false;
        }

        public bool InputConfirmPressed()
        {
            return Pad != null && Pad.IsButtonPressed(Buttons.X);
        }

        public bool InputConfirmPressed(float MinX, float MinY, float MaxX, float MaxY)
        {
            return InputConfirmPressed();
        }

        public bool InputCancelPressed()
        {
            return Pad != null && Pad.IsButtonPressed(Buttons.B);
        }

        public bool InputCommand1Pressed()
        {
            return false;
        }

        public bool InputCommand2Pressed()
        {
            return false;
        }

        public bool InputLButtonPressed()
        {
            return PreviousPressed();
        }

        public bool InputRButtonPressed()
        {
            return NextPressed();
        }

        public bool InputSkipPressed()
        {
            return false;
        }

        public bool InputLeftHold()
        {
            return Held(Buttons.DPadLeft, Stick().X < -Dead);
        }

        public bool InputRightHold()
        {
            return Held(Buttons.DPadRight, Stick().X > Dead);
        }

        public bool DPadUpHeld()
        {
            return Pad != null && Pad.IsButtonDown(Buttons.DPadUp);
        }

        public bool InputUpHold()
        {
            return Stick().Y > Dead;
        }

        public bool InputDownHold()
        {
            return Held(Buttons.DPadDown, Stick().Y < -Dead);
        }

        public bool IsInZone(float MinX, float MinY, float MaxX, float MaxY)
        {
            return false;
        }

        bool Pressed(Buttons Button, bool Hold, bool Was)
        {
            if (Pad != null && Pad.IsButtonPressed(Button))
            {
                return true;
            }

            return Hold && !Was;
        }

        bool Held(Buttons Button, bool Stick)
        {
            if (Stick)
            {
                return true;
            }

            return Pad != null && Pad.IsButtonDown(Button);
        }

        Vector2 Stick()
        {
            if (Pad == null)
            {
                return Vector2.Zero;
            }

            return Pad.LeftStickPosition();
        }
    }

    public class CombinedInput : PlayerInput
    {
        readonly List<PlayerInput> Sources;

        public CombinedInput(List<PlayerInput> Sources)
        {
            this.Sources = Sources ?? new List<PlayerInput>();
        }

        public bool InputLeftPressed()
        {
            return Any(Source => Source.InputLeftPressed());
        }

        public bool InputRightPressed()
        {
            return Any(Source => Source.InputRightPressed());
        }

        public bool InputUpPressed()
        {
            return Any(Source => Source.InputUpPressed());
        }

        public bool InputDownPressed()
        {
            return Any(Source => Source.InputDownPressed());
        }

        public bool InputMovePressed()
        {
            return Any(Source => Source.InputMovePressed());
        }

        public bool InputConfirmPressed()
        {
            return Any(Source => Source.InputConfirmPressed());
        }

        public bool InputConfirmPressed(float MinX, float MinY, float MaxX, float MaxY)
        {
            return Any(Source => Source.InputConfirmPressed(MinX, MinY, MaxX, MaxY));
        }

        public bool InputCancelPressed()
        {
            return Any(Source => Source.InputCancelPressed());
        }

        public bool InputCommand1Pressed()
        {
            return Any(Source => Source.InputCommand1Pressed());
        }

        public bool InputCommand2Pressed()
        {
            return Any(Source => Source.InputCommand2Pressed());
        }

        public bool InputLButtonPressed()
        {
            return Any(Source => Source.InputLButtonPressed());
        }

        public bool InputRButtonPressed()
        {
            return Any(Source => Source.InputRButtonPressed());
        }

        public bool InputSkipPressed()
        {
            return Any(Source => Source.InputSkipPressed());
        }

        public bool InputLeftHold()
        {
            return Any(Source => Source.InputLeftHold());
        }

        public bool InputRightHold()
        {
            return Any(Source => Source.InputRightHold());
        }

        public bool InputUpHold()
        {
            return Any(Source => Source.InputUpHold());
        }

        public bool InputDownHold()
        {
            return Any(Source => Source.InputDownHold());
        }

        public bool IsInZone(float MinX, float MinY, float MaxX, float MaxY)
        {
            return Any(Source => Source.IsInZone(MinX, MinY, MaxX, MaxY));
        }

        bool Any(System.Func<PlayerInput, bool> Read)
        {
            for (int i = 0; i < Sources.Count; i++)
            {
                if (Sources[i] != null && Read(Sources[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
