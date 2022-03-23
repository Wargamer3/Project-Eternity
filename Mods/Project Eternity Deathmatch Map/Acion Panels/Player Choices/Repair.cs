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
    public class ActionPanelRepair : ActionPanelDeathmatch
    {
        private const string PanelName = "Repair";

        private readonly Squad ActiveSquad;
        private readonly ActionPanel Owner;
        private List<MovementAlgorithmTile> ListMVChoice;
        private List<Vector3> ListMVPoints;

        public ActionPanelRepair(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            ListMVChoice = new List<MovementAlgorithmTile>();
        }

        public ActionPanelRepair(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad, List<MovementAlgorithmTile> ListMVChoice, List<Vector3> ListMVPoints)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.Owner = Owner;
            this.ListMVChoice = ListMVChoice;
            this.ListMVPoints = ListMVPoints;
        }

        public override void OnSelect()
        {
        }

        public static void AddIfUsable(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad)
        {
            List<MovementAlgorithmTile> ListMVChoice = new List<MovementAlgorithmTile>();

            if (ActiveSquad.CurrentLeader.Boosts.RepairModifier)
            {
                int SquadIndex;

                #region Self

                for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    if (ActiveSquad[U].HP < ActiveSquad[U].MaxHP)
                    {
                        ListMVChoice.Add(Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z));
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
                            ListMVChoice.Add(Map.GetTerrain(ActiveSquad.Position.X - 1, ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z));
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
                            ListMVChoice.Add(Map.GetTerrain(ActiveSquad.Position.X + 1, ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z));
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
                            ListMVChoice.Add(Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y - 1, (int)ActiveSquad.Position.Z));
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
                            ListMVChoice.Add(Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y + 1, (int)ActiveSquad.Position.Z));
                            break;
                        }
                    }
                }

                #endregion

                if (ListMVChoice.Count > 0)
                {
                    List<Vector3> ListMVPoints = new List<Vector3>();
                    foreach (MovementAlgorithmTile ActiveTerrain in ListMVChoice)
                    {
                        ListMVPoints.Add(new Vector3(ActiveTerrain.WorldPosition.X, ActiveTerrain.WorldPosition.Y, ActiveTerrain.LayerIndex));
                    }

                    Owner.AddChoiceToCurrentPanel(new ActionPanelRepair(Map, Owner, ActiveSquad, ListMVChoice, ListMVPoints));
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.CursorControl(ActiveInputManager);//Move the cursor
            Map.LayerManager.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));

            if (ActiveInputManager.InputConfirmPressed())
            {
                //Find if a current player Unit is under the cursor.
                int CursorSelect = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, Map.CursorPosition, Vector3.Zero);

                if (CursorSelect >= 0 && ListMVPoints.Contains(Map.CursorPosition))
                {
                    bool CanRepair = false;

                    for (int U = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].HP < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].MaxHP)
                        {
                            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].HealUnit(
                                (ActiveSquad.CurrentLeader.PilotLevel * 100) + (ActiveSquad.CurrentLeader.PilotSKL * 10));

                            CanRepair = true;
                        }
                    }

                    if (CanRepair)
                    {
                        LevelUpMenu BattleRecap = new LevelUpMenu(Map, ActiveSquad.CurrentLeader.Pilot, ActiveSquad.CurrentLeader, ActiveSquad, true);
                        BattleRecap.TotalExpGained += 25;
                        if (Constants.ShowBattleRecap)
                        {
                            Map.PushScreen(BattleRecap);
                        }
                        else
                        {
                            BattleRecap.LevelUp();
                        }
                        ActiveSquad.EndTurn();
                        RemoveAllSubActionPanels();
                    }
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            int AttackChoiceCount = BR.ReadInt32();
            ListMVChoice = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int M = 0; M < AttackChoiceCount; ++M)
            {
                ListMVChoice.Add(Map.GetTerrain(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32()));
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ListMVChoice.Count);

            for (int M = 0; M < ListMVChoice.Count; ++M)
            {
                BW.AppendFloat(ListMVChoice[M].WorldPosition.X);
                BW.AppendFloat(ListMVChoice[M].WorldPosition.Y);
                BW.AppendInt32((int)ListMVChoice[M].WorldPosition.Z);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelRepair(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
