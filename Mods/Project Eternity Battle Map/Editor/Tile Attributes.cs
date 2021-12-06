using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class TileAttributes : Form, ITileAttributes
    {
        private enum ItemSelectionChoices { BattleBackgroundAnimation };

        private ItemSelectionChoices ItemSelectionChoice;

        public Terrain ActiveTerrain;
        Terrain.TilesetPreset ActivePreset;

        Terrain ITileAttributes.ActiveTerrain => ActiveTerrain;

        public TileAttributes()
        {
            InitializeComponent();

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
        }

        public virtual void Init(Terrain ActiveTerrain, Terrain.TilesetPreset ActivePreset)
        {
            ActiveTerrain = new Terrain(ActiveTerrain);
            this.ActiveTerrain = ActiveTerrain;
            this.ActivePreset = ActivePreset;
            cboBattleAnimationBackground.Items.Clear();
            cboBattleAnimationBackground.Items.Add("None");
            foreach (string ActivePath in ActivePreset.ListBattleBackgroundAnimationPath)
            {
                cboBattleAnimationBackground.Items.Add(ActivePath);
            }
            cboBattleAnimationForeground.Items.Clear();
            cboBattleAnimationForeground.Items.Add("None");
            foreach (string ActivePath in ActivePreset.ListBattleBackgroundAnimationPath)
            {
                cboBattleAnimationForeground.Items.Add(ActivePath);
            }

            cboTerrainType.SelectedIndex = ActiveTerrain.TerrainTypeIndex;
            cboBattleAnimationBackground.SelectedIndex = ActiveTerrain.BattleBackgroundAnimationIndex + 1;
            cboBattleAnimationForeground.SelectedIndex = ActiveTerrain.BattleForegroundAnimationIndex + 1;

            lstTerrainBonus.Items.Clear();
            //Load the lstTerrainBonus.
            for (int i = 0; i < ActiveTerrain.ListActivation.Length; i++)
            {
                lstTerrainBonus.Items.Add((i + 1) + ". " + cboTerrainBonusType.Items[(int)ActiveTerrain.ListBonus[i]].ToString() + " (" + ActiveTerrain.ListBonusValue[i].ToString() + " ) - " + cboTerrainBonusActivation.Items[(int)ActiveTerrain.ListActivation[i]].ToString());
            }

            if (ActiveTerrain.ListActivation.Length > 0)
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

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveTerrain.TerrainTypeIndex = cboTerrainType.SelectedIndex;
        }

        private void txtMVEnterCost_TextChanged(object sender, EventArgs e)
        {
            if (txtMVEnterCost.Text.Length == 0)
                ActiveTerrain.MVEnterCost = 0;
            else
                ActiveTerrain.MVEnterCost = Convert.ToInt32(txtMVEnterCost.Text);
        }

        private void txtMVMoveCost_TextChanged(object sender, EventArgs e)
        {
            if (txtMVMoveCost.Text.Length == 0)
                ActiveTerrain.MVMoveCost = 0;
            else
                ActiveTerrain.MVMoveCost = Convert.ToInt32(txtMVMoveCost.Text);
        }

        private void btnAddNewBonus_Click(object sender, EventArgs e)
        {
            lstTerrainBonus.Items.Add((lstTerrainBonus.Items.Count + 1) + ". HP regen (5 ) - On every turn");
            ActiveTerrain.ListActivation = ActiveTerrain.ListActivation.Concat(new TerrainActivation[] { TerrainActivation.OnEveryTurns }).ToArray();
            ActiveTerrain.ListBonus = ActiveTerrain.ListBonus.Concat(new TerrainBonus[] { TerrainBonus.HPRegen }).ToArray();
            ActiveTerrain.ListBonusValue = ActiveTerrain.ListBonusValue.Concat(new int[] { 5 }).ToArray();
            lstTerrainBonus.SelectedIndex = lstTerrainBonus.Items.Count - 1;
        }

        private void btnRemoveBonus_Click(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex >= 0)
            {
                int Index = lstTerrainBonus.SelectedIndex;
                ActiveTerrain.ListActivation = ActiveTerrain.ListActivation.Where((source, index) => index != Index).ToArray();
                ActiveTerrain.ListBonus = ActiveTerrain.ListBonus.Where((source, index) => index != Index).ToArray();
                ActiveTerrain.ListBonusValue = ActiveTerrain.ListBonusValue.Where((source, index) => index != Index).ToArray();

                lstTerrainBonus.Items.RemoveAt(lstTerrainBonus.SelectedIndex);
                if (lstTerrainBonus.Items.Count > 0)
                {
                    if (Index >= lstTerrainBonus.Items.Count)
                        lstTerrainBonus.SelectedIndex = lstTerrainBonus.Items.Count - 1;
                    else
                        lstTerrainBonus.SelectedIndex = Index;
                    cboTerrainBonusActivation.SelectedIndex = (int)ActiveTerrain.ListActivation[lstTerrainBonus.SelectedIndex];
                    cboTerrainBonusType.SelectedIndex = (int)ActiveTerrain.ListBonus[lstTerrainBonus.SelectedIndex];
                    txtBonusValue.Text = ActiveTerrain.ListBonusValue[lstTerrainBonus.SelectedIndex].ToString();
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
                cboTerrainBonusActivation.SelectedIndex = (int)ActiveTerrain.ListActivation[lstTerrainBonus.SelectedIndex];
                cboTerrainBonusType.SelectedIndex = (int)ActiveTerrain.ListBonus[lstTerrainBonus.SelectedIndex];
                txtBonusValue.Text = ActiveTerrain.ListBonusValue[lstTerrainBonus.SelectedIndex].ToString();

                cboTerrainBonusActivation.Enabled = true;
                cboTerrainBonusType.Enabled = true;
                txtBonusValue.Enabled = true;
            }
        }

        private void cboTerrainBonusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex != -1)
            {
                ActiveTerrain.ListBonus[lstTerrainBonus.SelectedIndex] = (TerrainBonus)cboTerrainBonusType.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void cboTerrainBonusActivation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex != -1)
            {
                ActiveTerrain.ListActivation[lstTerrainBonus.SelectedIndex] = (TerrainActivation)cboTerrainBonusActivation.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void txtBonusValue_TextChanged(object sender, EventArgs e)
        {
            if (lstTerrainBonus.SelectedIndex != -1 && txtBonusValue.Text != "")
            {
                ActiveTerrain.ListBonusValue[lstTerrainBonus.SelectedIndex] = Convert.ToInt32(txtBonusValue.Text);
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void cboBattleAnimationBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveTerrain.BattleBackgroundAnimationIndex = cboBattleAnimationBackground.SelectedIndex - 1;
        }

        private void cboBattleAnimationForeground_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveTerrain.BattleForegroundAnimationIndex = cboBattleAnimationForeground.SelectedIndex - 1;
        }

        private void btnNewBattleAnimationBackground_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BattleBackgroundAnimation;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundsAll));
        }

        private void btnDeleteBattleAnimationBackground_Click(object sender, EventArgs e)
        {
            if (cboBattleAnimationBackground.SelectedIndex >= 0)
            {
                cboBattleAnimationBackground.Items.RemoveAt(cboBattleAnimationBackground.SelectedIndex);
            }
            if (cboBattleAnimationForeground.SelectedIndex >= 0)
            {
                cboBattleAnimationForeground.Items.RemoveAt(cboBattleAnimationForeground.SelectedIndex);
            }
        }

        private void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.BattleBackgroundAnimation:
                        string BackgroundPath = Items[I];
                        if (BackgroundPath != null)
                        {
                            BackgroundPath = BackgroundPath.Substring(0, BackgroundPath.Length - 5).Substring(19);

                            ActivePreset.ListBattleBackgroundAnimationPath.Add(BackgroundPath);
                            cboBattleAnimationBackground.Items.Add(BackgroundPath);
                            cboBattleAnimationForeground.Items.Add(BackgroundPath);
                        }
                        break;
                }
            }
        }
    }
}
