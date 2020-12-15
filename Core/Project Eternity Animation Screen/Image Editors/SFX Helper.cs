using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class SFXHelper : Form
    {
        public SFX SFX;

        public SFXHelper()
        {
            InitializeComponent();

            SFX = new SFX();
        }
        public SFXHelper(SFX SFX)
        {
            InitializeComponent();

            this.SFX = SFX;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnSelectSFX_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathSFX, "Select a SFX to use", false));
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(12);
                if (Name != null)
                {
                    SFX.SFXSound = new FMOD.FMODSound(GameScreen.FMODSystem, "Content/SFX/" + Name);
                    SFX.SFXPath = Name;
                }
            }
        }

        private void SFXHelper_Shown(object sender, EventArgs e)
        {
            btnSelectSFX_Click(sender, e);
        }
    }
}