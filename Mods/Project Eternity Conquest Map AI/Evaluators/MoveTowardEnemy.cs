using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder
    {
        /// <summary>
        /// Move before attacking an enemy with a Post movement Attack
        /// </summary>
        public class MoveTowardEnemy : ConquestScript, ScriptEvaluator
        {
            private bool _AttackAfterMoving;

            public MoveTowardEnemy()
                : base(150, 50, "Move Toward Enemy Conquest", new string[0], new string[1] { "Enemy" })
            {
                _AttackAfterMoving = true;
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                StartMovement();
                Result = new List<object>();
                IsCompleted = true;
            }

            private void StartMovement()
            {
                Tuple<int, int> Target = (Tuple<int, int>)ArrayReferences[0].ReferencedScript.GetContent();
                UnitConquest TargetSquad = Info.Map.ListPlayer[Target.Item1].ListUnit[Target.Item2];

                UnitConquest CurrentActiveUnit = Info.ActiveUnit;
                Vector3 ActiveSquadPosition = Info.ActiveUnit.Position;

                //Define the minimum and maximum value of the attack range.
                int MinRange = Info.ActiveUnit.CurrentAttack.RangeMinimum;
                int MaxRange = Info.ActiveUnit.CurrentAttack.RangeMaximum;
                if (MaxRange > 1)
                    MaxRange += CurrentActiveUnit.Boosts.RangeModifier;

                //Select a target.
                float DistanceUnit = Math.Abs(ActiveSquadPosition.X - TargetSquad.X) + Math.Abs(ActiveSquadPosition.Y - TargetSquad.Y);
                //Move to be in range.

                List<MovementAlgorithmTile> ListRealChoice = Info.Map.GetMVChoice(Info.ActiveUnit);
                for (int M = 0; M < ListRealChoice.Count; M++)
                {//Remove every MV that would make it impossible to attack.
                    float Distance = Math.Abs(ListRealChoice[M].Position.X - TargetSquad.X) + Math.Abs(ListRealChoice[M].Position.Y - TargetSquad.Y);
                    //Check if you can attack it if you moved.
                    if (Distance < MinRange || Distance > MaxRange)
                        ListRealChoice.RemoveAt(M--);
                }

                //Must find a spot to move if got there, just to make sure it won't crash in case of logic error.
                if (ListRealChoice.Count != 0)
                {
                    int Choice = RandomHelper.Next(ListRealChoice.Count);

                    //Movement initialisation.
                    Info.Map.MovementAnimation.Add(Info.ActiveUnit.Components, Info.ActiveUnit.Components.Position, ListRealChoice[Choice].Position);

                    //Prepare the Cursor to move.
                    Info.Map.CursorPosition.X = ListRealChoice[Choice].Position.X;
                    Info.Map.CursorPosition.Y = ListRealChoice[Choice].Position.Y;
                    Info.ActiveUnit.SetPosition(ListRealChoice[Choice].Position);

                    Info.Map.FinalizeMovement(Info.ActiveUnit);
                }
                else
                {
                    //Something is blocking the path.
                    Info.Map.FinalizeMovement(Info.ActiveUnit);
                }
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _AttackAfterMoving = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_AttackAfterMoving);
            }

            public override AIScript CopyScript()
            {
                return new MoveTowardEnemy();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute(true)]
            public bool AttackAfterMoving
            {
                get
                {
                    return _AttackAfterMoving;
                }
                set
                {
                    _AttackAfterMoving = value;
                }
            }
        }
    }
}
