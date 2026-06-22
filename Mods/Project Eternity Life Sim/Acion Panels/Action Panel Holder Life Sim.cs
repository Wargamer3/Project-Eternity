using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelHolderLifeSim : ActionPanelHolder
    {
        private readonly NavMapGameManager MapManager;

        public ActionPanelHolderLifeSim(NavMapGameManager MapManager)
        {
            this.MapManager = MapManager;
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
