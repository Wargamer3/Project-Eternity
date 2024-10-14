using System.Collections.Generic;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class DelayedAttack
    {
        public SorcererStreetUnit Owner;
        public int PlayerIndex;//Only decrement TurnsRemaining if the current player index correspond
        public int TurnsRemaining;
        public List<MovementAlgorithmTile> ListAttackPosition;
        public Tile3DHolder DrawableAttackPosition3D;
        public Tile3DHolder DrawableAttackExplosionPosition3D;
        public MAPAttackAttributes MAPAttributes;
        public ExplosionOptions ExplosionOption;

        public DelayedAttack(SorcererStreetUnit Owner, int PlayerIndex, List<MovementAlgorithmTile> ListAttackPosition)
        {
            this.Owner = Owner;
            this.PlayerIndex = PlayerIndex;
            TurnsRemaining = MAPAttributes.Delay;

            this.ListAttackPosition = new List<MovementAlgorithmTile>(ListAttackPosition.Count);
            foreach (MovementAlgorithmTile ActivePosition in ListAttackPosition)
            {
                this.ListAttackPosition.Add(ActivePosition);
            }
        }
    }
}
