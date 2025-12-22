using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class DestructibleTerrain
    {
        public TerrainConquest ReplacementTerrain;
        public DrawableTile ReplacementTile;
        public int RemainingHP;
        public int Defense;//Used to determine what can damage it

        public void DamageTile()
        {
            --RemainingHP;

            if (RemainingHP <= 0)
            {
            }
            else
            {
                UpdateTile();
            }
        }

        public void UpdateTile()
        {
            ReplacementTile.Origin.X -= ReplacementTile.Origin.Width;
        }

        public static void UpdateAllTemporaryTerrain(ConquestMap ActiveMap)
        {
            ActiveMap.DicTemporaryTerrain.Clear();
        }
    }
}
