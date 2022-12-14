using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class NonDemoGetSwordCutFrame : NonDemoBattleUnitFrame
    {
        private readonly SpriteFont fntUnitAttack;

        public NonDemoGetSwordCutFrame(NonDemoBattleUnitFrame OtherFrame, bool IsRight, SpriteFont fntUnitAttack)
            : base(OtherFrame.Map, OtherFrame.SharedUnitStats, OtherFrame.PositionX, OtherFrame.PositionY, IsRight)
        {
            this.fntUnitAttack = fntUnitAttack;
        }

        public override void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
        {
            DrawBackgroundBox(g, PositionX, PositionY);

            g.Draw(SharedUnitStats.SharedUnit.SpriteMap, new Vector2(
                PositionX + 2, PositionY + 8), Color.White);

            if (NonDemoAnimationTimer >= 38)
            {
                g.DrawString(fntUnitAttack, "SWORD CUT", new Vector2(
                    PositionX + 20, PositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
            }
            else if (NonDemoAnimationTimer >= 30)
            {
                g.DrawString(fntUnitAttack, "SWORD CUT", new Vector2(
                    PositionX + 20, PositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
            }
            else
            {
                g.DrawString(fntUnitAttack, "SWORD CUT", new Vector2(
                    PositionX + 20, PositionY + 5), Color.White);
            }
        }
    }
}
