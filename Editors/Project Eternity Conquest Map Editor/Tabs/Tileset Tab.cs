using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
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

        protected override void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapTilesets));
        }

        protected override void btnAddAutotile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Autotile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapAutotilesPresets));
        }
    }
}
