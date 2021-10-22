using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoGetHitSupportDefenceFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 46;

        private readonly SpriteFont fntNonDemoDamage;
        private readonly int Damage;
        private readonly string DisplayedDamage;
        private readonly AnimatedSprite sprExplosion;
        private readonly FMODSound sndNonDemoAttack;

        public NonDemoGetHitSupportDefenceFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight, SpriteFont fntNonDemoDamage, int Damage, AnimatedSprite sprExplosion, FMODSound sndNonDemoAttack)
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
            SharedUnitStats.VisibleHP = Math.Max(SharedUnitStats.SharedUnit.Boosts.HPMinModifier, SharedUnitStats.VisibleHP - Damage);
            float DrawPositionX = PositionX;

            if (IsRight)
            {
                DrawPositionX -= 120;
            }
            else
            {
                DrawPositionX += 120;
            }

            sprExplosion.Position.X = DrawPositionX + 16;
            sprExplosion.Position.Y = PositionY + 16;
            sprExplosion.RestartAnimation();

            sndNonDemoAttack.Play();
        }

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            float DrawPositionX = PositionX;

            if (IsRight)
            {
                DrawPositionX -= 120;
            }
            else
            {
                DrawPositionX += 120;
            }

            DrawBackgroundBox(g, DrawPositionX, PositionY);

            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                DrawPositionX + 2, PositionY + 8), Color.White);

            if (NonDemoAnimationTimer >= 38)
            {
                g.DrawString(fntNonDemoDamage, DisplayedDamage, new Vector2(
                    DrawPositionX + 20, PositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
            }
            else if (NonDemoAnimationTimer >= 30)
            {
                g.DrawString(fntNonDemoDamage, DisplayedDamage, new Vector2(
                    DrawPositionX + 20, PositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
            }
            else
            {
                g.DrawString(fntNonDemoDamage, DisplayedDamage, new Vector2(
                    DrawPositionX + 20, PositionY + 5), Color.White);
            }

            if (!sprExplosion.AnimationEnded)
                sprExplosion.Draw(g);
        }
    }
}
