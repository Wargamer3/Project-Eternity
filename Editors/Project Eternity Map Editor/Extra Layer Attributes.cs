using System.Windows.Forms;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class ExtraLayerAttributes : Form
    {
        public ExtraLayerAttributes()
        {
            InitializeComponent();
        }

        public ExtraLayerAttributes(int StartupDelay, int ToggleDelayOn, int ToggleDelayOff, float Depth)
        {
            InitializeComponent();

            txtAnimationStartupDelay.Value = StartupDelay;
            txtAnimationToggleDelayOn.Value = ToggleDelayOn;
            txtAnimationToggleDelayOff.Value = ToggleDelayOff;
            txtDepth.Value = (decimal)Depth;
        }
    }
}
