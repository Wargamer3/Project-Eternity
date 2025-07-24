using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class ProjectEternityConquestTerrainsAndMoveTypesEditor : Form
    {
        private class ConquestTerrainType
        {
            public string TerrainName;
            public byte DefenceValue;
            public Dictionary<byte, byte> DicMovementCostByMoveType;

            public ConquestTerrainType()
            {
                TerrainName = "New Terrain";
                DefenceValue = 1;

                DicMovementCostByMoveType = new Dictionary<byte, byte>();
            }

            public ConquestTerrainType(BinaryReader BR)
            {
                TerrainName = BR.ReadString();
                DefenceValue = BR.ReadByte();

                int ListeMovementCostByMoveTypeCount = BR.ReadInt32();
                DicMovementCostByMoveType = new Dictionary<byte, byte>(ListeMovementCostByMoveTypeCount);

                for (int i = 0; i < ListeMovementCostByMoveTypeCount; ++i)
                {
                    DicMovementCostByMoveType.Add(BR.ReadByte(), BR.ReadByte());
                }
            }

            public void Save(BinaryWriter BW)
            {
                BW.Write(TerrainName);
                BW.Write(DefenceValue);

                BW.Write(DicMovementCostByMoveType.Count);

                foreach(KeyValuePair<byte, byte> MovementCostByMoveType in DicMovementCostByMoveType)
                {
                    BW.Write(MovementCostByMoveType.Key);
                    BW.Write(MovementCostByMoveType.Value);
                }
            }
        }

        private List<string> ListMoveType;
        private List<ConquestTerrainType> ListConquestTerrainType;

        private bool AllowEvent;

        public ProjectEternityConquestTerrainsAndMoveTypesEditor()
        {
            InitializeComponent();

            AllowEvent = true;
            ListMoveType = new List<string>();
            ListConquestTerrainType = new List<ConquestTerrainType>();

        }

        private void ProjectEternityConquestTerrainsAndMoveTypesEditor_Load(object sender, EventArgs e)
        {
            FileStream FS = new FileStream("Content/Conquest Terrains And Movements.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);

            int ListMoveTypeCount = BR.ReadInt32();
            ListMoveType = new List<string>(ListMoveTypeCount);
            for (int i = 0; i < ListMoveTypeCount; ++i)
            {
                ListMoveType.Add(BR.ReadString());
                lsMoveTypes.Items.Add(ListMoveType[i]);
            }

            int ListConquestTerrainTypeCount = BR.ReadInt32();
            ListConquestTerrainType = new List<ConquestTerrainType>(ListConquestTerrainTypeCount);
            for (int i = 0; i < ListMoveTypeCount; ++i)
            {
                ListConquestTerrainType.Add(new ConquestTerrainType(BR));
                lsTerrains.Items.Add(ListConquestTerrainType[i].TerrainName);
            }

            BR.Close();
            FS.Close();
        }

        private void Save()
        {
            FileStream FS = new FileStream("Content/Conquest Terrains And Movements.bin", FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.Unicode);

            BW.Write(ListMoveType.Count);
            foreach (string ActiveMoveType in ListMoveType)
            {
                BW.Write(ActiveMoveType);
            }

            BW.Write(ListConquestTerrainType.Count);
            foreach (ConquestTerrainType ActiveTerrainType in ListConquestTerrainType)
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

            for (int i = 0; i < ListMoveType.Count; ++i)
            {
                dgvMoveTypes.Rows.Add();
                dgvMoveTypes.Rows[i].Cells[0].Value = ListMoveType[i];
                if (!ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType.ContainsKey((byte)i))
                {
                    ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType.Add((byte)i, 0);
                }
                dgvMoveTypes.Rows[i].Cells[1].Value = ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType[(byte)i];
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

            txtTerrainName.Text = ListConquestTerrainType[lsTerrains.SelectedIndex].TerrainName.ToString();
            ListConquestTerrainType[lsTerrains.SelectedIndex].DefenceValue = (byte)txtTerrainDefenceValue.Value;

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
            ListConquestTerrainType[lsTerrains.SelectedIndex].TerrainName = txtTerrainName.Text;

            AllowEvent = true;
        }

        private void txtTerrainDefenceValue_ValueChanged(object sender, EventArgs e)
        {
            if (lsTerrains.SelectedIndex < 0 || !AllowEvent)
            {
                return;
            }

            AllowEvent = false;

            txtTerrainDefenceValue.Value = ListConquestTerrainType[lsTerrains.SelectedIndex].DefenceValue;

            AllowEvent = true;
        }

        private void btnAddNewTerrain_Click(object sender, EventArgs e)
        {
            ListConquestTerrainType.Add(new ConquestTerrainType());
            lsTerrains.Items.Add("New Terrain");
        }

        private void btnDeleteTerrain_Click(object sender, EventArgs e)
        {
            if (lsTerrains.SelectedIndex < 0)
            {
                return;
            }

            int CurrentIndex = lsTerrains.SelectedIndex;
            ListConquestTerrainType.RemoveAt(CurrentIndex);
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
            ListMoveType[lsMoveTypes.SelectedIndex] = txtMoveTypeName.Text;

            UpdateGridView();
        }

        private void btnAddNewMoveType_Click(object sender, EventArgs e)
        {
            ListMoveType.Add("New Move Type");
            lsMoveTypes.Items.Add("New Move Type");
        }

        private void btnDeleteMoveType_Click(object sender, EventArgs e)
        {
            if (lsMoveTypes.SelectedIndex < 0)
            {
                return;
            }

            int CurrentIndex = lsMoveTypes.SelectedIndex;
            ListMoveType.RemoveAt(CurrentIndex);
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

            for (byte i = 0; i < ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType.Count; ++i)
            {
                DataGridViewRow ActiveRow = dgvMoveTypes.Rows[i];
                ListConquestTerrainType[lsTerrains.SelectedIndex].DicMovementCostByMoveType[i] = Convert.ToByte(ActiveRow.Cells[1].Value);
            }
        }
    }
}
