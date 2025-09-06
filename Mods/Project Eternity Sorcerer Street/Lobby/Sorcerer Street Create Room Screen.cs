using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetCreateRoomScreen : CreateRoomScreen
    {
        public SorcererStreetCreateRoomScreen(BattleMapOnlineClient OnlineClient, CommunicationClient OnlineCommunicationClient)
            : base(OnlineClient, OnlineCommunicationClient, "Sorcerer Street")
        {
        }

        public override GamePreparationScreen CreateRoom()
        {
            GamePreparationScreen NewScreen = new SorcererStreetGamePreparationScreen(null, null, new BattleMapRoomInformations("No ID needed", RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer));
            PlayerManager.ListLocalPlayer[0].OnlineClient.Roles.IsRoomHost = true;
            PushScreen(NewScreen);
            RemoveScreen(this);

            return NewScreen;
        }
    }
}
