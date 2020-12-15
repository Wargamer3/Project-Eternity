using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ControllerRobotInput : RobotInputHelper
    {
        private ControllerHelper Controller;

        private Buttons MoveLeftButtons;
        private Buttons MoveRightButtons;
        private Buttons JumpButtons;
        private Buttons CrouchButtons;
        private Buttons ShootButtons;

        public ControllerRobotInput(RobotAnimation Owner, Rectangle CameraBounds, PlayerIndex ControllerIndex)
            : base(Owner, CameraBounds)
        {
            this.Controller = new ControllerHelper(ControllerIndex);

            MoveLeftButtons = Buttons.LeftThumbstickLeft;
            MoveRightButtons = Buttons.LeftThumbstickRight;
            JumpButtons = Buttons.A;
            CrouchButtons = Buttons.LeftThumbstickDown;
            ShootButtons = Buttons.RightTrigger;
        }

        public override void Update(GameTime gameTime)
        {
            Controller.Update();

            UpdateOwner(gameTime);

            Vector2 AimTarget = Controller.RightStickPosition();
            AimTarget.Y = -AimTarget.Y;

            Vector2 Target = new Vector2(Owner.Position.X + AimTarget.X * Constants.Width, Owner.Position.Y + AimTarget.Y * Constants.Height);

            Owner.UpdateAllWeaponsAngle(AimTarget);
            UpdateCamera(Target);
        }

        private void UpdateOwner(GameTime gameTime)
        {
            bool IsIdle = true;
            if (Controller.IsButtonDown(MoveRightButtons))
            {
                IsIdle = false;
                Owner.Move(MovementInputs.Right);
            }
            else if (Controller.IsButtonDown(MoveLeftButtons))
            {
                IsIdle = false;
                Owner.Move(MovementInputs.Left);
            }

            if (Controller.IsButtonDown(JumpButtons))
            {
                IsIdle = false;
                Owner.Jump();
            }
            else if (Controller.IsButtonDown(CrouchButtons))
            {
                IsIdle = false;
                Owner.Crouch();
                //Disabled until I can specify which platforms to not fall through.
                //Owner.FallThroughFloor();
            }
            else if (KeyboardHelper.KeyHold(Keys.X))
            {
                IsIdle = false;
                Owner.GoProne();
            }

            if (IsIdle)
            {
                Owner.SetIdle();
            }

            if (Controller.IsButtonReleased(JumpButtons))
            {
                Owner.StopJump();
            }

            if (KeyboardHelper.KeyHold(Keys.Space))
            {
                Owner.UseJetpack(gameTime);
            }
            else
            {
                Owner.Freefall(gameTime);
            }

            if (Controller.IsButtonPressed(ShootButtons))
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightPress);
            }
            else if (Controller.IsButtonDown(ShootButtons))
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightHold);
            }

            if (MouseHelper.InputRightButtonPressed())
            {
                Owner.HolsterAndReplaceWeapon(Owner.Weapons.ActiveSecondaryWeapons[0]);
            }
            else if (MouseHelper.InputRightButtonReleased())
            {
                Owner.UseCombo(gameTime, AttackInputs.HeavyPress, Owner.Weapons.ActiveSecondaryWeapons[0], true);
                Owner.UnholsterWeaponsIfNeeded();
            }
            else if (MouseHelper.InputRightButtonHold())
            {
                Owner.UseCombo(gameTime, AttackInputs.HeavyHold, Owner.Weapons.ActiveSecondaryWeapons[0], false);
            }
        }
    }
}
