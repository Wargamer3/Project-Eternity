using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class TilesetOriginSelector : UITypeEditor
    {
        public struct ChangeTerrainAttribute
        {
            public string Tileset;
            public Microsoft.Xna.Framework.Point TileSize;
            public Microsoft.Xna.Framework.Rectangle Origin;

            public ChangeTerrainAttribute(string Tileset, Microsoft.Xna.Framework.Point TileSize, Microsoft.Xna.Framework.Rectangle Origin)
            {
                this.Tileset = Tileset;
                this.TileSize = TileSize;
                this.Origin = Origin;
            }

            public override string ToString()
            {
                return Origin.ToString();
            }
        }

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
                ChangeTerrainAttribute TerrainAttirubte = (ChangeTerrainAttribute)value;
                TilesetOriginEditor Editor = new TilesetOriginEditor();
                Editor.TilesetViewer.Preload();
                Editor.TilesetViewer.InitTileset(TerrainAttirubte.Tileset, TerrainAttirubte.TileSize);

                if (Editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TerrainAttirubte.Origin.X = Editor.TilesetViewer.ActiveTile.X;
                    TerrainAttirubte.Origin.Y = Editor.TilesetViewer.ActiveTile.Y;
                }
            }
            return value;
        }
    }
}
