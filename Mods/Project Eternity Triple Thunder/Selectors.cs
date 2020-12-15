using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Selectors
    {
        public class ExplosionOptionsSelector : UITypeEditor
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
                    Weapon.ExplosionOptions ActiveParams = (Weapon.ExplosionOptions)value;
                    ExplosionAttributesEditor NewEditor = new ExplosionAttributesEditor(ActiveParams);
                    if (NewEditor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        value = NewEditor.ExplosionAttributes;
                }
                return value;
            }
        }
    }
}
