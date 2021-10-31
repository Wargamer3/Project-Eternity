using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class KeyboardAndMouseRobotInput : RobotInputHelper
    {
        private enum InputStatus { FirstPress = 1, SecondPress = 50, ThirdPress = 80, FourthPress = 110, LastPress = 140 };

        public Keys[] MoveLeft = new Keys[] { Keys.Left };
        public Keys[] MoveRight = new Keys[] { Keys.Right };
        public Keys[] MoveUp = new Keys[] { Keys.Up };
        public Keys[] MoveDown = new Keys[] { Keys.Down };

        private double GrenadeCooldown;

        public KeyboardAndMouseRobotInput(RobotAnimation Owner, Rectangle CameraBounds)
            : base(Owner, CameraBounds)
        {
            this.Owner = Owner;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState MousePosition = MouseHelper.MouseStateCurrent;

            if (Owner.RespawnTimer <= 0)
            {
                UpdateOwner(gameTime);

                Owner.UpdateAllWeaponsAngle(new Vector2(MousePosition.X + Owner.Camera.X - Owner.Position.X, MousePosition.Y + Owner.Camera.Y - Owner.Position.Y));
            }

            UpdateCamera(new Vector2(MousePosition.X, MousePosition.Y));
        }

        private void UpdateOwner(GameTime gameTime)
        {
            if (GrenadeCooldown > 0)
            {
                GrenadeCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
            }

            bool IsIdle = true;
            if (InputHelper.InputRightHold() || KeyboardHelper.KeyHold(Keys.D))
            {
                IsIdle = false;
                Owner.Move(MovementInputs.Right);
            }
            else if (InputHelper.InputLeftHold() || KeyboardHelper.KeyHold(Keys.A))
            {
                IsIdle = false;
                Owner.Move(MovementInputs.Left);
            }

            if (InputHelper.InputUpHold() || KeyboardHelper.KeyHold(Keys.W))
            {
                IsIdle = false;
                Owner.Jump();
            }
            else if (InputHelper.InputDownPressed() || KeyboardHelper.KeyPressed(Keys.S))
            {
                IsIdle = false;
                Owner.StartCrouch();
                //Disabled until I can specify which platforms to not fall through.
                //Owner.FallThroughFloor();
            }
            else if (InputHelper.InputDownHold() || KeyboardHelper.KeyHold(Keys.S))
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

            if (InputHelper.InputUpReleased() || KeyboardHelper.KeyReleased(Keys.W))
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

            if (KeyboardHelper.KeyPressed(Keys.R))
            {
                for (int W = 0; W < Owner.Weapons.ActivePrimaryWeapons.Count; W++)
                {
                    Weapon ActiveWeapon = Owner.Weapons.ActivePrimaryWeapons[W];
                    if (ActiveWeapon.ReloadCombo != null && ActiveWeapon.ActiveCombo == null)
                    {
                        ActiveWeapon.ActiveCombo = ActiveWeapon.NoneCombo;
                    }
                }
                Owner.Reload();
            }
            else if (KeyboardHelper.KeyPressed(Keys.LeftShift) || KeyboardHelper.KeyPressed(Keys.RightShift))
            {
                Owner.DropActiveWeapons();
            }
            else if (MouseHelper.InputLeftButtonPressed())
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightPress);
            }
            else if (MouseHelper.InputLeftButtonHold())
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightHold);
            }

            if (GrenadeCooldown <= 0)
            {
                if (MouseHelper.InputRightButtonPressed())
                {
                    if (Owner.Weapons.ActiveSecondaryWeapons.Count > 0)
                    {
                        Owner.HolsterAndReplaceWeapon(Owner.Weapons.ActiveSecondaryWeapons[0]);
                    }
                    else
                    {
                        Owner.UseCombo(gameTime, AttackInputs.HeavyPress);
                    }
                }
                else if (MouseHelper.InputRightButtonReleased())
                {
                    if (Owner.Weapons.ActiveSecondaryWeapons.Count > 0)
                    {
                        Owner.UseCombo(gameTime, AttackInputs.HeavyPress, Owner.Weapons.ActiveSecondaryWeapons[0], true);
                        Owner.UnholsterWeaponsIfNeeded();
                        GrenadeCooldown = 1;
                    }
                }
                else if (MouseHelper.InputRightButtonHold())
                {
                    if (Owner.Weapons.ActiveSecondaryWeapons.Count > 0)
                    {
                        Owner.UseCombo(gameTime, AttackInputs.HeavyHold, Owner.Weapons.ActiveSecondaryWeapons[0], false);
                    }
                    else
                    {
                        Owner.UseCombo(gameTime, AttackInputs.HeavyHold);
                    }
                }
            }

            if (KeyboardHelper.KeyPressed(Keys.D1))
            {
                Owner.ChangeWeapon(-1);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D2))
            {
                Owner.ChangeWeapon(0);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D3))
            {
                Owner.ChangeWeapon(1);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D4))
            {
                Owner.ChangeWeapon(2);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D5))
            {
                Owner.ChangeWeapon(3);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D6))
            {
                Owner.ChangeWeapon(4);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D7))
            {
                Owner.ChangeWeapon(5);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D8))
            {
                Owner.ChangeWeapon(6);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D9))
            {
                Owner.ChangeWeapon(7);
            }
            else if (KeyboardHelper.KeyPressed(Keys.D0))
            {
                Owner.ChangeWeapon(8);
            }
        }
    }
}
