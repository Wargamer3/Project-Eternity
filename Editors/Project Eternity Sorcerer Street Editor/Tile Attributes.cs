using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    public partial class TileAttributes : Form, ITileAttributes
    {
        public byte TerrainTypeIndex;
        public float Height;

        TerrainSorcererStreet ActiveTerrain;

        Terrain ITileAttributes.ActiveTerrain => ActiveTerrain;

        public TileAttributes()
        {
            InitializeComponent();
        }

        public TileAttributes(byte TerrainTypeIndex, float Height)
        {
            InitializeComponent();

            cboTerrainType.SelectedIndex = TerrainTypeIndex;
            txtHeight.Value = (decimal)Height;
        }

        public void Init(Terrain ActiveTerrain,  BattleMap Map)
        {
            ActiveTerrain = this.ActiveTerrain = new TerrainSorcererStreet((TerrainSorcererStreet)ActiveTerrain, ActiveTerrain.InternalPosition, ActiveTerrain.LayerIndex);
            cboTerrainType.SelectedIndex = ActiveTerrain.TerrainTypeIndex;
        }

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;
        }

        private void txtHeight_ValueChanged(object sender, EventArgs e)
        {
            Height = (float)txtHeight.Value;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public string GetTerrainName(int Index)
        {
            return cboTerrainType.Items[Index].ToString();
        }
    }
}
