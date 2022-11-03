using System;
using ProjectEternity.Editors.MapEditor;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    public partial class SorcererStreetMapStatistics : MapStatistics
    {
        public SorcererStreetMapStatistics()
            : base()
        {
            InitializeComponent();
        }

        public SorcererStreetMapStatistics(SorcererStreetMap Map)
            : base(Map)
        {
            InitializeComponent();
            txtMagicAtStart.Value = Map.MagicAtStart;
            txtMagicPerLap.Value = Map.MagicGainPerLap;
            txtMagicPerTower.Value = Map.TowerMagicGain;
            txtMagicGoal.Value = Map.MagicGoal;
            txtHighestDieRoll.Value = Map.HighestDieRoll;
        }
    }
}
