using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.RacingScreen;
using static ProjectEternity.GameScreens.RacingScreen.Vehicule;

namespace ProjectEternity.UnitTests.Racing
{
    [TestClass]
    public class RacingTests
    {
        private static PolygonMesh Player;
        private static PolygonMesh CollisionBox;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
        }

        [TestInitialize()]
        public void Initialize()
        {
            Player = new PolygonMesh(new Vector3[8]
            {
                // Calculate the position of the vertices on the top face.
                new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, -1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),

                // Calculate the position of the vertices on the bottom face.
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, 1.0f),
            }, new Vector3[3]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
            });
            CollisionBox = new PolygonMesh(new Vector3[8]
            {
                // Calculate the position of the vertices on the top face.
                new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, -1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),

                // Calculate the position of the vertices on the bottom face.
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, 1.0f),
            }, new Vector3[3]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
            });
        }

        [TestMethod]
        public void TestScale()
        {
            CollisionBox.Scale(new Vector3(2, 1, 10));
            Assert.AreEqual(new Vector3(-2.0f, 1.0f, -10.0f), CollisionBox.ArrayVertex[0]);
            Assert.AreEqual(new Vector3(-2.0f, 1.0f, 10.0f), CollisionBox.ArrayVertex[1]);
            Assert.AreEqual(new Vector3(2.0f, 1.0f, -10.0f), CollisionBox.ArrayVertex[2]);
            Assert.AreEqual(new Vector3(2.0f, 1.0f, 10.0f), CollisionBox.ArrayVertex[3]);

            Assert.AreEqual(new Vector3(-2.0f, -1.0f, -10.0f), CollisionBox.ArrayVertex[4]);
            Assert.AreEqual(new Vector3(-2.0f, -1.0f, 10.0f), CollisionBox.ArrayVertex[5]);
            Assert.AreEqual(new Vector3(2.0f, -1.0f, -10.0f), CollisionBox.ArrayVertex[6]);
            Assert.AreEqual(new Vector3(2.0f, -1.0f, 10.0f), CollisionBox.ArrayVertex[7]);
        }

        [TestMethod]
        public void Test3DCollision()
        {
            PolygonMesh.PolygonMeshCollisionResult Result = PolygonMesh.PolygonCollisionSAT(Player, CollisionBox, Vector3.Zero);
            Assert.IsTrue(Result.Collided);

            Player.Offset(0, 5, 0);
            Result = PolygonMesh.PolygonCollisionSAT(Player, CollisionBox, Vector3.Zero);
            Assert.AreEqual(-1, Result.Distance);
        }

        [TestMethod]
        public void TestAngle()
        {
            Vector3 PlayerFoward = new Vector3(0f, 0f, 1f);
            Vector3 PlayerRight = new Vector3(1f, 0f, 0f);
            Vector3 Position = new Vector3(0f, 0f, 0f);

            Vector3 Tunnel1Foward = new Vector3(1f, 0f, 0f);
            Vector3 Tunnel2Foward = new Vector3(1f, 1f, 0f);
            Vector3 Tunnel3Foward = new Vector3(0f, 1f, 0f);

            float GetDirectionTunnel1 = GetDirectionFromInsideTunnel(PlayerRight, PlayerFoward, Tunnel1Foward);
            Assert.AreEqual(-1, GetDirectionTunnel1);

            float GetDirectionTunnel2 = GetDirectionFromInsideTunnel(PlayerRight, PlayerFoward, Tunnel2Foward);
            Assert.AreEqual(-1, GetDirectionTunnel2);

            float GetDirectionTunnel3 = GetDirectionFromInsideTunnel(PlayerRight, PlayerFoward, Tunnel3Foward);
            Assert.AreEqual(0, GetDirectionTunnel3);

            GetDirectionTunnel1 = GetDirectionFromOutsideTunnel(PlayerRight, Position, CollisionBox);
            Assert.AreEqual(0, GetDirectionTunnel1);

            GetDirectionTunnel2 = GetDirectionFromOutsideTunnel(PlayerRight, Position, CollisionBox);
            Assert.AreEqual(0, GetDirectionTunnel2);

            GetDirectionTunnel3 = GetDirectionFromOutsideTunnel(PlayerRight, Position, CollisionBox);
            Assert.AreEqual(0, GetDirectionTunnel3);
        }

        [TestMethod]
        public void TestAngleFromPlayer()
        {
            Vehicule FirstVehicule = Vehicule3D.GetUnitTestVehicule(new Vector3(0f, 0f, 0f));
            Vehicule SecondVehicule = Vehicule3D.GetUnitTestVehicule(new Vector3(0f, 0f, -50f));

            float AngleBetweenVehiculeFront = FirstVehicule.GetAngleBetweenTarget(SecondVehicule);

            SecondVehicule = Vehicule3D.GetUnitTestVehicule(new Vector3(50f, 0f, 0f));

            float AngleBetweenVehiculeRight = FirstVehicule.GetAngleBetweenTarget(SecondVehicule);

            Assert.AreEqual(0, MathHelper.ToDegrees(AngleBetweenVehiculeFront));
            Assert.AreEqual(-90, MathHelper.ToDegrees(AngleBetweenVehiculeRight));
        }

        [TestMethod]
        public void TestLateralFriction()
        {
            Matrix RotationMatrix = Matrix.CreateFromYawPitchRoll(0f, 0f, 0f);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Vector3 Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Vector3 Up = Vector3.Transform(Vector3.Up, RotationMatrix);
            Vector3 Speed = new Vector3(23, 55, 87);

            Speed = ApplyLateralFriction(Speed, Right, 5f);

            Assert.AreEqual(18, Speed.X);
            Assert.AreEqual(55, Speed.Y);
            Assert.AreEqual(87, Speed.Z);

            RotationMatrix = Matrix.CreateFromYawPitchRoll(2.5f, 0f, 0f);
            Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Up = Vector3.Transform(Vector3.Up, RotationMatrix);
            Speed = new Vector3(23, 55, 87);

            Speed = ApplyLateralFriction(Speed, Right, 5f);

            Assert.AreEqual(21, Speed.X, 1);
            Assert.AreEqual(55, Speed.Y, 1);
            Assert.AreEqual(82, Speed.Z, 1);

            RotationMatrix = Matrix.CreateFromYawPitchRoll(2.5f, 0f, 0f);
            Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Up = Vector3.Transform(Vector3.Up, RotationMatrix);
            Speed = new Vector3(-23, 55, 87);

            Speed = ApplyLateralFriction(Speed, Right, 5f);

            Assert.AreEqual(-21.3319, Speed.X, 1);
            Assert.AreEqual(55, Speed.Y, 1);
            Assert.AreEqual(82.28646, Speed.Z, 1);
        }

        [TestMethod]
        public void TestLateralAngleValues()
        {
            Matrix RotationMatrix = Matrix.CreateFromYawPitchRoll(0f, 0f, 0f);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Vector3 Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Vector3 Up = Vector3.Transform(Vector3.Up, RotationMatrix);
            Vector3 Speed = new Vector3(23, 55, 87);
            
            RotationMatrix = Matrix.CreateFromYawPitchRoll(2.5f, 0f, 0f);
            Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Up = Vector3.Transform(Vector3.Up, RotationMatrix);
            Vector3 SpeedStraight = Forward * 112 + new Vector3(0, 55, 0);
            Vector3 Speed45Angle = Forward * 112 + Right * 112 + new Vector3(0, 55, 0);
            Vector3 SpeedPerpendicular = Right * 112 + new Vector3(0, 55, 0);

            Vector3 HorizontalSpeedStraight = SpeedStraight - Up * SpeedStraight;
            Vector3 HorizontalSpeed45Angle = Speed45Angle - Up * Speed45Angle;
            Vector3 HorizontalSpeedPerpendicular = SpeedPerpendicular - Up * SpeedPerpendicular;

            Assert.AreEqual(1f, Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeedStraight)), 0.0001);
            Assert.AreEqual(0.70710f, Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeed45Angle)), 0.0001);
            Assert.AreEqual(0f, Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeedPerpendicular)), 0.0001);

            HorizontalSpeedStraight = RemoveLateralFriction(SpeedStraight, Forward, Right, Up) - Up * SpeedStraight;
            HorizontalSpeed45Angle = RemoveLateralFriction(Speed45Angle, Forward, Right, Up) - Up * Speed45Angle;
            HorizontalSpeedPerpendicular = RemoveLateralFriction(SpeedPerpendicular, Forward, Right, Up) - Up * SpeedPerpendicular;

            Assert.AreEqual(1f, Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeedStraight)), 0.0001);
            Assert.AreEqual(1f, Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeed45Angle)), 0.0001);
            Assert.AreEqual(1f, Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeedPerpendicular)), 0.0001);

            Assert.AreEqual(SpeedStraight.Length(), (HorizontalSpeedStraight + new Vector3(0, 55, 0)).Length(), 0.0001);
            //Since Speed45Angle is the same speed as SpeedStraight with added with Right * 112, if the lateral speed is correctly removed,
            //the new speed should be the same as the SpeedStraight.
            Assert.AreEqual(SpeedStraight.Length(), (HorizontalSpeed45Angle + new Vector3(0, 55, 0)).Length(), 0.0001);
            //No speed since perpendicular.
            Assert.AreEqual(0, HorizontalSpeedPerpendicular.Length(), 0.0001);
        }

        [TestMethod]
        public void TestTunnelChangePrediction()
        {//Only change direction if the vehicule will exit the next tunnel by going straight

            AITunnel NextTunnel = new AITunnel();
            NextTunnel.Rotate((float)-1.3258176636680326f, 0, 0);
            NextTunnel.Position += new Vector3(0, 0, -210);
            NextTunnel.Scale(new Vector3(10, 1f, 1f));

            Vehicule ActiveVehicule = Vehicule3D.GetUnitTestVehicule(new Vector3(0f, 0f, 0f));

            bool NeedToTurnRight;
            TunnelChangePredictionResults Prediction = ActiveVehicule.GetTunnelChangePrediction(NextTunnel, out NeedToTurnRight);

            if (Prediction == TunnelChangePredictionResults.Overshoot)//Check if you would be braking too soon and end up undershooting instead.
            {
                float ExpectedTimeToStop;
            }
            else if (Prediction == TunnelChangePredictionResults.Undershoot || Prediction == TunnelChangePredictionResults.Aligned)//Keep going
            {
            }
        }

        private Vector3 RemoveLateralFriction(Vector3 Speed, Vector3 Forward, Vector3 Right, Vector3 Up, float ReductionFactor = 1f)
        {
            Vector3 VerticalSpeed = Up * Speed;
            Vector3 HorizontalSpeed = Speed - VerticalSpeed;
            Vector3 NormalisedHorizontalSpeed = Vector3.Normalize(HorizontalSpeed);
            Vector3 Difference = NormalisedHorizontalSpeed - Forward;
            Vector3 SpeedDifference = Difference * HorizontalSpeed.Length();

            float SinValue = Vector3.Dot(Forward, Vector3.Normalize(HorizontalSpeed));
            Vector3 FinalSpeed = HorizontalSpeed - SpeedDifference * ReductionFactor;

            return VerticalSpeed + FinalSpeed * SinValue;
        }

        private Vector3 ApplyLateralFriction(Vector3 Speed, Vector3 Right, float LateralFriction)
        {
            Vector3 RightSpeed = Speed * Right;
            if (Math.Sign(RightSpeed.X) != Math.Sign(Speed.X))
            {
                RightSpeed.X = -RightSpeed.X;
            }
            if (Math.Sign(RightSpeed.Y) != Math.Sign(Speed.Y))
            {
                RightSpeed.Y = -RightSpeed.Y;
            }
            if (Math.Sign(RightSpeed.Z) != Math.Sign(Speed.Z))
            {
                RightSpeed.Z = -RightSpeed.Z;
            }

            float SpeedReduction = (RightSpeed.Length() - LateralFriction) / RightSpeed.Length();
            Vector3 RightSpeedReduced = RightSpeed * SpeedReduction;
            Vector3 SpeedDifference = RightSpeed - RightSpeedReduced;

            return Speed - SpeedDifference;
        }

        public float GetDirectionFromInsideTunnel(Vector3 PlayerRight, Vector3 PlayerFoward, Vector3 TunnelFoward)
        {
            float PlayerPosition = Vector3.Dot(PlayerRight, PlayerFoward);
            float TunnelPosition = Vector3.Dot(PlayerRight, TunnelFoward);

            if (PlayerPosition < TunnelPosition)
            {
                return -1;
            }
            else if (PlayerPosition > TunnelPosition)
            {
                return 1;
            }

            return 0;
        }

        public float GetDirectionFromOutsideTunnel(Vector3 PlayerRight, Vector3 PlayerPosition, PolygonMesh Tunnel)
        {
            float MinTunnel, MaxTunnel;

            PolygonMesh.ProjectPolygon(PlayerRight, Tunnel, out MinTunnel, out MaxTunnel);
            float FinalPlayerPosition = Vector3.Dot(PlayerRight, PlayerPosition);

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

        private List<AITunnel> GetNextTunnels(Vector3 PlayerPosition, AITunnel CurrentTunnel)
        {
            List<AITunnel> ListNextTunnel = new List<AITunnel>();
            List<AITunnel> ListPassedTunnel = new List<AITunnel>();
            List<AITunnel> ListOtherTunnel = new List<AITunnel>();

            float PlayerPositionInTunnel = Vector3.Dot(CurrentTunnel.Forward, PlayerPosition);

            foreach (AITunnel NextAITunnel in CurrentTunnel.ListNextAITunnel)
            {
                float Min, Max;
                NextAITunnel.GetEntryPoints(CurrentTunnel.Forward, out Min, out Max);

                //Tunnel is in front of the player
                if (Min > PlayerPositionInTunnel && Max > PlayerPositionInTunnel)
                {
                    ListNextTunnel.Add(NextAITunnel);
                }
                //Tunnel is behind of the player
                else if (Min < PlayerPositionInTunnel && Max < PlayerPositionInTunnel)
                {
                    ListPassedTunnel.Add(NextAITunnel);
                }
                else
                {
                    ListOtherTunnel.Add(NextAITunnel);
                }
            }

            if (ListNextTunnel.Count > 0)
            {
                return ListNextTunnel;
            }
            else if (ListOtherTunnel.Count > 0)
            {
                return ListOtherTunnel;
            }
            else
            {
                return ListPassedTunnel;
            }
        }
    }
}
