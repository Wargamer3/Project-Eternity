using System;
using ProjectEternity.Editors.MapEditor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimMapEditor
{
    public partial class LifeSimMapStatistics : MapStatistics
    {
        public LifeSimMapStatistics()
            : base()
        {
            InitializeComponent();
        }

        public LifeSimMapStatistics(LifeSimMap Map)
            : base(Map)
        {
            InitializeComponent();
        }
    }
}
