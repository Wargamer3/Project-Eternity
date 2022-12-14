﻿using Microsoft.Xna.Framework;
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
            if (Map.GlobalSorcererStreetBattleContext.InvaderItem != null)
            {
                Map.GlobalSorcererStreetBattleContext.InvaderPlayer.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.InvaderItem);
                Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Magic -= Map.GlobalSorcererStreetBattleContext.InvaderItem.MagicCost;
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, BattleEndRequirementName);
            }
            if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderPlayer.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.DefenderItem);
                Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Magic -= Map.GlobalSorcererStreetBattleContext.DefenderItem.MagicCost;
                Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, BattleEndRequirementName);
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
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Name, new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Name, new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 20;
            g.DrawStringMiddleAligned(Map.fntArial12, "-20", new Vector2(Constants.Width / 2 - 80, Y), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "+20", new Vector2(Constants.Width / 2 + 80, Y), Color.White);
            Y += 10;
            MenuHelper.DrawRightArrow(g);
            MenuHelper.DrawConfirmIcon(g, new Vector2(Constants.Width / 4 + Constants.Width / 2 - 40, Y - 20));
        }
    }
}
