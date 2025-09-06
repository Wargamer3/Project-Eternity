using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MovementAlgorithmTile
    {
        public Vector3 WorldPosition;//Z = Height + Layer index + Platform height, affected by World Matrix and used for cross Map checks. GridPosition * Tile Size * World
        public readonly Point GridPosition;//Position not affected by World Matrix, will be the same as WorldPosition if not a platform.
        public readonly int LayerIndex;
        public float LayerDepth;//Drawn Depth
        public float Height;//Grid Value
        public bool PreventLeavingUpward;
        public bool PreventLeavingDownward;
        public bool PreventLeavingLeft;
        public bool PreventLeavingRight;
        public float MovementCost;//How much energy required to move there.
        public byte TerrainTypeIndex;//What kind of terrain it is.
        public BattleMap Owner;

        public MovementAlgorithmTile ParentTemp;//Temporary variable used for A* and set to null after.
        public MovementAlgorithmTile ParentReal;

        protected MovementAlgorithmTile(int XPos, int YPos, int LayerIndex, float LayerDepth)
        {
            this.GridPosition = new Point(XPos, YPos);
            this.LayerIndex = LayerIndex;
            this.LayerDepth = LayerDepth;
        }
    }
}
