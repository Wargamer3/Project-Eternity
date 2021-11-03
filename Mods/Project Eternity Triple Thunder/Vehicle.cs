using System.IO;
using System.Text;
using ProjectEternity.Core.Item;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Vehicle : RobotAnimation
    {
        private List<RobotAnimation> ListUser;
        private string CaptureRule;

        public Vehicle(string Name, Layer CurrentLayer, Vector2 Position, int Team, PlayerInventory Equipment, ISFXGenerator PlayerSFXGenerator, List<Weapon> ListExtraWeapon)
            : base(Name, CurrentLayer, Position, Team, new EquipmentLoadout(), PlayerSFXGenerator)
        {
            ListUser = new List<RobotAnimation>();

            FileStream FS = new FileStream("Content/Units/Triple Thunder/Vehicles/" + Name + ".peuv", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MaxHP = BR.ReadInt32();
            MaxEN = BR.ReadInt32();
            Accel = BR.ReadSingle();
            MaxWalkSpeed = BR.ReadSingle();
            JumpSpeed = BR.ReadSingle();
            byte ControlType = BR.ReadByte();
            byte CaptureType = BR.ReadByte();
            HasKnockback = BR.ReadBoolean();
            IsDynamic = BR.ReadBoolean();

            int ListExtraAnimationCount = BR.ReadInt32();
            ListStanceAnimation = new List<Weapon>(ListExtraAnimationCount);
            for (int W = 0; W < ListExtraAnimationCount; ++W)
            {
                string ExtraAnimationPath = BR.ReadString();
                if (!string.IsNullOrEmpty(ExtraAnimationPath))
                {
                    ListStanceAnimation.Add(new Weapon(Name, ExtraAnimationPath, true, CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget));
                }
            }

            CurrentStanceAnimations = StandingAnimations;

            int ListWeaponCount = BR.ReadInt32();

            PrimaryWeapons = new WeaponHolder(ListWeaponCount);
            SecondaryWeapons = new WeaponHolder(0);
            for (int W = 0; W < ListWeaponCount; ++W)
            {
                string WeaponName = BR.ReadString();
                PrimaryWeapons.AddWeaponToStash(new Weapon(Name, WeaponName, false, CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget));
            }

            if (ListExtraWeapon != null)
            {
                foreach (Weapon ActiveWeapon in ListExtraWeapon)
                {
                    PrimaryWeapons.AddWeaponToStash(ActiveWeapon);
                    PrimaryWeapons.UseWeapon(ActiveWeapon);
                }
            }

            FS.Close();
            BR.Close();

            Load();

            SetAnimation(StandingAnimations.NoneCombo.AnimationName);
            CurrentMovementInput = MovementInputs.None;
            ActiveMovementStance = "None";
            Update(new GameTime());
            SetIdle();

            if (!PrimaryWeapons.HasActiveWeapons)
            {
                if (PrimaryWeapons.HasWeapons)
                    ChangeWeapon(0);
                else
                    ChangeWeapon(-1);
            }

            UpdateSkills(BaseSkillRequirement.OnCreatedRequirementName);
        }

        public override void Load()
        {
            base.Load();
            if (Content != null)
            {
            }
        }

        public override void Update(GameTime gameTime, Dictionary<uint, RobotAnimation> DicRobot)
        {
            base.Update(gameTime, DicRobot);

            foreach (RobotAnimation ActiveRobot in ListUser)
            {
                ActiveRobot.InputManager.Update(gameTime);
                ActiveRobot.Camera = Camera;
                ActiveRobot.Position = Position;
            }
            
            foreach (RobotAnimation ActiveRobot in DicRobot.Values)
            {
                if (ActiveRobot == this)
                    continue;

                if (CanGetIn(ActiveRobot))
                {
                    if (KeyboardHelper.KeyPressed(Keys.W))
                    {
                        Camera = ActiveRobot.Camera;
                        ListUser.Add(ActiveRobot);
                        ActiveRobot.IsUpdated = false;
                        //Change the pilot controls to match the one of the vehicle.
                        ActiveRobot.InputManagerHelper = new VehicleInputManager();
                        ActiveRobot.InputManagerHelper.GetRobotInput(GameplayTypes.MouseAndKeyboard, this, ActiveRobot.InputManager.CameraBounds);
                    }
                }
            }
        }

        public bool CanGetIn(RobotAnimation ActiveRobot)
        {
            return (this.Position - ActiveRobot.Position).Length() < 15;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
        }
    }

    public class ControllerSpaceshipVehicleInput : RobotInputHelper
    {
        private ControllerHelper Controller;

        private Buttons MoveLeftButtons;
        private Buttons MoveRightButtons;
        private Buttons JumpButtons;
        private Buttons CrouchButtons;
        private Buttons ShootButtons;

        public ControllerSpaceshipVehicleInput(RobotAnimation Owner, Rectangle CameraBounds, PlayerIndex ControllerIndex)
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
                Owner.HolsterAndReplaceWeapon(Owner.SecondaryWeapons.ActiveWeapons[0]);
            }
            else if (MouseHelper.InputRightButtonReleased())
            {
                Owner.UseCombo(gameTime, AttackInputs.HeavyPress, Owner.SecondaryWeapons.ActiveWeapons[0], true);
                Owner.UnholsterWeaponsIfNeeded();
            }
            else if (MouseHelper.InputRightButtonHold())
            {
                Owner.UseCombo(gameTime, AttackInputs.HeavyHold, Owner.SecondaryWeapons.ActiveWeapons[0], false);
            }
        }
    }

    public class KeyboardSpaceshipVehicleInput : RobotInputHelper
    {
        public Keys[] MoveLeft = new Keys[] { Keys.Left };
        public Keys[] MoveRight = new Keys[] { Keys.Right };
        public Keys[] MoveUp = new Keys[] { Keys.Up };
        public Keys[] MoveDown = new Keys[] { Keys.Down };

        public KeyboardSpaceshipVehicleInput(RobotAnimation Owner, Rectangle CameraBounds)
            : base(Owner, CameraBounds)
        {
            this.Owner = Owner;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateOwner(gameTime);

            MouseState MousePosition = MouseHelper.MouseStateCurrent;

            Owner.UpdateAllWeaponsAngle(new Vector2(MousePosition.X + Owner.Camera.X - Owner.Position.X, MousePosition.Y + Owner.Camera.Y - Owner.Position.Y));

            UpdateCamera(new Vector2(MousePosition.X, MousePosition.Y));
        }

        private void UpdateOwner(GameTime gameTime)
        {
            if (KeyboardHelper.KeyHold(MoveRight[0]))
            {
                Owner.Speed.X = 3;
            }
            else if (KeyboardHelper.KeyHold(MoveLeft[0]))
            {
                Owner.Speed.X = -3;
            }
            else if (KeyboardHelper.KeyHold(MoveDown[0]))
            {
                Owner.Speed.Y = 3;
            }
            else if (KeyboardHelper.KeyHold(MoveUp[0]))
            {
                Owner.Speed.Y = -3;
            }

            if (MouseHelper.InputLeftButtonPressed())
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightPress);
            }
            else if (MouseHelper.InputLeftButtonHold())
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightHold);
            }
        }
    }

    public class KeyboardScrollingSpaceshipVehicleInput : RobotInputHelper
    {
        public Keys[] MoveLeft = new Keys[] { Keys.A };
        public Keys[] MoveRight = new Keys[] { Keys.D };
        public Keys[] MoveUp = new Keys[] { Keys.W };
        public Keys[] MoveDown = new Keys[] { Keys.S };

        public KeyboardScrollingSpaceshipVehicleInput(RobotAnimation Owner, Rectangle CameraBounds)
            : base(Owner, CameraBounds)
        {
            this.Owner = Owner;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateOwner(gameTime);

            MouseState MousePosition = MouseHelper.MouseStateCurrent;

            Owner.UpdateAllWeaponsAngle(new Vector2(MousePosition.X + Owner.Camera.X - Owner.Position.X, MousePosition.Y + Owner.Camera.Y - Owner.Position.Y));

            Owner.Camera.X = (int)Owner.GetMapVariable("camerax");
        }

        private void UpdateOwner(GameTime gameTime)
        {
            Owner.Speed.X = 1;
            Owner.Speed.Y = 0;

            if (KeyboardHelper.KeyHold(MoveRight[0]))
            {
                Owner.Speed.X = 3;
            }
            if (KeyboardHelper.KeyHold(MoveLeft[0]))
            {
                Owner.Speed.X = -3;
            }
            if (KeyboardHelper.KeyHold(MoveDown[0]))
            {
                Owner.Speed.Y = 3;
            }
            if (KeyboardHelper.KeyHold(MoveUp[0]))
            {
                Owner.Speed.Y = -3;
            }

            if (MouseHelper.InputLeftButtonPressed())
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightPress);
            }
            else if (MouseHelper.InputLeftButtonHold())
            {
                Owner.UnholsterWeaponsIfNeeded();
                Owner.UseCombo(gameTime, AttackInputs.LightHold);
            }
        }
    }

    public class VehicleInputManager : RobotInputManager
    {
        public RobotInput GetRobotInput(GameplayTypes GameplayType, RobotAnimation Owner, Rectangle CameraBounds)
        {
            return new KeyboardSpaceshipVehicleInput(Owner, CameraBounds);
        }
    }
}
