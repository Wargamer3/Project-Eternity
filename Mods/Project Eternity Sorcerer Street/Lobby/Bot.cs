using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Bot
    {
        public PlayerCharacter Character;
        public Dictionary<string, CardBook> DicOwnedBook;

        public Bot(PlayerCharacter Character, Dictionary<string, CardBook> DicOwnedBook)
        {
            this.Character = Character;
            this.DicOwnedBook = DicOwnedBook;
        }
    }
}
