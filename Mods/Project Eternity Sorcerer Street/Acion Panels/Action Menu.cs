using System;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelSorcererStreet : BattleMapActionPanel
    {
        protected override PlayerInput ActiveInputManager => _ActiveInputManager;

        protected SorcererStreetMap Map;
        protected PlayerInput _ActiveInputManager;

        public ActionPanelSorcererStreet(string Name, SorcererStreetMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, CanCancel)
        {
            this.Map = Map;

            if (Map.ListPlayer.Count > 0)
            {
                _ActiveInputManager = Map.ListPlayer[Map.ActivePlayerIndex].InputManager;
            }
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }
    }
}
