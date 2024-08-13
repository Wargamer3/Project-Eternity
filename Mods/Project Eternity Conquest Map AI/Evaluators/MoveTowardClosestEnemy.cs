using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Online;
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
                Info.Map.CursorPosition = Info.ActiveUnit.Position;
                Info.Map.CursorPositionVisible = Info.ActiveUnit.Position;

                UnitConquest TargetSquad = null;
                //Movement initialisation.
                Vector3 StartPosition = Info.ActiveUnit.Position;
                float DistanceMax = 99999;
                //Select the nearest enemy as a target.
                for (int P = 0; P < Info.Map.ListPlayer.Count; P++)
                {
                    //If the player is from the same team as the current player or is dead, skip it.
                    if (Info.Map.ListPlayer[P].TeamIndex == Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].TeamIndex
                        || !Info.Map.ListPlayer[P].IsAlive)
                        continue;
                    for (int U = 0; U < Info.Map.ListPlayer[P].ListUnit.Count; U++)
                    {
                        if (Info.Map.ListPlayer[P].ListUnit[U].HP == 0)
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
                List<Vector3> ListMVChoice = new List<Vector3>();
                List<MovementAlgorithmTile> ListMVTile = Info.Map.GetMVChoice(Info.ActiveUnit);
                foreach (MovementAlgorithmTile ActiveTerrain in ListMVTile)
                {
                    ListMVChoice.Add(new Vector3(ActiveTerrain.WorldPosition.X, ActiveTerrain.WorldPosition.Y, ActiveTerrain.LayerIndex));
                }
                List<Vector3> ListMVPoints = FilterMVChoice(ListMVChoice);
                int FinalMV = 0;
                //If for some reason, there's no target on to move at, don't move.
                if (TargetSquad != null)
                {
                    //Remove everything that is closer then DistanceMax.
                    for (int M = 0; M < ListMVPoints.Count; M++)
                    {
                        float Distance = (Math.Abs(ListMVPoints[M].X - TargetSquad.X) + Math.Abs(ListMVPoints[M].Y - TargetSquad.Y));
                        //Remove MV choices if they are not at the furthest distance and if there is at least 1 MV(protection against bugs)
                        if (Distance < DistanceMax && ListMVPoints.Count > 1)
                        {
                            DistanceMax = Distance;
                            FinalMV = M;
                        }
                    }
                    if (DistanceMax < Math.Abs(Info.ActiveUnit.X - TargetSquad.X) + Math.Abs(Info.ActiveUnit.Y - TargetSquad.Y))
                    {
                        //Prepare the Cursor to move.
                        Info.Map.CursorPosition.X = ListMVTile[FinalMV].InternalPosition.X;
                        Info.Map.CursorPosition.Y = ListMVTile[FinalMV].InternalPosition.Y;
                        Info.Map.CursorPositionVisible = ListMVPoints[FinalMV];
                        //Move the Unit to the target position;
                        Info.ActiveUnit.SetPosition(ListMVPoints[FinalMV]);
                        Info.Map.FinalizeMovement(Info.ActiveUnit, (int)Info.Map.GetTerrain(Info.ActiveUnit.Components).MovementCost, new List<Vector3>());
                    }
                    else
                    {
                        Info.Map.FinalizeMovement(Info.ActiveUnit, 1, new List<Vector3>());
                    }
                }
                else
                {
                    Info.Map.FinalizeMovement(Info.ActiveUnit, 1, new List<Vector3>());
                }

                Info.Map.MovementAnimation.Add(Info.ActiveUnit.Components, StartPosition, Info.ActiveUnit.Position);

                if (Info.Map.IsServer)
                {
                    foreach (IOnlineConnection ActivePlayer in Info.Map.GameGroup.Room.ListUniqueOnlineConnection)
                    {
                        //ActivePlayer.Send(new MoveUnitScriptServer(StartPosition, Info.ActiveUnit.Position));
                    }
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
                    for (int CurrentSquadOffsetX = 0; CurrentSquadOffsetX < Info.ActiveUnit.Components.ArrayMapSize.GetLength(0) && CanMove; ++CurrentSquadOffsetX)
                    {
                        for (int CurrentSquadOffsetY = 0; CurrentSquadOffsetY < Info.ActiveUnit.Components.ArrayMapSize.GetLength(1) && CanMove; ++CurrentSquadOffsetY)
                        {
                            float RealX = ActiveMVChoice.X + CurrentSquadOffsetX;
                            float RealY = ActiveMVChoice.Y + CurrentSquadOffsetY;

                            if (!ListMVChoice.Contains(new Vector3((int)RealX, (int)RealY, (int)Info.ActiveUnit.Position.Z)))
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
