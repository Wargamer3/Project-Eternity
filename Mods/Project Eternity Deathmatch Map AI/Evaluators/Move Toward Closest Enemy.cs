using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class MoveTowardClosestEnemy : DeathmatchScript, ScriptEvaluator
        {
            public MoveTowardClosestEnemy()
                : base(150, 50, "Move Toward Closest Enemy", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Squad TargetSquad = null;
                //Movement initialisation.
                Info.Map.MovementAnimation.Add(Info.ActiveSquad.X, Info.ActiveSquad.Y, Info.ActiveSquad);
                float DistanceMax = 99999;
                //Select the nearest enemy as a target.
                for (int P = 0; P < Info.Map.ListPlayer.Count; P++)
                {
                    //If the player is from the same team as the current player or is dead, skip it.
                    if (Info.Map.ListPlayer[P].Team == Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].Team
                        || !Info.Map.ListPlayer[P].IsAlive)
                        continue;
                    for (int U = 0; U < Info.Map.ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (Info.Map.ListPlayer[P].ListSquad[U].CurrentLeader == null)
                            continue;

                        float Distance = (Math.Abs(Info.Map.ListPlayer[P].ListSquad[U].X - Info.ActiveSquad.X) + Math.Abs(Info.Map.ListPlayer[P].ListSquad[U].Y - Info.ActiveSquad.Y));
                        if (Distance < DistanceMax)
                        {
                            DistanceMax = Distance;
                            TargetSquad = Info.Map.ListPlayer[P].ListSquad[U];
                        }
                    }
                }

                DistanceMax = 99999;
                List<Vector3> ListMVChoice = FilterMVChoice(Info.Map.GetMVChoice(Info.ActiveSquad));
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
                    if (DistanceMax < Math.Abs(Info.ActiveSquad.X - TargetSquad.X) + Math.Abs(Info.ActiveSquad.Y - TargetSquad.Y))
                    {
                        //Prepare the Cursor to move.
                        Info.Map.CursorPosition.X = ListMVChoice[FinalMV].X;
                        Info.Map.CursorPosition.Y = ListMVChoice[FinalMV].Y;
                        Info.Map.CursorPositionVisible = Info.Map.CursorPosition;
                        //Move the Unit to the target position;
                        Info.ActiveSquad.SetPosition(ListMVChoice[FinalMV]);
                        Info.Map.FinalizeMovement(Info.ActiveSquad, (int)Info.Map.GetTerrain(Info.ActiveSquad).MovementCost);
                    }
                    else
                    {
                        Info.Map.FinalizeMovement(Info.ActiveSquad, 1);
                    }
                }
                else
                {
                    Info.Map.FinalizeMovement(Info.ActiveSquad, 1);
                }
                
                Result = new List<object>();
                IsCompleted = true;
            }

            private List<Vector3> FilterMVChoice(List<Vector3> ListMVChoice)
            {
                List<Vector3> ListFinalMVChoice = new List<Vector3>();

                foreach (Vector3 ActiveMVChoice in ListMVChoice)
                {
                    bool CanMove = true;
                    for (int CurrentSquadOffsetX = 0; CurrentSquadOffsetX < Info.ActiveSquad.ArrayMapSize.GetLength(0) && CanMove; ++CurrentSquadOffsetX)
                    {
                        for (int CurrentSquadOffsetY = 0; CurrentSquadOffsetY < Info.ActiveSquad.ArrayMapSize.GetLength(1) && CanMove; ++CurrentSquadOffsetY)
                        {
                            float RealX = ActiveMVChoice.X + CurrentSquadOffsetX;
                            float RealY = ActiveMVChoice.Y + CurrentSquadOffsetY;

                            if (!ListMVChoice.Contains(new Vector3((int)RealX, (int)RealY, (int)Info.ActiveSquad.Position.Z)))
                            {
                                CanMove = false;
                            }
                        }
                    }

                    if (CanMove)
                    {
                        ListFinalMVChoice.Add(ActiveMVChoice);
                    }
                }

                return ListFinalMVChoice;
            }


            public override AIScript CopyScript()
            {
                return new MoveTowardClosestEnemy();
            }
        }
    }
}
