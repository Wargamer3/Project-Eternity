using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelRepair : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private readonly ActionPanel Owner;
        private List<Vector3> ListMVChoice;

        public ActionPanelRepair(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad)
            : base("Repair", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.Owner = Owner;
            ListMVChoice = new List<Vector3>();
        }

        public override void OnSelect()
        {
            if (ActiveSquad.CurrentLeader.Boosts.RepairModifier)
            {
                int SquadIndex;

                #region Self

                for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    if (ActiveSquad[U].HP < ActiveSquad[U].MaxHP)
                    {
                        ListMVChoice.Add(ActiveSquad.Position);
                        break;
                    }
                }

                #endregion

                #region X - 1

                SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, ActiveSquad.Position, new Vector3(-1, 0, 0));
                if (SquadIndex >= 0)
                {
                    for (int U = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].HP < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxHP)
                        {
                            ListMVChoice.Add(ActiveSquad.Position + new Vector3(1, 0, 0));
                            break;
                        }
                    }
                }

                #endregion

                #region X + 1

                SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, ActiveSquad.Position, new Vector3(1, 0, 0));
                if (SquadIndex >= 0)
                {
                    for (int U = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].HP < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxHP)
                        {
                            ListMVChoice.Add(ActiveSquad.Position + new Vector3(1, 0, 0));
                            break;
                        }
                    }
                }

                #endregion

                #region Y - 1

                SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, ActiveSquad.Position, new Vector3(0, -1, 0));
                if (SquadIndex >= 0)
                {
                    for (int U = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].HP < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxHP)
                        {
                            ListMVChoice.Add(ActiveSquad.Position - new Vector3(0, 1, 0));
                            break;
                        }
                    }
                }

                #endregion

                #region Y + 1

                SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, ActiveSquad.Position, new Vector3(0, 1, 0));
                if (SquadIndex >= 0)
                {
                    for (int U = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].HP < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxHP)
                        {
                            ListMVChoice.Add(ActiveSquad.Position + new Vector3(0, 1, 0));
                            break;
                        }
                    }
                }

                #endregion

                if (ListMVChoice.Count > 0)
                    Owner.AddChoiceToCurrentPanel(this);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.CursorControl();//Move the cursor
            Map.ListLayer[Map.ActiveLayerIndex].LayerGrid.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));

            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (MouseHelper.InputLeftButtonReleased())
                {
                    if (MouseHelper.MouseStateCurrent.X < 0 || MouseHelper.MouseStateCurrent.X > Constants.Width ||
                        MouseHelper.MouseStateCurrent.Y < 0 || MouseHelper.MouseStateCurrent.Y > Constants.Height)
                        return;
                }
                //Find if a current player Unit is under the cursor.
                int CursorSelect = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, Map.CursorPosition, Vector3.Zero);

                if (CursorSelect >= 0 && ListMVChoice.Contains(Map.CursorPosition))
                {
                    bool CanRepair = false;

                    for (int U = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].HP < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].MaxHP)
                        {
                            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].HealUnit(
                                ((ActiveSquad.CurrentLeader.PilotLevel * 100) + (ActiveSquad.CurrentLeader.PilotSKL * 10)));

                            CanRepair = true;
                        }
                    }

                    if (CanRepair)
                    {
                        ActiveSquad.EndTurn();
                        RemoveAllSubActionPanels();
                    }
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
