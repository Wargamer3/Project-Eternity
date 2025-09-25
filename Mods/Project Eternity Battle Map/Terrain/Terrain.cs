using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public enum TerrainBonus { HPRegen, ENRegen, HPRestore, ENRestore, Armor, Accuracy, Evasion };//Regen = %, Restore = #.

    public enum TerrainActivation { OnEveryTurns, OnThisTurn, OnNextTurn, OnEnter, OnLeaved, OnAttack, OnHit, OnMiss, OnDefend, OnHited, OnMissed };

    public class DeathmatchTerrainBonusInfo
    {
        public TerrainActivation[] ListActivation;//Activation type of the bonuses.
        public TerrainBonus[] ListBonus;//Bonuses the terrain can give.
        public int[] ListBonusValue;//Value of the bonuses.

        public DeathmatchTerrainBonusInfo()
        {
            ListActivation = new TerrainActivation[0];
            ListBonus = ListBonus = new TerrainBonus[0];
            ListBonusValue = ListBonusValue = new int[0];
        }

        public DeathmatchTerrainBonusInfo(DeathmatchTerrainBonusInfo Other)
        {
            ListActivation = (TerrainActivation[])Other.ListActivation.Clone();
            ListBonus = (TerrainBonus[])Other.ListBonus.Clone();
            ListBonusValue = (int[])Other.ListBonusValue.Clone();
        }
    }

    public class Terrain : MovementAlgorithmTile
    {
        public byte BattleBackgroundAnimationIndex;
        public byte BattleForegroundAnimationIndex;
        public DeathmatchTerrainBonusInfo BonusInfo;

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public Terrain(Terrain Other, Point Position, int LayerIndex)
            : base(Position.X, Position.Y, LayerIndex, Other.LayerDepth)
        {
            this.Height = Other.Height;
            this.WorldPosition = Other.WorldPosition;
            this.TerrainTypeIndex = Other.TerrainTypeIndex;
            this.PreventLeavingUpward = Other.PreventLeavingUpward;
            this.PreventLeavingDownward = Other.PreventLeavingDownward;
            this.PreventLeavingLeft = Other.PreventLeavingLeft;
            this.PreventLeavingRight = Other.PreventLeavingRight;
            BattleBackgroundAnimationIndex = Other.BattleBackgroundAnimationIndex;
            BattleForegroundAnimationIndex = Other.BattleForegroundAnimationIndex;
            BonusInfo = new DeathmatchTerrainBonusInfo();
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public Terrain(int GridPosX, int GridPosY, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(GridPosX, GridPosY, LayerIndex, LayerDepth)
        {
            WorldPosition = new Vector3(GridPosX * TileSizeX, GridPosY * TileSizeY, LayerIndex * LayerHeight);
            TerrainTypeIndex = 0;
            BattleBackgroundAnimationIndex = 0;
            BattleForegroundAnimationIndex = 0;
            BonusInfo = new DeathmatchTerrainBonusInfo();
        }

        /// <summary>
        /// Create a new Tile with the index of the tile set and it's origin in it along with it's attribtues.
        /// </summary>
        /// <param name="TileSet">Which Tile Set to use from the ListTile.</param>
        /// <param name="Origin">X, Y origin from at which the tile is located in the TileSet.</param>
        /// <param name="TerrainTypeIndex">What kind of terrain it is.</param>
        /// <param name="MVEnterCost">How much energy is required to enter in it.</param>
        /// <param name="MVMoveCost">How much energy is required to move in it.</param>
        /// <param name="ListActivation">Activation type of the bonuses.</param>
        /// <param name="ListBonus">Bonuses the terrain can give.</param>
        /// <param name="ListBonusValue">//Value of the bonuses.</param>
        public Terrain(int GridPosX, int GridPosY, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex,
            TerrainActivation[] ListActivation, TerrainBonus[] ListBonus, int[] ListBonusValue)
            : this(GridPosX, GridPosY, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
            BonusInfo.ListActivation = ListActivation;
            BonusInfo.ListBonus = ListBonus;
            BonusInfo.ListBonusValue = ListBonusValue;
        }

        public Terrain(BinaryReader BR, int GridPosX, int GridPosY, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : this(GridPosX, GridPosY, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            TerrainTypeIndex = BR.ReadByte();
            Height = BR.ReadSingle();

            PreventLeavingUpward = BR.ReadBoolean();
            PreventLeavingDownward = BR.ReadBoolean();
            PreventLeavingLeft = BR.ReadBoolean();
            PreventLeavingRight = BR.ReadBoolean();

            int ArrayBonusLength = BR.ReadInt32();
            BonusInfo.ListBonus = new TerrainBonus[ArrayBonusLength];
            BonusInfo.ListActivation = new TerrainActivation[ArrayBonusLength];
            BonusInfo.ListBonusValue = new int[ArrayBonusLength];

            for (int i = 0; i < ArrayBonusLength; i++)
            {
                BonusInfo.ListBonus[i] = (TerrainBonus)BR.ReadInt32();
                BonusInfo.ListActivation[i] = (TerrainActivation)BR.ReadInt32();
                BonusInfo.ListBonusValue[i] = BR.ReadInt32();
            }

            BattleBackgroundAnimationIndex = BR.ReadByte();
            BattleForegroundAnimationIndex = BR.ReadByte();
        }

        public virtual void Save(BinaryWriter BW)
        {
            BW.Write(TerrainTypeIndex);
            BW.Write(Height);

            BW.Write(PreventLeavingUpward);
            BW.Write(PreventLeavingDownward);
            BW.Write(PreventLeavingLeft);
            BW.Write(PreventLeavingRight);

            BW.Write(BonusInfo.ListBonus.Length);
            for (int i = 0; i < BonusInfo.ListBonus.Length; i++)
            {
                BW.Write((int)BonusInfo.ListBonus[i]);
                BW.Write((int)BonusInfo.ListActivation[i]);
                BW.Write(BonusInfo.ListBonusValue[i]);
            }

            BW.Write(BattleBackgroundAnimationIndex);
            BW.Write(BattleForegroundAnimationIndex);
        }
    }
}
