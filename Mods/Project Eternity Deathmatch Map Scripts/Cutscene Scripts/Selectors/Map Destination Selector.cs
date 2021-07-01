using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MapDestinationSelector : UITypeEditor
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
                List<Vector2> ListTerrainChangeLocation = (List<Vector2>)value;
                MapDestinationEditor Editor = new MapDestinationEditor(ListTerrainChangeLocation);
                Editor.BattleMapViewer.Preload();

                if (Editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ListTerrainChangeLocation.Clear();
                    foreach (BattleMapScreen.EventPoint NewDestinationPoint in Editor.BattleMapViewer.ActiveMap.ListSingleplayerSpawns)
                    {
                        ListTerrainChangeLocation.Add(new Vector2(NewDestinationPoint.Position.X, NewDestinationPoint.Position.Y));
                    }

                    value = ListTerrainChangeLocation;
                }
            }

            return value;
        }
    }
}
