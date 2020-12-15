using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Attacks;
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

                for (int i = 0; i < Info.ActiveSquad.ListAttack.Count && ListEnemy.Count == 0; ++i)
                {
                    Attack ActiveAttack = Info.ActiveSquad.ListAttack[i];
                    Info.ActiveSquad.AttackIndex = i;

                    bool CanAttackPostMovement = (ActiveAttack.Sec & WeaponSecondaryProperty.PostMovement) == WeaponSecondaryProperty.PostMovement || Info.ActiveSquad.Boosts.PostMovementModifier.Attack;

                    List<Vector3> ListMVChoice;

                    if (_AddMovementToRange && CanAttackPostMovement)
                    {
                        ListMVChoice = Info.Map.GetMVChoice(Info.ActiveSquad);
                    }
                    else
                    {
                        ListMVChoice = new List<Vector3>();
                        ListMVChoice.Add(Info.ActiveSquad.Position);
                    }

                    if (i == 0)
                    {
                        //Remove everything that is closer then DistanceMax.
                        for (int M = 0; M < ListMVChoice.Count; M++)
                        {
                            List<Tuple<int, int>> ListTargetUnit = Info.Map.CanSquadAttackWeapon1((int)ListMVChoice[M].X, (int)ListMVChoice[M].Y, Info.Map.ActivePlayerIndex, Info.ActiveSquad.ArmourType.ToString(),
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
                            List<Tuple<int, int>> ListTargetUnit = Info.Map.CanSquadAttackWeapon2((int)ListMVChoice[M].X, (int)ListMVChoice[M].Y, Info.Map.ActivePlayerIndex, Info.ActiveSquad.ArmourType.ToString(),
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
