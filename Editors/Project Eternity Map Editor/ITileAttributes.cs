using ProjectEternity.GameScreens.BattleMapScreen;
using System.Windows.Forms;

namespace ProjectEternity.Editors.MapEditor
{
    public interface ITileAttributes
    {
        Terrain ActiveTerrain { get; }

        void Init(Terrain ActiveTerrain);
        DialogResult ShowDialog();
    }
}