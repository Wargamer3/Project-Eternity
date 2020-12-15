using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimatedBitmapSelector : UITypeEditor
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
                AnimatedBitmapSpawnerHelper NewSpawner = new AnimatedBitmapSpawnerHelper((string)value);
                if (NewSpawner.ShowDialog() == DialogResult.OK)
                    value = NewSpawner.SpawnViewer.Bitmap;
            }
            return value;
        }
    }

    public class SpriteSheetSelector : UITypeEditor
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
                SpriteSheetHelper NewSpawner = (SpriteSheetHelper)value;
                SpriteSheetHelper SpriteSheetHelperDialog = new SpriteSheetHelper();
                SpriteSheetHelperDialog.Replace = true;
                SpriteSheetHelperDialog.SpriteSheetViewer.Preload();
                SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet = new Dictionary<string, Texture2D>();

                SpriteSheetHelperDialog.SpriteSheetViewer.SpriteSheet = NewSpawner.SpriteSheetViewer.SpriteSheet;

                foreach (KeyValuePair<string, Texture2D> ActiveSpriteSheet in NewSpawner.SpriteSheetViewer.DicSpriteSheet)
                {
                    ListViewItem NewListViewItem = new ListViewItem(ActiveSpriteSheet.Key);
                    NewListViewItem.Tag = ActiveSpriteSheet.Value;

                    SpriteSheetHelperDialog.lvSpriteSheets.Items.Add(NewListViewItem);
                    SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet.Add(ActiveSpriteSheet.Key, ActiveSpriteSheet.Value);
                }

                if (SpriteSheetHelperDialog.ShowDialog() == DialogResult.OK)
                {
                    value = SpriteSheetHelperDialog;

                }
            }
            return value;
        }
    }

    public class AnimatedChainSelector : UITypeEditor
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
                AnimatedChainSpawnerHelper NewSpawner = new AnimatedChainSpawnerHelper((string)value);
                if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    value = NewSpawner;
                }
            }
            return value;
        }
    }
}
