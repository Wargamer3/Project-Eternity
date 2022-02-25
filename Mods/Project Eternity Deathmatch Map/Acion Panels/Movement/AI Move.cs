using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAIMoveBehavior : ActionPanelDeathmatch
    {
        private const string PanelName = "AI Attack Behavior";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;

        private int AITimer;
        private const int AITimerBase = 20;
        private const int AITimerBaseHalf = 10;
        private Vector3 AICursorNextPosition;
        private List<MovementAlgorithmTile> ListMovement;
        private List<MovementAlgorithmTile> ListMVChoice;
        private List<Vector3> ListMVHoverPoints;
        private ActionPanel NextPanel;

        public ActionPanelAIMoveBehavior(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            ListMovement = new List<MovementAlgorithmTile>();
            ListMVChoice = new List<MovementAlgorithmTile>();
            ListMVHoverPoints = new List<Vector3>();
        }

        public ActionPanelAIMoveBehavior(DeathmatchMap Map,  int ActivePlayerIndex, int ActiveSquadIndex,
            List<MovementAlgorithmTile> ListMovement, List<MovementAlgorithmTile> ListMVChoice, ActionPanel NextPanel = null)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMovement = ListMovement;
            this.ListMVChoice = ListMVChoice;
            this.NextPanel = NextPanel;

            AICursorNextPosition = ListMVChoice[ListMVChoice.Count - 1].Position;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Map.CursorPosition = ActiveSquad.Position;

            foreach (MovementAlgorithmTile ActiveTile in ListMovement)
            {
                ListMVHoverPoints.Add(ActiveTile.Position);
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
                Map.MovementAnimation.Add(ActiveSquad, ActiveSquad.Position, ListMVHoverPoints);

                //Move the Unit to the cursor position
                ActiveSquad.SetPosition(Map.CursorPosition);

                Map.CursorPosition = ActiveSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;

                RemoveFromPanelList(this);
                if (NextPanel != null)
                {
                    AddToPanelListAndSelect(NextPanel);
                }
                else
                {
                    //End the unit turn.
                    ActiveSquad.EndTurn();
                }

                AITimer = AITimerBase;
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAIAttackBehavior(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
