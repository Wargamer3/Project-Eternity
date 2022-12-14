using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoSwitchBackWithLeaderFrame : NonDemoBattleUnitFrame
    {
        public NonDemoSwitchBackWithLeaderFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            int SupportPosX = 150 - (int)(NonDemoAnimationTimer / NonDemoBattleFrame.SwitchLength * 120);

            float DrawPositionX;

            if (IsRight)
            {
                DrawPositionX = PositionX - 120 + SupportPosX;
            }
            else
            {
                DrawPositionX = PositionX + 120 - SupportPosX;
            }

            DrawBackgroundBox(g, DrawPositionX, PositionY);

            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                DrawPositionX + 2, PositionY + 8), Color.White);
        }
    }
}
