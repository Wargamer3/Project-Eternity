using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAITerritoryActions : ActionPanelTerritoryActions
    {
        private const string PanelName = "AI Territory Actions";

        private ActionPanelAIChooseTerritory.AITerritoryActions AITerritoryAction;
        private int AICursor;
        private double AITimer;

        public ActionPanelAITerritoryActions(SorcererStreetMap Map)
            : base(Map)
        {
        }

        public ActionPanelAITerritoryActions(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain, ActionPanelAIChooseTerritory.AITerritoryActions AITerritoryAction)
            : base(PanelName, Map, ActivePlayerIndex, ActiveTerrain)
        {
            this.AITerritoryAction = AITerritoryAction;
            AICursor = (int)AITerritoryAction;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            AITimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (AITimer > 0.6f)
            {
                AITimer = 0;

                if (ActionMenuCursor < AICursor)
                {
                    ++ActionMenuCursor;
                }
                else if (ActionMenuCursor > AICursor)
                {
                    --ActionMenuCursor;
                }
                else
                {
                    switch (ActionMenuCursor)
                    {
                        case 0:
                            if (ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).LandLevelLock)
                            {
                            }
                            else
                            {
                                AddToPanelListAndSelect(new ActionPanelTerrainLevelUpCommands(Map, ActivePlayerIndex, ActiveTerrain));
                            }
                            break;
                        case 1:
                            if (ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).IsDefensive)
                            {
                            }
                            else
                            {
                                AddToPanelListAndSelect(new ActionPanelTerrainChange(Map, ActivePlayerIndex, ActiveTerrain));
                            }
                            break;
                        case 2:
                            AddToPanelListAndSelect(new ActionPanelCreatureMovement(Map, ActivePlayerIndex, ActiveTerrain));
                            break;
                        case 3:
                            AddToPanelListAndSelect(new ActionPanelCreatureExchange(Map));
                            break;
                        case 4:
                            if (!HasTerritoryAbility)
                            {
                                RemoveFromPanelList(this);
                            }
                            break;
                        case 5:
                            RemoveFromPanelList(this);
                            break;
                    }
                }
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAITerritoryActions(Map);
        }
    }
}
