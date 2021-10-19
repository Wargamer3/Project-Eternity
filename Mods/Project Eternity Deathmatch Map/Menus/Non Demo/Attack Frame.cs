using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoAttackFrame : NonDemoBattleUnitFrame
    {
        public const int FrameLength = 46;

        public NonDemoAttackFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
        }

        public override void OnEnd()
        {
            SharedUnitStats.StartingEN = SharedUnitStats.EndEN;
        }

        protected override void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            if (IsRight)
            {
                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    PositionX + 2 - 8, PositionY + 8), Color.White);
            }
            else
            {
                g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                    PositionX + 2 + 8, PositionY + 8), Color.White);
            }
        }
    }
}
