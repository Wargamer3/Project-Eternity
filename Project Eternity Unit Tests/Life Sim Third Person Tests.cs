using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimThirdPersonTests
    {
        [TestMethod]
        public void MouseRightTurnsTheViewRight()
        {
            ThirdPersonRig Rig = new ThirdPersonRig();
            float Before = Rig.Forward.X;
            Rig.Orbit(20f, 0f);
            Assert.IsTrue(Rig.Forward.X < Before);
        }

        [TestMethod]
        public void ForwardMatchesTheCameraAndStrafeStandsAside()
        {
            ThirdPersonRig Rig = new ThirdPersonRig();
            Rig.Yaw = 0.7f;
            MoveWish Wish = new MoveWish();
            Wish.Facing = BodyFacing.Travel;
            Wish.Stick = new Vector2(0f, 1f);

            GroundStep Ahead = Rig.Plan(Wish, 0f);
            Assert.IsTrue(Vector3.Dot(Ahead.Direction, Rig.Forward) > 0.99f);
            Assert.AreEqual(0.7f, Rig.Yaw, 0.0001f);

            Wish.Stick = new Vector2(1f, 0f);
            GroundStep Aside = Rig.Plan(Wish, 0f);
            Assert.IsTrue(Math.Abs(Vector3.Dot(Aside.Direction, Rig.Forward)) < 0.001f);
            Assert.IsTrue(Vector3.Dot(Aside.Direction, Rig.Right) > 0.99f);
        }

        [TestMethod]
        public void BackingUpKeepsTheCameraAndAStableFacing()
        {
            ThirdPersonRig Rig = new ThirdPersonRig();
            Rig.Yaw = 0.35f;
            MoveWish Back = new MoveWish();
            Back.Stick = new Vector2(0f, -1f);
            Back.Facing = BodyFacing.Travel;

            GroundStep First = Rig.Plan(Back, 0.35f);
            GroundStep Again = Rig.Plan(Back, First.BodyYaw);
            Assert.AreEqual(0.35f, Rig.Yaw, 0.0001f);
            Assert.AreEqual(First.BodyYaw, Again.BodyYaw, 0.0001f);
            Assert.AreEqual(First.Direction, Again.Direction);
            Assert.IsTrue(Vector3.Dot(First.Direction, Rig.Forward) < -0.99f);
            Assert.IsTrue(Math.Abs(MathHelper.WrapAngle(First.BodyYaw - Rig.Yaw)) > 2.5f);
        }

        [TestMethod]
        public void WalkingBackwardTurnsOnceAndHolds()
        {
            ThirdPersonRig Rig = new ThirdPersonRig();
            Rig.Yaw = 0.35f;
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            Ada.RotationMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, Rig.Yaw);
            MoveWish Back = new MoveWish();
            Back.Stick = new Vector2(0f, -1f);
            Back.Facing = BodyFacing.Travel;

            int Sign = 0;
            int Reversals = 0;
            for (int i = 0; i < 40; i++)
            {
                float Before = ThirdPersonRig.YawOf(Ada.RotationMatrix);
                GroundStep Step = Rig.Plan(Back, Before);
                Ada.Move(Step.Direction, 1.0, 0.05, Step.BodyYaw);
                float Delta = MathHelper.WrapAngle(ThirdPersonRig.YawOf(Ada.RotationMatrix) - Before);
                if (Math.Abs(Delta) > 0.002f)
                {
                    int Next = Math.Sign(Delta);
                    if (Sign != 0 && Next != Sign)
                    {
                        Reversals++;
                    }

                    Sign = Next;
                }
            }

            GroundStep Landed = Rig.Plan(Back, ThirdPersonRig.YawOf(Ada.RotationMatrix));
            Assert.AreEqual(0, Reversals);
            Assert.AreEqual(0.35f, Rig.Yaw, 0.0001f);
            Assert.IsTrue(Math.Abs(MathHelper.WrapAngle(ThirdPersonRig.YawOf(Ada.RotationMatrix) - Landed.BodyYaw)) < 0.05f);
        }

        [TestMethod]
        public void CameraFacingBackpedalsWithoutTurningTheBody()
        {
            ThirdPersonRig Rig = new ThirdPersonRig();
            Rig.Yaw = -0.8f;
            MoveWish Back = new MoveWish();
            Back.Stick = new Vector2(0f, -1f);
            Back.Facing = BodyFacing.Camera;

            GroundStep Step = Rig.Plan(Back, 1.2f);
            Assert.IsFalse(Step.FaceTravel);
            Assert.AreEqual(Rig.Yaw, Step.BodyYaw, 0.0001f);
            Assert.IsTrue(Vector3.Dot(Step.Direction, Rig.Forward) < -0.99f);
        }

        [TestMethod]
        public void HoldFacingLeavesTheWishFreeForALaterMantle()
        {
            ThirdPersonRig Rig = new ThirdPersonRig();
            Rig.Yaw = 0.4f;
            MoveWish Along = new MoveWish();
            Along.Stick = new Vector2(0f, 1f);
            Along.Facing = BodyFacing.Hold;
            Along.HoldYaw = -1.1f;

            GroundStep Step = Rig.Plan(Along, 0.4f);
            Assert.AreEqual(-1.1f, Step.BodyYaw, 0.0001f);
            Assert.IsFalse(Step.FaceTravel);
            Assert.IsTrue(Vector3.Dot(Step.Direction, Rig.Forward) > 0.99f);
            Assert.AreEqual(0.4f, Rig.Yaw, 0.0001f);
        }
    }
}
