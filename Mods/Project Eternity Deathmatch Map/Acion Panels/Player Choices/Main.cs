using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMainMenu : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;
        private int ActivePlayerIndex;

        public ActionPanelMainMenu(DeathmatchMap Map, Squad ActiveSquad, int ActiveSquadIndex)
            : base("Menu", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActiveSquadIndex;
        }

        public override void OnSelect()
        {
            Map.ActiveSquadIndex = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad);
            ListNextChoice.Clear();

            //Update weapons to decide if the attack choice is drawn.
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[Map.ActivePlayerIndex].Team, ActiveSquad.CanMove);

            if (ActiveSquad.CanMove)
            {
                AddChoiceToCurrentPanel(new ActionPanelMovePart1(Map, Map.CursorPosition, Map.CameraPosition, ActiveSquad, ActivePlayerIndex));
                if (ActiveSquad.CurrentLeader.CanAttack)
                {
                    AddChoiceToCurrentPanel(new ActionPanelAttackPart1(ActiveSquad.CanMove, ActiveSquad, ActivePlayerIndex, Map));
                }
                AddChoiceToCurrentPanel(new ActionPanelSpirit(Map, ActiveSquad));
                new ActionPanelConsumableParts(Map, this, ActiveSquad).OnSelect();
            }

            if (ActiveSquad.CanTransportUnit && ActiveSquad.ListTransportedUnit.Count > 0)
            {
                AddToPanelListAndSelect(new ActionPanelDeploy(Map, ActiveSquad));
            }
            AddChoiceToCurrentPanel(new ActionPanelStatus(Map, ActiveSquad));
            AddChoiceToCurrentPanel(new ActionPanelDebug(ActiveSquad, Map));

            if (ActiveSquad.CanMove)
            {
                //Add the squad options.
                if (ActiveSquad.CurrentWingmanA != null)
                    AddChoiceToCurrentPanel(new ActionPanelFormation(Map, ActiveSquad));

                List<ActionPanel> DicOptionalPanel = ActiveSquad.OnMenuSelect(Map.ListActionMenuChoice);
                foreach (ActionPanel OptionalPanel in DicOptionalPanel)
                {
                    AddChoiceToCurrentPanel(OptionalPanel);
                }

                if (ActiveSquad.CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainLand)
                    && ActiveSquad.CurrentMovement != UnitStats.TerrainLand
                    && Map.GetTerrainType(ActiveSquad.X, ActiveSquad.Y, ActiveSquad.LayerIndex) == UnitStats.TerrainLand)
                {
                    AddChoiceToCurrentPanel(new ActionPanelLand(Map, ActiveSquad));
                }

                if (ActiveSquad.CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainUnderwater)
                    && ActiveSquad.CurrentMovement != UnitStats.TerrainUnderwater
                    && Map.GetTerrainType(ActiveSquad.X, ActiveSquad.Y, ActiveSquad.LayerIndex) == UnitStats.TerrainSea)
                {
                    AddChoiceToCurrentPanel(new ActionPanelDive(Map, ActiveSquad));
                }

                if (ActiveSquad.CurrentMovement != UnitStats.TerrainAir)
                {
                    if (ActiveSquad.CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                    {
                        if (ActiveSquad.CurrentWingmanA != null)
                        {
                            if (ActiveSquad.CurrentWingmanA.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                            {
                                if (ActiveSquad.CurrentWingmanB != null)
                                {
                                    if (ActiveSquad.CurrentWingmanB.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                                    {
                                        AddChoiceToCurrentPanel(new ActionPanelFly(Map, ActiveSquad));
                                    }
                                }
                                else
                                {
                                    AddChoiceToCurrentPanel(new ActionPanelFly(Map, ActiveSquad));
                                }
                            }
                        }
                        else
                        {
                            AddChoiceToCurrentPanel(new ActionPanelFly(Map, ActiveSquad));
                        }
                    }
                }

                if (ActiveSquad.CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainUnderground)
                    && ActiveSquad.CurrentMovement != UnitStats.TerrainUnderground
                    && Map.GetTerrainType(ActiveSquad.X, ActiveSquad.Y, ActiveSquad.LayerIndex) == UnitStats.TerrainLand)
                {
                    AddChoiceToCurrentPanel(new ActionPanelDig(Map, ActiveSquad));
                }

                CheckForMapSwitch();

                new ActionPanelRepair(Map, this, ActiveSquad).OnSelect();
                new ActionPanelResupply(Map, this, ActiveSquad).OnSelect();
            }
        }

        private void CheckForMapSwitch()
        {
            foreach (MapSwitchPoint ActivePoint in Map.ListMapSwitchPoint)
            {
                if (ActivePoint.Position == ActiveSquad.Position)
                {
                    AddChoiceToCurrentPanel(new ActionPanelMapSwitch(Map, ActiveSquad, ActivePoint));
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            NavigateThroughNextChoices(Map.sndSelection, Map.sndConfirm);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
