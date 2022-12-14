using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelObjectives : ActionPanelDeathmatch
    {
        private const string PanelName = "Objectives";

        private SpriteFont fntFinlanderFont;

        public ActionPanelObjectives(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            this.fntFinlanderFont = Map.fntFinlanderFont;
        }

        public ActionPanelObjectives(DeathmatchMap Map, SpriteFont fntFinlanderFont)
            : base(PanelName, Map, false)
        {
            this.fntFinlanderFont = fntFinlanderFont;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed() || ActiveInputManager.InputCancelPressed())
            {
                //Reset the cursor.
                RemoveFromPanelList(this);
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelObjectives(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            const float MaxBoxWidth = 600;
            string Output = GameScreen.FitTextToWidth(fntFinlanderFont, Map.VictoryCondition, MaxBoxWidth);

            GameScreen.DrawBox(g, new Vector2(10, 10), 620, 40, Color.Black);
            g.DrawString(fntFinlanderFont, "Victory Condition", new Vector2(20, 15), Color.White);
            GameScreen.DrawBox(g, new Vector2(10, 50), 620, 110, Color.White);
            g.DrawString(fntFinlanderFont, Output, new Vector2(20, 50), Color.White);

            Output = GameScreen.FitTextToWidth(fntFinlanderFont, Map.LossCondition, MaxBoxWidth);
            GameScreen.DrawBox(g, new Vector2(10, 165), 620, 40, Color.Black);
            g.DrawString(fntFinlanderFont, "Loss Condition", new Vector2(20, 170), Color.White);
            GameScreen.DrawBox(g, new Vector2(10, 205), 620, 110, Color.White);
            g.DrawString(fntFinlanderFont, Output, new Vector2(20, 205), Color.White);

            Output = GameScreen.FitTextToWidth(fntFinlanderFont, Map.SkillPoint, MaxBoxWidth);
            GameScreen.DrawBox(g, new Vector2(10, 320), 620, 40, Color.Black);
            g.DrawString(fntFinlanderFont, "Skill Point", new Vector2(20, 325), Color.White);
            GameScreen.DrawBox(g, new Vector2(10, 360), 620, 110, Color.White);
            g.DrawString(fntFinlanderFont, Output, new Vector2(20, 360), Color.White);
        }
    }
}
