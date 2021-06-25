using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoGetCriticalHitFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 46;

        private readonly Texture2D sprNonDemoCritical;

        public NonDemoGetCriticalHitFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight, Texture2D sprNonDemoCritical)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
            this.sprNonDemoCritical = sprNonDemoCritical;
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, PositionY + 8), Color.White);

            if (NonDemoAnimationTimer >= 38)
            {
                g.Draw(sprNonDemoCritical, new Vector2(
                    PositionX + 20, PositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
            }
            else if (NonDemoAnimationTimer >= 30)
            {
                g.Draw(sprNonDemoCritical, new Vector2(
                    PositionX + 20, PositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
            }
            else
            {
                g.Draw(sprNonDemoCritical, new Vector2(
                    PositionX + 20, PositionY + 5), Color.White);
            }
        }
    }
}
