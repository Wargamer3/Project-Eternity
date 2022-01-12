using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Builder
{
    public class ActionPanelBuildUnit : ActionPanelDeathmatch
    {
        private const string PanelName = "Build2";

        List<Vector3> ListBuildSpot;
        List<MovementAlgorithmTile> ListBuildTerrain;
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
            ListBuildTerrain = new List<MovementAlgorithmTile>();
            foreach (Vector3 ActiveTerrain in ListBuildSpot)
            {
                ListBuildTerrain.Add(Map.GetTerrain(ActiveTerrain.X, ActiveTerrain.Y, (int)ActiveTerrain.Z));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed())
            {
                List<Squad> ListOtherSquads = FindOtherSquad("Tower");
                Unit NewLeader = Unit.FromType("Normal", "Electric Wall", Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget);
                NewLeader.ArrayCharacterActive = new Character[0];

                //Initialise the Unit stats.
                NewLeader.Init();
                Map.SpawnSquad(0, new Squad("Tower", NewLeader), 0, Map.CursorPosition, (int)Map.CursorPosition.Z);

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
                            Position.Z += Math.Sign(Diff.Z);

                            if (!Map.CheckForObstacleAtPosition(Position, Vector3.Zero))
                            {
                                NewLeader = Unit.FromType("Normal", "Electric Wall", Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget);
                                NewLeader.ArrayCharacterActive = new Character[0];

                                //Initialise the Unit stats.
                                NewLeader.Init();
                                Map.SpawnSquad(Map.ActivePlayerIndex, new Squad("Tower", NewLeader), 0, Position, (int)Position.Z);
                            }
                        }
                    }
                }
                Map.sndConfirm.Play();
                CancelPanel();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
            }
            else
            {
                Map.CursorControl(ActiveInputManager);
            }

            Map.LayerManager.AddDrawablePoints(ListBuildTerrain, Color.Coral);
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
