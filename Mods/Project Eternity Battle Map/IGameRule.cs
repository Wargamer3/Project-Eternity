using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface IGameRule
    {
        void OnSquadDefeated(int DefeatedSquadPlayerIndex, Squad DefeatedSquad, Unit UnitDefeated);
    }
}