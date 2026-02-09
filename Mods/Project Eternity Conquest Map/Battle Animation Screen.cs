using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class BattleAnimationScreen : GameScreen
    {
        public enum QuoteTypes { BattleStart, Dodge, Damaged, Destroyed, SupportAttack, SupportDefend };

        private ConquestMap Map;
        private UnitConquest ActiveUnit;
        private UnitConquest EnemyUnit;
        private string ActiveTerrain;
        private bool HorizontalMirror;
        private AnimationScreen ActiveUnitAnimation;
        private AnimationScreen EnemyUnitAnimation;
        private int DamageTakenSelf;
        private int DamageTakenEnemy;
        private const int EnenmyDeathKeyFrame = 30;
        private Texture2D BackgroundRight;
        private Texture2D BackgroundLeft;

        public BattleAnimationScreen(ConquestMap Map, UnitConquest ActiveUnit, UnitConquest EnemyUnit, int DamageTakenSelf, int DamageTakenEnemy, string ActiveTerrain, bool HorizontalMirror)
            : base()
        {
            this.Map = Map;
            this.ActiveUnit = ActiveUnit;
            this.EnemyUnit = EnemyUnit;
            this.DamageTakenSelf = DamageTakenSelf;
            this.DamageTakenEnemy = DamageTakenEnemy;
            this.ActiveTerrain = ActiveTerrain;
            this.HorizontalMirror = HorizontalMirror;

            RequireFocus = true;
            RequireDrawFocus = true;
            IsOnTop = false;
        }

        public BattleAnimationScreen(ConquestMap Map, UnitConquest ActiveUnit, UnitConquest EnemyUnit,
            string ActiveTerrain, bool HorizontalMirror, AnimationScreen ActiveUnitAnimation, AnimationScreen EnemyUnitAnimation)
            : this(Map, ActiveUnit, EnemyUnit, 2, 2, ActiveTerrain, HorizontalMirror)
        {
            this.ActiveUnitAnimation = ActiveUnitAnimation;
            this.EnemyUnitAnimation = EnemyUnitAnimation;
        }

        public override void Load()
        {
            BackgroundRight = Content.Load<Texture2D>("Animations/Background Sprites/AW/Countryside road");
            BackgroundLeft = Content.Load<Texture2D>("Animations/Background Sprites/AW/Countryside road");

            int ActiveUnitHP = ActiveUnit.HP;
            int ActiveUnitHPAfterDamage = ActiveUnit.HP - DamageTakenSelf;
            int EnemyUnitHP = EnemyUnit.HP;
            int EnemyUnitHPAfterDamage = EnemyUnit.HP - DamageTakenEnemy;

            ActiveUnitAnimation = new AnimationScreen("Conquest/" + ActiveUnit.ArmourType + "/Attack by HP/" + ActiveUnitHP + " HP", ActiveUnit, ActiveTerrain, HorizontalMirror);
            ActiveUnitAnimation.Load();
            ActiveUnitAnimation.UpdateKeyFrame(0);

            AnimationScreen ActiveUnitAnimationAfterDamage = new AnimationScreen("Conquest/" + ActiveUnit.ArmourType + "/Idle by HP/" + Math.Max(0, ActiveUnitHPAfterDamage) + " HP", ActiveUnit, ActiveTerrain, HorizontalMirror);
            ActiveUnitAnimationAfterDamage.Load();
            ActiveUnitAnimationAfterDamage.UpdateKeyFrame(0);

            AnimationClass ActiveUnitAnimationDeath = new AnimationScreen("Conquest/" + ActiveUnit.ArmourType.ToString() + "/Death", ActiveUnit, ActiveTerrain, HorizontalMirror);
            ActiveUnitAnimationDeath.Load();
            ActiveUnitAnimationDeath.UpdateKeyFrame(0);

            EnemyUnitAnimation = new AnimationScreen("Conquest/" + ActiveUnit.ArmourType + "/Idle by HP/" + EnemyUnitHP + " HP", EnemyUnit, ActiveTerrain, !HorizontalMirror);
            EnemyUnitAnimation.Load();
            EnemyUnitAnimation.UpdateKeyFrame(0);

            //Todo, load Idle or Attacking animation depending of the situaton
            AnimationScreen EnemyUnitAnimationAfterDamage = new AnimationScreen("Conquest/" + ActiveUnit.ArmourType + "/Idle by HP/" + Math.Max(0, EnemyUnitHPAfterDamage) + " HP", EnemyUnit, ActiveTerrain, !HorizontalMirror);
            EnemyUnitAnimationAfterDamage.Load();
            EnemyUnitAnimationAfterDamage.UpdateKeyFrame(0);

            AnimationClass EnemyUnitAnimationDeath = new AnimationClass("Conquest/" + EnemyUnit.ArmourType.ToString() + "/Death");
            EnemyUnitAnimationDeath.DicTimeline = EnemyUnitAnimation.DicTimeline;
            EnemyUnitAnimationDeath.Load();

            InitBattleAnimation(EnemyUnitAnimation, EnemyUnitAnimationAfterDamage, "Conquest/" + EnemyUnit.ArmourType.ToString() + "/Death");

            InitBattleAnimation(ActiveUnitAnimation, ActiveUnitAnimationAfterDamage, "Conquest/" + ActiveUnit.ArmourType.ToString() + "/Death");
        }

        public static void InitBattleAnimation(AnimationClass EnemyUnitAnimation, AnimationClass EnemyUnitAnimationAfterDamage, string EnemyUnitDeathAnimation)
        {
            int NumberOfEnemies = EnemyUnitAnimation.ListAnimationLayer[0].ListActiveMarker.Count;
            int NumberOfEnemiesAfterDamage = EnemyUnitAnimationAfterDamage.ListAnimationLayer[0].ListActiveMarker.Count;
            int NumberOfDeadEnemies = NumberOfEnemies - NumberOfEnemiesAfterDamage;

            List<int> ListUnitAnimationIndex = new List<int>();
            Random Random = new Random();
            int DeathFrame = EnemyUnitAnimation.ListAnimationLayer[0].ListActiveMarker[0].DeathFrame;

            for (int i = 0; i < NumberOfEnemies; ++i)
            {
                ListUnitAnimationIndex.Add(i);
            }

            for (int i = 0; i < NumberOfDeadEnemies; ++i)
            {
                int ActiveIndex = ListUnitAnimationIndex[Random.Next(ListUnitAnimationIndex.Count)];
                MarkerTimeline ActiveUnitTimeline = EnemyUnitAnimation.ListAnimationLayer[0].ListActiveMarker[ActiveIndex];

                ActiveUnitTimeline.AnimationMarker = new AnimationClass(EnemyUnitDeathAnimation);
                ActiveUnitTimeline.AnimationMarker.DicTimeline = EnemyUnitAnimation.DicTimeline;
                ActiveUnitTimeline.AnimationMarker.Load();
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            if (gameTime.IsRunningSlowly)
            {
                return;
            }

            if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                RemoveScreen(this);
                Content.Unload();
            }

            //Both side are animated at the same time and are expected to have the same lenght.
            ActiveUnitAnimation.Update(gameTime);
            EnemyUnitAnimation.Update(gameTime);

            if (ActiveUnitAnimation.HasEnded)
            {
                EndBattle();
            }
        }

        private void EndBattle()
        {
            RemoveScreen(this);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            ActiveUnitAnimation.BeginDraw(g);
            EnemyUnitAnimation.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(BackgroundLeft, new Rectangle(0, 0, Constants.Width / 2, Constants.Height), Color.White);
            g.Draw(BackgroundRight, new Rectangle(Constants.Width / 2, 0, Constants.Width / 2, Constants.Height), Color.White);

            DrawLine(g, new Vector2(Constants.Width / 2, 0), new Vector2(Constants.Width / 2, Constants.Height), Color.Black, 3);
            ActiveUnitAnimation.Draw(g);
            EnemyUnitAnimation.Draw(g);

            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        }
    }
}
