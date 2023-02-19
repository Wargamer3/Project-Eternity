using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class DefaultGameModesConditions : Form
    {
        public DefaultGameModesConditions()
        {
            InitializeComponent();
        }

        public DefaultGameModesConditions(BattleMap ActiveMap)
        {
            InitializeComponent();

            txtPlayersMin.Value = ActiveMap.PlayersMin;
            txtPlayersMax.Value = ActiveMap.PlayersMax;

            foreach (GameModeInfo ActiveGameType in ActiveMap.ListGameType)
            {
                lstGameModes.Items.Add(ActiveGameType);
            }

            foreach (string ActiveMutatorName in ActiveMap.ListMandatoryMutator)
            {
                dgvMandatoryMutators.Rows.Add(ActiveMutatorName);
            }

            foreach (GameModeInfo ActiveGameType in ActiveMap.GetAvailableGameModes().Values)
            {
                cbGameMode.Items.Add(ActiveGameType);
            }
        }

        private void btnAddGameMode_Click(object sender, EventArgs e)
        {
            lstGameModes.Items.Add(((GameModeInfo)cbGameMode.Items[0]).Copy());
        }

        private void btnRemoveGameMode_Click(object sender, EventArgs e)
        {
            if (lstGameModes.SelectedIndex >= 0)
            {
                lstGameModes.Items.RemoveAt(lstGameModes.SelectedIndex);
            }
        }

        private void lstGameModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            pgGameModeAttributes.SelectedObject = lstGameModes.SelectedItem;
            cbGameMode.SelectedText = lstGameModes.SelectedItem.ToString();
        }

        private void cbGameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstGameModes.SelectedIndex >= 0)
            {
                lstGameModes.Items[lstGameModes.SelectedIndex] = ((GameModeInfo)cbGameMode.SelectedItem).Copy();
            }
        }
    }
}
