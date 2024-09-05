using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    class SorcererStreetTilesetTab : MapEditor.TilesetTab
    {
        protected override void btnTileAttributes_Click(object sender, EventArgs e)
        {
            if (ActiveMap.ListTilesetPreset.Count <= 0)
            {
                return;
            }

            Rectangle TilePos = BattleMapViewer.TilesetViewer.ListTileBrush[0];
            Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

            TileAttributes TA = new TileAttributes(SelectedTerrain.TerrainTypeIndex, SelectedTerrain.Height);
            if (TA.ShowDialog() == DialogResult.OK)
            {
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y].TerrainTypeIndex = TA.TerrainTypeIndex;
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y].Height = TA.Height;
            }
        }
    }
}
