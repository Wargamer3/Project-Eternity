using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectEternity.Editors.WorldMapEditor
{
    public partial class Properties_Editor : Form
    {
        public int MapWidth;
        public int MapHeight;

        public Properties_Editor(int MapWidth, int MapHeight)
        {
            InitializeComponent();

            this.MapWidth = MapWidth;
            this.MapHeight = MapHeight;
            txtMapWidth.Text = MapWidth.ToString();
            txtMapHeight.Text = MapHeight.ToString();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            MapWidth = Convert.ToInt32(txtMapWidth.Text);
            MapHeight = Convert.ToInt32(txtMapHeight.Text);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
