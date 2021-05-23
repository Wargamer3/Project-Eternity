using System;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.GameScreens.AdventureScreen;
using ProjectEternity.Core;

namespace ProjectEternity.UnitTests.AdventureTests
{
    [TestClass]
    public class CollisionZoneTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
        }

        [TestInitialize()]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestCollisionZoneAdd()
        {
            CollisionZone<Bullet> TestCollisionZone = new CollisionZone<Bullet>(2000, 100, -500, -500);
            Vector2[] LocalPoints1 = CreateCube(Vector2.Zero, 40);
            Vector2[] LocalPoints2 = CreateCube(Vector2.Zero, 5);

            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints1, 5, 5));
            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints2, 5, 5));
            Assert.AreEqual(2, TestCollisionZone.ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(2, TestCollisionZone.ArraySubZone[25].ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(TestCollisionZone.ArraySubZone[25], TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Value.Collision.ListParent.Find(TestCollisionZone.ArraySubZone[25]).Value);
            Assert.AreEqual(TestCollisionZone.ArraySubZone[25], TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Next.Value.Collision.ListParent.Find(TestCollisionZone.ArraySubZone[25]).Value);
        }

        [TestMethod]
        public void TestCollisionZoneAddAndMove()
        {
            CollisionZone<Bullet> TestCollisionZone = new CollisionZone<Bullet>(2000, 100, -500, -500);
            Vector2[] LocalPoints1 = CreateCube(Vector2.Zero, 40);
            Vector2[] LocalPoints2 = CreateCube(Vector2.Zero, 40);

            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints1, 5, 5));
            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints2, 5, 5));

            TestCollisionZone.Move(TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Value, Vector2.Zero);
            TestCollisionZone.Move(TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Next.Value, Vector2.Zero);

            Assert.AreEqual(2, TestCollisionZone.ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(2, TestCollisionZone.ArraySubZone[25].ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(TestCollisionZone.ArraySubZone[25], TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Value.Collision.ListParent.Find(TestCollisionZone.ArraySubZone[25]).Value);
            Assert.AreEqual(TestCollisionZone.ArraySubZone[25], TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Next.Value.Collision.ListParent.Find(TestCollisionZone.ArraySubZone[25]).Value);
        }

        [TestMethod]
        public void TestCollisionZoneCollision()
        {
            CollisionZone<Bullet> TestCollisionZone = new CollisionZone<Bullet>(2000, 100, -500, -500);
            Vector2[] LocalPoints1 = CreateCube(Vector2.Zero, 40);
            Vector2[] LocalPoints2 = CreateCube(Vector2.Zero, 40);

            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints1, 5, 5));
            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints2, 5, 5));
            Assert.AreEqual(true, DetectCollisionBetweenSelfAndOthers(TestCollisionZone, TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Value));
        }

        [TestMethod]
        public void TestCollisionZoneCollisionFail()
        {
            CollisionZone<Bullet> TestCollisionZone = new CollisionZone<Bullet>(2000, 100, -500, -500);
            Vector2[] LocalPoints1 = CreateCube(Vector2.Zero, 40);
            Vector2[] LocalPoints2 = CreateCube(new Vector2(500, 500), 40);

            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints1, 5, 5));
            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints2, 5, 5));
            Assert.AreEqual(1, TestCollisionZone.ArraySubZone[25].ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(1, TestCollisionZone.ArraySubZone[52].ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(false, DetectCollisionBetweenSelfAndOthers(TestCollisionZone, TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Value));
        }

        [TestMethod]
        public void TestCollisionZoneRemove()
        {
            CollisionZone<Bullet> TestCollisionZone = new CollisionZone<Bullet>(2000, 100, -500, -500);
            Vector2[] LocalPoints1 = CreateCube(Vector2.Zero, 40);
            Vector2[] LocalPoints2 = CreateCube(Vector2.Zero, 40);

            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints1, 5, 5));
            TestCollisionZone.AddToZone(new Bullet(null, LocalPoints2, 5, 5));

            TestCollisionZone.Remove(TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Value);

            Assert.AreEqual(1, TestCollisionZone.ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(1, TestCollisionZone.ArraySubZone[25].ListObjectInZoneAndOverlappingParents.Count);
            Assert.AreEqual(TestCollisionZone.ArraySubZone[25], TestCollisionZone.ListObjectInZoneAndOverlappingParents.First.Value.Collision.ListParent.Find(TestCollisionZone.ArraySubZone[25]).Value);
        }

        private bool DetectCollisionBetweenSelfAndOthers(CollisionZone<Bullet> BulletCollisions, Bullet ActiveBullet)
        {
            System.Collections.Generic.HashSet<Bullet> Obstacles = BulletCollisions.GetCollidableObjects(ActiveBullet);
            foreach (Bullet ActivePlayer in Obstacles)
            {
                if (ActivePlayer.Collision.CollideWith(ActiveBullet.Collision, Vector2.Zero, out _, out _, out _))
                {
                    return true;
                }
            }

            return false;
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
