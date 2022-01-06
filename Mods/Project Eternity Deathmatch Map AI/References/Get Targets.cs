using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetTargets : DeathmatchScript, ScriptReference
        {
            private bool _AddMovementToRange;

            public GetTargets()
                : base(100, 50, "Get Targets", new string[0], new string[1] { "Leader Attack" })
            {
                _AddMovementToRange = false;
            }

            public object GetContent()
            {
                Attack ActiveAttack = (Attack)ArrayReferences[0].ReferencedScript.GetContent();

                List<object> ListEnemy = new List<object>();

                bool CanAttackPostMovement = (ActiveAttack.Sec & WeaponSecondaryProperty.PostMovement) == WeaponSecondaryProperty.PostMovement || Info.ActiveSquad.CurrentLeader.Boosts.PostMovementModifier.Attack;
                
                Info.ActiveSquad.CurrentLeader.AttackIndex = Info.ActiveSquad.CurrentLeader.ListAttack.IndexOf(ActiveAttack);

                //Define the minimum and maximum value of the attack range.
                int MinRange = ActiveAttack.RangeMinimum;
                int MaxRange = ActiveAttack.RangeMaximum;
                if (MaxRange > 1)
                    MaxRange += Info.ActiveSquad.CurrentLeader.Boosts.RangeModifier;

                List<MovementAlgorithmTile> ListMVChoice;

                if (_AddMovementToRange && CanAttackPostMovement)
                {
                    ListMVChoice = Info.Map.GetMVChoice(Info.ActiveSquad);
                }
                else
                {
                    ListMVChoice = new List<MovementAlgorithmTile>();
                    ListMVChoice.Add(Info.Map.GetTerrain(Info.ActiveSquad));
                }

                //Remove everything that is closer then DistanceMax.
                for (int M = 0; M < ListMVChoice.Count; M++)
                {
                    List<Tuple<int, int>> ListTargetUnit = Info.Map.CanSquadAttackWeapon(Info.ActiveSquad, new Vector3(ListMVChoice[M].Position.X, ListMVChoice[M].Position.Y, ListMVChoice[M].LayerIndex),
                        ActiveAttack, MinRange, MaxRange, Info.ActiveSquad.CanMove, Info.ActiveSquad.CurrentLeader.Boosts);

                    //Priority goes to units with higher chances of hitting.
                    IOrderedEnumerable<Tuple<int, int>> ListHitRate = ListTargetUnit.OrderByDescending(Target =>
                        Info.Map.CalculateHitRate(Info.ActiveSquad.CurrentLeader, Info.ActiveSquad,
                        Info.Map.ListPlayer[Target.Item1].ListSquad[Target.Item2].CurrentLeader, Info.Map.ListPlayer[Target.Item1].ListSquad[Target.Item2],
                        Unit.BattleDefenseChoices.Attack));

                    foreach (Tuple<int, int> Target in ListHitRate)
                    {
                        if (!ListEnemy.Contains(Target))
                            ListEnemy.Add(Target);
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
