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

        private Player ActivePlayer;
        private double AITimer;

        private int TollPaid;

        public ActionPanelBattleDefenderWinPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            ActivePlayer = Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.SelfCreature.PlayerIndex];

            if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.SelfCreature.Item);
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.Gold -= Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.SelfCreature.PlayerIndex].GetFinalCardCost(Map.GlobalSorcererStreetBattleContext.SelfCreature.Item);
            }
            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item);
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.Gold -= Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.OpponentCreature.PlayerIndex].GetFinalCardCost(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item);
            }

            TollPaid = ActionPanelPayTollPhase.PayToll(Map, ActivePlayer, Map.GlobalSorcererStreetBattleContext.ActiveTerrain);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    RemoveFromPanelList(this);
                    Map.EndPlayerPhase();
                }
                return;
            }

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
            MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 4, Y), Constants.Width / 2, 170);
            Y += 10;
            g.DrawStringMiddleAligned(Map.fntMenuText, "Toll", new Vector2(Constants.Width / 2, Y), Color.White);
            Y = Constants.Height - Constants.Height / 5;
            Y += 20;
            g.DrawStringMiddleAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.Name, new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.Name, new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 40;
            g.DrawStringMiddleAligned(Map.fntMenuText, "-" + TollPaid, new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntMenuText, "+" + TollPaid, new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 10;
            MenuHelper.DrawRightArrow(g);
            MenuHelper.DrawConfirmIcon(g, new Vector2(Constants.Width / 4 + Constants.Width / 2 - 40, Y - 20));
        }
    }
}
