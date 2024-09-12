using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    class ConquestTilesetTab : MapEditor.TilesetTab
    {
        protected override void btnTileAttributes_Click(object sender, EventArgs e)
        {
            TileAttributes TA = new TileAttributes(0);
            if (TA.ShowDialog() == DialogResult.OK)
            {//Set the current tile attributes based on the TileAttibutes return.
            }
        }
    }
}
