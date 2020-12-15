using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Editor
{
    public partial class BlueprintEditor : Form
    {
        public Blueprint ActiveBlueprint;

        public BlueprintEditor(Blueprint NewBlueprint)
        {
            InitializeComponent();
            ActiveBlueprint = NewBlueprint;
        }
    }
}
