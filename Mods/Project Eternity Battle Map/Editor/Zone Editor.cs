using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class ZoneEditor : Form
    {
        public readonly MapZone ZoneToEdit;

        public ZoneEditor()
        {
            InitializeComponent();
        }

        public ZoneEditor(MapZone ZoneToEdit)
        {
            this.ZoneToEdit = ZoneToEdit.Copy();

            InitializeComponent();

            foreach (TimePeriod ActiveTimePeriod in ZoneToEdit.ListTimePeriod)
            {
                lsTimePeriod.Items.Add(ActiveTimePeriod.Name);
            }

            lsTimePeriod.SelectedIndex = 0;
        }

        #region Time periods

        private void lsTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                txtTimePeriodName.Text = ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].Name;
                txtTimePeriodTimeStart.Value = (decimal)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].TimeStart;
                txtTimePeriodDayStart.Value = (decimal)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].DayStart;
                txtTimePeriodPassiveSkill.Text = ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].PassiveSkillPath;

                cbWeatherType.SelectedIndex = (int)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].WeatherType;
                cbSkyType.SelectedIndex = (int)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].SkyType;
                cbVisibilityType.SelectedIndex = (int)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].VisibilityType;

                txtWindSpeed.Value = (decimal)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].WindSpeed;
                txtWindDirection.Value = (decimal)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].WindDirection;

                txtTransitionStartLength.Value = (decimal)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].TransitionStartLength;
                txtTransitionEndLength.Value = (decimal)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].TransitionEndLength;
                txtTransitionCrossfadeLength.Value = (decimal)ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].CrossfadeLength;
            }
        }

        private void btnAddTimePeriod_Click(object sender, EventArgs e)
        {
            lsTimePeriod.Items.Add("New Time Period");
            ZoneToEdit.ListTimePeriod.Add(new TimePeriod("New Time Period"));
            lsTimePeriod.SelectedIndex = lsTimePeriod.Items.Count - 1;
        }

        private void btnRemoveTimePeriod_Click(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0 && lsTimePeriod.Items.Count > 1)
            {
                ZoneToEdit.ListTimePeriod.RemoveAt(lsTimePeriod.SelectedIndex);
                lsTimePeriod.Items.RemoveAt(lsTimePeriod.SelectedIndex);
                lsTimePeriod.SelectedIndex = lsTimePeriod.Items.Count - 1;
            }
        }

        private void txtTimePeriodName_TextChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                lsTimePeriod.Items[lsTimePeriod.SelectedIndex] = txtTimePeriodName.Text;
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].Name = txtTimePeriodName.Text;
            }
        }

        private void txtTimePeriodTimeStart_ValueChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].TimeStart = (float)txtTimePeriodTimeStart.Value;
            }
        }

        private void txtTimePeriodDayStart_ValueChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].DayStart = (float)txtTimePeriodDayStart.Value;
            }
        }

        private void btnTimePeriodPassiveSkill_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        #endregion

        #region Time of day

        private void cbWeatherType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].WeatherType = (TimePeriod.WeatherTypes)cbWeatherType.SelectedIndex;
            }
        }

        private void cbSkyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].SkyType = (TimePeriod.SkyTypes)cbSkyType.SelectedIndex;
            }
        }

        private void cbVisibilityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].VisibilityType = (TimePeriod.VisibilityTypes)cbVisibilityType.SelectedIndex;
            }
        }

        #endregion

        private void txtWindSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].WindSpeed = (float)txtWindSpeed.Value;
            }
        }

        private void txtWindDirection_ValueChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].WindDirection = (float)txtWindDirection.Value;
            }
        }

        private void txtTransitionStartLength_ValueChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].TransitionStartLength = (float)txtTransitionStartLength.Value;
            }
        }

        private void txtTransitionEndLength_ValueChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].TransitionEndLength = (float)txtTransitionEndLength.Value;
            }
        }

        private void txtTransitionCrossfadeLength_ValueChanged(object sender, EventArgs e)
        {
            if (lsTimePeriod.SelectedIndex >= 0)
            {
                ZoneToEdit.ListTimePeriod[lsTimePeriod.SelectedIndex].CrossfadeLength = (float)txtTransitionCrossfadeLength.Value;
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                if (Items[I] == null)
                {
                    txtTimePeriodPassiveSkill.Text = "None";
                }
                else
                {
                    Name = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                    txtTimePeriodPassiveSkill.Text = Name;
                }

            }
        }
    }
}
