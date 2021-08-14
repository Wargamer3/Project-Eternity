using System.Windows.Forms;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface ITileAttributes
    {
        Terrain ActiveTerrain { get; }

        void Init(Terrain ActiveTerrain, Terrain.TilesetPreset ActivePreset);
        DialogResult ShowDialog();
    }
}