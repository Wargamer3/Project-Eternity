using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoGetMissedFrame : NonDemoBattleUnitFrame
    {
        private readonly Texture2D sprNonDemoMiss;

        public const int FrameLength = 46;

        public NonDemoGetMissedFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight, Texture2D sprNonDemoMiss)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
            this.sprNonDemoMiss = sprNonDemoMiss;
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, PositionY + 8), Color.White);

            if (NonDemoAnimationTimer >= 38)
            {
                g.Draw(sprNonDemoMiss, new Vector2(
                    PositionX + 20, PositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
            }
            else if (NonDemoAnimationTimer >= 30)
            {
                g.Draw(sprNonDemoMiss, new Vector2(
                    PositionX + 20, PositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
            }
            else
            {
                g.Draw(sprNonDemoMiss, new Vector2(
                    PositionX + 20, PositionY + 5), Color.White);
            }
        }
    }
}
