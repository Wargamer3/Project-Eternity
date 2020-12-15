using System;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ProjectEternity.GameScreens.AdventureScreen;

namespace ProjectEternity.UnitTests.AdventureTests
{
    [TestClass]
    public class AdventureMapTests
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
        public void TestPlayerBulletCollisionSuccess()
        {
            Vector2[] LocalPoints1 = CreateSquare(Vector2.Zero, 40);
            Vector2[] LocalPoints2 = CreateSquare(Vector2.Zero, 5);

            Map.Add(new Player(Map, LocalPoints1, 5, 5));
            Bullet NewBullet = new Bullet(Map, LocalPoints2, 5, 5);

            HashSet<Player> CollidingPlayers = Map.GetCollidingPlayers(NewBullet);

            Assert.AreEqual(1, CollidingPlayers.Count);
        }

        [TestMethod]
        public void TestPlayerBulletCollisionFail()
        {
            Vector2[] LocalPoints1 = CreateSquare(new Vector2(150, 50), 40);
            Vector2[] LocalPoints2 = CreateSquare(Vector2.Zero, 5);

            Map.Add(new Player(Map, LocalPoints1, 5, 5));
            Bullet NewBullet = new Bullet(Map, LocalPoints2, 5, 5);

            HashSet<Player> CollidingPlayers = Map.GetCollidingPlayers(NewBullet);

            Assert.AreEqual(0, CollidingPlayers.Count);
        }

        [TestMethod]
        public void TestMapCollisionUpdate()
        {
            Vector2[] LocalPoints1 = CreateSquare(Vector2.Zero, 40);
            Vector2[] LocalPoints2 = CreateSquare(Vector2.Zero, 5);

            Player NewPlayer = new Player(Map, LocalPoints1, 5, 5);
            Map.Add(NewPlayer);
            Bullet NewBullet = new Bullet(Map, LocalPoints2, 5, 5);
            Map.Add(NewBullet);

            Assert.IsTrue(NewBullet.IsAlive);
            Assert.AreEqual(100, NewPlayer.HP);

            Map.Update(new GameTime());

            Assert.IsFalse(NewBullet.IsAlive);
            Assert.AreEqual(95, NewPlayer.HP);
        }

        private static Vector2[] CreateSquare(Vector2 Position, float Size)
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
