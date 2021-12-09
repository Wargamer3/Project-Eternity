using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PlayerInventory
    {
        public List<SquadLoadout> ListSquadLoadout;
        public List<Squad> ListOwnedSquad;
        public List<Character> ListOwnedCharacter;

        public SquadLoadout ActiveLoadout;

        public PlayerInventory()
        {
            ListSquadLoadout = new List<SquadLoadout>();
            ListOwnedSquad = new List<Squad>();
            ListOwnedCharacter = new List<Character>();

            ActiveLoadout = new SquadLoadout();
            ListSquadLoadout.Add(ActiveLoadout);
        }
    }

    public class SquadLoadout
    {
        public List<Squad> ListSquad;

        public SquadLoadout()
        {
            ListSquad = new List<Squad>();
        }
    }
}
