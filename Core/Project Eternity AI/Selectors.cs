using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Core.AI
{
    public class Selectors
    {
        public class FollowingScriptOrderSelector : UITypeEditor
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
                    FollowingScripts[] ActiveParams = (FollowingScripts[])value;
                    FollowingScriptOrderEditor NewEditor = new FollowingScriptOrderEditor(ActiveParams);
                    NewEditor.ShowDialog();
                    value = NewEditor.ArrayFollowingScript;
                }
                return value;
            }
        }

        public class AISelector : UITypeEditor
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
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAIs);
                    if (Items != null && Items.Count > 0)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 5).Substring(12);
                    }
                }
                return value;
            }
        }
    }
}
