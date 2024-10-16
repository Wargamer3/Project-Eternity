﻿using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class TileAttributes : Form, ITileAttributes
    {
        public int TerrainTypeIndex;//What kind of terrain it is.

        TerrainConquest ActiveTerrain;

        Terrain ITileAttributes.ActiveTerrain => ActiveTerrain;

        public TileAttributes()
        {
            InitializeComponent();
        }

        public TileAttributes(int TerrainTypeIndex)
        {
            InitializeComponent();

            cboTerrainType.SelectedIndex = TerrainTypeIndex;
        }

        public void Init(Terrain ActiveTerrain, BattleMap Map)
        {
            ActiveTerrain = this.ActiveTerrain = new TerrainConquest(ActiveTerrain, ActiveTerrain.GridPosition, ActiveTerrain.LayerIndex);
            cboTerrainType.SelectedIndex = ActiveTerrain.TerrainTypeIndex;
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
