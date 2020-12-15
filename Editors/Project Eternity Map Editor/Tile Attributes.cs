using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class TileAttributes : Form
    {
        public int TerrainTypeIndex;//What kind of terrain it is.
        public int MVEnterCost;//How much energy is required to enter in it.
        public int MVMoveCost;//How much energy is required to move in it.
        public List<TerrainActivation> ListActivation;//Activation type of the bonuses.
        public List<TerrainBonus> ListBonus;//Bonuses the terrain can give.
        public List<int> ListBonusValue;//Value of the bonuses.

        public TileAttributes(Terrain ActiveTerrain)
        {
            InitializeComponent();

            this.TerrainTypeIndex = ActiveTerrain.TerrainTypeIndex;
            this.MVEnterCost = ActiveTerrain.MVEnterCost;
            this.MVMoveCost = ActiveTerrain.MVMoveCost;
            this.ListActivation = ActiveTerrain.ListActivation.ToList();
            this.ListBonus = ActiveTerrain.ListBonus.ToList();
            this.ListBonusValue = ActiveTerrain.ListBonusValue.ToList();

            txtMVEnterCost.Text = MVEnterCost.ToString();
            txtMVMoveCost.Text = MVMoveCost.ToString();

            cboTerrainType.SelectedIndex = TerrainTypeIndex;

            cboTerrainBonusActivation.Items.Add("On every turns");
            cboTerrainBonusActivation.Items.Add("On this turn");
            cboTerrainBonusActivation.Items.Add("On next turn");
            cboTerrainBonusActivation.Items.Add("On enter");
            cboTerrainBonusActivation.Items.Add("On leaved");
            cboTerrainBonusActivation.Items.Add("On attack");
            cboTerrainBonusActivation.Items.Add("On hit");
            cboTerrainBonusActivation.Items.Add("On miss");
            cboTerrainBonusActivation.Items.Add("On defend");
            cboTerrainBonusActivation.Items.Add("On hited");
            cboTerrainBonusActivation.Items.Add("On missed");

            cboTerrainBonusType.Items.Add("HP regen");
            cboTerrainBonusType.Items.Add("EN regen");
            cboTerrainBonusType.Items.Add("HP regain");
            cboTerrainBonusType.Items.Add("EN regain");
            cboTerrainBonusType.Items.Add("Armor");
            cboTerrainBonusType.Items.Add("Accuracy");
            cboTerrainBonusType.Items.Add("Evasion");

            //Load the lstTerrainBonus.
            for (int i = 0; i < ListActivation.Count; i++)
            {
                lstTerrainBonus.Items.Add((i + 1) + ". " + cboTerrainBonusType.Items[(int)ListBonus[i]].ToString() + " (" + ListBonusValue[i].ToString() + " ) - " + cboTerrainBonusActivation.Items[(int)ListActivation[i]].ToString());
            }

            if (ListActivation.Count > 0)
            {
                cboTerrainBonusActivation.Enabled = true;
                cboTerrainBonusType.Enabled = true;
                txtBonusValue.Enabled = true;
                lstTerrainBonus.SelectedIndex = 0;
            }
            else
            {
                cboTerrainBonusActivation.Enabled = false;
                cboTerrainBonusType.Enabled = false;
                txtBonusValue.Enabled = false;
            }
        }

        private void txtMVEnterCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMVMoveCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBonusValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerrainTypeIndex = cboTerrainType.SelectedIndex;
        }

        private void txtMVEnterCost_TextChanged(object sender, EventArgs e)
        {
            if (txtMVEnterCost.Text.Length == 0)
                MVEnterCost = 0;
            else
                MVEnterCost = Convert.ToInt32(txtMVEnterCost.Text);
        }

        private void txtMVMoveCost_TextChanged(object sender, EventArgs e)
        {
            if (txtMVMoveCost.Text.Length == 0)
                MVMoveCost = 0;
            else
                MVMoveCost = Convert.ToInt32(txtMVMoveCost.Text);
        }

        private void btnAddNewBonus_Click(object sender, EventArgs e)
        {
            lstTerrainBonus.Items.Add((lstTerrainBonus.Items.Count + 1) + ". HP regen (5 ) - On every turn");
            ListActivation.Add(TerrainActivation.OnEveryTurns);
            ListBonus.Add(TerrainBonus.HPRegen);
            ListBonusValue.Add(5);
            lstTerrainBonus.SelectedIndex = lstTerrainBonus.Items.Count - 1;
        }

        private void btnRemoveBonus_Click(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex >= 0)
            {
                int Index = lstTerrainBonus.SelectedIndex;
                ListActivation.RemoveAt(lstTerrainBonus.SelectedIndex);
                ListBonus.RemoveAt(lstTerrainBonus.SelectedIndex);
                ListBonusValue.RemoveAt(lstTerrainBonus.SelectedIndex);
                lstTerrainBonus.Items.RemoveAt(lstTerrainBonus.SelectedIndex);
                if (lstTerrainBonus.Items.Count > 0)
                {
                    if (Index >= lstTerrainBonus.Items.Count)
                        lstTerrainBonus.SelectedIndex = lstTerrainBonus.Items.Count - 1;
                    else
                        lstTerrainBonus.SelectedIndex = Index;
                    cboTerrainBonusActivation.SelectedIndex = (int)ListActivation[lstTerrainBonus.SelectedIndex];
                    cboTerrainBonusType.SelectedIndex = (int)ListBonus[lstTerrainBonus.SelectedIndex];
                    txtBonusValue.Text = ListBonusValue[lstTerrainBonus.SelectedIndex].ToString();
                }
                else
                {
                    cboTerrainBonusActivation.Text = "";
                    cboTerrainBonusType.Text = "";
                    txtBonusValue.Text = "";
                }
            }
        }

        private void btnClearBonuses_Click(object sender, EventArgs e)
        {
            if (lstTerrainBonus.Items.Count > 0)
            {
                lstTerrainBonus.SelectedIndex = 0;
                while (lstTerrainBonus.Items.Count > 0)
                    btnRemoveBonus_Click(sender, e);
            }
        }

        private void lstTerrainBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex != -1)
            {
                cboTerrainBonusActivation.SelectedIndex = (int)ListActivation[lstTerrainBonus.SelectedIndex];
                cboTerrainBonusType.SelectedIndex = (int)ListBonus[lstTerrainBonus.SelectedIndex];
                txtBonusValue.Text = ListBonusValue[lstTerrainBonus.SelectedIndex].ToString();

                cboTerrainBonusActivation.Enabled = true;
                cboTerrainBonusType.Enabled = true;
                txtBonusValue.Enabled = true;
            }
        }

        private void cboTerrainBonusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex != -1)
            {
                ListBonus[lstTerrainBonus.SelectedIndex] = (TerrainBonus)cboTerrainBonusType.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void cboTerrainBonusActivation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex != -1)
            {
                ListActivation[lstTerrainBonus.SelectedIndex] = (TerrainActivation)cboTerrainBonusActivation.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void txtBonusValue_TextChanged(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex != -1 && txtBonusValue.Text != "")
            {
                ListBonusValue[lstTerrainBonus.SelectedIndex] = Convert.ToInt32(txtBonusValue.Text);
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
