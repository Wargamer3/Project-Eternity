using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.UnitTests.ConquestTests
{
    [TestClass]
    public class ConquestAnimationScreenTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
        }

        [TestInitialize()]
        public void Initialize()
        {
            Constants.TotalGameTime = 0;
        }

        [TestMethod]
        public void TestAnimationScreen()
        {
            AnimationScreen DummyAnimation = CreateDummyAnimation(4);

            for (int i = 0; i < 60; ++i)
            {
                DummyAnimation.Update(new GameTime());

                Assert.AreEqual(4, DummyAnimation.ListAnimationLayer[0].ListActiveMarker.Count);
            }
        }

        [TestMethod]
        public void TestBattleAnimation()
        {
            AnimationScreen ActiveUnitAnimation;
            AnimationScreen EnemyUnitAnimation;
            BattleAnimationScreen DummyAnimation = CreateBattleAnimationScreen(out ActiveUnitAnimation, out EnemyUnitAnimation);
            for (int i = 0; i < 30; ++i)
            {
                DummyAnimation.Update(new GameTime());

                Assert.AreEqual(4, ActiveUnitAnimation.ListAnimationLayer[0].ListActiveMarker.Count);
                Assert.AreEqual(4, EnemyUnitAnimation.ListAnimationLayer[0].ListActiveMarker.Count);
            }
            for (int i = 0; i < 30; ++i)
            {
                DummyAnimation.Update(new GameTime());

                Assert.AreEqual(2, ActiveUnitAnimation.ListAnimationLayer[0].ListActiveMarker.Count);
                Assert.AreEqual(2, EnemyUnitAnimation.ListAnimationLayer[0].ListActiveMarker.Count);
            }
        }

        private AnimationScreen CreateDummyAnimation(int NumberOfUnits)
        {
            UnitConquest DummyUnit = new UnitConquest();
            AnimationScreen DummyAnimation = new AnimationScreen("", DummyUnit, "", false);

            DummyAnimation.ListAnimationLayer = new AnimationClass.AnimationLayerHolder();
            DummyAnimation.ListAnimationLayer.EngineLayer = AnimationClass.GameEngineLayer.EmptyGameEngineLayer(DummyAnimation);

            DummyAnimation.LoopEnd = 240;
            DummyAnimation.AnimationOrigin.Position = new Vector2(580, 300);
            DummyAnimation.AnimationOrigin.DicAnimationKeyFrame[0].Position = new Vector2(580, 300);

            AnimationClass.AnimationLayer DummyLayer = new AnimationClass.AnimationLayer(DummyAnimation, "Layer 1");

            DummyAnimation.ListAnimationLayer.Add(DummyLayer);

            for (int i = 0; i < NumberOfUnits; ++i)
            {
                DummyLayer.AddTimelineEvent(0, CreateDummyMarkerTimeline(new Vector2(Constants.Width - 50 - (i % 2) * 50, 40 + i * 100)));
            }

            DummyAnimation.UpdateKeyFrame(0);

            return DummyAnimation;
        }

        private MarkerTimeline CreateDummyMarkerTimeline(Vector2 Position)
        {
            MarkerTimeline DummyTimeline = new MarkerTimeline();
            DummyTimeline.DeathFrame = 60;

            DummyTimeline.AnimationMarker = new AnimationClass("");

            DummyTimeline.AnimationMarker.ListAnimationLayer = new AnimationClass.AnimationLayerHolder();
            DummyTimeline.AnimationMarker.ListAnimationLayer.EngineLayer = AnimationClass.GameEngineLayer.EmptyGameEngineLayer(DummyTimeline.AnimationMarker);

            DummyTimeline.AnimationMarker.LoopEnd = 60;
            DummyTimeline.AnimationMarker.AnimationOrigin.Position = new Vector2(580, 300);
            DummyTimeline.AnimationMarker.AnimationOrigin.DicAnimationKeyFrame[0].Position = new Vector2(580, 300);
            DummyTimeline.Add(0, new VisibleAnimationObjectKeyFrame(Position, true, -1));
            DummyTimeline.Position = Position;

            AnimationClass.AnimationLayer DummyLayer = new AnimationClass.AnimationLayer(DummyTimeline.AnimationMarker, "Layer 1");

            DummyLayer.AddTimelineEvent(0, new BlankTimeline(580, 300, 30, 50));

            DummyTimeline.AnimationMarker.ListAnimationLayer.Add(DummyLayer);

            return DummyTimeline;
        }

        private BattleAnimationScreen CreateBattleAnimationScreen(out AnimationScreen ActiveUnitAnimation, out AnimationScreen EnemyUnitAnimation)
        {
            ActiveUnitAnimation = CreateDummyAnimation(4);
            AnimationScreen ActiveUnitAnimationAfterDamage = CreateDummyAnimation(2);
            AnimationScreen ActiveUnitAnimationDeath = CreateDummyAnimation(1);
            EnemyUnitAnimation = CreateDummyAnimation(4);
            AnimationScreen EnemyUnitAnimationAfterDamage = CreateDummyAnimation(2);
            AnimationScreen EnemyUnitAnimationDeath = CreateDummyAnimation(1);

            BattleAnimationScreen DummyBattleAnimationScreen = new BattleAnimationScreen(null, null, null, "", false, ActiveUnitAnimation, EnemyUnitAnimation);

            //BattleAnimationScreen.InitBattleAnimation(EnemyUnitAnimation, EnemyUnitAnimationAfterDamage, EnemyUnitAnimationDeath);
            //BattleAnimationScreen.InitBattleAnimation(ActiveUnitAnimation, ActiveUnitAnimationAfterDamage, ActiveUnitAnimationDeath);

            return DummyBattleAnimationScreen;
        }
    }
}
