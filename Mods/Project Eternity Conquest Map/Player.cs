using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public delegate void PlayerStepDelegate(GameTime gameTime);

    public delegate void PlayerDrawDelegate(CustomSpriteBatch g);

    // Special EventArgs class to hold info about Shapes.
    public class NewPhaseEventArgs : EventArgs { }

    public class Player
    {
        public string Name;
        public Character CommandingOfficer;
        public string PlayerType;
        public bool IsHuman;
        public bool IsOnline;
        public List<UnitConquest> ListUnit;
        public List<EventPoint> ListSpawnPoint;
        public int Team;
        public Color Color;
        public bool IsAlive;//If the player can play (always true if it still have any active units).
        public PlayerStepDelegate PlayerStep;
        public PlayerDrawDelegate PlayerDraw;

        public Player(string Name, string PlayerType, bool IsHuman, bool IsOnline, int Team, Color Color)
        {
            this.ListUnit = new List<UnitConquest>();
            ListSpawnPoint = new List<EventPoint>();
            this.Name = Name;
            this.PlayerType = PlayerType;
            this.IsHuman = IsHuman;
            this.IsOnline = IsOnline;
            this.Team = Team;
            this.Color = Color;
            this.IsAlive = false;
        }
    }
}
