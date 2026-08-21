using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelThirdPersonControlCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "ThirdPersonControlCharacter";

        private const float CameraDistanceMin = 5f;
        private const float CameraDistanceMax = 100f;
        private readonly float CameraPitchMin = MathHelper.ToRadians(5f);
        private readonly float CameraPitchMax = MathHelper.ToRadians(85f);
        private const float MouseSensitivity = 0.005f;
        private const float ZoomSensitivity = 2.0f;
        private const float CameraRotationSpeed = 8.0f;

        private float CameraYaw = 0f;
        private float CameraPitch = MathHelper.ToRadians(25f);
        private float CameraDistance = 100f;
        private Vector3 CameraOffset;
        private Vector3 TargetOffset;
        private Camera3D Camera;

        private readonly PlayerCharacter ControlledCharacter;
        private readonly List<MapInfo> ListVisisbleMap;

        private PlayerCharacter HoverCharacter;

        public ActionPanelThirdPersonControlCharacter(PlayerOverseer Owner, PlayerCharacter ControlledCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
            this.ControlledCharacter = ControlledCharacter;

            Camera = new DefaultCamera(GameScreen.GraphicsDevice);
            ListVisisbleMap = new List<MapInfo>();

            CameraOffset = new Vector3(0, -10f, 100f);
            TargetOffset = new Vector3(0, 5, 0);
        }

        public override void OnSelect()
        {
            ListVisisbleMap.Add(MapManager.RootMapContainer);
            Owner.Camera = Camera;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            HoverCharacter = null;

            CameraControl();

            if (ControlledCharacter.ListAutomatedAction.Count == 0)
            {
                Vector2 PlayerInput = Vector2.Zero;

                if (ActiveInputManager.InputRightHold())
                {
                    PlayerInput.X -= 1f;
                }
                else if (ActiveInputManager.InputLeftHold())
                {
                    PlayerInput.X += 1f;
                }

                if (ActiveInputManager.InputUpHold())
                {
                    PlayerInput.Y += 1f;
                }
                else if (ActiveInputManager.InputDownHold())
                {
                    PlayerInput.Y -= 1f;
                }

                float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (PlayerInput.LengthSquared() > 0)
                {
                    PlayerInput.Normalize();

                    Vector3 ForwardMovement = new Vector3(-(float)Math.Sin(CameraYaw), (float)Math.Cos(CameraYaw), 0f);
                    Vector3 LateralMovement = new Vector3((float)Math.Cos(CameraYaw), (float)Math.Sin(CameraYaw), 0f);

                    Vector3 Movement = (LateralMovement * PlayerInput.X) + (ForwardMovement * PlayerInput.Y);
                    Movement.Z = 0f;

                    ControlledCharacter.Move(Movement, 1.0, DeltaTime);

                    float targetYaw = (float)Math.Atan2(-Movement.X, Movement.Y);
                    float yawLerpFactor = 1.0f - (float)Math.Exp(-CameraRotationSpeed * DeltaTime);
                    CameraYaw = SmoothAngle(CameraYaw, targetYaw, yawLerpFactor);
                }
                else
                {
                    ControlledCharacter.Move(Vector3.Zero, 0.0, DeltaTime);
                }

                CameraOffset = CalculateCameraOffset(CameraYaw, CameraPitch, CameraDistance);
            }

            UpdateCamera(gameTime);
        }

        private void UpdateCamera(GameTime gameTime)
        {
            double CameraFollowSpeed = 12;
            float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 TarterPosition = new Vector3(ControlledCharacter.Position.X, ControlledCharacter.Position.Z, ControlledCharacter.Position.Y);
            Vector3 CameraPosition = TarterPosition + CameraOffset;
            Vector3 TarterPositionFinal = TarterPosition + TargetOffset;

            if (Camera.CameraPosition3D == Vector3.Zero)
            {
                Camera.CameraPosition3D = CameraPosition;
                Camera.LookAtTarget = TarterPositionFinal;
            }
            else
            {
                float CameraTransition = 1.0f - (float)Math.Exp(-(float)CameraFollowSpeed * DeltaTime);
                Camera.CameraPosition3D = Vector3.Lerp(Camera.CameraPosition3D, CameraPosition, CameraTransition);
                Camera.LookAtTarget = Vector3.Lerp(Camera.LookAtTarget, TarterPositionFinal, CameraTransition);
            }

            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Camera.LookAtTarget, Vector3.Up);
            Camera.CameraRotation = Camera.View;
        }

        private Vector3 CalculateCameraOffset(float Yaw, float Pitch, float Distance)
        {
            double HorizontalDistance = Distance * Math.Cos(Pitch);

            double X = HorizontalDistance * Math.Sin(Yaw);
            double Y = Distance * Math.Sin(Pitch);
            double Z = -HorizontalDistance * Math.Cos(Yaw);

            return new Vector3((float)X, (float)Y, (float)Z);
        }

        private float SmoothAngle(float current, float target, float factor)
        {
            float diff = MathHelper.WrapAngle(target - current);
            return current + diff * MathHelper.Clamp(factor, 0f, 1f);
        }

        private void CameraControl()
        {
            if (MouseHelper.InputRightButtonHold())
            {
                float MouseX = MouseHelper.MouseStateCurrent.X - MouseHelper.MouseStateLast.X;
                float MouseY = MouseHelper.MouseStateCurrent.Y - MouseHelper.MouseStateLast.Y;

                CameraYaw -= MouseX * MouseSensitivity;
                CameraPitch += MouseY * MouseSensitivity;

                CameraPitch = MathHelper.Clamp(CameraPitch, CameraPitchMin, CameraPitchMax);
            }

            int ScrollValue = MouseHelper.InputMouseWheelValue();
            if (ScrollValue != 0)
            {
                CameraDistance -= (ScrollValue / 120f) * ZoomSensitivity;
                CameraDistance = MathHelper.Clamp(CameraDistance, CameraDistanceMin, CameraDistanceMax);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelThirdPersonControlCharacter(Owner, ControlledCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            foreach (MapInfo ActiveMap in ListVisisbleMap)
            {
                ActiveMap.MapMapContainer.ActiveMap.LayerManager.LayerHolderDrawable.SetCamera(Owner.Camera);
                ActiveMap.MapMapContainer.ActiveMap.Draw(g);
            }
        }

        public override string ToString()
        {
            return ControlledCharacter.Name;
        }
    }
}
