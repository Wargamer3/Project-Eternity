using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Player
    {
        public string Name;
        public string PlayerType;//Only used for multiplayer
        public bool IsHuman;
        public bool IsOnline;
        public List<Squad> ListSquad;
        public List<EventPoint> ListSpawnPoint;
        public int Team;
        public Color Color;
        public bool IsAlive;//If the player can play (always true if it still have any active units).

        public Player(string Name, string PlayerType, bool IsHuman, bool IsOnline, int Team, Color Color)
        {
            this.ListSquad = new List<Squad>();
            ListSpawnPoint = new List<EventPoint>();
            this.Name = Name;
            this.PlayerType = PlayerType;
            this.IsHuman = IsHuman;
            this.IsOnline = IsOnline;
            this.Team = Team;
            this.Color = Color;
            this.IsAlive = false;
        }

        public void UpdateAliveStatus()
        {
            this.IsAlive = false;
            foreach (Squad ActiveSquad in ListSquad)
            {
                if (!ActiveSquad.IsDead)
                {
                    this.IsAlive = true;
                }
            }
        }
    }
}
