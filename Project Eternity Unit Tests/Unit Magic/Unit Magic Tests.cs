using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Magic;
using ProjectEternity.Units.Magic;

namespace ProjectEternity.UnitTests.UnitMagicTests
{
    [TestClass]
    public class UnitMagicTests
    {
        private static MagicProjectileSandbox Sandbox;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Sandbox = new MagicProjectileSandbox();
        }

        [TestInitialize()]
        public void Initialize()
        {
            Constants.TotalGameTime = 0;
            Sandbox.Reset();
        }

        [TestMethod]
        public void TestMagicLoad()
        {
            MagicElement.LoadAllMagicElements();
        }

        [TestMethod]
        public void TestMagicUnit()
        {
            List<MagicSpell> ListMagicSpell = new List<MagicSpell>();
            UnitMagic ActiveUser = new UnitMagic("Dummy Magic", ListMagicSpell);
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);
            ProjectileContext GlobalContext = new ProjectileContext();
            ProjectileParams MagicProjectileParams = new ProjectileParams(GlobalContext);
            GlobalContext.OwnerSandbox = Sandbox;
            MagicSpell FireballSpell = new MagicSpell(ActiveUser);
            ListMagicSpell.Add(FireballSpell);

            MagicCoreFireball FireballCore1 = new MagicCoreFireball(Params, MagicProjectileParams);
            FireballCore1.ListLinkedMagicElement.Add(new ChannelExternalManaSource(Params));
            FireballCore1.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            FireballSpell.ListMagicCore.Add(FireballCore1);

            ActiveUser.Init();

            Assert.AreEqual(1, ActiveUser.ListAttack.Count);
        }

        [TestMethod]
        public void TestMagicSanbox()
        {
            List<MagicSpell> ListMagicSpell = new List<MagicSpell>();
            UnitMagic ActiveUser = new UnitMagic("Dummy Magic", ListMagicSpell);

            Character DummyCharacter = new Character();
            DummyCharacter.Name = "Dummy Pilot";
            DummyCharacter.Level = 1;
            DummyCharacter.ArrayLevelMEL = new int[1] { 100 };
            DummyCharacter.ArrayLevelRNG = new int[1] { 100 };
            DummyCharacter.ArrayLevelDEF = new int[1] { 100 };
            DummyCharacter.ArrayLevelSKL = new int[1] { 100 };
            DummyCharacter.ArrayLevelEVA = new int[1] { 100 };
            DummyCharacter.ArrayLevelHIT = new int[1] { 200 };
            DummyCharacter.ArrayLevelMaxSP = new int[1] { 50 };
            ActiveUser.ArrayCharacterActive = new Character[1] { DummyCharacter };

            ActiveUser.ChanneledMana = 1000;
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams MagicParams = new MagicUserParams(ActiveSpell.GlobalContext);
            ProjectileContext GlobalContext = new ProjectileContext();
            ProjectileParams MagicProjectileParams = new ProjectileParams(GlobalContext);
            GlobalContext.OwnerSandbox = Sandbox;
            MagicSpell FireballSpell = new MagicSpell(ActiveUser);
            ListMagicSpell.Add(FireballSpell);

            MagicCoreFireball FireballCore1 = new MagicCoreFireball(MagicParams, MagicProjectileParams);

            FireballSpell.ListMagicCore.Add(FireballCore1);

            ActiveUser.Init();
            FireballSpell.ComputeSpell()[0].AddSkillEffectsToTarget("");
            Assert.AreEqual(1, Sandbox.ListProjectile.Count);
        }

        [TestMethod]
        public void TestMagicSanboxSimulation()
        {
            List<MagicSpell> ListMagicSpell = new List<MagicSpell>();
            UnitMagic ActiveUser = new UnitMagic("Dummy Magic", ListMagicSpell);

            Character DummyCharacter = new Character();
            DummyCharacter.Name = "Dummy Pilot";
            DummyCharacter.Level = 1;
            DummyCharacter.ArrayLevelMEL = new int[1] { 100 };
            DummyCharacter.ArrayLevelRNG = new int[1] { 100 };
            DummyCharacter.ArrayLevelDEF = new int[1] { 100 };
            DummyCharacter.ArrayLevelSKL = new int[1] { 100 };
            DummyCharacter.ArrayLevelEVA = new int[1] { 100 };
            DummyCharacter.ArrayLevelHIT = new int[1] { 200 };
            DummyCharacter.ArrayLevelMaxSP = new int[1] { 50 };
            ActiveUser.ArrayCharacterActive = new Character[1] { DummyCharacter };

            ActiveUser.ChanneledMana = 1000;
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);
            ProjectileContext GlobalContext = new ProjectileContext();
            ProjectileParams MagicProjectileParams = new ProjectileParams(GlobalContext);
            GlobalContext.OwnerSandbox = Sandbox;
            ListMagicSpell.Add(ActiveSpell);

            MagicCoreFireball FireballCore1 = new MagicCoreFireball(Params, MagicProjectileParams);
            FireballCore1.ListLinkedMagicElement.Add(new ChannelExternalManaSource(Params));
            FireballCore1.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            MagicCoreFireball FireballCore2 = new MagicCoreFireball(Params, MagicProjectileParams);
            FireballCore2.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore2.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            MagicCoreFireball FireballCore3 = new MagicCoreFireball(Params, MagicProjectileParams);
            FireballCore3.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore3.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            FireballCore1.ListLinkedMagicElement.Add(FireballCore2);
            FireballCore1.ListLinkedMagicElement.Add(FireballCore3);

            ActiveSpell.ListMagicCore.Add(FireballCore1);

            ActiveUser.Init();

            List<Core.Item.BaseAutomaticSkill> ListAttackAttribute = ListMagicSpell[0].ComputeSpell();
            CreateFireballEffect FireballEffect = (CreateFireballEffect)ListAttackAttribute[0].CurrentSkillLevel.ListActivation[1].ListEffect[0];
            FireballEffect.ChanneledMana = 2000;
            Constants.TotalGameTime = 10;

            Sandbox.SimulateAttack(ListAttackAttribute);

            Assert.AreEqual(10, Constants.TotalGameTime);
            Assert.AreEqual(15, Sandbox.TotalDamage);
        }
    }
}
