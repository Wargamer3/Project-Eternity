using ProjectEternity.Core.Attacks;
using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class DestructibleTileTypePicker : Form
    {
        public DestructibleTileTypePicker()
        {
            InitializeComponent();

            foreach (object ActiveType in Enum.GetValues(typeof(DestructibleTilesAttackAttributes.DestructibleTypes)))
            {
                cbType.Items.Add(ActiveType);
            }
        }
    }
}
