using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MovementAlgorithmTile
    {
        public Vector3 Position;
        public int MVEnterCost;//How much energy is required to enter in it.
        public int MVMoveCost;//How much energy is required to move in it.
        public float MovementCost;//How much energy required to move there.
        public int TerrainTypeIndex;//What kind of terrain it is.

        public MovementAlgorithmTile Parent;//Used for A*.
    }
}
