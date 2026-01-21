using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelPayTollPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "PayToll";

        private readonly TerrainSorcererStreet ActiveTerrain;

        public ActionPanelPayTollPhase(SorcererStreetMap Map)
            : base(PanelName, Map, CreatureCard.CreatureCardType)
        {
        }

        public ActionPanelPayTollPhase(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, CreatureCard.CreatureCardType, "Pay")
        {
            this.ActiveTerrain = ActiveTerrain;
        }

        public override void OnCardSelected(Card CardSelected)
        {
            AddToPanelListAndSelect(new ActionPanelConfirmCreatureSummonBattle(Map, ActivePlayerIndex, (CreatureCard)CardSelected));
        }

        public override void OnEndCardSelected()
        {
            int Toll = ActiveTerrain.CurrentToll;

            int TollOverride = ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollOverride;

            if (TollOverride >= 0)
            {
                Toll = TollOverride;
            }

            if (!ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Battle).TollProtection
                && !ActiveTerrain.PlayerOwner.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Battle).TollLimit)
            {
                ActivePlayer.Gold -= Toll;
                ActiveTerrain.PlayerOwner.Gold += Toll;
            }

            foreach (Player TollSharePlayer in Map.ListPlayer)
            {
                if (TollSharePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollGainShareMultiplier > 0)
                {
                    TollSharePlayer.Gold += (int)(ActiveTerrain.CurrentToll * TollSharePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollGainShareMultiplier);
                }
            }

            Map.UpdateTotalMagic();

            Map.EndPlayerPhase();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer);
            ActionPanelPlayerDefault.DrawPhase(g, Map, "Creature Selection");
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPayTollPhase(Map);
        }
    }
}
