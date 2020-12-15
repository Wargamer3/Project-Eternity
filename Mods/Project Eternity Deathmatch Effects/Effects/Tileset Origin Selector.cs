using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class TilesetOriginSelector : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                ChangeTerrainEffect.ChangeTerrainAttribute TerrainAttirubte = (ChangeTerrainEffect.ChangeTerrainAttribute)value;
                TilesetOriginEditor Editor = new TilesetOriginEditor();
                Editor.TilesetViewer.Preload();
                Editor.TilesetViewer.InitTileset(TerrainAttirubte.Tileset, TerrainAttirubte.TileSize);

                if (Editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                }
            }
            return value;
        }
    }
}
