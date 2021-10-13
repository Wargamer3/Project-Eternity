using System;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public class BattleMapActionPanelHolder : ActionPanelHolder
    {
        private readonly BattleMap Map;

        public BattleMapActionPanelHolder(BattleMap Map)
        {
            this.Map = Map;
        }

        public override void Add(ActionPanel NewActionPanel)
        {
            base.Add(NewActionPanel);

            if (!Map.IsOfflineOrServer)
            {
                Map.OnlineClient.Host.Send(new OpenMenuScriptClient(NewActionPanel));
            }
        }

        public override void AddToPanelListAndSelect(ActionPanel NewActionPanel)
        {
            base.AddToPanelListAndSelect(NewActionPanel);

            if (!Map.IsOfflineOrServer)
            {
                Map.OnlineClient.Host.Send(new OpenMenuScriptClient(NewActionPanel));
            }
        }
    }
}
