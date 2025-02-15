using System;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelSorcererStreet : BattleMapActionPanel
    {
        protected SorcererStreetMap Map;

        public ActionPanelSorcererStreet(string Name, SorcererStreetMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, null, CanCancel)
        {
            this.Map = Map;

            if (Map.ListPlayer.Count > 0)
            {
                ActiveInputManager = Map.ListPlayer[Map.ActivePlayerIndex].InputManager;
            }
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
