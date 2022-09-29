using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using ProjectEternity.Core.Units;

namespace ProjectEternity.Editors.SystemListEditor
{

    public partial class TerrainAndUnitTypesEditor : Form
    {
        private bool AllowEvents;
        private SolidBrush GridBrush;

        private UnitAndTerrainValues Values;

        public TerrainAndUnitTypesEditor()
        {
            InitializeComponent();

            Values = UnitAndTerrainValues.Default;

            GridBrush = new SolidBrush(dgvTerrainRanks.RowHeadersDefaultCellStyle.ForeColor);
            AllowEvents = true;
            cbRestrictionCategory.Items.Add(new UnitTypeRestriction(Values));
            cbRestrictionCategory.Items.Add(new UnitSizeRestriction(Values));
            cbRestrictionCategory.Items.Add(new UnitRankRestriction(Values));
            cbRestrictionCategory.Items.Add(new TagRestriction(Values));

            foreach (string ActiveUnitType in Values.ListUnitType)
            {
                lsUnitTypes.Items.Add(ActiveUnitType);
            }

            foreach (UnitMovementType ActiveMovementType in Values.ListUnitMovement)
            {
                lsUnitMovementTypes.Items.Add(ActiveMovementType);
            }

            foreach (TerrainType ActiveTerrainType in Values.ListTerrainType)
            {
                lsTerrainTypes.Items.Add(ActiveTerrainType);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            Values.Save();
        }

        #region Unit Type

        private void lsUnitTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsUnitTypes.SelectedIndex >= 0)
            {
                txtUnitType.Text = lsUnitTypes.Items[lsUnitTypes.SelectedIndex].ToString();
            }
        }

        private void txtUnitType_TextChanged(object sender, EventArgs e)
        {
            if (lsUnitTypes.SelectedIndex >= 0)
            {
                lsUnitTypes.Items[lsUnitTypes.SelectedIndex] = txtUnitType.Text;
                Values.ListUnitType[lsUnitTypes.SelectedIndex] = txtUnitType.Text;
            }
        }

        private void btnAddUnitType_Click(object sender, EventArgs e)
        {
            lsUnitTypes.Items.Add("New type");
            Values.ListUnitType.Add("New type");
            lsUnitTypes.SelectedIndex = lsUnitTypes.Items.Count - 1;
        }

        private void btnRemoveUnitType_Click(object sender, EventArgs e)
        {
            if (lsUnitTypes.SelectedIndex >= 0)
            {
                Values.ListUnitType.RemoveAt(lsUnitTypes.SelectedIndex);
                lsUnitTypes.Items.RemoveAt(lsUnitTypes.SelectedIndex);
            }
        }

        #endregion

        #region Unit Movement Types

        private void lsUnitMovementTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsUnitMovementTypes.SelectedIndex >= 0)
            {
                UnitMovementType SelectedUnitMovementType = (UnitMovementType)lsUnitMovementTypes.SelectedItem;
                txtUnitMovementTypeName.Text = SelectedUnitMovementType.Name;
                txtUnitMovementTypeActivationName.Text = SelectedUnitMovementType.ActivationName;
                txtUnitMovementTypeAnnulationName.Text = SelectedUnitMovementType.AnnulatioName;
            }
        }

        private void txtUnitMovementTypeName_TextChanged(object sender, EventArgs e)
        {
            if (lsUnitMovementTypes.SelectedIndex >= 0)
            {
                UnitMovementType SelectedUnitMovementType = (UnitMovementType)lsUnitMovementTypes.SelectedItem;
                SelectedUnitMovementType.Name = txtUnitMovementTypeName.Text;
                Values.ListUnitMovement[lsUnitMovementTypes.SelectedIndex].Name = txtUnitMovementTypeName.Text;

                lsUnitMovementTypes.Items[lsUnitMovementTypes.SelectedIndex] = SelectedUnitMovementType;
            }
        }

        private void txtUnitMovementTypeActivationName_TextChanged(object sender, EventArgs e)
        {
            if (lsUnitMovementTypes.SelectedIndex >= 0)
            {
                UnitMovementType SelectedUnitMovementType = (UnitMovementType)lsUnitMovementTypes.SelectedItem;
                SelectedUnitMovementType.ActivationName = txtUnitMovementTypeActivationName.Text;
                Values.ListUnitMovement[lsUnitMovementTypes.SelectedIndex].ActivationName = txtUnitMovementTypeActivationName.Text;

                lsUnitMovementTypes.Items[lsUnitMovementTypes.SelectedIndex] = SelectedUnitMovementType;
            }
        }

        private void txtUnitMovementTypeAnnulationName_TextChanged(object sender, EventArgs e)
        {
            if (lsUnitMovementTypes.SelectedIndex >= 0)
            {
                UnitMovementType SelectedUnitMovementType = (UnitMovementType)lsUnitMovementTypes.SelectedItem;
                SelectedUnitMovementType.AnnulatioName = txtUnitMovementTypeAnnulationName.Text;
                Values.ListUnitMovement[lsUnitMovementTypes.SelectedIndex].AnnulatioName = txtUnitMovementTypeAnnulationName.Text;

                lsUnitMovementTypes.Items[lsUnitMovementTypes.SelectedIndex] = SelectedUnitMovementType;
            }
        }

        private void btnAddUnitMovementType_Click(object sender, EventArgs e)
        {
            UnitMovementType NewMovementType = new UnitMovementType();
            lsUnitMovementTypes.Items.Add(NewMovementType);
            Values.ListUnitMovement.Add(NewMovementType);
        }

        private void btnRemoveUnitMovementType_Click(object sender, EventArgs e)
        {
            if (lsUnitMovementTypes.SelectedIndex >= 0)
            {
                Values.ListUnitMovement.RemoveAt(lsUnitMovementTypes.SelectedIndex);
                lsUnitMovementTypes.Items.RemoveAt(lsUnitMovementTypes.SelectedIndex);
            }
        }

        #endregion

        #region Terrain Type

        private void lsTerrainTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsTerrainTypes.SelectedIndex >= 0)
            {
                TerrainType SelectedTerrain = (TerrainType)lsTerrainTypes.SelectedItem;
                txtTerrainTypeName.Text = SelectedTerrain.Name;
                txtWallHardness.Value = SelectedTerrain.WallHardness;

                tvTerrainRestrictions.Nodes.Clear();
                foreach (ITerrainRestriction ActiveRestriction in SelectedTerrain.ListRestriction)
                {
                    TreeNode NewNode = new TreeNode(ActiveRestriction.ToString());
                    NewNode.Tag = ActiveRestriction;
                    tvTerrainRestrictions.Nodes.Add(NewNode);
                }

                tvTerrainRestrictions.SelectedNode = tvTerrainRestrictions.Nodes[0];
            }
        }

        private void txtTerrainTypeName_TextChanged(object sender, EventArgs e)
        {
            if (lsTerrainTypes.SelectedIndex >= 0)
            {
                TerrainType SelectedTerrain = (TerrainType)lsTerrainTypes.SelectedItem;
                SelectedTerrain.Name = txtTerrainTypeName.Text;
                lsTerrainTypes.Items[lsTerrainTypes.SelectedIndex] = SelectedTerrain;
            }
        }

        private void txtWallHardness_ValueChanged(object sender, EventArgs e)
        {
            if (lsTerrainTypes.SelectedIndex >= 0)
            {
                TerrainType SelectedTerrain = (TerrainType)lsTerrainTypes.SelectedItem;
                SelectedTerrain.WallHardness = (byte)txtWallHardness.Value;
            }
        }

        private void btnAddTerrainType_Click(object sender, EventArgs e)
        {
            TerrainType NewTerrainType = new TerrainType();
            lsTerrainTypes.Items.Add(NewTerrainType);
            Values.ListTerrainType.Add(NewTerrainType);
            lsTerrainTypes.SelectedItem = lsTerrainTypes.Items[lsTerrainTypes.Items.Count - 1];
        }

        private void btnRemoveTerrainType_Click(object sender, EventArgs e)
        {
            if (lsTerrainTypes.SelectedIndex >= 0)
            {
                lsTerrainTypes.Items.RemoveAt(lsTerrainTypes.SelectedIndex);
            }
        }

        #endregion

        #region Restrictions

        private void tvTerrainRestrictions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvTerrainRestrictions.SelectedNode != null)
            {
                ITerrainRestriction NewRestriction = (ITerrainRestriction)tvTerrainRestrictions.SelectedNode.Tag;

                AllowEvents = false;

                cbRestrictionCategory.Text = NewRestriction.ToString();

                txtEntryCost.Value = (decimal)NewRestriction.EntryCost;
                txtMovementCost.Value = (decimal)NewRestriction.MovementCost;
                txtENCostToMove.Value = (decimal)NewRestriction.ENCostToMove;
                txtENCostPerTurn.Value = (decimal)NewRestriction.ENCostPerTurn;

                UpdateRestrictionGrid(NewRestriction);

                AllowEvents = true;
            }
        }

        private void UpdateRestrictionGrid(ITerrainRestriction NewRestriction)
        {
            AllowEvents = false;
            dgvTerrainRanks.Rows.Clear();
            dgvTerrainRanks.Columns.Clear();
            DataGridValue ListColumn = NewRestriction.GetColumns();
            for (int i = 0; i < ListColumn.ListRow[0].ListValues.Count; ++i)
            {
                dgvTerrainRanks.Columns.Add(new DataGridViewTextBoxColumn());
            }

            this.dgvTerrainRanks.RowHeadersDefaultCellStyle.Padding = new Padding(this.dgvTerrainRanks.RowHeadersWidth);
            foreach (RowWithSelectedValue ActiveRow in ListColumn.ListRow)
            {
                int NewRowIndex = dgvTerrainRanks.Rows.Add();
                dgvTerrainRanks.Rows[NewRowIndex].HeaderCell.Value = ActiveRow.RowName;
                for (int C = 0; C < ActiveRow.ListValues.Count; C++)
                {
                    CellValue ActivCell = ActiveRow.ListValues[C];
                    if (ActiveRow.ListValues.Count > 0)
                    {
                        DataGridViewComboBoxCell NewComboBoxColumn = new DataGridViewComboBoxCell();

                        foreach (string ActiveChoice in ActivCell.ListValues)
                        {
                            NewComboBoxColumn.Items.Add(ActiveChoice);
                        }

                        foreach (string ActiveChoice in ActivCell.ListValues)
                        {
                            NewComboBoxColumn.Items.Add("!" + ActiveChoice);
                        }

                        dgvTerrainRanks.Rows[NewRowIndex].Cells[C] = NewComboBoxColumn;
                    }

                    dgvTerrainRanks.Rows[NewRowIndex].Cells[C].Value = ActivCell.SelectedValue;
                }
            }

            AllowEvents = true;
        }

        private void btnAddTerrainRestriction_Click(object sender, EventArgs e)
        {
            if (lsTerrainTypes.SelectedIndex >= 0)
            {
                TerrainType SelectedTerrain = (TerrainType)lsTerrainTypes.SelectedItem;
                ITerrainRestriction NewRestriction = new UnitTypeRestriction(Values);
                SelectedTerrain.ListRestriction.Add(NewRestriction);
                TreeNode NewNode = new TreeNode(NewRestriction.ToString());
                NewNode.Tag = NewRestriction;
                tvTerrainRestrictions.Nodes.Add(NewNode);
            }
        }

        private void btnRemoveTerrainRestriction_Click(object sender, EventArgs e)
        {
            if (lsTerrainTypes.SelectedIndex >= 0 && tvTerrainRestrictions.SelectedNode != null)
            {
                TerrainType SelectedTerrain = (TerrainType)lsTerrainTypes.SelectedItem;
                SelectedTerrain.ListRestriction.Remove((ITerrainRestriction)tvTerrainRestrictions.SelectedNode.Tag);
                tvTerrainRestrictions.Nodes.Remove(tvTerrainRestrictions.SelectedNode);
            }
        }

        private void btnAddSubTerrainRestriction_Click(object sender, EventArgs e)
        {

        }

        private void OnTerrainAttributeChanged(object sender, EventArgs e)
        {
            if (AllowEvents && lsTerrainTypes.SelectedIndex >= 0 && tvTerrainRestrictions.SelectedNode != null)
            {
                TerrainType SelectedTerrain = (TerrainType)lsTerrainTypes.SelectedItem;
                ITerrainRestriction SelectedRestriction = (ITerrainRestriction)tvTerrainRestrictions.SelectedNode.Tag;

                if (cbRestrictionCategory.SelectedItem != null && SelectedRestriction.GetType() != cbRestrictionCategory.SelectedItem.GetType())
                {
                    int IndexOf = SelectedTerrain.ListRestriction.IndexOf(SelectedRestriction);
                    SelectedRestriction = ((ITerrainRestriction)cbRestrictionCategory.SelectedItem).Copy();
                    tvTerrainRestrictions.SelectedNode.Tag = SelectedRestriction;
                    SelectedTerrain.ListRestriction[IndexOf] = SelectedRestriction;
                }

                SelectedRestriction.EntryCost = (float)txtEntryCost.Value;
                SelectedRestriction.MovementCost = (float)txtMovementCost.Value;
                tvTerrainRestrictions.Refresh();

                UpdateRestrictionGrid(SelectedRestriction);
            }
        }

        #endregion

        private void dgvTerrainRanks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!AllowEvents)
                return;

            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (datagridview.Rows[e.RowIndex].Cells[0] is DataGridViewComboBoxCell && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void dgvTerrainRanks_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            dgvTerrainRanks.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (tvTerrainRestrictions.SelectedNode != null)
            {
                ITerrainRestriction NewRestriction = (ITerrainRestriction)tvTerrainRestrictions.SelectedNode.Tag;

                List<List<string>> ListValuePerRow = new List<List<string>>();

                for (int i = 0; i < dgvTerrainRanks.Rows.Count; i++)
                {
                    List<string> ListValue = new List<string>();

                    for (int C = 0; C < dgvTerrainRanks.Rows[i].Cells.Count; C++)
                    {
                        if (dgvTerrainRanks.Rows[i].Cells[C].Value != null)
                        {
                            ListValue.Add((string)dgvTerrainRanks.Rows[i].Cells[C].Value);
                        }
                    }

                    ListValuePerRow.Add(ListValue);
                }

                if (ListValuePerRow.Count > 0)
                {
                    NewRestriction.SetValues(ListValuePerRow);
                }

                UpdateRestrictionGrid(NewRestriction);
            }
        }

        private void dgvTerrainRanks_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            object o = dgvTerrainRanks.Rows[e.RowIndex].HeaderCell.Value;

            e.Graphics.DrawString(
                o != null ? o.ToString() : "",
                dgvTerrainRanks.Font,
                GridBrush,
                new PointF((float)e.RowBounds.Left + 2, (float)e.RowBounds.Top + 4));
        }
    }
}
