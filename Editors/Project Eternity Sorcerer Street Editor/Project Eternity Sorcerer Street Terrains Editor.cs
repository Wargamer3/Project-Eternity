using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    public partial class ProjectEternitySorcererStreetTerrainsEditor : Form
    {
        private SorcererStreetTerrainHolder TerrainHolder;

        private bool AllowEvent;

        public ProjectEternitySorcererStreetTerrainsEditor()
        {
            InitializeComponent();

            AllowEvent = true;

            TerrainHolder = new SorcererStreetTerrainHolder();
        }

        private void ProjectEternityConquestTerrainsAndMoveTypesEditor_Load(object sender, EventArgs e)
        {
            TerrainHolder.LoadData();

            for (int i = 0; i < TerrainHolder.ListTerrainType.Count; ++i)
            {
                lsMoveTypes.Items.Add(TerrainHolder.ListTerrainType[i]);
            }
        }

        private void Save()
        {
            FileStream FS = new FileStream("Content/Sorcerer Street Terrains.bin", FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.Unicode);

            BW.Write(TerrainHolder.ListTerrainType.Count);
            foreach (string ActiveMoveType in TerrainHolder.ListTerrainType)
            {
                BW.Write(ActiveMoveType);
            }

            BW.Flush();
            BW.Close();
            FS.Close();
        }

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
            TerrainHolder.ListTerrainType[lsMoveTypes.SelectedIndex] = txtMoveTypeName.Text;
        }

        private void btnAddNewMoveType_Click(object sender, EventArgs e)
        {
            TerrainHolder.ListTerrainType.Add("New Move Type");
            lsMoveTypes.Items.Add("New Move Type");
        }

        private void btnDeleteMoveType_Click(object sender, EventArgs e)
        {
            if (lsMoveTypes.SelectedIndex < 0)
            {
                return;
            }

            int CurrentIndex = lsMoveTypes.SelectedIndex;
            TerrainHolder.ListTerrainType.RemoveAt(CurrentIndex);
            lsMoveTypes.Items.RemoveAt(CurrentIndex);

            if (CurrentIndex >= lsMoveTypes.Items.Count)
            {
                CurrentIndex = lsMoveTypes.Items.Count - 1;
            }

            lsMoveTypes.SelectedIndex = CurrentIndex;
        }
    }
}
