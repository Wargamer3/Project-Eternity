using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MovementAlgorithmTile
    {
        public Vector3 WorldPosition;//Z = Height + Layer index + Platform height, affected by World Matrix and used for cross Map checks.
        public readonly Point InternalPosition;//Position not affected by World Matrix, will be the same as WorldPosition if not a platform.
        public readonly int LayerIndex;
        public float LayerDepth;//Drawn Depth
        public float Height;
        public bool PreventLeavingUpward;
        public bool PreventLeavingDownward;
        public bool PreventLeavingLeft;
        public bool PreventLeavingRight;
        public float MovementCost;//How much energy required to move there.
        public byte TerrainTypeIndex;//What kind of terrain it is.
        public BattleMap Owner;
        public DrawableTile DrawableTile;//Used to know the type of slope.

        public MovementAlgorithmTile ParentTemp;//Temporary variable used for A* and set to null after.
        public MovementAlgorithmTile ParentReal;

        protected MovementAlgorithmTile(int XPos, int YPos, int LayerIndex, float LayerDepth)
        {
            this.InternalPosition = new Point(XPos, YPos);
            this.LayerIndex = LayerIndex;
            this.LayerDepth = LayerDepth;
        }

        public Vector3 GetRealPosition(Vector3 Position)
        {
            Vector2 PositionInTile = new Vector2(Position.X - InternalPosition.X, Position.Y - InternalPosition.Y);

            return WorldPosition + new Vector3(PositionInTile, DrawableTile.Terrain3DInfo.GetZOffset(PositionInTile, Height));
        }
    }
}
