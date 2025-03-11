using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Online
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
        private GamePreparationScreen NewPreparationScreen;

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

            SorcererStreetRoomInformations NewRoomInformation = new SorcererStreetRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapPath, ListJoiningPlayerID, ScreenOwner.Content, RoomData);

            NewRoom = NewRoomInformation;

            NewPreparationScreen = new SorcererStreetGamePreparationScreen(OnlineGameClient, OnlineCommunicationClient, NewRoomInformation);

            DicNewGameServerScript.Add(CreateGameScriptClient.ScriptName, new CreateGameScriptClient(OnlineGameClient, OnlineCommunicationClient, ScreenOwner.ListGameScreen, NewRoomInformation));
            DicNewGameServerScript.Add(ChangeRoomExtrasBattleScriptClient.ScriptName, new ChangeRoomExtrasBattleScriptClient(NewRoomInformation, NewPreparationScreen));

            if (HasGame)
            {
                //ScreenOwner.PushScreen(new LoadingScreen(NewFightingZone, OnlineGameClient));
            }

            DicNewGameServerScript.Add(PlayerJoinedScriptClient.ScriptName, new PlayerJoinedScriptClient(NewPreparationScreen));
            DicNewGameServerScript.Add(PlayerLeftScriptClient.ScriptName, new PlayerLeftScriptClient(NewRoom, OnlineGameClient, NewPreparationScreen));
            DicNewGameServerScript.Add(ChangeLoadoutScriptClient.ScriptName, new ChangeLoadoutScriptClient(NewRoom, NewPreparationScreen));
            DicNewGameServerScript.Add(ChangePlayerRolesScriptClient.ScriptName, new ChangePlayerRolesScriptClient(NewRoom, NewPreparationScreen));
            DicNewGameServerScript.Add(ChangeTeamScriptClient.ScriptName, new ChangeTeamScriptClient(NewRoom));
            DicNewGameServerScript.Add(ChangeMapScriptClient.ScriptName, new ChangeMapScriptClient(NewRoom, NewPreparationScreen));
            DicNewGameServerScript.Add(ChangeRoomSubtypeScriptClient.ScriptName, new ChangeRoomSubtypeScriptClient(NewPreparationScreen));

            Host.AddOrReplaceScripts(DicNewGameServerScript);

            if (OnlineCommunicationClient.Host != null)
            {
                OnlineCommunicationClient.Chat.InsertTab(RoomID, "Chat");
                OnlineCommunicationClient.Chat.CloseTab("Global");
                OnlineCommunicationClient.Host.Send(new CreateOrJoinCommunicationGroupScriptClient(RoomID, false));
                OnlineCommunicationClient.Host.Send(new LeaveCommunicationGroupScriptClient("Global"));
            }

            //Send selected book to server
            foreach (Player ActivePlayer in PlayerManager.ListLocalPlayer)
            {
                Host.Send(new AskChangBookScriptClient(ActivePlayer.ConnectionID, ActivePlayer.Inventory.Character.Character.CharacterPath, ActivePlayer.Inventory.ActiveBook));
            }

            OnlineGameClient.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            ScreenOwner.PushScreen(NewPreparationScreen);
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
