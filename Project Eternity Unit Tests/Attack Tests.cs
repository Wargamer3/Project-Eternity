using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class AttackTests
    {
        [TestMethod]
        public void TestMapAttackDirectionalUpAligned()
        {
            MAPAttackAttributes MAPAttack = CreateMAPAttack(1, 3);
            MAPAttack.ListChoice[1][0] = true;
            MAPAttack.ListChoice[1][1] = true;
            MAPAttack.ListChoice[1][2] = true;
            MAPAttack.ListChoice[1][3] = true;
            MAPAttack.ListChoice[1][4] = true;
            MAPAttack.ListChoice[1][5] = false;
            MAPAttack.ListChoice[1][6] = false;

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 2, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 3, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 4, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 5, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 6, 0), new Vector3(0, 0, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 1, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 2, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 3, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 4, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 5, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 6, 0), new Vector3(0, 0, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(1, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 2, 0), new Vector3(1, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 3, 0), new Vector3(1, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 4, 0), new Vector3(1, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 5, 0), new Vector3(1, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 6, 0), new Vector3(1, 0, 0), 0, 0));
        }

        [TestMethod]
        public void TestMapAttackDirectionalDownAligned()
        {
            MAPAttackAttributes MAPAttack = CreateMAPAttack(1, 3);
            MAPAttack.ListChoice[1][0] = true;
            MAPAttack.ListChoice[1][1] = true;
            MAPAttack.ListChoice[1][2] = true;
            MAPAttack.ListChoice[1][3] = true;
            MAPAttack.ListChoice[1][4] = true;
            MAPAttack.ListChoice[1][5] = false;
            MAPAttack.ListChoice[1][6] = false;

            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(0, 2, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(0, 3, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(0, 4, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(0, 5, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(0, 6, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 2, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 3, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 4, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 5, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 6, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 2, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 3, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 4, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 5, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 6, 0), 0, 0));
        }

        [TestMethod]
        public void TestMapAttackDirectionalLeftAligned()
        {
            MAPAttackAttributes MAPAttack = CreateMAPAttack(1, 3);
            MAPAttack.ListChoice[1][0] = true;
            MAPAttack.ListChoice[1][1] = true;
            MAPAttack.ListChoice[1][2] = true;
            MAPAttack.ListChoice[1][3] = true;
            MAPAttack.ListChoice[1][4] = true;
            MAPAttack.ListChoice[1][5] = false;
            MAPAttack.ListChoice[1][6] = false;

            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(2, 0, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(3, 0, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(4, 0, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(5, 0, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(6, 0, 0), new Vector3(0, 0, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 1, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(2, 1, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(3, 1, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(4, 1, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(5, 1, 0), new Vector3(0, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(6, 1, 0), new Vector3(0, 0, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(1, 0, 0), new Vector3(0, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(2, 0, 0), new Vector3(0, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(3, 0, 0), new Vector3(0, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(4, 0, 0), new Vector3(0, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(5, 0, 0), new Vector3(0, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(6, 0, 0), new Vector3(0, 1, 0), 0, 0));
        }

        [TestMethod]
        public void TestMapAttackDirectionalRightAligned()
        {
            MAPAttackAttributes MAPAttack = CreateMAPAttack(1, 3);
            MAPAttack.ListChoice[1][0] = true;
            MAPAttack.ListChoice[1][1] = true;
            MAPAttack.ListChoice[1][2] = true;
            MAPAttack.ListChoice[1][3] = true;
            MAPAttack.ListChoice[1][4] = true;
            MAPAttack.ListChoice[1][5] = false;
            MAPAttack.ListChoice[1][6] = false;

            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(2, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(3, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(4, 0, 0), 0, 0));
            Assert.IsTrue(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(5, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(6, 0, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(1, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(2, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(3, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(4, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(5, 0, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 1, 0), new Vector3(6, 0, 0), 0, 0));

            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(1, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(2, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(3, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(4, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(5, 1, 0), 0, 0));
            Assert.IsFalse(MAPAttack.CanAttackTarget(new Vector3(0, 0, 0), new Vector3(6, 1, 0), 0, 0));
        }

        private MAPAttackAttributes CreateMAPAttack(int Width, int Height)
        {
            MAPAttackAttributes MAPAttack = new MAPAttackAttributes();
            List<List<bool>> ListChoice = new List<List<bool>>(Width * 2 + 1);
            for (int X = 0; X < Width * 2 + 1; X++)
            {
                ListChoice.Add(new List<bool>(Height * 2 + 1));
                for (int Y = 0; Y < Height * 2 + 1; Y++)
                    ListChoice[X].Add(false);
            }

            MAPAttack.Width = Width;
            MAPAttack.Height = Height;
            MAPAttack.ListChoice = ListChoice;
            MAPAttack.Property = WeaponMAPProperties.Direction;
            MAPAttack.FriendlyFire = false;
            MAPAttack.Delay = 0;

            return MAPAttack;
        }
    }
}
