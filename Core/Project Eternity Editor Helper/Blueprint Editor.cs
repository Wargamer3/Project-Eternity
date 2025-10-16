using System;
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
