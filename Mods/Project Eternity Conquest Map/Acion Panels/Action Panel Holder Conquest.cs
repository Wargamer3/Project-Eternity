using System;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelHolderConquest : ActionPanelHolder
    {
        private readonly ConquestMap Map;

        public ActionPanelHolderConquest(ConquestMap Map)
        {
            this.Map = Map;
        }

        public override void Add(ActionPanel NewActionPanel)
        {
            base.Add(NewActionPanel);

            if (Map.IsOnlineClient)
            {
                Map.OnlineClient.Host.Send(new OpenMenuScriptClient(NewActionPanel));
            }
            else if (Map.IsServer && !Map.ListPlayer[Map.ActivePlayerIndex].IsPlayerControlled)
            {
                foreach (IOnlineConnection ActivePlayer in Map.GameGroup.Room.ListUniqueOnlineConnection)
                {
                    ActivePlayer.Send(new OpenMenuScriptClient(NewActionPanel));
                }
            }
        }

        public override void AddToPanelListAndSelect(ActionPanel NewActionPanel)
        {
            base.AddToPanelListAndSelect(NewActionPanel);

            if (Map.IsOnlineClient)
            {
                Map.OnlineClient.Host.Send(new OpenMenuScriptClient(NewActionPanel));
            }
            else if (Map.IsServer && !Map.ListPlayer[Map.ActivePlayerIndex].IsPlayerControlled)
            {
                foreach (IOnlineConnection ActivePlayer in Map.GameGroup.Room.ListUniqueOnlineConnection)
                {
                    ActivePlayer.Send(new OpenMenuScriptClient(NewActionPanel));
                }
            }
        }
    }
}
