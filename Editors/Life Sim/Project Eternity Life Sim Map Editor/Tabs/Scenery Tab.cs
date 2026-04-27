using System;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.LifeSimMapEditor
{
    public class LifeSimSceneryTab : MapEditor.SceneryTab
    {
        protected override void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapDestroyableTilesPresetsLifeSim));
        }
    }
}
