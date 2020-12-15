using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RosterUnit
    {
        public string UnitTypeName;
        public string FilePath;
        public string EventID;
        public Dictionary<string, UnitStats.UnitLinkTypes> DicUnitLink;//List which Characters it can link to and how.

        public RosterUnit(string UnitTypeName, string FilePath, string EventID)
        {
            this.UnitTypeName = UnitTypeName;
            this.FilePath = FilePath;
            this.EventID = EventID;

            DicUnitLink = new Dictionary<string, UnitStats.UnitLinkTypes>();
        }
    }
}
