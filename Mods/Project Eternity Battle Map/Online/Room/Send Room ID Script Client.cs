using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class SendRoomIDScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Send Room ID";

        private readonly BattleMapOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private readonly GameScreen ScreenOwner;
        private readonly string RoomName;
        private readonly string RoomType;
        private readonly string RoomSubtype;
        private readonly byte MinNumberOfPlayer;
        private readonly byte MaxNumberOfPlayer;

        private string RoomID;
        private GameScreen NewScreen;

        public SendRoomIDScriptClient(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, GameScreen ScreenOwner,
            string RoomName, string RoomType, string RoomSubtype, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
            : base(ScriptName)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.ScreenOwner = ScreenOwner;
            this.RoomName = RoomName;
            this.RoomType = RoomType;
            this.RoomSubtype = RoomSubtype;
            this.MinNumberOfPlayer = MinNumberOfPlayer;
            this.MaxNumberOfPlayer = MaxNumberOfPlayer;
        }

        public override OnlineScript Copy()
        {
            return new SendRoomIDScriptClient(OnlineGameClient, OnlineCommunicationClient, ScreenOwner, RoomName, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            OnlineGameClient.RoomID = RoomID;

            Dictionary<string, OnlineScript> DicNewGameServerScript = new Dictionary<string, OnlineScript>();
            GamePreparationScreen NewMissionSelectScreen;

            PVPRoomInformations MissionRoom = new PVPRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer);

            GamePreparationScreen NewMissionSelect = new GamePreparationScreen(OnlineGameClient, OnlineCommunicationClient, MissionRoom);
            NewScreen = NewMissionSelect;
            NewMissionSelectScreen = NewMissionSelect;

            DicNewGameServerScript.Add(CreateGameScriptClient.ScriptName, new CreateGameScriptClient(OnlineGameClient, ScreenOwner.ListGameScreen, MissionRoom));
            DicNewGameServerScript.Add(ChangeRoomExtrasMissionScriptClient.ScriptName, new ChangeRoomExtrasMissionScriptClient(MissionRoom, NewMissionSelectScreen));

            DicNewGameServerScript.Add(PlayerJoinedScriptClient.ScriptName, new PlayerJoinedScriptClient(NewMissionSelectScreen));
            DicNewGameServerScript.Add(PlayerLeftScriptClient.ScriptName, new PlayerLeftScriptClient(MissionRoom, OnlineGameClient, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangeLoadoutScriptClient.ScriptName, new ChangeLoadoutScriptClient(MissionRoom, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangePlayerTypeScriptClient.ScriptName, new ChangePlayerTypeScriptClient(MissionRoom, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangeTeamScriptClient.ScriptName, new ChangeTeamScriptClient(MissionRoom));
            DicNewGameServerScript.Add(ChangeMapScriptClient.ScriptName, new ChangeMapScriptClient(MissionRoom, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangeRoomSubtypeScriptClient.ScriptName, new ChangeRoomSubtypeScriptClient(NewMissionSelectScreen));

            Host.AddOrReplaceScripts(DicNewGameServerScript);

            if (OnlineCommunicationClient.Host != null)
            {
                Dictionary<string, OnlineScript> DicNewCommunicationServerScript = new Dictionary<string, OnlineScript>();
                DicNewCommunicationServerScript.Add(ReceiveGroupMessageScriptClient.ScriptName, new ReceiveGroupMessageScriptClient(OnlineCommunicationClient));
                OnlineCommunicationClient.Host.AddOrReplaceScripts(DicNewCommunicationServerScript);

                OnlineCommunicationClient.Chat.InsertTab(RoomID, "Chat");
                OnlineCommunicationClient.Chat.CloseTab("Global");
                OnlineCommunicationClient.Host.Send(new CreateOrJoinCommunicationGroupScriptClient(RoomID, false));
                OnlineCommunicationClient.Host.Send(new LeaveCommunicationGroupScriptClient("Global"));
            }

            Host.Send(new AskChangeLoadoutScriptClient(PlayerManager.ListLocalPlayer[0]));

            ScreenOwner.RemoveScreen(ScreenOwner);

            OnlineGameClient.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            ScreenOwner.PushScreen(NewScreen);
        }

        protected override void Read(OnlineReader Sender)
        {
            RoomID = Sender.ReadString();
        }
    }
}
