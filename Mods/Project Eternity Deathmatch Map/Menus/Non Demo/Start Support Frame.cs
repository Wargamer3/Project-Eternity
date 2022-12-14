using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoStartSupportFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 8;

        public NonDemoStartSupportFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            if (IsRight)
            {
                DrawBackgroundBox(g, PositionX - 120, PositionY);
                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    PositionX + 2 - 8 - 120, PositionY + 8), Color.White);
            }
            else
            {
                DrawBackgroundBox(g, PositionX + 120, PositionY);
                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    PositionX + 2 + 8 + 120, PositionY + 8), Color.White);
            }
        }
    }
}
