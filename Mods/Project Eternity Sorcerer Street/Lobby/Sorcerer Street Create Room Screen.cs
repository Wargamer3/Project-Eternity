using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using SendRoomIDScriptClient = ProjectEternity.GameScreens.SorcererStreetScreen.Online.SendRoomIDScriptClient;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetCreateRoomScreen : CreateRoomScreen
    {
        public SorcererStreetCreateRoomScreen(BattleMapOnlineClient OnlineClient, CommunicationClient OnlineCommunicationClient, string RoomType)
            : base(OnlineClient, OnlineCommunicationClient, RoomType)
        {
        }

        public override void CreateRoom()
        {
            sndButtonClick.Play();
            OKButton.Disable();

            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                Dictionary<string, OnlineScript> DicCreateRoomScript = new Dictionary<string, OnlineScript>();

                DicCreateRoomScript.Add(SendRoomIDScriptClient.ScriptName, new SendRoomIDScriptClient(OnlineGameClient, OnlineCommunicationClient, this,
                    RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer));

                OnlineGameClient.Host.AddOrReplaceScripts(DicCreateRoomScript);

                OnlineGameClient.CreateRoom(RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer);
            }
            else
            {
                PushScreen(new SorcererStreetGamePreparationScreen(null, null, new SorcererStreetRoomInformations("No ID needed", RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer)));
                RemoveScreen(this);
            }
        }
    }
}
