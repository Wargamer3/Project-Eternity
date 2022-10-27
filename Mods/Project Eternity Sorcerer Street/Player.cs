using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Player
    {
        public enum Directions { None, Up, Down, Left, Right }

        public string Name;
        public string PlayerType;
        public bool IsHuman;
        public bool IsOnline;
        public int Team;
        public int Rank;//Rank in the game between players
        public SorcererStreetUnit GamePiece;
        public int Magic;
        public Directions CurrentDirection;
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

            CurrentDirection = Directions.None;
            GamePiece = new SorcererStreetUnit();
            ListRemainingCardInDeck = new List<Card>(ArrayCardInDeck);
            ListCardInHand = new List<Card>();
        }
    }
}
