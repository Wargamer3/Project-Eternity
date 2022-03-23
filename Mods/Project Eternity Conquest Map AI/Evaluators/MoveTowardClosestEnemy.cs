using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder
    {
        public class MoveTowardClosestEnemy : ConquestScript, ScriptEvaluator
        {
            public MoveTowardClosestEnemy()
                : base(150, 50, "Move Toward Closest Enemy Conquest", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                UnitConquest TargetSquad = null;
                //Movement initialisation.
                Vector3 StartPosition = Info.ActiveUnit.Components.Position;
                List<Unit> ListChoice = new List<Unit>();
                float DistanceMax = 99999;
                //Select the nearest enemy as a target.
                for (int P = 0; P < Info.Map.ListPlayer.Count; P++)
                {
                    //If the player is from the same team as the current player or is dead, skip it.
                    if (Info.Map.ListPlayer[P].Team == Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].Team
                        || !Info.Map.ListPlayer[P].IsAlive)
                        continue;
                    for (int U = 0; U < Info.Map.ListPlayer[P].ListUnit.Count; U++)
                    {
                        if (Info.Map.ListPlayer[P].ListUnit[U].HP <= 0)
                            continue;

                        float Distance = (Math.Abs(Info.Map.ListPlayer[P].ListUnit[U].X - Info.ActiveUnit.X) + Math.Abs(Info.Map.ListPlayer[P].ListUnit[U].Y - Info.ActiveUnit.Y));
                        if (Distance < DistanceMax)
                        {
                            DistanceMax = Distance;
                            TargetSquad = Info.Map.ListPlayer[P].ListUnit[U];
                        }
                    }
                }

                DistanceMax = 99999;
                List<MovementAlgorithmTile> ListMVChoice = Info.Map.GetMVChoice(Info.ActiveUnit);
                int FinalMV = 0;
                //If for some reason, there's no target on to move at, don't move.
                if (TargetSquad != null)
                {
                    //Remove everything that is closer then DistanceMax.
                    for (int M = 0; M < ListMVChoice.Count; M++)
                    {
                        float Distance = (Math.Abs(ListMVChoice[M].WorldPosition.X - TargetSquad.X) + Math.Abs(ListMVChoice[M].WorldPosition.Y - TargetSquad.Y));
                        //Remove MV choices if they are not at the furthest distance and if there is at least 1 MV(protection against bugs)
                        if (Distance < DistanceMax && ListMVChoice.Count > 1)
                        {
                            DistanceMax = Distance;
                            FinalMV = M;
                        }
                    }
                    if (DistanceMax < Math.Abs(Info.ActiveUnit.X - TargetSquad.X) + Math.Abs(Info.ActiveUnit.Y - TargetSquad.Y))
                    {
                        //Prepare the Cursor to move.
                        Info.Map.CursorPosition.X = ListMVChoice[FinalMV].WorldPosition.X;
                        Info.Map.CursorPosition.Y = ListMVChoice[FinalMV].WorldPosition.Y;
                        Info.Map.CursorPositionVisible = Info.Map.CursorPosition;
                        //Move the Unit to the target position;
                        Info.ActiveUnit.SetPosition(ListMVChoice[FinalMV].WorldPosition);
                        Info.Map.FinalizeMovement(Info.ActiveUnit);
                    }
                    else
                    {
                        Info.Map.FinalizeMovement(Info.ActiveUnit);
                    }
                }
                else
                {
                    Info.Map.FinalizeMovement(Info.ActiveUnit);
                }

                Info.Map.MovementAnimation.Add(Info.ActiveUnit.Components, StartPosition, Info.ActiveUnit.Components.Position);

                Result = new List<object>();
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new MoveTowardClosestEnemy();
            }
        }
    }
}
