﻿using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MovementAlgorithmTile
    {
        public Vector3 WorldPosition;//Z = Height + Layer index + Platform height, affected by World Matrix and used for cross Map checks.
        public readonly Point InternalPosition;//Position not affected by World Matrix, will be the same as WorldPosition if not a platform.
        public readonly int LayerIndex;
        public float Height;
        public int MVEnterCost;//How much energy is required to enter in it.
        public int MVMoveCost;//How much energy is required to move in it.
        public float MovementCost;//How much energy required to move there.
        public int TerrainTypeIndex;//What kind of terrain it is.
        public BattleMap Owner;
        public DrawableTile DrawableTile;//Used to know the type of slope.

        public MovementAlgorithmTile ParentTemp;//Temporary variable used for A* and set to null after.
        public MovementAlgorithmTile ParentReal;

        protected MovementAlgorithmTile(int XPos, int YPos, int LayerIndex)
        {
            this.InternalPosition = new Point(XPos, YPos);
            this.LayerIndex = LayerIndex;
        }
    }
}
