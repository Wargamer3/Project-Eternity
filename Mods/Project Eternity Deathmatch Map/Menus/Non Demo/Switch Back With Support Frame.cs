using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoSwitchBackWithSupportFrame : NonDemoBattleUnitFrame
    {
        public NonDemoSwitchBackWithSupportFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            int LeaderPosY = (int)(NonDemoAnimationTimer / NonDemoBattleFrame.SwitchLength * 50);
            float DrawPositionY = PositionY - LeaderPosY;

            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, DrawPositionY + 8), Color.White);
        }
    }
}
