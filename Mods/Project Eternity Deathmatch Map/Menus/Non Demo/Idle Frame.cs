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

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            DrawBackgroundBox(g, PositionX, PositionY);

            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, PositionY + 8), Color.White);
        }
    }
}
