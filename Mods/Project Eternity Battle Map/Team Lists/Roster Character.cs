using System;
using System.Collections.Generic;
using static ProjectEternity.Core.Characters.Character;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RosterCharacter
    {
        public string FilePath;
        public string ID;
        public Dictionary<string, CharacterLinkTypes> DicCharacterLink;//List which Characters it can link to and how.

        public RosterCharacter(string FilePath, string ID)
        {
            this.FilePath = FilePath;
            this.ID = ID;

            DicCharacterLink = new Dictionary<string, CharacterLinkTypes>();
        }
    }
}
