using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.TilesetEditor
{
    public partial class TileEditor : Form
    {
        private enum ItemSelectionChoices { Tile, BattleBackgroundAnimation };

        private ItemSelectionChoices ItemSelectionChoice;

        private Terrain ActiveTerrain;

        public TileEditor()
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

            cboBattleAnimationBackground.Items.Add("None");
        }

        public void LoadTileset(Terrain.TilesetPreset NewTilesetPreset)
        {
            cboBattleAnimationBackground.Items.Clear();
            cboBattleAnimationBackground.Items.Add("None");

            foreach (string BattleBackgroundAnimationPath in NewTilesetPreset.ListBattleBackgroundAnimationPath)
            {
                if (!string.IsNullOrEmpty(BattleBackgroundAnimationPath) && !cboBattleAnimationBackground.Items.Contains(BattleBackgroundAnimationPath))
                {
                    cboBattleAnimationBackground.Items.Add(BattleBackgroundAnimationPath);
                }
            }
        }

        public void SelectTerrain(Terrain ActiveTerrain)
        {
            this.ActiveTerrain = ActiveTerrain;
            cboTerrainType.SelectedIndex = ActiveTerrain.TerrainTypeIndex;

            if (ActiveTerrain.BonusInfo.BattleBackgroundAnimationIndex <= cboBattleAnimationBackground.Items.Count)
            {
                cboBattleAnimationBackground.SelectedIndex = ActiveTerrain.BonusInfo.BattleBackgroundAnimationIndex;
            }
            else
            {
                cboBattleAnimationBackground.SelectedIndex = 0;
            }

            lstTerrainBonus.Items.Clear();

            //Load the lstTerrainBonus.
            for (int i = 0; i < ActiveTerrain.BonusInfo.ListActivation.Length; i++)
            {
                string ActiveBonus = cboTerrainBonusType.Items[(int)ActiveTerrain.BonusInfo.ListBonus[i]].ToString();
                string ActiveBonusValue = ActiveTerrain.BonusInfo.ListBonusValue[i].ToString();
                string ActiveBonusActivation = cboTerrainBonusActivation.Items[(int)ActiveTerrain.BonusInfo.ListActivation[i]].ToString();

                lstTerrainBonus.Items.Add((i + 1) + ". " + ActiveBonus + " (" + ActiveBonusValue.ToString() + " ) - " + ActiveBonusActivation);
            }

            if (lstTerrainBonus.Items.Count > 0)
            {
                lstTerrainBonus.SelectedIndex = 0;
            }
        }

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveTerrain != null)
            {
                ActiveTerrain.TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;
            }
        }

        private void btnAddNewBonus_Click(object sender, EventArgs e)
        {
            if (ActiveTerrain != null)
            {
                lstTerrainBonus.Items.Add((lstTerrainBonus.Items.Count + 1) + ". HP regen (5 ) - On every turn");

                int LastBonusIndex = ActiveTerrain.BonusInfo.ListActivation.Length;

                Array.Resize(ref ActiveTerrain.BonusInfo.ListActivation, ActiveTerrain.BonusInfo.ListActivation.Length + 1);
                Array.Resize(ref ActiveTerrain.BonusInfo.ListBonus, ActiveTerrain.BonusInfo.ListBonus.Length + 1);
                Array.Resize(ref ActiveTerrain.BonusInfo.ListBonusValue, ActiveTerrain.BonusInfo.ListBonusValue.Length + 1);

                ActiveTerrain.BonusInfo.ListActivation[LastBonusIndex] = TerrainActivation.OnEveryTurns;
                ActiveTerrain.BonusInfo.ListBonus[LastBonusIndex] = TerrainBonus.HPRegen;
                ActiveTerrain.BonusInfo.ListBonusValue[LastBonusIndex] = 5;
            }
        }

        private void btnRemoveBonus_Click(object sender, EventArgs e)
        {
            if (ActiveTerrain != null && lstTerrainBonus.SelectedIndex >= 0)
            {
                int Index = lstTerrainBonus.SelectedIndex;
                Array.Resize(ref ActiveTerrain.BonusInfo.ListActivation, ActiveTerrain.BonusInfo.ListActivation.Length - 1);
                Array.Resize(ref ActiveTerrain.BonusInfo.ListBonus, ActiveTerrain.BonusInfo.ListBonus.Length - 1);
                Array.Resize(ref ActiveTerrain.BonusInfo.ListBonusValue, ActiveTerrain.BonusInfo.ListBonusValue.Length - 1);
                lstTerrainBonus.Items.RemoveAt(lstTerrainBonus.SelectedIndex);

                if (lstTerrainBonus.Items.Count > 0)
                {
                    if (Index >= lstTerrainBonus.Items.Count)
                        lstTerrainBonus.SelectedIndex = lstTerrainBonus.Items.Count - 1;
                    else
                        lstTerrainBonus.SelectedIndex = Index;
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
            if (ActiveTerrain != null && lstTerrainBonus.SelectedIndex >= 0)
            {
                cboTerrainBonusActivation.SelectedIndex = (int)ActiveTerrain.BonusInfo.ListActivation[lstTerrainBonus.SelectedIndex];
                cboTerrainBonusType.SelectedIndex = (int)ActiveTerrain.BonusInfo.ListBonus[lstTerrainBonus.SelectedIndex];
                txtBonusValue.Text = ActiveTerrain.BonusInfo.ListBonusValue[lstTerrainBonus.SelectedIndex].ToString();
            }
        }

        private void cboTerrainBonusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveTerrain != null && lstTerrainBonus.SelectedIndex >= 0)
            {
                ActiveTerrain.BonusInfo.ListBonus[lstTerrainBonus.SelectedIndex] = (TerrainBonus)cboTerrainBonusType.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void cboTerrainBonusActivation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveTerrain != null && lstTerrainBonus.SelectedIndex >= 0)
            {
                ActiveTerrain.BonusInfo.ListActivation[lstTerrainBonus.SelectedIndex] = (TerrainActivation)cboTerrainBonusActivation.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void txtBonusValue_TextChanged(object sender, EventArgs e)
        {
            if (ActiveTerrain != null && lstTerrainBonus.SelectedIndex >= 0 && txtBonusValue.Text != "")
            {
                ActiveTerrain.BonusInfo.ListBonusValue[lstTerrainBonus.SelectedIndex] = (int)txtBonusValue.Value;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;
            }
        }

        private void cboBattleAnimationBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveTerrain != null)
            {
                ActiveTerrain.BonusInfo.BattleBackgroundAnimationIndex = (byte)cboBattleAnimationBackground.SelectedIndex;
            }
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
        }

        protected void ListMenuItemsSelected(List<string> Items)
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
                            cboBattleAnimationBackground.Items.Add(BackgroundPath.Substring(0, BackgroundPath.Length - 5).Substring(19));
                        }
                        break;
                }
            }
        }

    }
}
