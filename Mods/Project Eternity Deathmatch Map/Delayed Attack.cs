using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DelayedAttack
    {
        public Attack ActiveAttack;
        public Squad Owner;
        public int PlayerIndex;//Only decrement TurnsRemaining if the current player index correspond
        public int TurnsRemaining;
        public List<Vector3> ListAttackPosition;

        public DelayedAttack(Attack ActiveAttack, Squad Owner, int PlayerIndex, List<Vector3> ListAttackPosition)
        {
            this.ActiveAttack = ActiveAttack;
            this.Owner = Owner;
            this.PlayerIndex = PlayerIndex;
            TurnsRemaining = ActiveAttack.MAPAttributes.Delay;

            this.ListAttackPosition = new List<Vector3>(ListAttackPosition.Count);
            foreach (Vector3 ActivePosition in ListAttackPosition)
            {
                this.ListAttackPosition.Add(ActivePosition);
            }
        }
    }
}
