using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMainMenu : ActionPanelDeathmatch
    {
        private const string PanelName = "Menu";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;

        private Squad ActiveSquad;

        public ActionPanelMainMenu(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelMainMenu(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            Map.ActiveSquadIndex = ActiveSquadIndex;

            //Update weapons to decide if the attack choice is drawn.
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].Team, ActiveSquad.CanMove);

            ListNextChoice.Clear();

            if (ActiveSquad.CanMove)
            {
                AddChoiceToCurrentPanel(new ActionPanelMovePart1(Map, ActivePlayerIndex, ActiveSquadIndex, Map.CursorPosition, Map.CameraPosition));
                if (ActiveSquad.CurrentLeader.CanAttack)
                {
                    AddChoiceToCurrentPanel(new ActionPanelAttackPart1(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CanMove));
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
                    AddChoiceToCurrentPanel(new ActionPanelFormation(Map, ActivePlayerIndex, ActiveSquadIndex));

                List<ActionPanel> DicOptionalPanel = ActiveSquad.OnMenuSelect(ActivePlayerIndex, Map.ListActionMenuChoice);
                foreach (ActionPanel OptionalPanel in DicOptionalPanel)
                {
                    AddChoiceToCurrentPanel(OptionalPanel);
                }

                foreach (ActionPanel OptionalPanel in GetPropPanelsOnUnitSelected(ActiveSquad))
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

        private List<ActionPanel> GetPropPanelsOnUnitSelected(Squad SelectedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            foreach (InteractiveProp ActiveProp in Map.ListLayer[ActiveSquad.LayerIndex].ListProp)
            {
                ListPanel.AddRange(ActiveProp.OnUnitSelected(SelectedUnit));
            }

            return ListPanel;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            NavigateThroughNextChoices(Map.sndSelection, Map.sndConfirm);
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Map.ActiveSquadIndex = ActiveSquadIndex;

            //Update weapons to decide if the attack choice is drawn.
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].Team, ActiveSquad.CanMove);
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMainMenu(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
