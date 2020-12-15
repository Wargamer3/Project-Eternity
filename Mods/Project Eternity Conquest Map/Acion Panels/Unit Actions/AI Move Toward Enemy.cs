using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAIMoveTowardEnemy : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;

        public ActionPanelAIMoveTowardEnemy(ConquestMap Map, UnitConquest ActiveUnit)
            : base("AI Move Toward Enemy", Map)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            UnitConquest TargetSquad = null;
            //Movement initialisation.
            Map.MovementAnimation.Add(ActiveUnit.X, ActiveUnit.Y, ActiveUnit.Components);
            List<Unit> ListChoice = new List<Unit>();
            float DistanceMax = 99999;
            //Select the nearest enemy as a target.
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //If the player is from the same team as the current player or is dead, skip it.
                if (Map.ListPlayer[P].Team == Map.ListPlayer[Map.ActivePlayerIndex].Team
                    || !Map.ListPlayer[P].IsAlive)
                    continue;

                for (int U = 0; U < Map.ListPlayer[P].ListUnit.Count; U++)
                {
                    float Distance = (Math.Abs(Map.ListPlayer[P].ListUnit[U].X - ActiveUnit.X) + Math.Abs(Map.ListPlayer[P].ListUnit[U].Y - ActiveUnit.Y));
                    if (Distance < DistanceMax)
                    {
                        DistanceMax = Distance;
                        TargetSquad = Map.ListPlayer[P].ListUnit[U];
                    }
                }
            }
            DistanceMax = 99999;
            List<Vector3> ListMVChoice = Map.GetMVChoice(ActiveUnit);
            int FinalMV = 0;

            //If for some reason, there's no target on to move at, don't move.
            if (TargetSquad != null)
            {
                //Remove everything that is closer then DistanceMax.
                for (int M = 0; M < ListMVChoice.Count; M++)
                {
                    float Distance = (Math.Abs(ListMVChoice[M].X - TargetSquad.X) + Math.Abs(ListMVChoice[M].Y - TargetSquad.Y));
                    //Remove MV choices if they are not at the furthest distance and if there is at least 1 MV(protection against bugs)
                    if (Distance < DistanceMax && ListMVChoice.Count > 1)
                    {
                        DistanceMax = Distance;
                        FinalMV = M;
                    }
                }
                if (DistanceMax < Math.Abs(ActiveUnit.X - TargetSquad.X) + Math.Abs(ActiveUnit.Y - TargetSquad.Y))
                {
                    //Prepare the Cursor to move.
                    Map.CursorPosition.X = ListMVChoice[FinalMV].X;
                    Map.CursorPosition.Y = ListMVChoice[FinalMV].Y;
                    //Move the Unit to the target position;
                    ActiveUnit.SetPosition(ListMVChoice[FinalMV]);
                }
            }

            Map.FinalizeMovement(ActiveUnit);
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
