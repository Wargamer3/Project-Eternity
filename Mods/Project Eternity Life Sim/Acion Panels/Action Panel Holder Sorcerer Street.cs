using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelHolderLifeSim : ActionPanelHolder
    {
        private readonly LifeSimMap Map;

        public ActionPanelHolderLifeSim(LifeSimMap Map)
        {
            this.Map = Map;
        }

        public override void Add(ActionPanel NewActionPanel)
        {
            base.Add(NewActionPanel);
        }

        public override void AddToPanelListAndSelect(ActionPanel NewActionPanel)
        {
            base.AddToPanelListAndSelect(NewActionPanel);
        }
    }
}
