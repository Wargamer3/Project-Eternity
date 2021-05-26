using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Magic;
using static ProjectEternity.Core.Magic.TriggerAfterTimeEllapsed;

namespace ProjectEternity.UnitTests.TripleThunderTests
{
    [TestClass]
    public class TripleThunderMagicTests
    {
        private static FightingZone DummyMap;
        private static TripleThunderAttackContext AttackContext;
        private static TripleThunderAttackParams AttackParams;
        private static TripleThunderRobotContext RobotContext;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            AutomaticSkillTargetType.DicTargetType.Clear();

            AttackContext = new TripleThunderAttackContext();
            RobotContext = new TripleThunderRobotContext();
            DummyMap = new FightingZone(RobotContext, AttackContext);
            AttackParams = DummyMap.AttackParams;
            DummyMap.ListLayer.Add(new Layer(DummyMap));
            DummyMap.LoadTripleThunderEffects();
            DummyMap.LoadTripleThunderRequirements();
            DummyMap.LoadTripleThunderTargetTypes();

            RobotContext.Map = DummyMap;
            AttackContext.OwnerSandbox = DummyMap.ListLayer[0];
        }

        [TestInitialize()]
        public void Startup()
        {
            Constants.TotalGameTime = 0;
            DummyMap.ListLayer[0].ListAttackCollisionBox.Clear();

            RobotContext.Map = DummyMap;
            AttackContext.OwnerSandbox = DummyMap.ListLayer[0];
        }

        private RobotAnimation CreateDummyRobot()
        {
            Weapon DummyWeapon = new Weapon(5, false, 1);

            RobotAnimation NewRobotAnimation = new RobotAnimation(DummyMap.ListLayer[0], Vector2.Zero, 0, new List<Weapon>() { DummyWeapon });

            NewRobotAnimation.MaxHP = 100;
            NewRobotAnimation.MaxEN = 50;

            return NewRobotAnimation;
        }

        [TestMethod]
        public void TestExplodingFireballCreation()
        {
            MagicUser ActiveUser = new MagicUser();
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);

            InvisibleMagicCoreFireball FireballCore1 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore1.ListLinkedMagicElement.Add(new IncreasePower(Params));
            FireballCore1.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore1.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params));

            MagicCoreEnchantlLevel1 EnchantCore = new MagicCoreEnchantlLevel1(Params);
            EnchantCore.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params));
            FireballCore1.ListLinkedMagicElement.Add(EnchantCore);

            InvisibleMagicCoreFireball FireballCore2 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore2.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params));
            FireballCore1.ListLinkedMagicElement.Add(FireballCore2);

            InvisibleMagicCoreFireball FireballCore3 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore3.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params));
            FireballCore1.ListLinkedMagicElement.Add(FireballCore3);

            ActiveSpell.ListMagicCore.Add(FireballCore1);

            BaseAutomaticSkill NewSkill = new MagicSpell(ActiveSpell, ActiveUser).ComputeSpell()[0];

            Assert.AreEqual("Fireball Effect", NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].EffectTypeName);
            Assert.AreEqual(TimeEllapsedRequirement.Name, NewSkill.CurrentSkillLevel.ListActivation[1].ListRequirement[1].SkillRequirementName);

            Assert.AreEqual(3, NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].ListFollowingSkill.Count);
            Assert.AreEqual("Enchant", NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].ListFollowingSkill[0].CurrentSkillLevel.ListActivation[1].ListEffect[0].EffectTypeName);
            Assert.AreEqual("Fireball Effect", NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].ListFollowingSkill[1].CurrentSkillLevel.ListActivation[1].ListEffect[0].EffectTypeName);
            Assert.AreEqual("Fireball Effect", NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].ListFollowingSkill[2].CurrentSkillLevel.ListActivation[1].ListEffect[0].EffectTypeName);

            Assert.AreEqual(TimeEllapsedRequirement.Name, NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].ListFollowingSkill[0].CurrentSkillLevel.ListActivation[1].ListRequirement[1].SkillRequirementName);
            Assert.AreEqual(TimeEllapsedRequirement.Name, NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].ListFollowingSkill[1].CurrentSkillLevel.ListActivation[1].ListRequirement[1].SkillRequirementName);
            Assert.AreEqual(TimeEllapsedRequirement.Name, NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0].ListFollowingSkill[2].CurrentSkillLevel.ListActivation[1].ListRequirement[1].SkillRequirementName);
        }

        [TestMethod]
        public void TestFireballChannelingActivation()
        {
            AttackContext.Owner = CreateDummyRobot();

            MagicUser ActiveUser = new MagicUser();
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);
            ActiveUser.ManaReserves = 1000;
            ActiveUser.CurrentMana = 100;

            InvisibleMagicCoreFireball FireballCore1 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore1.ListLinkedMagicElement.Add(new ChannelExternalManaSource(Params));

            ActiveSpell.ListMagicCore.Add(FireballCore1);
            ActiveSpell = new MagicSpell(ActiveSpell, ActiveUser);
            BaseAutomaticSkill NewSkill = ActiveSpell.ComputeSpell()[0];
            CreateFireballEffectTripleThunder Fireball = (CreateFireballEffectTripleThunder)NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0];

            for (int i = 0; i < 9; i++)
            {
                NewSkill.AddSkillEffectsToTarget(string.Empty);

                Assert.AreEqual(i + 1, Fireball.ChanneledMana);
            }

            NewSkill.AddSkillEffectsToTarget(string.Empty);

            Assert.AreEqual(1, ((InvisibleMagicCoreFireball)ActiveSpell.ListMagicCore[0]).NumberOfExecutions);
            Assert.AreEqual(0, ActiveUser.ChanneledMana);
            Assert.AreEqual(100, ActiveUser.CurrentMana);
        }

        [TestMethod]
        public void TestExplodingFireballActivation()
        {
            AttackContext.Owner = CreateDummyRobot();

            MagicUser ActiveUser = new MagicUser();
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);
            ActiveUser.ManaReserves = 1000;
            ActiveUser.CurrentMana = 100;

            InvisibleMagicCoreFireball FireballCore1 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore1.ListLinkedMagicElement.Add(new ChannelExternalManaSource(Params));
            FireballCore1.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            InvisibleMagicCoreFireball FireballCore2 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore2.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore2.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            InvisibleMagicCoreFireball FireballCore3 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore3.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore3.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            FireballCore1.ListLinkedMagicElement.Add(FireballCore2);
            FireballCore1.ListLinkedMagicElement.Add(FireballCore3);

            ActiveSpell.ListMagicCore.Add(FireballCore1);
            ActiveSpell = new MagicSpell(ActiveSpell, ActiveUser);
            BaseAutomaticSkill NewSkill = ActiveSpell.ComputeSpell()[0];
            CreateFireballEffectTripleThunder FireballEffect = (CreateFireballEffectTripleThunder)NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0];
            FireballEffect.ChanneledMana = 2000;

            NewSkill.AddSkillEffectsToTarget(string.Empty);

            Assert.AreEqual(0, DummyMap.ListLayer[0].ListAttackCollisionBox.Count);
            Assert.AreEqual(0, ((InvisibleMagicCoreFireball)ActiveSpell.ListMagicCore[0]).NumberOfExecutions);
            Assert.AreEqual(0, ((InvisibleMagicCoreFireball)ActiveSpell.ListMagicCore[0].ListLinkedMagicElement[2]).NumberOfExecutions);
            Assert.AreEqual(0, ((InvisibleMagicCoreFireball)ActiveSpell.ListMagicCore[0].ListLinkedMagicElement[3]).NumberOfExecutions);
            Constants.TotalGameTime = 10;
            NewSkill.AddSkillEffectsToTarget(string.Empty);

            Assert.AreEqual(1, ((InvisibleMagicCoreFireball)ActiveSpell.ListMagicCore[0]).NumberOfExecutions);
            Assert.AreEqual(0, ((InvisibleMagicCoreFireball)ActiveSpell.ListMagicCore[0].ListLinkedMagicElement[2]).NumberOfExecutions);
            Assert.AreEqual(0, ((InvisibleMagicCoreFireball)ActiveSpell.ListMagicCore[0].ListLinkedMagicElement[3]).NumberOfExecutions);

            Assert.AreEqual(1, DummyMap.ListLayer[0].ListAttackCollisionBox.Count);
            Assert.AreEqual(2, DummyMap.ListLayer[0].ListAttackCollisionBox[0].ListActiveSkill.Count);
            InvisibleFireball Fireball1 = (InvisibleFireball)DummyMap.ListLayer[0].ListAttackCollisionBox[0];
            IMagicUser FireballParent = Fireball1.Parent;

            //Channel enough Mana for 2 fireballs
            for (int i = 0; i < 50; i++)
            {
                DummyMap.ListLayer[0].ListAttackCollisionBox[0].Update(new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 0, 16)));
                Assert.AreEqual(2000 - i * 2, FireballParent.CurrentMana);
            }

            Constants.TotalGameTime = 20;

            Assert.AreEqual(true, DummyMap.ListLayer[0].ListAttackCollisionBox.Contains(Fireball1));

            DummyMap.ListLayer[0].ListAttackCollisionBox[0].Update(new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 50, 0)));

            Assert.AreEqual(false, DummyMap.ListLayer[0].ListAttackCollisionBox.Contains(Fireball1));
            Assert.AreEqual(2, DummyMap.ListLayer[0].ListAttackCollisionBox.Count);
        }

        [TestMethod]
        public void TestExplodingFireballPosition()
        {
            AttackContext.Owner = CreateDummyRobot();

            MagicUser ActiveUser = new MagicUser();
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);
            ActiveUser.ManaReserves = 1000;
            ActiveUser.CurrentMana = 100;

            InvisibleMagicCoreFireball FireballCore1 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore1.ListLinkedMagicElement.Add(new ChannelExternalManaSource(Params));
            FireballCore1.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            InvisibleMagicCoreFireball FireballCore2 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore2.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore2.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            InvisibleMagicCoreFireball FireballCore3 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore3.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore3.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            FireballCore1.ListLinkedMagicElement.Add(FireballCore2);
            FireballCore1.ListLinkedMagicElement.Add(FireballCore3);

            ActiveSpell.ListMagicCore.Add(FireballCore1);
            ActiveSpell = new MagicSpell(ActiveSpell, ActiveUser);
            BaseAutomaticSkill NewSkill = ActiveSpell.ComputeSpell()[0];
            CreateFireballEffectTripleThunder Fireball1 = (CreateFireballEffectTripleThunder)NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0];
            Fireball1.ChanneledMana = 2000;
            NewSkill.AddSkillEffectsToTarget(string.Empty);
            Constants.TotalGameTime = 10;
            NewSkill.AddSkillEffectsToTarget(string.Empty);
            DummyMap.ListLayer[0].ListAttackCollisionBox[0].Collision.ListCollisionPolygon[0].Center = new Vector2(10, 10);

            //Channel enough Mana for 2 fireballs
            for (int i = 0; i < 50; i++)
            {
                DummyMap.ListLayer[0].ListAttackCollisionBox[0].Update(new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 0, 16)));
            }

            Constants.TotalGameTime = 20;

            DummyMap.ListLayer[0].ListAttackCollisionBox[0].Update(new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 50, 0)));

            Assert.AreEqual(2, DummyMap.ListLayer[0].ListAttackCollisionBox.Count);
            Assert.AreEqual(new Vector2(15, 10) , DummyMap.ListLayer[0].ListAttackCollisionBox[0].Collision.ListCollisionPolygon[0].Center);
            Assert.AreEqual(new Vector2(15, 10), DummyMap.ListLayer[0].ListAttackCollisionBox[1].Collision.ListCollisionPolygon[0].Center);
        }

        [TestMethod]
        public void TestExplodingFireballPositionWithOffset()
        {
            AttackContext.Owner = CreateDummyRobot();

            MagicUser ActiveUser = new MagicUser();
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);
            ActiveUser.ManaReserves = 1000;
            ActiveUser.CurrentMana = 100;

            InvisibleMagicCoreFireball FireballCore1 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore1.ListLinkedMagicElement.Add(new ChannelExternalManaSource(Params));
            FireballCore1.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));

            InvisibleMagicCoreFireball FireballCore2 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore2.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore2.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));
            FireballCore2.ListLinkedMagicElement.Add(new MagicCoreOffset(Params, AttackParams, -15, 0));

            InvisibleMagicCoreFireball FireballCore3 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore3.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));
            FireballCore3.ListLinkedMagicElement.Add(new TriggerAfterTimeEllapsed(Params, 5));
            FireballCore3.ListLinkedMagicElement.Add(new MagicCoreOffset(Params, AttackParams, 55, 0));

            FireballCore1.ListLinkedMagicElement.Add(FireballCore2);
            FireballCore1.ListLinkedMagicElement.Add(FireballCore3);

            ActiveSpell.ListMagicCore.Add(FireballCore1);
            ActiveSpell = new MagicSpell(ActiveSpell, ActiveUser);
            BaseAutomaticSkill NewSkill = ActiveSpell.ComputeSpell()[0];
            CreateFireballEffectTripleThunder Fireball1 = (CreateFireballEffectTripleThunder)NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect[0];
            Fireball1.ChanneledMana = 2000;
            NewSkill.AddSkillEffectsToTarget(string.Empty);
            Constants.TotalGameTime = 10;
            NewSkill.AddSkillEffectsToTarget(string.Empty);
            DummyMap.ListLayer[0].ListAttackCollisionBox[0].Collision.ListCollisionPolygon[0].Center = new Vector2(10, 10);

            //Channel enough Mana for 2 fireballs
            for (int i = 0; i < 50; i++)
            {
                DummyMap.ListLayer[0].ListAttackCollisionBox[0].Update(new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 0, 16)));
            }

            Constants.TotalGameTime = 20;

            DummyMap.ListLayer[0].ListAttackCollisionBox[0].Update(new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 50, 0)));

            Assert.AreEqual(0, DummyMap.ListLayer[0].ListAttackCollisionBox[0].Collision.ListCollisionPolygon[0].Center.X);
            Assert.AreEqual(70, DummyMap.ListLayer[0].ListAttackCollisionBox[1].Collision.ListCollisionPolygon[0].Center.X);
        }

        [TestMethod]
        public void TestManaReserveUsage()
        {
            AttackContext.Owner = CreateDummyRobot();

            MagicUser ActiveUser = new MagicUser();
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);
            ActiveUser.ManaReserves = 1000;
            ActiveUser.CurrentMana = 100;

            InvisibleMagicCoreFireball FireballCore1 = new InvisibleMagicCoreFireball(Params, AttackParams);
            FireballCore1.ListLinkedMagicElement.Add(new ChannelInternalManaSource(Params));

            ActiveSpell.ListMagicCore.Add(FireballCore1);
            ActiveSpell = new MagicSpell(ActiveSpell, ActiveUser);
            BaseAutomaticSkill NewSkill = ActiveSpell.ComputeSpell()[0];

            for (int i = 0; i < 10; i++)
            {
                NewSkill.AddSkillEffectsToTarget(string.Empty);
            }

            Assert.AreEqual(0, ActiveUser.ChanneledMana);
            Assert.AreEqual(990, ActiveUser.ManaReserves);
            Assert.AreEqual(100, ActiveUser.CurrentMana);
        }
    }
}
