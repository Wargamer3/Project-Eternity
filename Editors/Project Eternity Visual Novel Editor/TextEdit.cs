using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public partial class TextEdit : Form
    {
        TextBox OuputTextBox;

        public TextEdit(string StartingText, TextBox OuputTextBox)
        {
            InitializeComponent();
            this.OuputTextBox = OuputTextBox;
            txtEditbox.Text = StartingText;
        }

        private void txtEditbox_TextChanged(object sender, EventArgs e)
        {
            OuputTextBox.Text = txtEditbox.Text;
        }

        private void txtEditbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
        }
    }
}
