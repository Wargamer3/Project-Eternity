using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class ProjectEternityConquestTerrainsAndMoveTypesEditor : Form
    {
        private ConquestTerrainHolder TerrainHolder;

        private bool AllowEvent;

        public ProjectEternityConquestTerrainsAndMoveTypesEditor()
        {
            InitializeComponent();

            AllowEvent = true;

            TerrainHolder = new ConquestTerrainHolder();
        }

        private void ProjectEternityConquestTerrainsAndMoveTypesEditor_Load(object sender, EventArgs e)
        {
            TerrainHolder.LoadData();

            for (int i = 0; i < TerrainHolder.ListMoveType.Count; ++i)
            {
                lsMoveTypes.Items.Add(TerrainHolder.ListMoveType[i]);
            }

            for (int i = 0; i < TerrainHolder.ListConquestTerrainType.Count; ++i)
            {
                lsTerrains.Items.Add(TerrainHolder.ListConquestTerrainType[i].TerrainName);
            }
        }

        private void Save()
        {
            FileStream FS = new FileStream("Content/Conquest/Conquest Terrains And Movements.bin", FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.Unicode);

            BW.Write(TerrainHolder.ListMoveType.Count);
            foreach (string ActiveMoveType in TerrainHolder.ListMoveType)
            {
                BW.Write(ActiveMoveType);
            }

            BW.Write(TerrainHolder.ListConquestTerrainType.Count);
            foreach (ConquestTerrainTypeAttributes ActiveTerrainType in TerrainHolder.ListConquestTerrainType)
            {
                ActiveTerrainType.Save(BW);
            }

            BW.Flush();
            BW.Close();
            FS.Close();
        }

        private void UpdateGridView()
        {
            if (lsTerrains.SelectedIndex < 0)
            {
                dgvMoveTypes.Rows.Clear();
                dgvMoveTypes.Enabled = false;
                return;
            }

            AllowEvent = false;

            dgvMoveTypes.Enabled = true;
            dgvMoveTypes.Rows.Clear();

            for (int i = 0; i < TerrainHolder.ListMoveType.Count; ++i)
            {
                dgvMoveTypes.Rows.Add();
                dgvMoveTypes.Rows[i].Cells[0].Value = TerrainHolder.ListMoveType[i];
                if (!TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType.ContainsKey((byte)i))
                {
                    TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType.Add((byte)i, 0);
                }
                dgvMoveTypes.Rows[i].Cells[1].Value = TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType[(byte)i];
            }

            AllowEvent = true;
        }

        #region Terrain

        private void lsTerrains_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsTerrains.SelectedIndex < 0 || !AllowEvent)
            {
                UpdateGridView();
                return;
            }

            AllowEvent = false;

            txtTerrainName.Text = TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].TerrainName.ToString();
            TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].DefenceValue = (byte)txtTerrainDefenceValue.Value;

            UpdateGridView();

            AllowEvent = true;
        }

        private void txtTerrainName_TextChanged(object sender, EventArgs e)
        {
            if (lsTerrains.SelectedIndex < 0 || !AllowEvent)
            {
                return;
            }

            AllowEvent = false;

            lsTerrains.Items[lsTerrains.SelectedIndex] = txtTerrainName.Text;
            TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].TerrainName = txtTerrainName.Text;

            AllowEvent = true;
        }

        private void txtTerrainDefenceValue_ValueChanged(object sender, EventArgs e)
        {
            if (lsTerrains.SelectedIndex < 0 || !AllowEvent)
            {
                return;
            }

            AllowEvent = false;

            txtTerrainDefenceValue.Value = TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].DefenceValue;

            AllowEvent = true;
        }

        private void btnAddNewTerrain_Click(object sender, EventArgs e)
        {
            TerrainHolder.ListConquestTerrainType.Add(new ConquestTerrainTypeAttributes());
            lsTerrains.Items.Add("New Terrain");
        }

        private void btnDeleteTerrain_Click(object sender, EventArgs e)
        {
            if (lsTerrains.SelectedIndex < 0)
            {
                return;
            }

            int CurrentIndex = lsTerrains.SelectedIndex;
            TerrainHolder.ListConquestTerrainType.RemoveAt(CurrentIndex);
            lsTerrains.Items.RemoveAt(CurrentIndex);

            if (CurrentIndex >= lsTerrains.Items.Count)
            {
                CurrentIndex = lsTerrains.Items.Count - 1;
            }

            lsTerrains.SelectedIndex = CurrentIndex;
        }

        #endregion

        private void tsmSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        #region Move Types

        private void lsMoveTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsMoveTypes.SelectedIndex < 0)
            {
                return;
            }

            txtMoveTypeName.Text = lsMoveTypes.Items[lsMoveTypes.SelectedIndex].ToString();
        }

        #endregion

        private void txtMoveTypeName_TextChanged(object sender, EventArgs e)
        {
            if (lsMoveTypes.SelectedIndex < 0)
            {
                return;
            }

            lsMoveTypes.Items[lsMoveTypes.SelectedIndex] = txtMoveTypeName.Text;
            TerrainHolder.ListMoveType[lsMoveTypes.SelectedIndex] = txtMoveTypeName.Text;

            UpdateGridView();
        }

        private void btnAddNewMoveType_Click(object sender, EventArgs e)
        {
            TerrainHolder.ListMoveType.Add("New Move Type");
            lsMoveTypes.Items.Add("New Move Type");
        }

        private void btnDeleteMoveType_Click(object sender, EventArgs e)
        {
            if (lsMoveTypes.SelectedIndex < 0)
            {
                return;
            }

            int CurrentIndex = lsMoveTypes.SelectedIndex;
            TerrainHolder.ListMoveType.RemoveAt(CurrentIndex);
            lsMoveTypes.Items.RemoveAt(CurrentIndex);

            if (CurrentIndex >= lsMoveTypes.Items.Count)
            {
                CurrentIndex = lsMoveTypes.Items.Count - 1;
            }

            lsMoveTypes.SelectedIndex = CurrentIndex;
        }

        private void dgvMoveTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (lsTerrains.SelectedIndex < 0 || !AllowEvent)
            {
                return;
            }

            for (byte i = 0; i < TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType.Count; ++i)
            {
                DataGridViewRow ActiveRow = dgvMoveTypes.Rows[i];
                TerrainHolder.ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType[i] = Convert.ToByte(ActiveRow.Cells[1].Value);
            }
        }
    }
}
