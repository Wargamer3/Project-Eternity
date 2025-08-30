using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    class ConquestTilesetTab : MapEditor.TilesetTab
    {
        protected override void btnTileAttributes_Click(object sender, EventArgs e)
        {
            if (cboTiles.SelectedIndex >= 0)
            {
                Rectangle TilePos = BattleMapViewer.TilesetViewer.ListTileBrush[0];
                Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                TileAttributes TA = new TileAttributes();
                TA.Init(SelectedTerrain, ActiveMap);

                if (TA.ShowDialog() == DialogResult.OK)
                {
                    ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y] = TA.ActiveTerrain;
                }
            }
        }
    }
}
