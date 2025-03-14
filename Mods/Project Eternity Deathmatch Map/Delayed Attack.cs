﻿using System.Collections.Generic;
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
        public List<MovementAlgorithmTile> ListAttackPosition;
        public Tile3DHolder DrawableAttackPosition3D;
        public Tile3DHolder DrawableAttackExplosionPosition3D;

        public DelayedAttack(Attack ActiveAttack, Squad Owner, int PlayerIndex, List<MovementAlgorithmTile> ListAttackPosition)
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
