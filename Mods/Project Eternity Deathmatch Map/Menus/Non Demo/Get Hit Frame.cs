using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoGetHitFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 46;

        private readonly SpriteFont fntNonDemoDamage;
        private readonly int Damage;
        private readonly string DisplayedDamage;
        private readonly AnimatedSprite sprExplosion;
        private readonly FMODSound sndNonDemoAttack;

        public NonDemoGetHitFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight, SpriteFont fntNonDemoDamage, int Damage, AnimatedSprite sprExplosion, FMODSound sndNonDemoAttack)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
            this.fntNonDemoDamage = fntNonDemoDamage;
            this.Damage = Damage;
            this.sprExplosion = sprExplosion;
            this.sndNonDemoAttack = sndNonDemoAttack;

            DisplayedDamage = Damage.ToString();
        }

        public override void Update(GameTime gameTime)
        {
            if (!sprExplosion.AnimationEnded)
                sprExplosion.Update(gameTime);
        }

        public override void OnEnd()
        {
            SharedUnitStats.StartingHP = Math.Max(SharedUnitStats.SharedUnit.Boosts.HPMinModifier, SharedUnitStats.StartingHP - Damage);

            sprExplosion.Position.X = PositionX;
            sprExplosion.Position.Y = PositionY;
            sprExplosion.RestartAnimation();

            sndNonDemoAttack.Play();
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, PositionY + 8), Color.White);

            if (NonDemoAnimationTimer >= 38)
            {
                g.DrawString(fntNonDemoDamage, DisplayedDamage, new Vector2(
                    PositionX + 20, PositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
            }
            else if (NonDemoAnimationTimer >= 30)
            {
                g.DrawString(fntNonDemoDamage, DisplayedDamage, new Vector2(
                    PositionX + 20, PositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
            }
            else
            {
                g.DrawString(fntNonDemoDamage, DisplayedDamage, new Vector2(
                    PositionX + 20, PositionY + 5), Color.White);
            }

            if (!sprExplosion.AnimationEnded)
                sprExplosion.Draw(g);
        }
    }
}
