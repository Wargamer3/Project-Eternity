using System;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.MapEditor
{
    public class ConquestSceneryTab : MapEditor.SceneryTab
    {
        protected override void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapDestroyableTilesPresetsConquest));
        }
    }
}
