using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoEndFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 8;

        public NonDemoEndFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            if (IsRight)
            {
                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    PositionX + 2 - NonDemoAnimationTimer, PositionY + 8), Color.White);
            }
            else
            {
                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    PositionX + 2 + NonDemoAnimationTimer, PositionY + 8), Color.White);
            }
        }
    }
}
