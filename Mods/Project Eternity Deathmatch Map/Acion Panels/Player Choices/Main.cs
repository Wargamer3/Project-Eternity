using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
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
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].TeamIndex, ActiveSquad.CanMove);

            ListNextChoice.Clear();

            if (ActiveSquad.CanMove)
            {
                AddChoiceToCurrentPanel(new ActionPanelMovePart1(Map, ActivePlayerIndex, ActiveSquadIndex, Map.GetTerrainUnderCursor(), ActiveSquad.Direction, Map.Camera2DPosition));
                if (ActiveSquad.CurrentLeader.CanAttack)
                {
                    AddChoiceToCurrentPanel(new ActionPanelAttackPart1(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CanMove, new List<Vector3>()));
                }
                AddChoiceToCurrentPanel(new ActionPanelSpirit(Map, ActiveSquad));
                new ActionPanelConsumableParts(Map, this, ActiveSquad).OnSelect();
            }

            if (ActiveSquad.CanTransportUnit && ActiveSquad.ListTransportedUnit.Count > 0)
            {
                AddToPanelListAndSelect(new ActionPanelDeploy(Map, ActiveSquad));
            }

            ActionPanelChangeTerrain.AddIfUsable(Map, this, ActiveSquad);

            AddChoiceToCurrentPanel(new ActionPanelStatus(Map, ActiveSquad));
            AddChoiceToCurrentPanel(new ActionPanelDebug(ActiveSquad, Map));

            if (ActiveSquad.CanMove)
            {
                //Add the squad options.
                if (ActiveSquad.UnitsAliveInSquad > 1)
                    AddChoiceToCurrentPanel(new ActionPanelFormation(Map, ActivePlayerIndex, ActiveSquadIndex));

                ActiveSquad.OnMenuSelect(this, ActivePlayerIndex, Map.ListActionMenuChoice);

                AddPropPanelsOnUnitSelected();

                CheckForMapSwitch();

                ActionPanelJump.AddIfUsable(Map, this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad);
                ActionPanelRepair.AddIfUsable(Map, this, ActiveSquad);
                ActionPanelResupply.AddIfUsable(Map, this, ActiveSquad);
            }

            AddMutatorPanels();
        }

        public static void AddIfUsable(DeathmatchMap Map, ActionPanel Owner)
        {
            bool UnitFound = false;
            //Loop through the players to find a Unit to control.
            for (int P = 0; P < Map.ListPlayer.Count && (Map.ActiveSquadIndex < 0 && Map.TargetSquadIndex < 0); P++)
            {
                //Find if a current player Unit is under the cursor.
                int CursorSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);

                #region Unit found

                if (CursorSelect >= 0)
                {
                    UnitFound = true;
                    ActionPanelMainMenu NewActionMenu = new ActionPanelMainMenu(Map, P, CursorSelect);

                    if (P == Map.ActivePlayerIndex && Map.ListPlayer[P].ListSquad[CursorSelect].IsPlayerControlled)//Player controlled Squad.
                    {
                        NewActionMenu.OnSelect();
                    }
                    else//Enemy.
                    {
                        NewActionMenu.AddChoiceToCurrentPanel(new ActionPanelStatus(Map, Map.ListPlayer[P].ListSquad[CursorSelect]));
                    }

                    Owner.AddToPanelList(NewActionMenu);
                }

                #endregion
            }

            //You select the tile under the cursor.
            if (!UnitFound)
            {
                Owner.AddToPanelListAndSelect(new ActionPanelTileStatus(Map));
            }
        }

        private void CheckForMapSwitch()
        {
            foreach (MapSwitchPoint ActivePoint in Map.LayerManager[(int)ActiveSquad.Position.Z].ListMapSwitchPoint)
            {
                if (ActivePoint.Position == ActiveSquad.Position)
                {
                    AddChoiceToCurrentPanel(new ActionPanelMapSwitch(Map, ActiveSquad, ActivePoint));
                }
            }
        }

        private void AddPropPanelsOnUnitSelected()
        {
            foreach (InteractiveProp ActiveProp in Map.LayerManager[(int)ActiveSquad.Position.Z].ListProp)
            {
                ActiveProp.OnUnitSelected(this, ActiveSquad);
            }
        }

        private void AddMutatorPanels()
        {
            foreach (DeathmatchMutator ActiveMutator in Map.ListMutator)
            {
                ActiveMutator.OnSquadSelected(this, ActivePlayerIndex, ActiveSquadIndex);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(Map.sndSelection))
            {
            }
            else if (ConfirmNextChoices(Map.sndConfirm))
            {
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Map.ActiveSquadIndex = ActiveSquadIndex;

            //Update weapons to decide if the attack choice is drawn.
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].TeamIndex, ActiveSquad.CanMove);
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
