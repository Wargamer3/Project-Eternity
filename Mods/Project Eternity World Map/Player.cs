using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class Player
    {
        public string Name;
        public string PlayerType;
        public bool IsHuman;
        public List<UnitMap> ListUnit;
        public int Team;
        public Color Color;
        public bool IsAlive;//If the player can play (always true if it still have any active units).
        public Factions Faction;

        public bool IsResearchCenterBuilt;

        public int EnergyOuput;
        public int EnergyReserve;
        public int EnergyCostThisTurn;

        public List<Construction> ListConstruction;//List of Constructions on the field.
        public List<Construction> ListConstructionInProgress;//List of Constructions being builded.
        public List<Construction> ListConstructionChoice;//List of Constructions the player can build.
        public List<Construction> ListConstructionChoiceVisible;//List of Constructions the player can build with his current technology level.

        public Player(string Name, string PlayerType, bool IsHuman, int Team, Factions Faction, Color Color)
        {
            this.ListUnit = new List<UnitMap>();
            this.Name = Name;
            this.PlayerType = PlayerType;
            this.IsHuman = IsHuman;
            this.Team = Team;
            this.Color = Color;
            this.IsAlive = false;
            this.Faction = Faction;

            IsResearchCenterBuilt = false;

            EnergyOuput = 0;
            EnergyReserve = 0;
            EnergyCostThisTurn = 0;
            ListConstruction = new List<Construction>();
            ListConstructionInProgress = new List<Construction>();
            ListConstructionChoice = new List<Construction>();
            ListConstructionChoiceVisible = new List<Construction>();
        }

        public void OpenConstructionMenu()
        {
            ListConstructionChoiceVisible.Clear();
            ListConstructionChoiceVisible.Add(ListConstructionChoice[0]);
            ListConstructionChoiceVisible.Add(ListConstructionChoice[1]);
        }
    }
}
