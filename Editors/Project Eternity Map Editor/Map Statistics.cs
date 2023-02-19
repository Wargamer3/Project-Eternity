using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class MapStatistics : Form
    {
        public List<string> ListBackgroundsPath;
        public List<string> ListForegroundsPath;

        private enum ItemSelectionChoices { Backgrounds, Foregrounds };
        private ItemSelectionChoices ItemSelectionChoice;

        public DefaultGameModesConditions frmDefaultGameModesConditions;

        public MapStatistics()
        {
            InitializeComponent();
        }

        public MapStatistics(BattleMap ActiveMap)
        {
            InitializeComponent();
            txtMapName.Text = ActiveMap.MapName;
            txtMapWidth.Text = ActiveMap.MapSize.X.ToString();
            txtMapHeight.Text = ActiveMap.MapSize.Y.ToString();
            txtTileWidth.Text = ActiveMap.TileSize.X.ToString();
            txtTileHeight.Text = ActiveMap.TileSize.Y.ToString();
            cbCameraType.SelectedIndex = 0;
            if (ActiveMap.CameraType == "3D")
            {
                cbCameraType.SelectedIndex = 1;
            }
            txtCameraStartPositionX.Value = (int)Math.Max(0, ActiveMap.CameraPosition.X);
            txtCameraStartPositionY.Value = (int)Math.Max(0, ActiveMap.CameraPosition.Y);
            txtOrderNumber.Value = ActiveMap.OrderNumber;
            txtDescription.Text = ActiveMap.Description;
            ListBackgroundsPath = new List<string>();
            ListForegroundsPath = new List<string>();
            txtTimeStart.Value = (decimal)ActiveMap.MapEnvironment.TimeStart;
            txtHoursInDay.Value = (decimal)ActiveMap.MapEnvironment.HoursInDay;

            if (ActiveMap.MapEnvironment.TimeLoopType == EnvironmentManager.TimeLoopTypes.FirstDay)
            {
                rbLoopFirstDay.Checked = true;
            }
            else if (ActiveMap.MapEnvironment.TimeLoopType == EnvironmentManager.TimeLoopTypes.LastDay)
            {
                rbLoopLastDay.Checked = true;
            }
            else
            {
                rbStopTime.Checked = true;
            }

            txtlblTimeMultiplier.Value = (decimal)ActiveMap.MapEnvironment.TimeMultiplier;
            if (ActiveMap.MapEnvironment.TimePeriodType == EnvironmentManager.TimePeriods.Turns)
            {
                rbUseTurns.Checked = true;
            }
            else
            {
                rbUseRealTime.Checked = true;
            }

            frmDefaultGameModesConditions = new DefaultGameModesConditions(ActiveMap);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
        }

        private void btnSetBackgrounds_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Backgrounds;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundsAll, "Select which backgrounds to use.", true));
        }

        private void btnSetForegrounds_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Foregrounds;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundsAll, "Select which foregrounds to use.", true));
        }

        private void btnSetDefaultGameModesConditions_Click(object sender, EventArgs e)
        {
            frmDefaultGameModesConditions.ShowDialog();
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                string BackgroundName = Items[I].Substring(0, Items[0].Length - 5).Substring(19);

                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Backgrounds:
                        ListBackgroundsPath.Clear();
                        ListBackgroundsPath.Add(BackgroundName);
                        break;

                    case ItemSelectionChoices.Foregrounds:
                        ListForegroundsPath.Clear();
                        ListForegroundsPath.Add(BackgroundName);
                        break;
                }
            }
        }
    }
}
