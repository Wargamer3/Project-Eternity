﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetLobby : Lobby
    {
        public SorcererStreetLobby(bool UseOnline)
            : base(UseOnline)
        {
        }

        protected override void PopulateGameClientScripts(Dictionary<string, OnlineScript> DicOnlineGameClientScripts)
        {
            base.PopulateGameClientScripts(DicOnlineGameClientScripts);
            DicOnlineGameClientScripts.Add(PlayerInventoryScriptClient.ScriptName, new PlayerInventoryScriptClient());
            DicOnlineGameClientScripts[JoinRoomLocalScriptClient.ScriptName] = new JoinRoomLocalScriptClient(OnlineGameClient, OnlineCommunicationClient, this, false);
        }

        protected override void InitOfflinePlayer()
        {
            Player NewPlayer = new Player(PlayerManager.OnlinePlayerID, null, OnlinePlayerBase.PlayerTypes.Host, false, 0, true, Color.Blue);

            PlayerManager.ListLocalPlayer.Add(NewPlayer);
            NewPlayer.LoadLocally(GameScreen.ContentFallback);
        }

        protected override void SelectLocalPlayers()
        {
            PushScreen(new SorcererStreetLocalPlayerSelectionScreen());
            sndButtonClick.Play();
        }

        protected override void CreateARoom()
        {
            PushScreen(new SorcererStreetCreateRoomScreen(OnlineGameClient, OnlineCommunicationClient, "Sorcerer Street"));
            sndButtonClick.Play();
        }

        protected override void OpenInventory()
        {
            PushScreen(new SorcererStreetInventoryScreen((Player)PlayerManager.ListLocalPlayer[0]));
            sndButtonClick.Play();
        }
    }
}
