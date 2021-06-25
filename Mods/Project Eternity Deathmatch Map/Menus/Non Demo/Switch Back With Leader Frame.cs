using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoSwitchBackWithLeaderFrame : NonDemoBattleUnitFrame
    {
        public NonDemoSwitchBackWithLeaderFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            int SupportPosX = 150 - (int)(NonDemoAnimationTimer / NonDemoBattleFrame.SwitchLength * 120);
            if (IsRight)
            {
                float DrawPositionX = PositionX + SupportPosX;

                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    DrawPositionX + 2, PositionY + 8), Color.White);
            }
            else
            {
                float DrawPositionX = PositionX - SupportPosX;

                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    DrawPositionX + 2, PositionY + 8), Color.White);
            }
        }
    }
}
