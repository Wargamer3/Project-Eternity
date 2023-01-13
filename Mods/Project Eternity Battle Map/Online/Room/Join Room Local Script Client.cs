using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class JoinRoomLocalScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Join Room Local";

        private readonly BattleMapOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private readonly GameScreen ScreenOwner;
        private readonly bool RemoveOwner;

        private string RoomID;
        private string RoomType;
        private string RoomSubtype;
        private List<string> ListJoiningPlayerID;
        private string RoomName;
        private string CurrentDifficulty;
        private string MapPath;
        private byte[] RoomData;
        private bool HasGame;
        private GamePreparationScreen NewScreen;

        public JoinRoomLocalScriptClient(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, GameScreen ScreenOwner, bool RemoveOwner = false)
            : base(ScriptName)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.ScreenOwner = ScreenOwner;
            this.RemoveOwner = RemoveOwner;

            HasGame = false;
            ListJoiningPlayerID = new List<string>();
        }

        public override OnlineScript Copy()
        {
            return new JoinRoomLocalScriptClient(OnlineGameClient, OnlineCommunicationClient, ScreenOwner, RemoveOwner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            OnlineGameClient.RoomID = RoomID;
            Host.Roles.IsRoomHost = false;
            Host.Roles.IsRoomReady = false;

            Dictionary<string, OnlineScript> DicNewGameServerScript = new Dictionary<string, OnlineScript>();
            RoomInformations NewRoom;
            //FightingZone NewFightingZone = null;

            if (HasGame)
            {
               /* NewFightingZone = new FightingZone(OnlineGameClient);
                NewFightingZone.ListGameScreen = ScreenOwner.ListGameScreen;

                DicNewGameServerScript = OnlineHelper.GetTripleThunderScriptsClient(OnlineGameClient);
                Host.IsGameReady = true;*/
            }

            BattleMapRoomInformations MissionRoom = new BattleMapRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapPath, ListJoiningPlayerID, ScreenOwner.Content, RoomData);

            NewRoom = MissionRoom;

            NewScreen = new GamePreparationScreen(OnlineGameClient, OnlineCommunicationClient, MissionRoom);

            DicNewGameServerScript.Add(CreateGameScriptClient.ScriptName, new CreateGameScriptClient(OnlineGameClient, OnlineCommunicationClient, ScreenOwner.ListGameScreen, MissionRoom));
            DicNewGameServerScript.Add(ChangeRoomExtrasBattleScriptClient.ScriptName, new ChangeRoomExtrasMissionScriptClient(MissionRoom, NewScreen));

            if (HasGame)
            {
                //ScreenOwner.PushScreen(new LoadingScreen(NewFightingZone, OnlineGameClient));
            }

            DicNewGameServerScript.Add(PlayerJoinedScriptClient.ScriptName, new PlayerJoinedScriptClient(NewScreen));
            DicNewGameServerScript.Add(PlayerLeftScriptClient.ScriptName, new PlayerLeftScriptClient(NewRoom, OnlineGameClient, NewScreen));
            DicNewGameServerScript.Add(ChangeLoadoutScriptClient.ScriptName, new ChangeLoadoutScriptClient(NewRoom, NewScreen));
            DicNewGameServerScript.Add(ChangePlayerRolesScriptClient.ScriptName, new ChangePlayerRolesScriptClient(NewRoom, NewScreen));
            DicNewGameServerScript.Add(ChangeTeamScriptClient.ScriptName, new ChangeTeamScriptClient(NewRoom));
            DicNewGameServerScript.Add(ChangeMapScriptClient.ScriptName, new ChangeMapScriptClient(NewRoom, NewScreen));
            DicNewGameServerScript.Add(ChangeRoomSubtypeScriptClient.ScriptName, new ChangeRoomSubtypeScriptClient(NewScreen));

            Host.AddOrReplaceScripts(DicNewGameServerScript);

            if (OnlineCommunicationClient.Host != null)
            {
                OnlineCommunicationClient.Chat.InsertTab(RoomID, "Chat");
                OnlineCommunicationClient.Chat.CloseTab("Global");
                OnlineCommunicationClient.Host.Send(new CreateOrJoinCommunicationGroupScriptClient(RoomID, false));
                OnlineCommunicationClient.Host.Send(new LeaveCommunicationGroupScriptClient("Global"));
            }

            Host.Send(new AskChangeLoadoutScriptClient((BattleMapPlayer)PlayerManager.ListLocalPlayer[0]));

            OnlineGameClient.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            ScreenOwner.PushScreen(NewScreen);
        }

        protected override void Read(OnlineReader Sender)
        {
            RoomID = Sender.ReadString();
            RoomName = Sender.ReadString();
            MapPath = Sender.ReadString();
            RoomType = Sender.ReadString();
            RoomSubtype = Sender.ReadString();
            CurrentDifficulty = Sender.ReadString();

            int ListJoiningPlayerCount = Sender.ReadInt32();
            for (int P = 0; P < ListJoiningPlayerCount; ++P)
            {
                ListJoiningPlayerID.Add(Sender.ReadString());
            }

            RoomData = Sender.ReadByteArray();

            HasGame = Sender.ReadBoolean();
        }
    }
}
