namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public partial class PlayerCharacter
    {
        public readonly LifeSheet Life = new LifeSheet();
        public double SwingRemaining;
        public double SimHour;
        public int DayAttackBonus;
        public int DayCraftBonus;
        public string PartyId = "";
        public string TownPersonId = "";
        public int CityId = -1;
    }
}
