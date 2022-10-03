using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class PropPicker : Form
    {
        private BattleMap Map;
        private InteractiveProp Owner;

        public PropPicker(BattleMap Map, InteractiveProp Owner, InteractiveProp OldValue)
        {
            InitializeComponent();
            this.Map = Map;
            this.Owner = Owner;

            foreach (InteractiveProp Instance in Map.DicInteractiveProp.Values)
            {
                if (Instance.PropCategory == InteractiveProp.PropCategories.Interactive)
                {
                    lsInteractiveProps.Items.Add(Instance);
                }
                else if (Instance.PropCategory == InteractiveProp.PropCategories.Physical)
                {
                    lsPhysicalProps.Items.Add(Instance);
                }
                else if (Instance.PropCategory == InteractiveProp.PropCategories.Visual)
                {
                    lsVisualProps.Items.Add(Instance);
                }
            }

            pgPropProperties.SelectedObject = OldValue;
        }

        private void lsInteractiveProps_SelectedIndexChanged(object sender, EventArgs e)
        {
            InteractiveProp ActiveProp = null;
            if (tabPropsChoices.SelectedIndex == 0 && lsInteractiveProps.SelectedItem != null)
            {
                ActiveProp = (InteractiveProp)lsInteractiveProps.SelectedItem;
            }
            else if (tabPropsChoices.SelectedIndex == 1 && lsPhysicalProps.SelectedItem != null)
            {
                ActiveProp = (InteractiveProp)lsPhysicalProps.SelectedItem;
            }
            else if (tabPropsChoices.SelectedIndex == 2 && lsVisualProps.SelectedItem != null)
            {
                ActiveProp = (InteractiveProp)lsVisualProps.SelectedItem;
            }
            else
            {
                return;
            }

            ActiveProp = ActiveProp.Copy(Owner.Position, (int)Owner.Position.Z);
            pgPropProperties.SelectedObject = ActiveProp;
        }
    }
}
