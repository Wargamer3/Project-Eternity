﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Player : BattleMapPlayer
    {
        public List<Squad> ListSquad;
        public List<EventPoint> ListSpawnPoint;
        public bool IsAlive;//If the player can play (always true if it still have any active units).

        public Player(string Name, string PlayerType, bool IsPlayerControlled, bool IsOnline, int Team, Color Color)
            : base("", Name, PlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            this.ListSquad = new List<Squad>();
            ListSpawnPoint = new List<EventPoint>();
            this.IsPlayerControlled = IsPlayerControlled;
            this.Color = Color;
            this.IsAlive = false;
        }

        public Player(BattleMapPlayer Clone)
            : base(Clone)
        {
            this.ListSquad = new List<Squad>();
            ListSpawnPoint = new List<EventPoint>();
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
