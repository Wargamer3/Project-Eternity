using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class TerrainSorcererStreet : Terrain
    {
        public const string FireElement = "Fire";
        public const string WaterElement = "Water";
        public const string EarthElement = "Earth";
        public const string AirElement = "Air";
        public const string EastGate = "East Gate";
        public const string WestGate = "Weast Gate";
        public const string SouthGate = "South Gate";
        public const string NorthGate = "North Gate";

        public CreatureCard DefendingCreature;
        public Player Owner;
        public int TerrainLevel;

        public TerrainSorcererStreet(Terrain Other)
            : base(Other)
        {
            TerrainTypeIndex = Other.TerrainTypeIndex;
            TerrainLevel = 0;
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public TerrainSorcererStreet(int XPos, int YPos, int LayerIndex)
            : base(XPos, YPos, LayerIndex)
        {
            TerrainTypeIndex = 0;
            MVMoveCost = 1;
            TerrainLevel = 0;
        }
        
        public TerrainSorcererStreet(int XPos, int YPos, int LayerIndex, int TerrainTypeIndex)
            : base(XPos, YPos, LayerIndex)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
            TerrainLevel = 0;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(TerrainTypeIndex);
        }

        public virtual void OnSelect(SorcererStreetMap Map, int ActivePlayerIndex)
        {

        }
    }
}
