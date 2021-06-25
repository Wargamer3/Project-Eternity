﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoBarrierFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 46;

        private readonly SpriteFont fntUnitAttack;
        private readonly string BarrierName;

        public NonDemoBarrierFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight, SpriteFont fntUnitAttack)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
            this.fntUnitAttack = fntUnitAttack;

            this.BarrierName = OtherFrame.SharedUnitStats.AttackerSquadResult.Barrier;
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, PositionY + 8), Color.White);

            if (NonDemoAnimationTimer >= 38)
            {
                g.DrawString(fntUnitAttack, BarrierName, new Vector2(
                    PositionX + 20, PositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
            }
            else if (NonDemoAnimationTimer >= 30)
            {
                g.DrawString(fntUnitAttack, BarrierName, new Vector2(
                    PositionX + 20, PositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
            }
            else
            {
                g.DrawString(fntUnitAttack, BarrierName, new Vector2(
                    PositionX + 20, PositionY + 5), Color.White);
            }
        }
    }
}
