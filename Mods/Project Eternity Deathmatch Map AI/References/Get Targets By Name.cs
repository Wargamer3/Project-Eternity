using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetTargetsByName : DeathmatchScript, ScriptReference
        {
            private string _EnemyName;

            private bool _AddMovementToRange;

            public GetTargetsByName()
                : base(100, 50, "Get Targets By Name", new string[0], new string[1] { "Leader Attack" })
            {
                _AddMovementToRange = false;
            }

            public object GetContent()
            {
                Attack ActiveAttack = (Attack)ArrayReferences[0].ReferencedScript.GetContent();

                List<object> ListEnemy = new List<object>();

                bool CanAttackPostMovement = ActiveAttack.IsPostMovement(Info.ActiveSquad.CurrentLeader);

                if (!Info.ActiveSquad.CanMove && !CanAttackPostMovement)
                {
                    return ListEnemy;
                }

                Info.ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;

                //Define the minimum and maximum value of the attack range.
                int MinRange = ActiveAttack.RangeMinimum;
                int MaxRange = ActiveAttack.RangeMaximum;
                if (MaxRange > 1)
                    MaxRange += Info.ActiveSquad.CurrentLeader.Boosts.RangeModifier;

                int SquadMovement = 0;

                if (_AddMovementToRange)
                {
                    SquadMovement = Info.Map.GetSquadMaxMovement(Info.ActiveSquad);
                }

                Info.Map.UpdateAllAttacks(Info.ActiveSquad.CurrentLeader, Info.ActiveSquad.Position,
                    Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].Team, Info.ActiveSquad.CanMove & _AddMovementToRange);

                var ListTargetUnit = Info.Map.CanSquadAttackWeapon(Info.ActiveSquad, Info.ActiveSquad.Position,
                    ActiveAttack, MinRange, MaxRange, _AddMovementToRange, Info.ActiveSquad.CurrentLeader);

                //Priority goes to units with higher chances of hitting.
                IOrderedEnumerable<Tuple<int, int>> ListHitRate = ListTargetUnit.OrderByDescending(Target =>
                    Info.Map.CalculateHitRate(Info.ActiveSquad.CurrentLeader, Info.ActiveSquad,
                    Info.Map.ListPlayer[Target.Item1].ListSquad[Target.Item2].CurrentLeader, Info.Map.ListPlayer[Target.Item1].ListSquad[Target.Item2],
                    Unit.BattleDefenseChoices.Attack));


                foreach (Tuple<int, int> Target in ListHitRate)
                {
                    if (Info.Map.ListPlayer[Target.Item1].ListSquad[Target.Item2].CurrentLeader.RelativePath == _EnemyName)
                        ListEnemy.Add(Target);
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
                return new GetTargetsByName();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string EnemyName
            {
                get
                {
                    return _EnemyName;
                }
                set
                {
                    _EnemyName = value;
                }
            }

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
        }
    }
}
