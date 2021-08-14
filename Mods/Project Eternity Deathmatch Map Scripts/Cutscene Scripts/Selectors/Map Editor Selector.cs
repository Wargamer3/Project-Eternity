using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MapEditorSelector : UITypeEditor
    {
        public struct ChangeTerrainAttribute
        {
            public List<string> ListTileset;
            public List<Terrain> ListTerrainChangeLocation;
            public List<DrawableTile> ListTileChangeLocation;
            public Point TileSize;

            public ChangeTerrainAttribute(Point TileSize)
            {
                this.TileSize = TileSize;
                this.ListTileset = new List<string>();
                this.ListTerrainChangeLocation = new List<Terrain>();
                ListTileChangeLocation = new List<DrawableTile>();
            }

            public override string ToString()
            {
                return ListTerrainChangeLocation.Count.ToString();
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
                ChangeTerrainAttribute TerrainAttribute = (ChangeTerrainAttribute)value;
                MapEditor Editor = new MapEditor(TerrainAttribute);
                Editor.BattleMapViewer.Preload();

                if (Editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    value = Editor.TerrainAttribute;
                }
            }

            return value;
        }
    }
}
