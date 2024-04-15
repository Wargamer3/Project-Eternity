using System.Windows.Forms;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface ITileAttributes
    {
        Terrain ActiveTerrain { get; }
        void Init(Terrain ActiveTerrain, BattleMap Map);
        string GetTerrainName(int Index);
        DialogResult ShowDialog();
    }
}