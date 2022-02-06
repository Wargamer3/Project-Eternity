using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class MoveTowardPosition : DeathmatchScript, ScriptEvaluator
        {
            private Vector2 _TargetPosition;

            public MoveTowardPosition()
                : base(150, 50, "Move Toward Position", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                //Movement initialisation.
                Vector3 StartPosition = Info.ActiveSquad.Position;

                float DistanceMax = 99999;
                List<Vector3> ListMVChoice = new List<Vector3>();
                foreach (MovementAlgorithmTile ActiveTerrain in Info.Map.GetMVChoice(Info.ActiveSquad))
                {
                    ListMVChoice.Add(new Vector3(ActiveTerrain.Position.X, ActiveTerrain.Position.Y, ActiveTerrain.LayerIndex));
                }
                ListMVChoice = FilterMVChoice(ListMVChoice);
                int FinalMV = 0;
                //If for some reason, there's no target on to move at, don't move.
                //Remove everything that is closer then DistanceMax.
                for (int M = 0; M < ListMVChoice.Count; M++)
                {
                    float Distance = (Math.Abs(ListMVChoice[M].X - TargetPosition.X) + Math.Abs(ListMVChoice[M].Y - TargetPosition.Y));
                    //Remove MV choices if they are not at the furthest distance and if there is at least 1 MV(protection against bugs)
                    if (Distance < DistanceMax && ListMVChoice.Count > 1)
                    {
                        DistanceMax = Distance;
                        FinalMV = M;
                    }
                }
                if (DistanceMax < Math.Abs(Info.ActiveSquad.X - TargetPosition.X) + Math.Abs(Info.ActiveSquad.Y - TargetPosition.Y))
                {
                    //Prepare the Cursor to move.
                    Info.Map.CursorPosition.X = ListMVChoice[FinalMV].X;
                    Info.Map.CursorPosition.Y = ListMVChoice[FinalMV].Y;
                    Info.Map.CursorPositionVisible = Info.Map.CursorPosition;
                    //Move the Unit to the target position;
                    Info.ActiveSquad.SetPosition(ListMVChoice[FinalMV]);
                    Info.Map.FinalizeMovement(Info.ActiveSquad, (int)Info.Map.GetTerrain(Info.ActiveSquad).MovementCost, new List<Vector3>());
                }
                else
                {
                    Info.Map.FinalizeMovement(Info.ActiveSquad, 1, new List<Vector3>());
                }

                Info.Map.MovementAnimation.Add(Info.ActiveSquad, StartPosition, Info.ActiveSquad.Position);

                Result = new List<object>();
                IsCompleted = true;
            }
            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _TargetPosition = new Vector2(BR.ReadSingle(), BR.ReadSingle());
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

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_TargetPosition.X);
                BW.Write(_TargetPosition.Y);
            }

            public override AIScript CopyScript()
            {
                return new MoveTowardPosition();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public Vector2 TargetPosition
            {
                get
                {
                    return _TargetPosition;
                }
                set
                {
                    _TargetPosition = value;
                }
            }
        }
    }
}
