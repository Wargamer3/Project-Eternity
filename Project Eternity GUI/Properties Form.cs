using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectEternity.GUI
{
    public partial class PropertiesForm : Form
    {
        public string ItemName;
        public string FileName;

        public PropertiesForm()
        {
            InitializeComponent();
        }
        public PropertiesForm(string ItemName, string FileName)
            : this()
        {
            this.ItemName = ItemName;
            this.FileName = FileName;

            txtName.Text = Name;
            txtFileName.Text = FileName;
        }
    }
}
