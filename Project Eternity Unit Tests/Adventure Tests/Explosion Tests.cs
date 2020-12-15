using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.GameScreens.AdventureScreen;

namespace ProjectEternity.UnitTests.AdventureTests
{
    [TestClass]
    public class ExplosionTests
    {
        private static AdventureMap Map;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Map = new AdventureMap();
        }

        [TestInitialize()]
        public void Initialize()
        {
            Map.Reset();
        }

        [TestMethod]
        public void TestExplosionWorldCollisionSuccess()
        {
            Vector2[] LocalPoints1 = CreateCube(Vector2.Zero, 16);

            Map.Add(new WorldObject(LocalPoints1, 5, 5));
            ComplexLinearExplosion NewExplosion = new ComplexLinearExplosion(Map, new Vector2(0, 0), new Vector2(0, -5));

            HashSet<WorldObject> CollidingPlayers = Map.GetCollidingWorldObjects(NewExplosion);

            Assert.AreEqual(1, CollidingPlayers.Count);
            Assert.IsTrue(CollidingPlayers.First().Collision.CollideWith(NewExplosion.Collision, Vector2.Zero, out _, out _, out _));
        }

        [TestMethod]
        public void TestExplosionWorldWithMovementCollisionSuccess()
        {
            Vector2[] LocalPoints = CreateCube(Vector2.Zero, 16);

            Map.Add(new WorldObject(LocalPoints, 5, 5));
            ComplexLinearExplosion NewExplosion = new ComplexLinearExplosion(Map, new Vector2(0, 50), new Vector2(0, -5));

            HashSet<WorldObject> CollidingPlayers = Map.GetCollidingWorldObjects(NewExplosion);

            Assert.AreEqual(1, CollidingPlayers.Count);
            Assert.IsFalse(CollidingPlayers.First().Collision.CollideWith(NewExplosion.Collision, Vector2.Zero, out _, out _, out _));
            //NewExplosion.Update(new GameTime());
            //Collision box extended to hit the wall but didn't progress further.
            Assert.AreEqual(new Vector2(-32, 18), NewExplosion.Collision.ListCollisionPolygon[0].ArrayVertex[0]);
            Assert.AreEqual(new Vector2(32, 18), NewExplosion.Collision.ListCollisionPolygon[0].ArrayVertex[3]);
        }

        [TestMethod]
        public void TestExplosionWorldSlopeWithMovementCollisionSuccess()
        {
            float RotationAngle = MathHelper.ToRadians(45);
            Vector2[] LocalPoints = CreateCube(Vector2.Zero, 16);
            for (int P = 0; P < LocalPoints.Length; ++P)
            {
                double cosTheta = Math.Cos(RotationAngle);
                double sinTheta = Math.Sin(RotationAngle);
                double RotatedX = cosTheta * LocalPoints[P].X - sinTheta * LocalPoints[P].Y;
                double RotatedY = sinTheta * LocalPoints[P].X + cosTheta * LocalPoints[P].Y;

                LocalPoints[P] = new Vector2((float)RotatedX, (float)RotatedY);
            }

            Map.Add(new WorldObject(LocalPoints, 5, 5));
            ComplexLinearExplosion NewExplosion = new ComplexLinearExplosion(Map, new Vector2(0, 60), new Vector2(0, -25));

            HashSet<WorldObject> CollidingPlayers = Map.GetCollidingWorldObjects(NewExplosion);

            Assert.AreEqual(1, CollidingPlayers.Count);
            Assert.IsFalse(CollidingPlayers.First().Collision.CollideWith(NewExplosion.Collision, Vector2.Zero, out _, out _, out _));
            //NewExplosion.Update(new GameTime());
            //Collision box extended to hit the wall but didn't progress further.
            Assert.AreEqual(new Vector2(-32, 28), NewExplosion.Collision.ListCollisionPolygon[0].ArrayVertex[0]);
            Assert.AreEqual(new Vector2(32, 28), NewExplosion.Collision.ListCollisionPolygon[0].ArrayVertex[3]);
        }

        [TestMethod]
        public void TestSimpleExplosionWorldSlopeWithMovementCollisionSuccess()
        {
            float RotationAngle = MathHelper.ToRadians(45);
            Vector2[] LocalPoints = CreateCube(Vector2.Zero, 16);
            for (int P = 0; P < LocalPoints.Length; ++P)
            {
                double cosTheta = Math.Cos(RotationAngle);
                double sinTheta = Math.Sin(RotationAngle);
                double RotatedX = cosTheta * LocalPoints[P].X - sinTheta * LocalPoints[P].Y;
                double RotatedY = sinTheta * LocalPoints[P].X + cosTheta * LocalPoints[P].Y;

                LocalPoints[P] = new Vector2((float)RotatedX, (float)RotatedY);
            }

            Map.Add(new WorldObject(LocalPoints, 5, 5));
            SimpleLinearExplosion NewExplosion = new SimpleLinearExplosion(Map, new Vector2(50, 60), new Vector2(0, -25));
            Map.Add(NewExplosion);

            HashSet<WorldObject> CollidingPlayers = Map.GetCollidingWorldObjects(NewExplosion);

            Assert.AreEqual(1, Map.GetExplosions().Count);
            Assert.AreEqual(1, CollidingPlayers.Count);
            Assert.IsFalse(CollidingPlayers.First().Collision.CollideWith(NewExplosion.Collision, Vector2.Zero, out _, out _, out _));
            NewExplosion.Update(new GameTime());
            Assert.AreEqual(2, Map.GetExplosions().Count);
            SimpleLinearExplosion SlopeExplosion = Map.GetExplosions()[1];
            //Collision box extended to hit the wall but didn't progress further.
        }

        private static Vector2[] CreateCube(Vector2 Position, float Size)
        {
            Vector2[] LocalPoints = new Vector2[4]
            {
                new Vector2(Position.X - Size, Position.Y - Size),
                new Vector2(Position.X - Size, Position.Y + Size),
                new Vector2(Position.X + Size, Position.Y + Size),
                new Vector2(Position.X + Size, Position.Y - Size),
            };

            return LocalPoints;
        }
    }
}
