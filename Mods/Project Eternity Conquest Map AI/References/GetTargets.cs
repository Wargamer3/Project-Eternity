using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder
    {
        public class GetTargets : ConquestScript, ScriptReference
        {
            private bool _AddMovementToRange;

            public GetTargets()
                : base(150, 50, "Get Targets Conquest", new string[0], new string[0])
            {
                _AddMovementToRange = false;
            }

            public object GetContent()
            {
                List<object> ListEnemy = new List<object>();

                for (int i = 0; i < Info.ActiveUnit.ListAttack.Count && ListEnemy.Count == 0; ++i)
                {
                    Attack ActiveAttack = Info.ActiveUnit.ListAttack[i];
                    Info.ActiveUnit.CurrentAttack = ActiveAttack;

                    bool CanAttackPostMovement = ActiveAttack.IsPostMovement(Info.ActiveUnit);

                    List<MovementAlgorithmTile> ListMVChoice;

                    if (_AddMovementToRange && CanAttackPostMovement)
                    {
                        ListMVChoice = Info.Map.GetMVChoice(Info.ActiveUnit);
                    }
                    else
                    {
                        ListMVChoice = new List<MovementAlgorithmTile>();
                        ListMVChoice.Add(Info.Map.GetTerrain(Info.ActiveUnit.Components));
                    }

                    if (i == 0)
                    {
                        //Remove everything that is closer then DistanceMax.
                        for (int M = 0; M < ListMVChoice.Count; M++)
                        {
                            List<Tuple<int, int>> ListTargetUnit = Info.Map.CanSquadAttackWeapon1((int)ListMVChoice[M].Position.X, (int)ListMVChoice[M].Position.Y, Info.Map.ActivePlayerIndex, Info.ActiveUnit.ArmourType.ToString(),
                                ActiveAttack);

                            foreach (Tuple<int, int> Target in ListTargetUnit)
                            {
                                ListEnemy.Add(Target);
                            }
                        }
                    }
                    else if (i == 1)
                    {
                        //Remove everything that is closer then DistanceMax.
                        for (int M = 0; M < ListMVChoice.Count; M++)
                        {
                            List<Tuple<int, int>> ListTargetUnit = Info.Map.CanSquadAttackWeapon2((int)ListMVChoice[M].Position.X, (int)ListMVChoice[M].Position.Y, Info.Map.ActivePlayerIndex, Info.ActiveUnit.ArmourType.ToString(),
                                ActiveAttack);

                            foreach (Tuple<int, int> Target in ListTargetUnit)
                            {
                                ListEnemy.Add(Target);
                            }
                        }
                    }
                }

                return ListEnemy;
            }
            
            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _AddMovementToRange = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_AddMovementToRange);
            }

            public override AIScript CopyScript()
            {
                return new GetTargets();
            }

            #region Properties

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("Add Movement To Range"),
            DefaultValueAttribute("")]
            public bool AddMovementToRange
            {
                get
                {
                    return _AddMovementToRange;
                }
                set
                {
                    _AddMovementToRange = value;
                }
            }

            #endregion
        }
    }
}
