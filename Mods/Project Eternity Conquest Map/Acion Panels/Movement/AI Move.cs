using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAIMoveBehavior : ActionPanelConquest
    {
        private const string PanelName = "AI Move Behavior";

        private UnitConquest ActiveUnit;
        private int ActivePlayerIndex;
        private int ActiveUnitIndex;

        private int AITimer;
        private const int AITimerBase = 20;
        private const int AITimerBaseHalf = 10;
        private Vector3 AICursorNextPosition;
        private List<MovementAlgorithmTile> ListMovement;
        private List<MovementAlgorithmTile> ListMVChoice;
        private List<Vector3> ListMVHoverPoints;
        private ActionPanel NextPanel;

        public ActionPanelAIMoveBehavior(ConquestMap Map)
            : base(PanelName, Map)
        {
            ListMovement = new List<MovementAlgorithmTile>();
            ListMVChoice = new List<MovementAlgorithmTile>();
            ListMVHoverPoints = new List<Vector3>();
        }

        public ActionPanelAIMoveBehavior(ConquestMap Map,  int ActivePlayerIndex, int ActiveUnitIndex,
            List<MovementAlgorithmTile> ListMovement, List<MovementAlgorithmTile> ListMVChoice, ActionPanel NextPanel = null)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ListMovement = ListMovement;
            this.ListMVChoice = ListMVChoice;
            this.NextPanel = NextPanel;

            AICursorNextPosition = ListMovement[ListMovement.Count - 1].WorldPosition;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            Map.CursorPosition = ActiveUnit.Position;
            Map.CursorPositionVisible = Map.CursorPosition;

            ListMVHoverPoints = new List<Vector3>();
            foreach (MovementAlgorithmTile ActiveTile in ListMovement)
            {
                ListMVHoverPoints.Add(new Vector3(ActiveTile.WorldPosition.X, ActiveTile.WorldPosition.Y, ActiveTile.LayerIndex));
            }
        }

        public override void OnSelect()
        {
            AITimer = AITimerBase;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
            Map.LayerManager.AddDrawablePath(ListMovement);

            if (AITimer >= 0)
            {
                AITimer--;
            }
            if (Map.CursorPosition.X != AICursorNextPosition.X || Map.CursorPosition.Y != AICursorNextPosition.Y)
            {
                if (AITimer == AITimerBaseHalf)
                {
                    if (Map.CursorPosition.Y < AICursorNextPosition.Y)
                        Map.CursorPosition.Y += 1;
                    else if (Map.CursorPosition.Y > AICursorNextPosition.Y)
                        Map.CursorPosition.Y -= 1;
                    else if (Map.CursorPosition.X < AICursorNextPosition.X)
                        Map.CursorPosition.X += 1;
                    else if (Map.CursorPosition.X > AICursorNextPosition.X)
                        Map.CursorPosition.X -= 1;
                    else if (Map.CursorPosition.Z < AICursorNextPosition.Z)
                        Map.CursorPosition.Z += 1;
                    else if (Map.CursorPosition.Z > AICursorNextPosition.Z)
                        Map.CursorPosition.Z -= 1;
                    else
                        AITimer = -1;
                }
                else if (AITimer == 0)
                {
                    AITimer = AITimerBase;
                    if (Map.CursorPosition.X < AICursorNextPosition.X)
                        Map.CursorPosition.X += 1;
                    else if (Map.CursorPosition.X > AICursorNextPosition.X)
                        Map.CursorPosition.X -= 1;
                }
            }
            else
            {
                //Movement initialisation.
                Map.MovementAnimation.Add(ActiveUnit.Components, ActiveUnit.Position, ListMVHoverPoints, true);

                //Move the Unit to the cursor position
                ActiveUnit.SetPosition(Map.CursorPosition);

                Map.CursorPositionVisible = Map.CursorPosition;

                Map.FinalizeMovement(ActiveUnit, (int)Map.GetTerrain(ActiveUnit.Components).MovementCost, ListMVHoverPoints);

                ActiveUnit.EndTurn();

                RemoveFromPanelList(this);
                if (NextPanel != null)
                {
                    AddToPanelListAndSelect(NextPanel);
                }

                AITimer = AITimerBase;
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAIMoveBehavior(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
