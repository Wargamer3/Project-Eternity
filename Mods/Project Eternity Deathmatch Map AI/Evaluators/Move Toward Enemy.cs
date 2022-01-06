using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        /// <summary>
        /// Move before attacking an enemy with a Post movement Attack
        /// </summary>
        public class MoveTowardEnemy : DeathmatchScript, ScriptEvaluator
        {
            private bool _AttackAfterMoving;

            public MoveTowardEnemy()
                : base(150, 50, "Move Toward Enemy", new string[0], new string[1] { "Enemy" })
            {
                _AttackAfterMoving = true;
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                StartMovement();
                Result = new List<object>();
                IsCompleted = true;
                if (_AttackAfterMoving)
                {
                    Info.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAIAttackBehavior(
                        Info.Map,
                        Info.Map.ActivePlayerIndex,
                        Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad),
                        (Tuple<int, int>)ArrayReferences[0].ReferencedScript.GetContent()));

                    IsCompleted = false;
                    Result = new List<object>() { "break" };
                }
            }

            private void StartMovement()
            {
                Tuple<int, int> Target = (Tuple<int, int>)ArrayReferences[0].ReferencedScript.GetContent();
                Squad TargetSquad = Info.Map.ListPlayer[Target.Item1].ListSquad[Target.Item2];

                Unit CurrentActiveUnit = Info.ActiveSquad.CurrentLeader;
                Vector3 ActiveSquadPosition = Info.ActiveSquad.Position;

                //Define the minimum and maximum value of the attack range.
                int MinRange = Info.ActiveSquad.CurrentLeader.CurrentAttack.RangeMinimum;
                int MaxRange = Info.ActiveSquad.CurrentLeader.CurrentAttack.RangeMaximum;
                if (MaxRange > 1)
                    MaxRange += CurrentActiveUnit.Boosts.RangeModifier;

                //Select a target.
                Info.Map.TargetPlayerIndex = Target.Item1;
                Info.Map.TargetSquadIndex = Target.Item2;
                float DistanceUnit = Math.Abs(ActiveSquadPosition.X - TargetSquad.X) + Math.Abs(ActiveSquadPosition.Y - TargetSquad.Y);
                //Move to be in range.

                List<Vector3> ListRealChoice = new List<Vector3>();
                foreach (MovementAlgorithmTile ActiveTerrain in Info.Map.GetMVChoice(Info.ActiveSquad))
                {
                    ListRealChoice.Add(new Vector3(ActiveTerrain.Position.X, ActiveTerrain.Position.Y, ActiveTerrain.LayerIndex));
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
                    Info.Map.MovementAnimation.Add(Info.ActiveSquad.X, Info.ActiveSquad.Y, Info.ActiveSquad);

                    //Prepare the Cursor to move.
                    Info.Map.CursorPosition.X = ListRealChoice[Choice].X;
                    Info.Map.CursorPosition.Y = ListRealChoice[Choice].Y;
                    Info.ActiveSquad.SetPosition(ListRealChoice[Choice]);

                    Info.Map.FinalizeMovement(Info.ActiveSquad, (int)Info.Map.GetTerrain(Info.ActiveSquad).MovementCost);
                }
                else
                {
                    //Something is blocking the path.
                    Info.Map.FinalizeMovement(Info.ActiveSquad, 1);
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
