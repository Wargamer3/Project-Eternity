using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.UnitTests.TripleThunderTests
{
    [TestClass]
    public class TripleThunderTests
    {
        private static FightingZone DummyMap;
        private static TripleThunderAttackContext AttackContext;
        private static TripleThunderRobotContext RobotContext;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            DummyMap.DicAutomaticSkillTarget.Clear();

            AttackContext = new TripleThunderAttackContext();
            RobotContext = new TripleThunderRobotContext();
            DummyMap = new FightingZone(RobotContext, AttackContext);
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
            DummyMap.ListLayer[0].ListAttackCollisionBox.Clear();
        }

        private BaseAutomaticSkill CreateDummySkill(BaseSkillRequirement Requirement, AutomaticSkillTargetType Target, BaseEffect Effect)
        {
            BaseAutomaticSkill NewSkill = new BaseAutomaticSkill();
            NewSkill.Name = "Dummy";
            NewSkill.ListSkillLevel.Add(new BaseSkillLevel());
            NewSkill.CurrentLevel = 1;

            BaseSkillActivation NewActivation = new BaseSkillActivation();
            NewSkill.CurrentSkillLevel.ListActivation.Add(NewActivation);

            NewActivation.ListRequirement.Add(Requirement);
            
            NewActivation.ListEffect.Add(Effect);
            NewActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>());
            NewActivation.ListEffectTargetReal[0].Add(Target);

            return NewSkill;
        }

        private RobotAnimation CreateDummyRobot()
        {
            ComboWeapon DummyWeapon = new ComboWeapon(5, false, 1);

            RobotAnimation NewRobotAnimation = new RobotAnimation(DummyMap.ListLayer[0], Vector2.Zero, 0, new List<WeaponBase>() { DummyWeapon });

            NewRobotAnimation.MaxHP = 100;
            NewRobotAnimation.MaxEN = 50;
            
            return NewRobotAnimation;
        }

        [TestMethod]
        public void TestLoad()
        {
            CreateDummySkill(DummyMap.DicRequirement[TimeAliveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[SelfAttackTargetType.Name].Copy(),
                                                            DummyMap.DicEffect[LaunchAttackEffect.Name].Copy());
        }

        [TestMethod]
        public void TestSkillActivation()
        {
            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[TimeAliveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[SelfAttackTargetType.Name].Copy(),
                                                            DummyMap.DicEffect[LaunchAttackEffect.Name].Copy());

            AttackContext.Owner = CreateDummyRobot();
            AttackContext.OwnerProjectile = new HitscanBox(0, new ExplosionOptions(), AttackContext.Owner, Vector2.Zero, 0);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
        }

        [TestMethod]
        public void TestTimeAliveRequirementFail()
        {
            TimeAliveRequirement NewRequirement = (TimeAliveRequirement)DummyMap.DicRequirement[TimeAliveRequirement.Name].Copy();
            NewRequirement.TimeToWait = 1000;
            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement, DummyMap.DicAutomaticSkillTarget[SelfAttackTargetType.Name].Copy(),
                                                            DummyMap.DicEffect[LaunchAttackEffect.Name].Copy());

            AttackContext.Owner = CreateDummyRobot();
            AttackContext.OwnerProjectile = new HitscanBox(0, new ExplosionOptions(), AttackContext.Owner, Vector2.Zero, 0);

            DummySkill.AddSkillEffectsToTarget("Shoot");
            List<BaseEffect> ListActiveEffect = AttackContext.Owner.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void TestTimeAliveRequirementFail2()
        {
            TimeAliveRequirement NewRequirement = (TimeAliveRequirement)DummyMap.DicRequirement[TimeAliveRequirement.Name].Copy();
            NewRequirement.TimeToWait = 1000;
            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement, DummyMap.DicAutomaticSkillTarget[SelfAttackTargetType.Name].Copy(),
                                                            DummyMap.DicEffect[LaunchAttackEffect.Name].Copy());

            AttackContext.Owner = CreateDummyRobot();
            AttackContext.OwnerProjectile = new HitscanBox(0, new ExplosionOptions(), AttackContext.Owner, Vector2.Zero, 0);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = AttackContext.Owner.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void TestTimeAliveRequirementSuccess()
        {
            TimeAliveRequirement NewRequirement = (TimeAliveRequirement)DummyMap.DicRequirement[TimeAliveRequirement.Name].Copy();
            NewRequirement.TimeToWait = 1000;
            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement, DummyMap.DicAutomaticSkillTarget[SelfAttackTargetType.Name].Copy(),
                                                            DummyMap.DicEffect[LaunchAttackEffect.Name].Copy());

            AttackContext.Owner = CreateDummyRobot();
            AttackContext.OwnerProjectile = new HitscanBox(0, new ExplosionOptions(), AttackContext.Owner, Vector2.Zero, 0);
            DummySkill.AddSkillEffectsToTarget(string.Empty);

            AttackContext.OwnerProjectile.Update(new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));
            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = AttackContext.Owner.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);//Should be 0 as shooting doesn't store its effect.
        }

        [TestMethod]
        public void TestShootSkillRequirementFail()
        {
            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[ShootRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[SelfTargetType.Name].Copy(),
                                                            DummyMap.DicEffect[ShootWeaponEffect.Name].Copy());

            RobotContext.Target = CreateDummyRobot();

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = RobotContext.Target.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }
        
        [TestMethod]
        public void TestShootSkillRequirementSuccess()
        {
            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[ShootRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[SelfTargetType.Name].Copy(),
                                                            DummyMap.DicEffect[ShootWeaponEffect.Name].Copy());

            RobotContext.Target = CreateDummyRobot();
            RobotContext.TargetWeapon = RobotContext.Target.PrimaryWeapons.ActiveWeapons[0];

            DummySkill.AddSkillEffectsToTarget("Shoot");
            List<BaseEffect> ListActiveEffect = RobotContext.Target.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);//Should be 0 as shooting doesn't store its effect.
        }

        [TestMethod]
        public void TestSkillChain()
        {
            ChangeAttackSpeedEffect TimedBulletSkillEffect = (ChangeAttackSpeedEffect)DummyMap.DicEffect[ChangeAttackSpeedEffect.Name].Copy();
            TimedBulletSkillEffect.Speed = 5;
            TimedBulletSkillEffect.NumberType = Core.Operators.NumberTypes.Absolute;
            TimedBulletSkillEffect.SignOperator = Core.Operators.SignOperators.PlusEqual;
            TimeAliveRequirement TimedBulletSkillRequirement = (TimeAliveRequirement)DummyMap.DicRequirement[TimeAliveRequirement.Name].Copy();
            TimedBulletSkillRequirement.TimeToWait = 0.4;
            
            BaseAutomaticSkill TimedBulletSkill = CreateDummySkill(TimedBulletSkillRequirement,
                                                            DummyMap.DicAutomaticSkillTarget[SelfAttackTargetType.Name].Copy(),
                                                            TimedBulletSkillEffect);

            BaseEffect DummyBulletInitSkillEffect = DummyMap.DicEffect[LaunchAttackEffect.Name].Copy();
            DummyBulletInitSkillEffect.ListFollowingSkill = new List<BaseAutomaticSkill>() { TimedBulletSkill };

            BaseAutomaticSkill DummyBulletInitSkill = CreateDummySkill(new OnCreatedRequirement(),
                                                            DummyMap.DicAutomaticSkillTarget[SelfAttackTargetType.Name].Copy(),
                                                            DummyBulletInitSkillEffect);

            BaseEffect ShootEffect1 = DummyMap.DicEffect[ShootWeaponEffect.Name].Copy();
            ShootEffect1.IsStacking = true;
            ShootEffect1.MaximumStack = 5;
            ShootEffect1.ListFollowingSkill = new List<BaseAutomaticSkill>() { DummyBulletInitSkill };
            BaseEffect ShootEffect2 = DummyMap.DicEffect[ShootWeaponEffect.Name].Copy();
            ShootEffect2.IsStacking = true;
            ShootEffect2.MaximumStack = 5;

            BaseAutomaticSkill DummyInitShootSkill1 = CreateDummySkill(DummyMap.DicRequirement[ShootRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[SelfTargetType.Name].Copy(),
                                                            ShootEffect1);

            BaseAutomaticSkill DummyInitShootSkill2 = CreateDummySkill(DummyMap.DicRequirement[ShootRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[SelfTargetType.Name].Copy(),
                                                            ShootEffect2);

            RobotContext.Target = CreateDummyRobot();
            RobotContext.TargetWeapon = RobotContext.Target.PrimaryWeapons.ActiveWeapons[0];

            List<BaseAutomaticSkill> ListSkill = new List<BaseAutomaticSkill>() { DummyInitShootSkill1, DummyInitShootSkill2 };
            for (int i = 0; i < ListSkill.Count; i++)
            {
                BaseAutomaticSkill ActiveSkill = ListSkill[i];
                ActiveSkill.AddSkillEffectsToTarget("Shoot");
            }
            List<BaseEffect> ListActiveEffect = RobotContext.Target.Effects.GetActiveEffects("Dummy");

            Assert.AreEqual(3, DummyMap.ListLayer[0].ListAttackCollisionBox.Count);

            Assert.AreEqual(1, DummyMap.ListLayer[0].ListAttackCollisionBox[0].ListActiveSkill.Count);
            Assert.AreEqual(DummyBulletInitSkill.CurrentSkillLevel.ListActivation[0].ListEffect[0], 
                DummyMap.ListLayer[0].ListAttackCollisionBox[0].ListActiveSkill[0].CurrentSkillLevel.ListActivation[0].ListEffect[0]);

            Assert.AreEqual(2, DummyMap.ListLayer[0].ListAttackCollisionBox[1].ListActiveSkill.Count);
            Assert.AreEqual(TimedBulletSkill.CurrentSkillLevel.ListActivation[0].ListEffect[0],
                DummyMap.ListLayer[0].ListAttackCollisionBox[1].ListActiveSkill[0].CurrentSkillLevel.ListActivation[0].ListEffect[0]);
            
            Assert.AreEqual(new Vector2(100000, 0), DummyMap.ListLayer[0].ListAttackCollisionBox[1].Speed);

            DummyMap.ListLayer[0].ListAttackCollisionBox[1].Update(new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));
            Assert.AreEqual(new Vector2(100005, 0), DummyMap.ListLayer[0].ListAttackCollisionBox[1].Speed);
        }
    }
}
