using System;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelHolderDeathmatch : ActionPanelHolder
    {
        private readonly DeathmatchMap Map;

        public ActionPanelHolderDeathmatch(DeathmatchMap Map)
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
                foreach (IOnlineConnection ActivePlayer in Map.GameGroup.Room.ListOnlinePlayer)
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
                foreach (IOnlineConnection ActivePlayer in Map.GameGroup.Room.ListOnlinePlayer)
                {
                    ActivePlayer.Send(new OpenMenuScriptClient(NewActionPanel));
                }
            }
        }
    }
}
