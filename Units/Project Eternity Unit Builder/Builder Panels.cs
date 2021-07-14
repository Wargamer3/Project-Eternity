using Microsoft.Xna.Framework;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units.Builder
{
    public class ActionPanelBuild : ActionPanelDeathmatch
    {
        private int SelectedUnitIndex;
        UnitBuilder ActiveUnit;

        public ActionPanelBuild(DeathmatchMap Map, UnitBuilder ActiveUnit)
                : base("Build", Map, true)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            SelectedUnitIndex = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                AddToPanelListAndSelect(new ActionPanelBuildUnit(Map, ActiveUnit));
                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
            else if (InputHelper.InputUpPressed())
            {
                SelectedUnitIndex -= (SelectedUnitIndex > 0) ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                SelectedUnitIndex += (SelectedUnitIndex < 0) ? 1 : 0;
                Map.sndSelection.Play();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int X = (int)(Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            int Y = (int)(Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth - MinActionMenuWidth;
            
            for (int U = 0; U < 1; ++U)
            {
                GameScreen.DrawBox(g, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), MinActionMenuWidth, PannelHeight, Color.White);
                TextHelper.DrawText(g, "Electric Wall", new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
            }
            g.Draw(GameScreen.sprPixel, new Rectangle(X + MinActionMenuWidth, Y + SelectedUnitIndex * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
        }

        protected override void OnCancelPanel()
        {
        }
    }

    public class ActionPanelBuildUnit : ActionPanelDeathmatch
    {
        List<Vector3> ListBuildSpot;
        UnitBuilder ActiveUnit;

        public ActionPanelBuildUnit(DeathmatchMap Map, UnitBuilder ActiveUnit)
                : base("Build", Map, true)
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
                                Map.SpawnSquad(Map.ActiveLayerIndex, new Squad("Tower", NewLeader), 0, Position);
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

        public override void Draw(CustomSpriteBatch g)
        {
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
