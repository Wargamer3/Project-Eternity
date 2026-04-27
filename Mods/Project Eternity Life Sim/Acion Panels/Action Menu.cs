using System;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class ActionPanelLifeSim : BattleMapActionPanel
    {
        protected LifeSimMap Map;

        public ActionPanelLifeSim(string Name, LifeSimMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, null, CanCancel)
        {
            this.Map = Map;
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
