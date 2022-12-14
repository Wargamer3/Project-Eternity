using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoSwitchWithSupportHoldFrame : NonDemoBattleUnitFrame
    {
        public NonDemoSwitchWithSupportHoldFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            float DrawPositionY = PositionY - 50;

            DrawBackgroundBox(g, PositionX, DrawPositionY);

            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, DrawPositionY + 8), Color.White);
        }
    }
}
