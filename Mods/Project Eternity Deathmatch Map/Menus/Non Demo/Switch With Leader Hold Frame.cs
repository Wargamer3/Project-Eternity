using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoSwitchWithLeaderHoldFrame : NonDemoBattleUnitFrame
    {
        public NonDemoSwitchWithLeaderHoldFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            float DrawPositionX;

            if (IsRight)
            {
                DrawPositionX = PositionX - 120;
            }
            else
            {
                DrawPositionX = PositionX + 120;
            }

            DrawBackgroundBox(g, DrawPositionX, PositionY);

            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                DrawPositionX + 2, PositionY + 8), Color.White);
        }
    }
}
