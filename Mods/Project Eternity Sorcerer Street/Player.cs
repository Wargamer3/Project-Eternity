using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Player
    {
        public string Name;
        public string PlayerType;
        public bool IsHuman;
        public bool IsOnline;
        public int Team;
        public SorcererStreetUnit GamePiece;
        public int Rank;//Rank in the game between players
        public int Magic;
        public int TotalMagic;
        public int CompletedLaps;
        public List<SorcererStreetMap.Checkpoints> ListPassedCheckpoint;
        public Dictionary<byte, byte> DicChainLevelByTerrainTypeIndex;
        public bool IsPlayerControlled;
        public Color Color;
        public readonly Card[] ArrayCardInDeck;
        public readonly List<Card> ListCardInHand;
        public readonly List<Card> ListRemainingCardInDeck;

        public Player(string Name, string PlayerType, bool IsHuman, bool IsOnline, int Team, Card[] ArrayCardInDeck)
        {
            this.Name = Name;
            this.PlayerType = PlayerType;
            this.IsHuman = IsHuman;
            this.IsOnline = IsOnline;
            this.Team = Team;
            this.ArrayCardInDeck = ArrayCardInDeck;

            if (Team == 0)
            {
                Color = Color.Blue;
            }
            else
            {
                Color = Color.Red;
            }

            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicChainLevelByTerrainTypeIndex = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = Core.Units.UnitMapComponent.Directions.None;
            ListRemainingCardInDeck = new List<Card>(ArrayCardInDeck);
            ListCardInHand = new List<Card>();
        }

        public void IncreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicChainLevelByTerrainTypeIndex.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicChainLevelByTerrainTypeIndex.Add(TerrainTypeIndex, 1);
            }
            else
            {
                DicChainLevelByTerrainTypeIndex[TerrainTypeIndex] = (byte)(ChainValue + 1);
            }
        }

        public void DecreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicChainLevelByTerrainTypeIndex.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicChainLevelByTerrainTypeIndex.Add(TerrainTypeIndex, 0);
            }
            else
            {
                DicChainLevelByTerrainTypeIndex[TerrainTypeIndex] = (byte)(ChainValue - 1);
            }
        }
    }
}
