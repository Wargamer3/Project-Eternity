using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimMapEditor
{
    public partial class LifeSimTileAttributes : Form, ITileAttributes
    {
        public byte TerrainTypeIndex;
        public float TerrainHeight;

        Terrain ActiveTerrain;

        Terrain ITileAttributes.ActiveTerrain => ActiveTerrain;

        public LifeSimTileAttributes()
        {
            InitializeComponent();
        }

        public LifeSimTileAttributes(byte TerrainTypeIndex, float Height)
        {
            InitializeComponent();

            cboTerrainType.SelectedIndex = TerrainTypeIndex;
            txtHeight.Value = (decimal)Height;
        }

        public void Init(Terrain ActiveTerrain,  BattleMap Map)
        {
            ActiveTerrain = this.ActiveTerrain = new Terrain((Terrain)ActiveTerrain, ActiveTerrain.GridPosition, ActiveTerrain.LayerIndex);
            cboTerrainType.SelectedIndex = ActiveTerrain.TerrainTypeIndex;
        }

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;
        }

        private void txtHeight_ValueChanged(object sender, EventArgs e)
        {
            TerrainHeight = (float)txtHeight.Value;
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
