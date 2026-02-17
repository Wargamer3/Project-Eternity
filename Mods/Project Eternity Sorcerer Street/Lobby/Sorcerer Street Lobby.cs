using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen.Online;
using static ProjectEternity.GameScreens.BattleMapScreen.GameOptionsSelectMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetLobby : LobbyWhite
    {
        public SorcererStreetLobby(bool UseOnline)
            : base(UseOnline)
        {
            MapInfo.MapInfoTemplate = new SorcererStreetMapInfo();
            Card.Init();
            MenuHelper.Init(Content);
            CardSymbols.Load(Content);
            IconHolder.Load(Content);
            Card.InitTextParser(null, Content, CardSymbols.Symbols, Content.Load<Microsoft.Xna.Framework.Graphics.SpriteFont>("Fonts/Arial30"), SorcererStreetMap.TextColor);
        }

        protected override void PopulateGameClientScripts(Dictionary<string, OnlineScript> DicOnlineGameClientScripts)
        {
            base.PopulateGameClientScripts(DicOnlineGameClientScripts);
            DicOnlineGameClientScripts.Add(PlayerInventoryScriptClient.ScriptName, new PlayerInventoryScriptClient());
            DicOnlineGameClientScripts[JoinRoomLocalScriptClient.ScriptName] = new JoinRoomLocalScriptClient(OnlineGameClient, OnlineCommunicationClient, this, false);
        }

        protected override void InitOfflinePlayer()
        {
            Player NewPlayer = new Player(PlayerManager.OnlinePlayerID, null, OnlinePlayerBase.PlayerTypes.Player, false, 0, true, Color.Blue);

            PlayerManager.ListLocalPlayer.Add(NewPlayer);
            NewPlayer.LoadLocally(GameScreen.ContentFallback);
        }

        protected override void SelectLocalPlayers()
        {
            PushScreen(new SorcererStreetLocalPlayerSelectionScreen());
            sndButtonClick.Play();
        }

        public override CreateRoomScreen CreateARoom()
        {
            var NewScreen = new SorcererStreetCreateRoomScreen(OnlineGameClient, OnlineCommunicationClient);

            PushScreen(NewScreen);
            sndButtonClick.Play();

            return NewScreen;
        }

        public override void OpenInventory()
        {
            PushScreen(new SorcererStreetInventoryScreen((Player)PlayerManager.ListLocalPlayer[0]));
            sndButtonClick.Play();
        }

        public override void OpenShop()
        {
            PushScreen(new ShopScreen(OnlineGameClient));
            sndButtonClick.Play();
        }

        protected override void OpenHelpMenu()
        {
            PushScreen(new IntroPopup(this));
        }
    }
}
