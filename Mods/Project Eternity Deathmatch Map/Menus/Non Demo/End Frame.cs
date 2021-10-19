﻿using System;
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

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            DrawBackgroundBox(g, PositionX, PositionY);

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
