using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class JoinRoomLocalScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Join Room Local";

        private readonly TripleThunderOnlineClient OnlineGameClient;
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
        private List<Player> ListPlayer;
        private GameScreen NewScreen;

        public JoinRoomLocalScriptClient(TripleThunderOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, GameScreen ScreenOwner, bool RemoveOwner = false)
            : base(ScriptName)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.ScreenOwner = ScreenOwner;
            this.RemoveOwner = RemoveOwner;

            HasGame = false;
            ListJoiningPlayerID = new List<string>();
            ListPlayer = new List<Player>();
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

            Dictionary<string, OnlineScript> DicNewGameServerScript = new Dictionary<string, OnlineScript>();
            IMissionSelect NewMissionSelectScreen;
            RoomInformations NewRoom;
            FightingZone NewFightingZone = null;

            if (HasGame)
            {
                NewFightingZone = new FightingZone(OnlineGameClient);
                NewFightingZone.ListGameScreen = ScreenOwner.ListGameScreen;

                DicNewGameServerScript = OnlineHelper.GetTripleThunderScriptsClient(OnlineGameClient);
                Host.IsGameReady = true;
            }
            if (RoomType == RoomInformations.RoomTypeMission)
            {
                MissionRoomInformations MissionRoom = new MissionRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapPath, ListJoiningPlayerID, RoomData);
                if (HasGame)
                {
                    NewFightingZone.Rules = new MissionGameRules(MissionRoom, NewFightingZone);
                }

                NewRoom = MissionRoom;

                MissionSelect NewMissionSelect = new MissionSelect(OnlineGameClient, OnlineCommunicationClient, MissionRoom);
                NewScreen = NewMissionSelect;
                NewMissionSelectScreen = NewMissionSelect;

                DicNewGameServerScript.Add(CreateGameMissionScriptClient.ScriptName, new CreateGameMissionScriptClient(OnlineGameClient, ScreenOwner.ListGameScreen, MissionRoom));
                DicNewGameServerScript.Add(ChangeRoomExtrasBattleScriptClient.ScriptName, new ChangeRoomExtrasMissionScriptClient(MissionRoom, NewMissionSelect));
            }
            else
            {
                BattleRoomInformations BattleRoom = new BattleRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapPath, ListJoiningPlayerID, RoomData);
                if (HasGame)
                {
                    NewFightingZone.Rules = new BattleGameRules(BattleRoom, NewFightingZone);
                }

                NewRoom = BattleRoom;

                BattleSelect NewBattleSelect = new BattleSelect(OnlineGameClient, OnlineCommunicationClient, BattleRoom);
                NewScreen = NewBattleSelect;
                NewMissionSelectScreen = NewBattleSelect;

                DicNewGameServerScript.Add(CreateGameMissionScriptClient.ScriptName, new CreateGameBattleScriptClient(OnlineGameClient, ScreenOwner.ListGameScreen, BattleRoom));
                DicNewGameServerScript.Add(ChangeRoomExtrasBattleScriptClient.ScriptName, new ChangeRoomExtrasBattleScriptClient(BattleRoom, NewBattleSelect));
            }

            if (HasGame)
            {
                ScreenOwner.PushScreen(new LoadingScreen(NewFightingZone, OnlineGameClient));
            }

            DicNewGameServerScript.Add(PlayerJoinedScriptClient.ScriptName, new PlayerJoinedScriptClient(NewMissionSelectScreen));
            DicNewGameServerScript.Add(PlayerLeftScriptClient.ScriptName, new PlayerLeftScriptClient(NewRoom, OnlineGameClient, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangeCharacterScriptClient.ScriptName, new ChangeCharacterScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangePlayerTypeScriptClient.ScriptName, new ChangePlayerTypeScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangeTeamScriptClient.ScriptName, new ChangeTeamScriptClient(NewRoom));
            DicNewGameServerScript.Add(ChangeMapScriptClient.ScriptName, new ChangeMapScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewGameServerScript.Add(ChangeRoomSubtypeScriptClient.ScriptName, new ChangeRoomSubtypeScriptClient(NewMissionSelectScreen));

            Host.AddOrReplaceScripts(DicNewGameServerScript);

            OnlineCommunicationClient.Chat.InsertTab(RoomID, "Chat");
            OnlineCommunicationClient.Chat.CloseTab("Global");
            OnlineCommunicationClient.Host.Send(new CreateOrJoinCommunicationGroupScriptClient(RoomID));
            OnlineCommunicationClient.Host.Send(new LeaveCommunicationGroupScriptClient("Global"));

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
