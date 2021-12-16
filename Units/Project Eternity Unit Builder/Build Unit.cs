using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Builder
{
    public class ActionPanelBuildUnit : ActionPanelDeathmatch
    {
        private const string PanelName = "Build2";

        List<Vector3> ListBuildSpot;
        UnitBuilder ActiveUnit;

        public ActionPanelBuildUnit(DeathmatchMap Map)
                : base(PanelName, Map, true)
        {
        }

        public ActionPanelBuildUnit(DeathmatchMap Map, UnitBuilder ActiveUnit)
                : base(PanelName, Map, true)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            ListBuildSpot = Map.ComputeRange(Map.CursorPosition, 1, 5);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                List<Squad> ListOtherSquads = FindOtherSquad("Tower");
                Unit NewLeader = Unit.FromType("Normal", "Electric Wall", Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget);
                NewLeader.ArrayCharacterActive = new Character[0];

                //Initialise the Unit stats.
                NewLeader.Init();
                Map.SpawnSquad(0, new Squad("Tower", NewLeader), 0, Map.CursorPosition);

                if (ListOtherSquads.Count > 0)
                {
                    foreach (Squad ActiveOtherSquad in ListOtherSquads)
                    {
                        Vector3 Position = ActiveOtherSquad.Position;

                        while ((Map.CursorPosition.X != Position.X && Map.CursorPosition.Y == Position.Y)
                            || (Map.CursorPosition.X == Position.X && Map.CursorPosition.Y != Position.Y))
                        {
                            Vector3 Diff = Map.CursorPosition - Position;

                            Position.X += Math.Sign(Diff.X);
                            Position.Y += Math.Sign(Diff.Y);

                            if (!Map.CheckForObstacleAtPosition(Position, Vector3.Zero))
                            {
                                NewLeader = Unit.FromType("Normal", "Electric Wall", Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget);
                                NewLeader.ArrayCharacterActive = new Character[0];

                                //Initialise the Unit stats.
                                NewLeader.Init();
                                Map.SpawnSquad(Map.ActivePlayerIndex, new Squad("Tower", NewLeader), 0, Position);
                            }
                        }
                    }
                }
                Map.sndConfirm.Play();
                CancelPanel();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
            else
            {
                Map.CursorControl();
            }

            Map.ListLayer[0].LayerGrid.AddDrawablePoints(ListBuildSpot, Color.Coral);
        }

        private List<Squad> FindOtherSquad(string SquadName)
        {
            List<Squad> ListOtherSquad = new List<Squad>();

            for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; ++S)
            {
                if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[S].SquadName == SquadName)
                {
                    ListOtherSquad.Add(Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[S]);
                }
            }
            return ListOtherSquad;
        }

        public override void DoRead(ByteReader BR)
        {
            int ListBuildSpotCount = BR.ReadInt32();
            ListBuildSpot = new List<Vector3>(ListBuildSpotCount);
            for (int B = 0; B < ListBuildSpotCount; ++B)
            {
                ListBuildSpot.Add(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ListBuildSpot.Count);
            for (int B = 0; B < ListBuildSpot.Count; ++B)
            {
                BW.AppendFloat(ListBuildSpot[B].X);
                BW.AppendFloat(ListBuildSpot[B].Y);
                BW.AppendFloat(ListBuildSpot[B].Z);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBuildUnit(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
