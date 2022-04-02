using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class MapOverlays : Form
    {
        public MapOverlays()
        {
            InitializeComponent();
        }

        #region Time periods

        private void btnAddTimePeriod_Click(object sender, EventArgs e)
        {
            lsTimePeriod.Items.Add("New Time Period");
        }

        private void btnRemoveTimePeriod_Click(object sender, EventArgs e)
        {

        }

        private void txtTimePeriodTimeStart_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtTimePeriodDayStart_ValueChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Time of day

        private void cbWeather_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbTypeOfSky_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbVisibility_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtlblTimeMultiplier_ValueChanged(object sender, EventArgs e)
        {

        }

        private void rbUseTurns_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbUseRealTime_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

        private void txtWindSpeed_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtWindDirection_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
