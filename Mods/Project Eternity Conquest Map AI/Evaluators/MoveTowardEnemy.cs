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
                Info.Map.CursorPosition = Info.ActiveUnit.Position;
                Info.Map.CursorPositionVisible = Info.Map.CursorPosition;

                StartMovement();
                Result = new List<object>();
                IsCompleted = true;
                if (_AttackAfterMoving)
                {
                    Info.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAIAttackBehavior(
                        Info.Map,
                        Info.Map.ActivePlayerIndex,
                        Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListUnit.IndexOf(Info.ActiveUnit),
                        (Tuple<int, int>)ArrayReferences[0].ReferencedScript.GetContent()));

                    IsCompleted = false;
                    Result = new List<object>() { "break" };
                }
            }

            private void StartMovement()
            {
                Tuple<int, int> Target = (Tuple<int, int>)ArrayReferences[0].ReferencedScript.GetContent();
                UnitConquest TargetSquad = Info.Map.ListPlayer[Target.Item1].ListUnit[Target.Item2];

                UnitConquest CurrentActiveUnit = Info.ActiveUnit;
                Vector3 ActiveUnitPosition = Info.ActiveUnit.Position;

                //Define the minimum and maximum value of the attack range.
                int MinRange = Info.ActiveUnit.CurrentAttack.RangeMinimum;
                int MaxRange = Info.ActiveUnit.CurrentAttack.RangeMaximum;
                if (MaxRange > 1)
                    MaxRange += CurrentActiveUnit.Boosts.RangeModifier;

                //Select a target.
                Info.Map.TargetPlayerIndex = Target.Item1;
                Info.Map.TargetSquadIndex = Target.Item2;
                float DistanceUnit = Math.Abs(ActiveUnitPosition.X - TargetSquad.X) + Math.Abs(ActiveUnitPosition.Y - TargetSquad.Y);
                //Move to be in range.

                List<Vector3> ListRealChoice = new List<Vector3>();
                List<MovementAlgorithmTile> ListMVTile = Info.Map.GetMVChoice(Info.ActiveUnit, Info.Map);
                foreach (MovementAlgorithmTile ActiveTerrain in ListMVTile)
                {
                    ListRealChoice.Add(new Vector3(ActiveTerrain.WorldPosition.X, ActiveTerrain.WorldPosition.Y, ActiveTerrain.LayerIndex));
                }
                for (int M = 0; M < ListRealChoice.Count; M++)
                {//Remove every MV that would make it impossible to attack.
                    float Distance = Math.Abs(ListRealChoice[M].X - TargetSquad.X) + Math.Abs(ListRealChoice[M].Y - TargetSquad.Y);
                    //Check if you can attack it if you moved.
                    if (Distance < MinRange || Distance > MaxRange)
                        ListRealChoice.RemoveAt(M--);
                }

                ListRealChoice = FilterMVChoice(ListRealChoice);
                //Must find a spot to move if got there, just to make sure it won't crash in case of logic error.
                if (ListRealChoice.Count != 0)
                {
                    int Choice = RandomHelper.Next(ListRealChoice.Count);

                    //Movement initialisation.
                    Info.Map.MovementAnimation.Add(Info.ActiveUnit.Components, Info.ActiveUnit.Position, ListRealChoice[Choice]);

                    //Prepare the Cursor to move.
                    Info.Map.CursorPosition.X = ListRealChoice[Choice].X;
                    Info.Map.CursorPosition.Y = ListRealChoice[Choice].Y;
                    Info.ActiveUnit.SetPosition(ListRealChoice[Choice]);

                    Info.Map.FinalizeMovement(Info.ActiveUnit, (int)Info.Map.GetTerrain(Info.ActiveUnit.Components).MovementCost, new List<Vector3>());
                }
                else
                {
                    //Something is blocking the path.
                    Info.Map.FinalizeMovement(Info.ActiveUnit, 1, new List<Vector3>());
                }
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _AttackAfterMoving = BR.ReadBoolean();
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
