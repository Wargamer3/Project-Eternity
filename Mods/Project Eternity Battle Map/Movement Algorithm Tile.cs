using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MovementAlgorithmTile
    {
        public Vector3 Position;//Z = Height + Layer index + Platform height
        public float Height;
        public int MVEnterCost;//How much energy is required to enter in it.
        public int MVMoveCost;//How much energy is required to move in it.
        public float MovementCost;//How much energy required to move there.
        public int TerrainTypeIndex;//What kind of terrain it is.
        public int LayerIndex;
        public BattleMap Owner;
        public DrawableTile DrawableTile;//Used to know the type of slope.

        public MovementAlgorithmTile ParentTemp;//Temporary variable used for A* and set to null after.
        public MovementAlgorithmTile ParentReal;
    }
}
