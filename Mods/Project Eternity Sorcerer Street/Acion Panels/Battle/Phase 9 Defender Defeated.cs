﻿using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleDefenderDefeatedPhase : ActionPanelSorcererStreet
    {
        public static string BattleEndRequirementName = "Sorcerer Street Battle End";
        private const string PanelName = "BattleDefenderDefeated";

        public static string RequirementName = "Sorcerer Street Battle Defender Defeated";

        public ActionPanelBattleDefenderDefeatedPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            if (Map.GlobalSorcererStreetBattleContext.Invader.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.Invader.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.Invader.Item);
                Map.GlobalSorcererStreetBattleContext.Invader.Owner.Gold -= Map.GlobalSorcererStreetBattleContext.Invader.Item.MagicCost;
            }
            if (Map.GlobalSorcererStreetBattleContext.Defender.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.Defender.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.Defender.Item);
                Map.GlobalSorcererStreetBattleContext.Defender.Owner.Gold -= Map.GlobalSorcererStreetBattleContext.Defender.Item.MagicCost;
            }

            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.Invader.Owner.GamePiece.Position);

            ActiveTerrain.DefendingCreature = Map.GlobalSorcererStreetBattleContext.Invader.Creature;
            ActiveTerrain.PlayerOwner = Map.GlobalSorcererStreetBattleContext.Invader.Owner;

            Map.GlobalSorcererStreetBattleContext.Invader.OwnerTeam.IncreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
            Map.GlobalSorcererStreetBattleContext.Defender.OwnerTeam.DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);

            Map.RemoveCreature(Map.GlobalSorcererStreetBattleContext.Defender.Creature);
            Map.SummonCreature(Map.GlobalSorcererStreetBattleContext.Invader.Creature);

            Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.Invader.Owner);
            Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.Defender.Owner);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelLandChainUpdate(Map));
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
            return new ActionPanelBattleDefenderDefeatedPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int Y = Constants.Height - Constants.Height / 4;
            MenuHelper.DrawBox(g, new Vector2(Constants.Width / 4, Y), Constants.Width / 2, Constants.Height / 6);
        }
    }
}
