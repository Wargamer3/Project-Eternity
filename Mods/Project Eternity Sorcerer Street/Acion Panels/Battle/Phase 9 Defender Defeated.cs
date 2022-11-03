using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleDefenderDefeatedPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleDefenderDefeated";

        public static string RequirementName = "Sorcerer Street Battle Defender Defeated";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleDefenderDefeatedPhase(SorcererStreetMap Map)
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
                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.InvaderPlayer.GamePiece);

                ActiveTerrain.DefendingCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                ActiveTerrain.PlayerOwner = Map.GlobalSorcererStreetBattleContext.InvaderPlayer;

                Map.GlobalSorcererStreetBattleContext.InvaderPlayer.IncreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
                Map.GlobalSorcererStreetBattleContext.DefenderPlayer.DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
                Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.InvaderPlayer);
                Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.DefenderPlayer);
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
            return new ActionPanelBattleDefenderDefeatedPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw creature being replaced
        }
    }
}
