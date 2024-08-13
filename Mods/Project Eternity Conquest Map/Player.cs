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

    public class Player : BattleMapPlayer
    {
        public List<UnitConquest> ListUnit;
        public List<Commander> ListCommander;
        public List<EventPoint> ListSpawnPoint;
        public bool IsAlive;//If the player can play (always true if it still have any active units).
        public int Kills;
        public int Death;

        public Character CommandingOfficer;
        public string PlayerType;
        public bool IsHuman;
        public PlayerStepDelegate PlayerStep;
        public PlayerDrawDelegate PlayerDraw;

        public Player(string Name, string PlayerType, bool IsPlayerControlled, bool IsOnline, int Team, Color Color)
            : this("", Name, PlayerType, IsPlayerControlled, IsOnline, Team, Color)
        {
        }

        public Player(string ID, string Name, string PlayerType, bool IsPlayerControlled, bool IsOnline, int Team, Color Color)
            : base(ID, Name, PlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            ListUnit = new List<UnitConquest>();
            ListCommander = new List<Commander>();
            ListSpawnPoint = new List<EventPoint>();
            this.IsPlayerControlled = IsPlayerControlled;
            this.Color = Color;
            this.IsAlive = false;
        }


        public Player(BattleMapPlayer Clone)
            : base(Clone)
        {
            ListUnit = new List<UnitConquest>();
            ListCommander = new List<Commander>();
            ListSpawnPoint = new List<EventPoint>();
            this.IsAlive = false;
        }

        public void UpdateAliveStatus()
        {
            this.IsAlive = false;
            foreach (UnitConquest ActiveUnit in ListUnit)
            {
                if (ActiveUnit.HP > 0)
                {
                    this.IsAlive = true;
                }
            }
        }
    }
}
