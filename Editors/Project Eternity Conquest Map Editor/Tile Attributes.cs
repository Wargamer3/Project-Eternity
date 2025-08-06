using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class TileAttributes : Form, ITileAttributes
    {
        Terrain ITileAttributes.ActiveTerrain => ActiveTerrain;

        public TerrainConquest ActiveTerrain;

        private ConquestTerrainHolder TerrainHolder;

        private bool AllowEvents;

        public TileAttributes()
        {
            InitializeComponent();
            TerrainHolder = new ConquestTerrainHolder();
        }

        public void Init(Terrain ActiveTerrain, BattleMap Map)
        {
            AllowEvents = false;
            TerrainHolder.LoadData();

            cboTerrainType.Items.Clear();
            for (int i = 0; i < TerrainHolder.ListConquestTerrainType.Count; ++i)
            {
                cboTerrainType.Items.Add(TerrainHolder.ListConquestTerrainType[i].TerrainName);
            }

            ActiveTerrain = this.ActiveTerrain = new TerrainConquest(ActiveTerrain, ActiveTerrain.GridPosition, ActiveTerrain.LayerIndex);
            cboTerrainType.SelectedIndex = ActiveTerrain.TerrainTypeIndex;

            AllowEvents = true;
        }

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
            {
                return;
            }

            ActiveTerrain.TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;
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
