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
            PayToll(Map, ActivePlayer, ActiveTerrain);

            Map.EndPlayerPhase();
        }

        public static int PayToll(SorcererStreetMap Map, Player PayingPlayer, TerrainSorcererStreet ActiveTerrain)
        {
            int Toll = ActiveTerrain.CurrentToll;

            int TollOverride = ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollOverride;

            if (TollOverride >= 0)
            {
                Toll = TollOverride;
            }

            if (!PayingPlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Battle).TollProtection
                && !ActiveTerrain.PlayerOwner.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Battle).TollLimit)
            {
                PayingPlayer.Gold -= Toll;
                ActiveTerrain.PlayerOwner.Gold += Toll;
            }

            foreach (Player TollSharePlayer in Map.ListPlayer)
            {
                if (TollSharePlayer.TeamIndex < 0)
                {
                    continue;
                }

                if (TollSharePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollGainShareMultiplier > 0)
                {
                    TollSharePlayer.Gold += (int)(ActiveTerrain.CurrentToll * TollSharePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollGainShareMultiplier);
                }
            }

            Map.UpdateTotalMagic();

            return Toll;
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
