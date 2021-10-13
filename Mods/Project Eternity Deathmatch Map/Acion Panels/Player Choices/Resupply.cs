using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelResupply : ActionPanelDeathmatch
    {
        private const string PanelName = "Resupply";

        private readonly Squad ActiveSquad;
        private readonly ActionPanel Owner;
        private List<Vector3> ListMVChoice;

        public ActionPanelResupply(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelResupply(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.Owner = Owner;
            ListMVChoice = new List<Vector3>();
        }

        public override void OnSelect()
        {
            if (ActiveSquad.CurrentLeader.Boosts.ResupplyModifier)
            {
                int SquadIndex;

                #region Self

                for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    if (ActiveSquad[U].EN < ActiveSquad[U].MaxEN)
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
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].EN < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxEN)
                        {
                            ListMVChoice.Add(ActiveSquad.Position - new Vector3(1, 0, 0));
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
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].EN < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxEN)
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
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].EN < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxEN)
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
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].EN < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex][U].MaxEN)
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
                    bool CanResupply = false;

                    for (int U = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].HP < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].MaxHP)
                        {
                            //Refill EN.
                            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].RefillEN(Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].MaxEN);
                            //Refill Weapons.
                            for (int W = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].ListAttack.Count - 1; W >= 0; --W)
                                Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].ListAttack[W].Ammo = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].ListAttack[W].MaxAmmo;
                            //Drop Morale by 10 points.
                            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].Pilot.Will = Math.Max(50, Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[CursorSelect][U].Pilot.Will - 10);

                            CanResupply = true;
                        }
                    }

                    if (CanResupply)
                    {
                        ActiveSquad.EndTurn();
                        RemoveAllSubActionPanels();
                    }
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            int AttackChoiceCount = BR.ReadInt32();
            ListMVChoice = new List<Vector3>(AttackChoiceCount);
            for (int M = 0; M < AttackChoiceCount; ++M)
            {
                ListMVChoice.Add(new Vector3(BR.ReadFloat(), BR.ReadFloat(), 0f));
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ListMVChoice.Count);

            for (int M = 0; M < ListMVChoice.Count; ++M)
            {
                BW.AppendFloat(ListMVChoice[M].X);
                BW.AppendFloat(ListMVChoice[M].Y);
            }
        }

        protected override ActionPanel Copy()
        {
            throw new NotImplementedException();
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
