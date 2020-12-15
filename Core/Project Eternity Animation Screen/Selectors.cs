using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class QuoteSetSelector : UITypeEditor
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
                QuoteSetHelper NewSpawner = new QuoteSetHelper((Quote)value);
                if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    value = NewSpawner.ActiveQuote;
            }
            return value;
        }
    }

    public class SFXSelector : UITypeEditor
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
                SFXHelper NewSpawner = new SFXHelper((SFX)value);
                if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    value = NewSpawner.SFX;
            }
            return value;
        }
    }

    public class MarkerSelector : UITypeEditor
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
                MarkerHelper NewSpawner = new MarkerHelper((MarkerTimeline)value);
                if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    value = NewSpawner.MarkerViewer.ActiveMarker;
            }
            return value;
        }
    }

    public class PolygonCutterSelector : UITypeEditor
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
                PolygonCutterTimeline.PolygonCutterKeyFrame KeyFrame = (PolygonCutterTimeline.PolygonCutterKeyFrame)context.Instance;

                PolygonCutterHelper NewSpawner = new PolygonCutterHelper(KeyFrame.ActiveLayer.renderTarget, KeyFrame.ListPolygon, false);
                if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    value = NewSpawner.PolygonCutterViewer.ListPolygon;
                }
            }
            return value;
        }
    }
}
