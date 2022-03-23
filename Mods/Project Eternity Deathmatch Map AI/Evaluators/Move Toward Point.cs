using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class MoveTowardPoint : DeathmatchScript, ScriptEvaluator
        {
            private int _MinDistanceFromPoint;
            private bool _AttackAfterMove;

            public MoveTowardPoint()
                : base(150, 50, "Move Toward Point", new string[0], new string[1] { "Point" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Vector3 Target = (Vector3)ArrayReferences[0].ReferencedScript.GetContent();
                List<MovementAlgorithmTile> ListMovement = Info.Map.GetMVChoicesTowardPoint(Info.ActiveSquad, Target, false);
                List<MovementAlgorithmTile> ListMVChoice = Info.Map.GetMVChoice(Info.ActiveSquad);

                if (ListMovement.Count == 0)
                {
                    ListMovement.Add(ListMVChoice[RandomHelper.Next(ListMVChoice.Count)]);
                }

                MovementAlgorithmTile FinalTile = ListMovement[ListMovement.Count - 1];
                Vector3 FinalTilePosition = new Vector3(FinalTile.WorldPosition.X, FinalTile.WorldPosition.Y, FinalTile.LayerIndex);

                if (FinalTilePosition == Target && _MinDistanceFromPoint > 0)
                {
                    ListMovement.Clear();
                    List<MovementAlgorithmTile> ListFinalChoice = new List<MovementAlgorithmTile>(ListMVChoice);

                    for (int M = ListFinalChoice.Count - 1; M >= 0; M--)
                    {
                        float Distance = (Math.Abs(ListFinalChoice[M].WorldPosition.X - Target.X) + Math.Abs(ListFinalChoice[M].WorldPosition.Y - Target.Y));
                        if (Distance != _MinDistanceFromPoint)
                        {
                            ListFinalChoice.RemoveAt(M);
                        }
                    }

                    foreach (MovementAlgorithmTile ActiveSquadTile in Info.Map.GetAllTerrain(Info.ActiveSquad))
                    {
                        ListFinalChoice.Remove(ActiveSquadTile);
                    }

                    FinalTile = ListFinalChoice[RandomHelper.Next(ListFinalChoice.Count)];
                    while (FinalTile != null)
                    {
                        if (ListMovement.Contains(FinalTile.ParentReal))
                        {
                            break;
                        }
                        if (Info.ActiveSquad.Position.X == FinalTile.WorldPosition.X && Info.ActiveSquad.Position.Y == FinalTile.WorldPosition.Y && Info.ActiveSquad.Position.Z == FinalTile.LayerIndex)
                        {
                            ListMovement.Add(FinalTile);
                            break;
                        }

                        ListMovement.Add(FinalTile);

                        FinalTile = FinalTile.ParentReal;
                    }

                    ListMovement.Reverse();
                }

                if (_AttackAfterMove)
                {
                    Info.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAIMoveBehavior(
                        Info.Map,
                        Info.Map.ActivePlayerIndex,
                        Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad),
                        ListMovement, ListMVChoice,
                        new ActionPanelAIAttackBehavior(Info.Map, Info.Map.ActivePlayerIndex, Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad))));
                }
                else
                {
                    Info.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAIMoveBehavior(
                        Info.Map,
                        Info.Map.ActivePlayerIndex,
                        Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad),
                        ListMovement, ListMVChoice));
                }

                IsCompleted = false;
                Result = new List<object>() { "break" };
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _AttackAfterMove = BR.ReadBoolean();
                _MinDistanceFromPoint = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_AttackAfterMove);
                BW.Write(_MinDistanceFromPoint);
            }

            public override AIScript CopyScript()
            {
                return new MoveTowardPoint();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public bool AttackAfterMove
            {
                get
                {
                    return _AttackAfterMove;
                }
                set
                {
                    _AttackAfterMove = value;
                }
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public int MinDistanceFromPoint
            {
                get
                {
                    return _MinDistanceFromPoint;
                }
                set
                {
                    _MinDistanceFromPoint = value;
                }
            }
        }
    }
}
