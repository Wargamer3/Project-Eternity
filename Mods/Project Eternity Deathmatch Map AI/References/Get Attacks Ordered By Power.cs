using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetAttacksOrderedByPower : DeathmatchScript, ScriptReference
        {
            public GetAttacksOrderedByPower()
                : base(100, 50, "Get Attacks Ordered By Power", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                IEnumerable<Attack> ListAttackOrderedByPower = Info.ActiveSquad.CurrentLeader.ListAttack.OrderByDescending(Attack => Attack.GetPower(Info.ActiveSquad.CurrentLeader, Info.Map.Params.ActiveParser));
                List<object> ListAttack = new List<object>();
                foreach (Attack ActiveAttack in ListAttackOrderedByPower)
                {
                    ListAttack.Add(ActiveAttack);
                }

                return ListAttack;
            }

            public override AIScript CopyScript()
            {
                return new GetAttacksOrderedByPower();
            }
        }
    }
}
