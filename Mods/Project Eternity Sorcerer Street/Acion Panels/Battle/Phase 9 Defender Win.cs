using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleDefenderWinPhase : ActionPanelSorcererStreet
    {
        public static string BattleEndRequirementName = "Sorcerer Street Battle End";
        private const string PanelName = "BattleDefenderWin";

        public static string RequirementName = "Sorcerer Street Battle Defender Win";

        public ActionPanelBattleDefenderWinPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            if (Map.GlobalSorcererStreetBattleContext.Invader.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.Invader.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.Invader.Item);
                Map.GlobalSorcererStreetBattleContext.Invader.Owner.Magic -= Map.GlobalSorcererStreetBattleContext.Invader.Item.MagicCost;
            }
            if (Map.GlobalSorcererStreetBattleContext.Defender.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.Defender.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.Defender.Item);
                Map.GlobalSorcererStreetBattleContext.Defender.Owner.Magic -= Map.GlobalSorcererStreetBattleContext.Defender.Item.MagicCost;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                RemoveFromPanelList(this);
                Map.EndPlayerPhase();
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActionPanelBattle.ReadPlayerInfo(BR, Map);
            OnSelect();
        }

        public override void DoWrite(ByteWriter BW)
        {
            ActionPanelBattle.WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleDefenderWinPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int Y = Constants.Height - Constants.Height / 4;
            MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 4, Y), Constants.Width / 2, Constants.Height / 8);
            Y += 10;
            g.DrawStringMiddleAligned(Map.fntArial12, "Toll", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height - Constants.Height / 5;
            Y += 20;
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Owner.Name, new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Owner.Name, new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 20;
            g.DrawStringMiddleAligned(Map.fntArial12, "-20", new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "+20", new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 10;
            MenuHelper.DrawRightArrow(g);
            MenuHelper.DrawConfirmIcon(g, new Vector2(Constants.Width / 4 + Constants.Width / 2 - 40, Y - 20));
        }
    }
}
