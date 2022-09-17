using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Units;
using ProjectEternity.Units.Magic;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class AnimationScreenTests
    {
        private static DeathmatchMap DummyMap;
        private static BattleContext GlobalDeathmatchContext;
        Squad DummySquad;
        Squad EnemySquad;
        AnimationScreen DummyAnimation;
        AnimationClass.AnimationLayer DummyLayer;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            GlobalDeathmatchContext = new BattleContext();
            DummyMap = DeathmatchMapHelper.CreateDummyMap(GlobalDeathmatchContext);
        }

        [TestInitialize()]
        public void Initialize()
        {
            Constants.TotalGameTime = 0;
        }

        //A Battle animation is made of multiple AnimationScreen, one for each animation part, this only test the Start animation.
        [TestMethod]
        public void TestAnimationStart()
        {
            CreateDummyAnimation(CreateDummySquad(), CreateDummySquad());

            for (int i = 0; i < 10; ++i)
            {
                int Milliseconds = (int)(i * (1000f / 60f));
                GameTime gameTime = new GameTime(new TimeSpan(0, 0, 0, 0, Milliseconds), new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f)));
                Constants.TotalGameTime = gameTime.TotalGameTime.TotalSeconds;
                Assert.AreEqual(0, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][0]).ListProjectile.Count);
                DummyAnimation.Update(new GameTime());
            }

            Assert.AreEqual(1, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][0]).ListProjectile.Count);
        }

        [TestMethod]
        public void TestAnimationStartFireballMovement()
        {
            CreateDummyAnimation(CreateDummySquad(), CreateDummySquad());

            for (int i = 0; i < 10; ++i)
            {
                int Milliseconds = (int)(i * (1000f / 60f));
                GameTime gameTime = new GameTime(new TimeSpan(0, 0, 0, 0, Milliseconds), new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f)));
                Constants.TotalGameTime = gameTime.TotalGameTime.TotalSeconds;
                DummyAnimation.Update(new GameTime());
            }

            Assert.AreEqual(1, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][0]).ListProjectile.Count);
            Assert.AreEqual(580, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][0]).ListProjectile[0].Collision.ListCollisionPolygon[0].Center.X, 0.01);

            for (int i = 0; i <= 60; ++i)
            {
                int Milliseconds = (int)(i * (1000f / 60f));
                GameTime gameTime = new GameTime(new TimeSpan(0, 0, 0, 0, Milliseconds), new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f)));
                Constants.TotalGameTime = gameTime.TotalGameTime.TotalSeconds;
                DummyAnimation.Update(new GameTime());

                Assert.AreEqual(1, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][0]).ListProjectile.Count);
                Assert.AreEqual(580 - (i + 1) * 6, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][0]).ListProjectile[0].Collision.ListCollisionPolygon[0].Center.X, 0.01);
            }
        }

        [TestMethod]
        public void TestAnimationStartEndsAfterFireballLeaveScreen()
        {
            CreateDummyAnimation(CreateDummySquad(), CreateDummySquad());

            Assert.AreEqual(109, DummyAnimation.LoopEnd);
        }

        [TestMethod]
        public void TestAnimationEndHitMovement()
        {
            CreateDummyAnimation(CreateDummySquad(), CreateDummySquad(), 0);
            CreateDummyAnimation(DummySquad, EnemySquad, 1);

            Assert.AreEqual(1, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][1]).ListProjectile.Count);
            Assert.AreEqual(612, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][1]).ListProjectile[0].Collision.ListCollisionPolygon[0].Center.X, 0.01);

            for (int i = 0; i < 49; ++i)
            {
                int Milliseconds = (int)(i * (1000f / 60f));
                GameTime gameTime = new GameTime(new TimeSpan(0, 0, 0, 0, Milliseconds), new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f)));
                Constants.TotalGameTime = gameTime.TotalGameTime.TotalSeconds;
                DummyAnimation.Update(new GameTime());

                Assert.AreEqual(1, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][1]).ListProjectile.Count);
                Assert.AreEqual(612 - (i + 1) * 6, ((MagicTimeline)DummyLayer.DicTimelineEvent[0][1]).ListProjectile[0].Collision.ListCollisionPolygon[0].Center.X, 0.01);
            }
        }

        [TestMethod]
        public void TestAnimationEndHitCollision()
        {
            CreateDummyAnimation(CreateDummySquad(), CreateDummySquad(), 0);
            CreateDummyAnimation(DummySquad, EnemySquad, 1);
            Assert.AreEqual(85, DummyAnimation.LoopEnd);
        }

        public Unit CreateDummyUnit()
        {
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
            DummyCharacter.Init();

            List<MagicSpell> ListMagicSpell = new List<MagicSpell>();
            UnitMagic DummyUnit = new UnitMagic("Dummy Unit", ListMagicSpell);
            ListMagicSpell.Add(CreateFireball(DummyUnit, DummyUnit.MagicProjectileParams));

            DummyUnit.MaxHP = 10000;
            DummyUnit.MaxEN = 200;
            DummyUnit.Armor = 100;
            DummyUnit.Mobility = 50;
            DummyUnit.MaxMovement = 5;

            Attack DummyAttack = new Attack("Dummy Attack", string.Empty, 0, "10000", 0, 5, WeaponPrimaryProperty.None,
                WeaponSecondaryProperty.None, 10, 0, 6, 1, 100, "Laser",
                new Dictionary<string, char>() { { "Air", 'S' }, { "Land", 'S' }, { "Sea", 'S' }, { "Space", 'S' } });

            DummyAttack.PostMovementLevel = 1;
            DummyUnit.ArrayCharacterActive = new Character[1] { DummyCharacter };
            DummyUnit.ListAttack.Add(DummyAttack);
            DummyUnit.CurrentAttack = DummyAttack;
            DummyUnit.UnitStat.DicTerrainValue.Add("Air", 1);
            DummyUnit.UnitStat.DicTerrainValue.Add("Land", 1);
            DummyUnit.UnitStat.DicTerrainValue.Add("Sea", 1);
            DummyUnit.UnitStat.DicTerrainValue.Add("Space", 1);

            return DummyUnit;
        }

        private Squad CreateDummySquad()
        {
            Unit DummyLeader = CreateDummyUnit();
            Squad DummySquad = new Squad("Dummy", DummyLeader);
            DummySquad.Init(GlobalDeathmatchContext);

            return DummySquad;
        }

        private void CreateDummyAnimation(Squad DummySquad, Squad EnemySquad, int AnimationIndex = 0)
        {
            this.DummySquad = DummySquad;
            this.EnemySquad = EnemySquad;

            DummyAnimation = new AnimationScreen("", DummyMap, DummySquad,
                EnemySquad, DummySquad.CurrentLeader.CurrentAttack,
                new GameScreens.BattleMapScreen.BattleMap.SquadBattleResult(), new AnimationScreen.AnimationUnitStats(DummySquad, EnemySquad, false), null, null, "", false);

            DummyAnimation.ListAnimationLayer = new AnimationClass.AnimationLayerHolder();
            DummyAnimation.ListAnimationLayer.EngineLayer = AnimationClass.GameEngineLayer.EmptyGameEngineLayer(DummyAnimation);

            DummyAnimation.LoopEnd = 30;
            DummyAnimation.AnimationOrigin.Position = new Vector2(580, 300);
            DummyAnimation.AnimationOrigin.DicAnimationKeyFrame[0].Position = new Vector2(580, 300);

            DummyLayer = new AnimationClass.AnimationLayer(DummyAnimation, "Layer 1");

            if (AnimationIndex > 0)
            {
                MarkerTimeline DummyMarker = CreateDummyMarkerTimeline();
                DummyLayer.AddTimelineEvent(0, DummyMarker);
            }

            DummyAnimation.ListAnimationLayer.Add(DummyLayer);

            DummyAnimation.UpdateKeyFrame(0);

            foreach (KeyValuePair<int, Timeline> ActiveExtraTimeline in DummySquad.CurrentLeader.ListAttack[1].Animations[0].Animations.ArrayAnimationPath[AnimationIndex].GetExtraTimelines(DummyAnimation))
            {
                DummyLayer.AddTimelineEvent(ActiveExtraTimeline.Key, ActiveExtraTimeline.Value);
            }
        }

        private MarkerTimeline CreateDummyMarkerTimeline()
        {
            MarkerTimeline DummyTimeline = new MarkerTimeline();
            DummyTimeline.DeathFrame = 60;

            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummyTimeline.AnimationMarker = new AnimationScreen("", DummyMap, DummySquad,
                EnemySquad, DummySquad.CurrentLeader.CurrentAttack,
                new GameScreens.BattleMapScreen.BattleMap.SquadBattleResult(), new AnimationScreen.AnimationUnitStats(DummySquad, EnemySquad, false), null, null, "", false);

            DummyTimeline.AnimationMarker.ListAnimationLayer = new AnimationClass.AnimationLayerHolder();
            DummyTimeline.AnimationMarker.ListAnimationLayer.EngineLayer = AnimationClass.GameEngineLayer.EmptyGameEngineLayer(DummyTimeline.AnimationMarker);

            DummyTimeline.AnimationMarker.LoopEnd = 60;
            DummyTimeline.AnimationMarker.AnimationOrigin.Position = new Vector2(580, 300);
            DummyTimeline.AnimationMarker.AnimationOrigin.DicAnimationKeyFrame[0].Position = new Vector2(580, 300);
            DummyTimeline.Add(0, new VisibleAnimationObjectKeyFrame(new Vector2(50, 300), true, -1));

            AnimationClass.AnimationLayer DummyLayer = new AnimationClass.AnimationLayer(DummyTimeline.AnimationMarker, "Layer 1");

            DummyLayer.AddTimelineEvent(0, new BlankTimeline(580, 300, 30, 50));

            DummyTimeline.AnimationMarker.ListAnimationLayer.Add(DummyLayer);

            return DummyTimeline;
        }

        private MagicSpell CreateFireball(UnitMagic ActiveUser, Projectile2DParams MagicProjectileParams)
        {
            MagicSpell ActiveSpell = new MagicSpell(ActiveUser, ActiveUser);
            MagicUserParams Params = new MagicUserParams(ActiveSpell.GlobalContext);

            MagicCoreFireball FireballCore1 = (MagicCoreFireball)new MagicCoreFireball(Params, MagicProjectileParams).Copy();
            FireballCore1.ListLinkedMagicElement.Add(new ChannelExternalManaSource(Params).Copy());
            
            ActiveSpell.ListMagicCore.Add(FireballCore1);

            return ActiveSpell;
        }
    }
}
