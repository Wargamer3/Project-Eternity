using System;
using System.Collections.Generic;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class DelayedAttack
    {
        public Attack ActiveAttack;
        public UnitConquest Owner;
        public int PlayerIndex;//Only decrement TurnsRemaining if the current player index correspond
        public int TurnsRemaining;
        public List<MovementAlgorithmTile> ListAttackPosition;
        public Tile3DHolder DrawableAttackPosition3D;
        public Tile3DHolder DrawableAttackExplosionPosition3D;

        public DelayedAttack(Attack ActiveAttack, UnitConquest Owner, int PlayerIndex, List<MovementAlgorithmTile> ListAttackPosition)
        {
            this.ActiveAttack = ActiveAttack;
            this.Owner = Owner;
            this.PlayerIndex = PlayerIndex;
            TurnsRemaining = ActiveAttack.MAPAttributes.Delay;

            this.ListAttackPosition = new List<MovementAlgorithmTile>(ListAttackPosition.Count);
            foreach (MovementAlgorithmTile ActivePosition in ListAttackPosition)
            {
                this.ListAttackPosition.Add(ActivePosition);
            }
        }
    }
}
