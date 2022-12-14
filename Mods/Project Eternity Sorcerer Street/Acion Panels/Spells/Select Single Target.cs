using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Acion_Panels
{
    public class ActionPanelSelectSingleSpellTarget : ActionPanelSorcererStreet
    {
        public ActionPanelSelectSingleSpellTarget(SorcererStreetMap Map)
            : base("Select Single Spell Target", Map, true)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public void FinishPhase()
        {
        }

        public void SelectTarget()
        {
            int BoardX = 0;
            int BoardY = 0;
            Map.GetTerrain(BoardX, BoardY, 0);
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSelectSingleSpellTarget(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {

            GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 100, Constants.Height - 70), 200, 30, Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Select a target", new Vector2(Constants.Width / 2, Constants.Height - 65), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width / 2 + 76, Constants.Height - 65, 18, 18), Color.White);
        }
    }
}
