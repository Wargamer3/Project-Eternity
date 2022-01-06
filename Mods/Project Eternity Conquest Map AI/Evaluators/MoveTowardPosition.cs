using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder
    {
        public class MoveTowardPosition : ConquestScript, ScriptEvaluator
        {
            private Vector2 _TargetPosition;

            public MoveTowardPosition()
                : base(150, 50, "Move Toward Position Conquest", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                //Movement initialisation.
                Info.Map.MovementAnimation.Add(Info.ActiveSquad.X, Info.ActiveSquad.Y, Info.ActiveSquad.Components);
                List<Unit> ListChoice = new List<Unit>();

                float DistanceMax = 99999;
                List<MovementAlgorithmTile> ListMVChoice = Info.Map.GetMVChoice(Info.ActiveSquad);
                int FinalMV = 0;
                //If for some reason, there's no target on to move at, don't move.
                //Remove everything that is closer then DistanceMax.
                for (int M = 0; M < ListMVChoice.Count; M++)
                {
                    float Distance = (Math.Abs(ListMVChoice[M].Position.X - TargetPosition.X) + Math.Abs(ListMVChoice[M].Position.Y - TargetPosition.Y));
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
                    Info.Map.CursorPosition.X = ListMVChoice[FinalMV].Position.X;
                    Info.Map.CursorPosition.Y = ListMVChoice[FinalMV].Position.Y;
                    Info.Map.CursorPositionVisible = Info.Map.CursorPosition;
                    //Move the Unit to the target position;
                    Info.ActiveSquad.SetPosition(ListMVChoice[FinalMV].Position);
                    Info.Map.FinalizeMovement(Info.ActiveSquad);
                }
                else
                {
                    Info.Map.FinalizeMovement(Info.ActiveSquad);
                }

                Result = new List<object>();
                IsCompleted = true;
            }
            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _TargetPosition = new Vector2(BR.ReadSingle(), BR.ReadSingle());
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
