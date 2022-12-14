using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public abstract class Vehicule : Object3D
    {
        public enum TunnelChangePredictionResults { Aligned, Turn, Overshoot, Undershoot }

        public AITunnel CurrentAITunnel;
        public AITunnel NextAITunnel;

        public Vector3 Speed;
        public readonly float Acceleration;
        public readonly float MaxSpeed;
        public readonly float Friction;
        public readonly float LateralFriction;
        public readonly float BrakePower;
        public readonly float RotationMaxSpeedPerSecond;
        public double BoostPower;

        public ActionPanelHolder ListActionMenuChoice;

        protected readonly List<AITunnel> ListAITunnel;
        protected readonly List<Object3D> ListCollisionBox;
        protected AntiGravPropulsor[] ArrayAntiGravPropulsor;

        public Vehicule(List<AITunnel> ListAITunnel, List<Object3D> ListCollisionBox)
            : base()
        {
            ListActionMenuChoice = new ActionPanelHolder();
            Acceleration = 100f;
            Friction = 1f;
            LateralFriction = 0.07f;
            RotationMaxSpeedPerSecond = MathHelper.ToRadians(35);
            MaxSpeed = 200f;
            BrakePower = 100f;
            ArrayAntiGravPropulsor = new AntiGravPropulsor[0];

            this.ListAITunnel = ListAITunnel;
            this.ListCollisionBox = ListCollisionBox;
        }

        public Vehicule(GraphicsDevice g, Matrix Projection, List<AITunnel> ListAITunnel, List<Object3D> ListCollisionBox)
            : base(g, Projection)
        {
            ListActionMenuChoice = new ActionPanelHolder();
            Acceleration = 100f;
            Friction = 1f;
            LateralFriction = 0.07f;
            RotationMaxSpeedPerSecond = MathHelper.ToRadians(35);
            MaxSpeed = 200f;
            BrakePower = 100f;
            ArrayAntiGravPropulsor = new AntiGravPropulsor[0];

            this.ListAITunnel = ListAITunnel;
            this.ListCollisionBox = ListCollisionBox;
        }

        public abstract void Load(ContentManager Content, GraphicsDevice g);

        public void Update(GameTime gameTime)
        {
            ListActionMenuChoice.Last().Update(gameTime);
            Speed += RacingMap.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ApplyFriction(gameTime.ElapsedGameTime.TotalSeconds);

            foreach (AntiGravPropulsor AntiGravPropulsor in ArrayAntiGravPropulsor)
            {
                Speed += AntiGravPropulsor.UpdatePropulsorTrust(gameTime, Speed, ListCollisionBox);
            }

            UpdatePosition(gameTime);

            foreach (AntiGravPropulsor AntiGravPropulsor in ArrayAntiGravPropulsor)
            {
                SnapPropulsorToVehicule(AntiGravPropulsor, new Vector3(0f, 2f, 0f));
            }

            DoUpdate(gameTime);
        }

        private void UpdatePosition(GameTime gameTime)
        {
            Vector3 FinalSpeed = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            TeleportRelative(FinalSpeed);
            /*PolygonMesh.PolygonMeshCollisionResult Result = GetClosestObject(ListCollisionBox, FinalSpeed);
            if (Result.Collided)
            {
                TeleportRelative(FinalSpeed + Result.Axis * Result.Distance);
                float CollisionSpeed = Vector3.Dot(Result.Axis, Speed);
                Speed -= Result.Axis * CollisionSpeed;
            }
            else
            {
                TeleportRelative(FinalSpeed);
            }*/
        }

        protected abstract void DoUpdate(GameTime gameTime);

        protected AntiGravPropulsor CreateAntiGravPropulsor(GraphicsDevice g, Matrix Projection, float Length, Vector3 Position)
        {
            AntiGravPropulsor NewAntiGravPropulsor = new AntiGravPropulsor(g, Projection, Length);
            NewAntiGravPropulsor.Teleport(Position);

            return NewAntiGravPropulsor;
        }
        
        public void ApplyFriction(double TotalSeconds)
        {
            Speed = ComputeSpeedAfterFriction(TotalSeconds);

            Speed = ComputeSpeedAfterLateralFriction(Speed);
        }

        public Vector3 ComputeSpeedAfterFriction(double TotalSeconds)
        {
            double FinalSpeedX = Speed.X * Math.Pow(Friction, TotalSeconds);
            double FinalSpeedY = Speed.Y * Math.Pow(Friction, TotalSeconds);
            double FinalSpeedZ = Speed.Z * Math.Pow(Friction, TotalSeconds);

            return new Vector3((float)FinalSpeedX, (float)FinalSpeedY, (float)FinalSpeedZ);
        }

        public Vector3 ComputeSpeedAfterFriction(Vector3 Speed, double TotalSeconds)
        {
            double FinalSpeedX = Speed.X * Math.Pow(Friction, TotalSeconds);
            double FinalSpeedY = Speed.Y * Math.Pow(Friction, TotalSeconds);
            double FinalSpeedZ = Speed.Z * Math.Pow(Friction, TotalSeconds);

            return new Vector3((float)FinalSpeedX, (float)FinalSpeedY, (float)FinalSpeedZ);
        }

        public Vector3 ComputeSpeedAfterLateralFriction(Vector3 Speed)
        {
            if (Speed.Length() == 0)
            {
                return Speed;
            }

            Vector3 VerticalSpeed = Up * Speed;
            Vector3 HorizontalSpeed = Speed - VerticalSpeed;
            if (HorizontalSpeed.Length() == 0)
            {
                return Speed;
            }

            Vector3 NormalisedHorizontalSpeed = Vector3.Normalize(HorizontalSpeed);
            Vector3 Difference = NormalisedHorizontalSpeed - Forward;
            Vector3 SpeedDifference = Difference * HorizontalSpeed.Length();

            float SinValue = Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeed));
            Vector3 FinalSpeed = HorizontalSpeed - SpeedDifference * LateralFriction;

            return VerticalSpeed + FinalSpeed;
        }
        
        public void Accelerate(GameTime gameTime)
        {
            if (BoostPower > 0)
            {
                Speed += Forward * BrakePower * (float)gameTime.ElapsedGameTime.TotalSeconds;
                BoostPower -= BrakePower * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (BoostPower < 0)
                {
                    BoostPower = 0;
                }
            }
            else
            {
                if (Vector3.Dot(Forward, Speed) < MaxSpeed)
                    Speed += Forward * Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Decelerate(GameTime gameTime)
        {
            float CurrentSpeed = Speed.Length();
            Vector3 NormalizedSpeed;
            if (CurrentSpeed == 0)
            {
                NormalizedSpeed = Forward;
            }
            else
            {
                NormalizedSpeed = Vector3.Normalize(Speed);
            }
            Speed = NormalizedSpeed * (CurrentSpeed - BrakePower * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void PrepareBoost(GameTime gameTime)
        {
            Decelerate(gameTime);
            BoostPower += BrakePower * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (BoostPower >= BrakePower)
            {
                BoostPower = BrakePower;
            }
        }
        
        //-1 for left, 1 for right
        public void Turn(GameTime gameTime, float RotationSpeed)
        {
            Rotate(gameTime, RotationSpeed);
        }

        public bool IsWrongWay()
        {
            float a = Vector3.Dot(CurrentAITunnel.Forward, Forward);
            return a < 0;
        }

        public List<AITunnel> GetTunnelsInCollision()
        {
            List<AITunnel> ListTunnelInCollision = new List<AITunnel>();
            for (int T = ListAITunnel.Count - 1; T >= 0; --T)
            {
                if (CollideWith(ListAITunnel[T]))
                {
                    ListTunnelInCollision.Add(ListAITunnel[T]);
                }
            }
            return ListTunnelInCollision;
        }

        public List<AITunnel> GetNextAITunnels()
        {
            List<AITunnel> ListTunnelInCollision = new List<AITunnel>();
            for (int T = CurrentAITunnel.ListNextAITunnel.Count - 1; T >= 0; --T)
            {
                if (GetDistanceToEntryPointOfAITunnel(CurrentAITunnel.ListNextAITunnel[T]) > 0)
                {
                    ListTunnelInCollision.Add(CurrentAITunnel.ListNextAITunnel[T]);
                }
            }

            if (ListTunnelInCollision.Count == 0)
                return CurrentAITunnel.ListNextAITunnel;

            return ListTunnelInCollision;
        }

        public float GetDistanceToEntryPointOfAITunnel(AITunnel ActiveTunnel)
        {
            float ActiveAITunnelPosition = ActiveTunnel.GetDistanceEntryPoint();
            float FinalPosition = Vector3.Dot(ActiveTunnel.Forward, Position);

            return ActiveAITunnelPosition - FinalPosition;
        }

        public float GetHorizontalDistanceToEntryPointOfAITunnel(AITunnel ActiveTunnel)
        {
            float Min, Max;
            ActiveTunnel.GetEntryPoints(Right, out Min, out Max);
            float FinalPosition = Vector3.Dot(Right, Position);

            if (FinalPosition < Min && FinalPosition < Max)
            {//Tunnel is at your left.
                return -1;
            }
            else if (FinalPosition > Min && FinalPosition > Max)
            {//Tunnel is at your right.
                return 1;
            }

            return 0;
        }

        public float GetHorizontalDistanceToExitPointOfAITunnel(AITunnel ActiveTunnel)
        {
            float Min, Max;
            ActiveTunnel.GetExitPoints(Right, out Min, out Max);
            float FinalPosition = Vector3.Dot(Right, Position);

            if (FinalPosition < Min && FinalPosition < Max)
            {//Tunnel is at your left.
                return -1;
            }
            else if (FinalPosition > Min && FinalPosition > Max)
            {//Tunnel is at your right.
                return 1;
            }

            return 0;
        }

        public float GetHorizontalDistanceToAITunnel(AITunnel ActiveTunnel)
        {
            if (IsMovingAway(ActiveTunnel))
            {
                return GetDirectionFromInsideTunnel(ActiveTunnel);
            }

            float Min, Max;
            PolygonMesh.ProjectPolygon(Right, ActiveTunnel.CollisionBox, out Min, out Max);

            float FinalPosition = Vector3.Dot(Right, Position);

            if (FinalPosition < Min && FinalPosition < Max)
            {
                return -1;
            }
            else if (FinalPosition > Min && FinalPosition > Max)
            {
                return 1;
            }
            //Inside Tunnel
            else if (FinalPosition > Min && FinalPosition < Max)
            {
                return GetHorizontalDistanceToExitPointOfAITunnel(ActiveTunnel);
            }

            //if perpendicular
            float a = Vector3.Dot(ActiveTunnel.Forward, Right);
            if (a == 0)
                return -1;

            return 0;
        }

        public bool IsMovingAway(AITunnel ActiveTunnel)
        {
            float Min, Max;
            PolygonMesh.ProjectPolygon(ActiveTunnel.Right, ActiveTunnel.CollisionBox, out Min, out Max);

            float FinalPosition = Vector3.Dot(ActiveTunnel.Right, Position);
            float RelativePosition = 0;
            //On the Right
            if (FinalPosition < Min && FinalPosition < Max)
            {
                RelativePosition = 1;
            }
            //On the Left
            else if (FinalPosition > Min && FinalPosition > Max)
            {
                RelativePosition = -1;
            }

            float DirectionFromTunnel = GetDirectionFromInsideTunnel(ActiveTunnel);
            
            if (RelativePosition != 0 && DirectionFromTunnel  != 0 && DirectionFromTunnel != RelativePosition)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get an Angle between self and an object. The method assume the target is in front.
        /// </summary>
        /// <param name="Target">Target Vehicule</param>
        /// <returns>Returns value between -1 and 1</returns>
        public float GetAngleBetweenTarget(Vehicule Target)
        {
            Vector3 PositionAngle = Vector3.Normalize(Target.Position - Position);
            if (Vector3.Dot(Right, PositionAngle) > 0)
            {
                return -(float)Math.Acos(Vector3.Dot(Target.Forward, PositionAngle));
            }

            return (float)Math.Acos(Vector3.Dot(Target.Forward, PositionAngle));
        }

        public float GetRandomEntryPointOfAITunnel(AITunnel ActiveTunnel)
        {
            float Min, Max;
            ActiveTunnel.GetEntryPoints(Right, out Min, out Max);
            float FinalPosition = Vector3.Dot(Right, Position);

            if (FinalPosition < Min && FinalPosition < Max)
            {
                return Min - FinalPosition;
            }
            else if (FinalPosition > Min && FinalPosition > Max)
            {
                return FinalPosition - Max;
            }

            return 0;
        }

        public float GetDirectionFromInsideTunnel(AITunnel ActiveTunnel)
        {
            float PlayerPosition = Vector3.Dot(Right, Forward);
            float TunnelPosition = Vector3.Dot(Right, ActiveTunnel.Forward);

            if (TunnelPosition > 0)
            {
                return -1;
            }
            if (TunnelPosition < 0)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Used for edge cases when moving from one tunnel to another and going out of bound when turning.
        /// </summary>
        /// <returns></returns>
        public bool IsBehindTunnel()
        {
            float MinTunnel, MaxTunnel;
            PolygonMesh.ProjectPolygon(CurrentAITunnel.Forward, CurrentAITunnel.CollisionBox, out MinTunnel, out MaxTunnel);
            float MinPlayer, MaxPlayer;
            PolygonMesh.ProjectPolygon(CurrentAITunnel.Forward, CollisionBox, out MinPlayer, out MaxPlayer);
            
            return MaxPlayer < MinTunnel;
        }

        /// <summary>
        /// Used for edge cases when moving from one tunnel to another and going out of bound when turning.
        /// </summary>
        /// <returns></returns>
        public bool IsInFrontOfTunnel()
        {
            float MinTunnel, MaxTunnel;
            PolygonMesh.ProjectPolygon(CurrentAITunnel.Forward, CurrentAITunnel.CollisionBox, out MinTunnel, out MaxTunnel);
            float MinPlayer, MaxPlayer;
            PolygonMesh.ProjectPolygon(CurrentAITunnel.Forward, CollisionBox, out MinPlayer, out MaxPlayer);

            return MinPlayer > MaxTunnel;
        }

        public float GetDirectionFromOutsideTunnel(AITunnel ActiveTunnel)
        {
            float MinTunnel, MaxTunnel;

            PolygonMesh.ProjectPolygon(Right, ActiveTunnel.CollisionBox, out MinTunnel, out MaxTunnel);
            float FinalPlayerPosition = Vector3.Dot(Right, Position);

            if (MinTunnel < FinalPlayerPosition && MaxTunnel < FinalPlayerPosition)
            {
                return -1;
            }
            if (MinTunnel > FinalPlayerPosition && MaxTunnel > FinalPlayerPosition)
            {
                return 1;
            }

            return 0;
        }

        public void AlignWithTunnel(AITunnel CurrentAITunnel)
        {
            Rotate(CurrentAITunnel.Direction.X, Direction.Y, Direction.Z);
        }

        /// <summary>
        /// Rotate Vehicule left (-1) or right (1).
        /// </summary>
        /// <param name="RotationSpeed">Speed between -1 and 1 inclusively</param>
        public void Rotate(GameTime gameTime, float RotationSpeed)
        {
            float FinalRotationSpeed = ComputeRotation(gameTime.ElapsedGameTime.TotalSeconds, RotationSpeed);
            Rotate(Direction.X + FinalRotationSpeed, Direction.Y, Direction.Z);
        }

        public float ComputeRotation(double TotalSeconds, float RotationSpeed)
        {
            return RotationMaxSpeedPerSecond * RotationSpeed * (float)TotalSeconds;
        }

        public void SnapPropulsorToVehicule(Object3D ActivePropulsor, Vector3 RelativePosition)
        {
            ActivePropulsor.Position = Position + RelativePosition;
            ActivePropulsor.Rotate(Direction.X, Direction.Y + (float)Math.PI, Direction.Z);
        }
        
        protected void TeleportRelative(Vector3 Movement)
        {
            Position += Movement;
            CollisionBox.Offset(Movement.X, Movement.Y, Movement.Z);
        }

        public TunnelChangePredictionResults GetTunnelChangePrediction(AITunnel NextTunnel, out bool NeedToTurnRight)
        {
            if (Forward == NextTunnel.Forward)
            {
                NeedToTurnRight = false;
                return TunnelChangePredictionResults.Aligned;
            }

            double VehiculeYaw = Math.Atan2(Forward.Z, Forward.X);
            double TunnelYaw = Math.Atan2(NextTunnel.Forward.Z, NextTunnel.Forward.X);
            double AngleDifference = VehiculeYaw - TunnelYaw;
            double ExpectedSecondsToAlignWithNextTurn = Math.Abs(AngleDifference / RotationMaxSpeedPerSecond);

            Vector3 ExpectedSpeedAfterAlignWithNextTurn = Forward * Acceleration * (float)ExpectedSecondsToAlignWithNextTurn;
            ExpectedSpeedAfterAlignWithNextTurn += Speed;
            ExpectedSpeedAfterAlignWithNextTurn = ComputeSpeedAfterFriction(ExpectedSpeedAfterAlignWithNextTurn, ExpectedSecondsToAlignWithNextTurn);
            ExpectedSpeedAfterAlignWithNextTurn = ComputeSpeedAfterLateralFriction(ExpectedSpeedAfterAlignWithNextTurn);

            Vector3 ExpectedPositionAfterAlignWithNextTurn = Position + ExpectedSpeedAfterAlignWithNextTurn;

            float PlayerPosition = Vector3.Dot(NextTunnel.Right, ExpectedPositionAfterAlignWithNextTurn);
            float TunnelPositionLeftEntryPoint;
            float TunnelPositionRightEntryPoint;
            NextTunnel.GetEntryPoints(NextTunnel.Right, out TunnelPositionLeftEntryPoint, out TunnelPositionRightEntryPoint);

            float NextTunnelAngle = Vector3.Dot(Right, NextTunnel.Forward);

            NeedToTurnRight = NextTunnelAngle > 0;

            if (NeedToTurnRight)
            {
                //Entry would at the right of the player after turning, meaning you overshoot and need to brake.
                if (TunnelPositionLeftEntryPoint > PlayerPosition && TunnelPositionRightEntryPoint > PlayerPosition)
                {
                    return TunnelChangePredictionResults.Overshoot;
                }
                //Entry would at the left of the player after turning, meaning you undershoot and will hit a wall if turning right now.
                else if (TunnelPositionLeftEntryPoint < PlayerPosition && TunnelPositionRightEntryPoint < PlayerPosition)
                {
                    return TunnelChangePredictionResults.Undershoot;
                }
            }
            else
            {
                //Entry would at the left of the player after turning, meaning you overshoot and need to brake.
                if (TunnelPositionLeftEntryPoint < PlayerPosition && TunnelPositionRightEntryPoint < PlayerPosition)
                {
                    return TunnelChangePredictionResults.Overshoot;
                }
                //Entry would at the right of the player after turning, meaning you undershoot and will hit a wall if turning right now.
                else if (TunnelPositionLeftEntryPoint > PlayerPosition && TunnelPositionRightEntryPoint > PlayerPosition)
                {
                    return TunnelChangePredictionResults.Undershoot;
                }
            }

            return TunnelChangePredictionResults.Turn;
        }
    }
}
