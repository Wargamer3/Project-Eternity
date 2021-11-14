using System;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelSorcererStreet : BattleMapActionPanel
    {
        protected SorcererStreetMap Map;

        public ActionPanelSorcererStreet(string Name, SorcererStreetMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, new KeyboardInput(), CanCancel)
        {
            this.Map = Map;
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
