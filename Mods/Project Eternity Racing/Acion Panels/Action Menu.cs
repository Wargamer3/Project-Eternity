using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public abstract class ActionPanelRacing : ActionPanel
    {
        protected Vehicule ActiveVehicule;

        public ActionPanelRacing(string Name, Vehicule ActiveVehicule, bool CanCancel = true)
            : base(Name, ActiveVehicule.ListActionMenuChoice, CanCancel)
        {
            this.ActiveVehicule = ActiveVehicule;
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
