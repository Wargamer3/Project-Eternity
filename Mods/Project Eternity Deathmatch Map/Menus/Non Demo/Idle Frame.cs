using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoIdleFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 46;

        public NonDemoIdleFrame(DeathmatchMap Map, NonDemoSharedUnitStats SharedUnitStats, float PositionX, float PositionY, bool IsRight)
            : base(Map, SharedUnitStats, PositionX, PositionY, IsRight)
        {
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, PositionY + 8), Color.White);
        }
    }
}
