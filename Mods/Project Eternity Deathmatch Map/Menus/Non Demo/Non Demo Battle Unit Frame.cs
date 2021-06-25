using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class NonDemoScreen
    {
        public abstract class NonDemoBattleUnitFrame
        {
            public readonly bool IsRight;
            public readonly DeathmatchMap Map;
            public readonly NonDemoSharedUnitStats SharedUnitStats;
            public readonly float PositionX;
            public readonly float PositionY;

            public NonDemoBattleUnitFrame(DeathmatchMap Map, NonDemoSharedUnitStats SharedUnitStats, float PositionX, float PositionY, bool IsRight)
            {
                this.Map = Map;
                this.SharedUnitStats = SharedUnitStats;
                this.PositionX = PositionX;
                this.PositionY = PositionY;
                this.IsRight = IsRight;
            }

            public virtual void Update(GameTime gameTime)
            {

            }

            public virtual void OnEnd()
            {

            }

            public void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
            {
                DrawBox(g, new Vector2(PositionX, PositionY), 113, 45, Color.White);

                DrawBar(g, Map.sprBarSmallBackground, Map.sprBarSmallHP, new Vector2(PositionX + 55, PositionY + 9), SharedUnitStats.StartingHP, SharedUnitStats.SharedUnit.MaxHP);
                DrawBar(g, Map.sprBarSmallBackground, Map.sprBarSmallEN, new Vector2(PositionX + 55, PositionY + 26), SharedUnitStats.StartingEN, SharedUnitStats.SharedUnit.MaxEN);

                g.DrawStringRightAligned(Map.fntBattleNumberSmall, SharedUnitStats.StartingHP.ToString(), new Vector2(PositionX + 102, PositionY + 1), Color.White);

                g.DrawStringRightAligned(Map.fntBattleNumberSmall, SharedUnitStats.StartingEN.ToString(), new Vector2(PositionX + 103, PositionY + 18), Color.White);

                DoDraw(g, NonDemoAnimationTimer);
            }

            protected abstract void DoDraw(CustomSpriteBatch g, int NonDemoAnimationTimer);
        }
    }
}
