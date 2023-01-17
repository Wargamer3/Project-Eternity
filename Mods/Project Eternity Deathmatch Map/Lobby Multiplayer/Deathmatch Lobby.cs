using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DeathmatchMapLobby : Lobby
    {
        public DeathmatchMapLobby(bool UseOnline)
            : base(UseOnline)
        {
        }

        protected override void PopulateGameClientScripts(Dictionary<string, OnlineScript> DicOnlineGameClientScripts)
        {
            base.PopulateGameClientScripts(DicOnlineGameClientScripts);
            DicOnlineGameClientScripts.Add(PlayerInventoryScriptClient.ScriptName, new PlayerInventoryScriptClient());
        }

        protected override void InitOfflinePlayer()
        {
            Player NewPlayer = new Player(PlayerManager.OnlinePlayerID, null, OnlinePlayerBase.PlayerTypePlayer, true, false, 0, Color.Blue);

            PlayerManager.ListLocalPlayer.Add(NewPlayer);
            NewPlayer.LoadLocally(GameScreen.ContentFallback);
        }

        protected override void CreateARoom()
        {
            PushScreen(new CreateRoomScreen(OnlineGameClient, OnlineCommunicationClient, "Deathmatch"));
            sndButtonClick.Play();
        }
    }
}
