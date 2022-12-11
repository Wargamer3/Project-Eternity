using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleDefenderDefeatedPhase : ActionPanelSorcererStreet
    {
        public static string BattleEndRequirementName = "Sorcerer Street Battle End";
        private const string PanelName = "BattleDefenderDefeated";

        public static string RequirementName = "Sorcerer Street Battle Defender Defeated";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleDefenderDefeatedPhase(SorcererStreetMap Map)
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

            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.InvaderPlayer.GamePiece);

            ActiveTerrain.DefendingCreature = Map.GlobalSorcererStreetBattleContext.Invader;
            ActiveTerrain.PlayerOwner = Map.GlobalSorcererStreetBattleContext.InvaderPlayer;

            Map.GlobalSorcererStreetBattleContext.InvaderPlayer.IncreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
            Map.GlobalSorcererStreetBattleContext.DefenderPlayer.DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
            Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.InvaderPlayer);
            Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.DefenderPlayer);
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
