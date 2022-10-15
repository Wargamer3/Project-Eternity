using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleDefenderWinPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleDefenderWin";

        public static string RequirementName = "Sorcerer Street Battle Defender Win";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleDefenderWinPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                Map.EndPlayerPhase();
                RemoveFromPanelList(this);
            }
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
            return new ActionPanelBattleDefenderWinPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int Y = Constants.Height - Constants.Height / 4;
            GameScreen.DrawBox(g, new Vector2(Constants.Width / 4, Y), Constants.Width / 2, Constants.Height / 6, Color.White);
            Y += 10;
            g.DrawStringMiddleAligned(Map.fntArial12, "Toll", new Vector2(Constants.Width / 2, Y), Color.White);
            Y += 20;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Name, new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Name, new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 20;
            g.DrawStringMiddleAligned(Map.fntArial12, "-20", new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "+20", new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 10;
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, Y), null, Color.White, MathHelper.ToRadians(90), new Vector2(Map.sprArrowUp.Width / 2, Map.sprArrowUp.Height / 2), 1f, SpriteEffects.None, 0f);
            g.Draw(Map.sprMenuHand, new Vector2(Constants.Width / 2 + 140, Y - 15), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
        }
    }
}
