using ProjectEternity.GameScreens.AnimationScreen;
using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    public partial class BackgroundProperties : Form
    {
        private AnimationBackground3D ActiveAnimationBackground;

        public BackgroundProperties(AnimationBackground3D ActiveAnimationBackground)
        {
            InitializeComponent();
            this.ActiveAnimationBackground = ActiveAnimationBackground;
        }

        private void BackgroundProperties_Shown(object sender, EventArgs e)
        {
            if (ActiveAnimationBackground.WorldType == AnimationBackground3D.WorldTypes.Infinite)
                rbWorldTypeInfinite.Checked = true;
            else if (ActiveAnimationBackground.WorldType == AnimationBackground3D.WorldTypes.Limited)
                rbWorldTypeLimited.Checked = true;
            else if (ActiveAnimationBackground.WorldType == AnimationBackground3D.WorldTypes.Looped)
                rbWorldTypeLooped.Checked = true;

            txtWorldSizeWidth.Value = ActiveAnimationBackground.WorldWidth;
            txtWorldSizeDepth.Value = ActiveAnimationBackground.WorldDepth;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (rbWorldTypeInfinite.Checked)
                ActiveAnimationBackground.WorldType = AnimationBackground3D.WorldTypes.Infinite;
            else if (rbWorldTypeLimited.Checked)
                ActiveAnimationBackground.WorldType = AnimationBackground3D.WorldTypes.Limited;
            else if (rbWorldTypeLooped.Checked)
                ActiveAnimationBackground.WorldType = AnimationBackground3D.WorldTypes.Looped;

            ActiveAnimationBackground.WorldWidth = (int)txtWorldSizeWidth.Value;
            ActiveAnimationBackground.WorldDepth = (int)txtWorldSizeDepth.Value;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void rbWorldTypeInfinite_CheckedChanged(object sender, EventArgs e)
        {
            gbWorldSize.Enabled = !rbWorldTypeInfinite.Checked;
        }
    }
}
