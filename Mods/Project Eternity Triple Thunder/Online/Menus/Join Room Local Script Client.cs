using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class JoinRoomLocalScriptClient : OnlineScript
    {
        public const string ScriptName = "Join Room Local";

        private readonly TripleThunderOnlineClient Owner;
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

        public JoinRoomLocalScriptClient(TripleThunderOnlineClient Owner, GameScreen ScreenOwner, bool RemoveOwner = false)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ScreenOwner = ScreenOwner;
            this.RemoveOwner = RemoveOwner;

            HasGame = false;
            ListJoiningPlayerID = new List<string>();
            ListPlayer = new List<Player>();
        }

        public override OnlineScript Copy()
        {
            return new JoinRoomLocalScriptClient(Owner, ScreenOwner, RemoveOwner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.RoomID = RoomID;

            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();
            IMissionSelect NewMissionSelectScreen;
            RoomInformations NewRoom;
            FightingZone NewFightingZone = null;

            if (HasGame)
            {
                NewFightingZone = new FightingZone(Owner);
                NewFightingZone.ListGameScreen = ScreenOwner.ListGameScreen;

                DicNewScript = OnlineHelper.GetTripleThunderScriptsClient(Owner);
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
                MissionSelect NewScreen = new MissionSelect(Owner, MissionRoom);
                DicNewScript.Add(CreateGameMissionScriptClient.ScriptName, new CreateGameMissionScriptClient(Owner, ScreenOwner.ListGameScreen, MissionRoom));
                DicNewScript.Add(ChangeRoomExtrasBattleScriptClient.ScriptName, new ChangeRoomExtrasMissionScriptClient(MissionRoom, NewScreen));
                NewMissionSelectScreen = NewScreen;
                ScreenOwner.PushScreen(NewScreen);
            }
            else
            {
                BattleRoomInformations BattleRoom = new BattleRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, CurrentDifficulty, MapPath, ListJoiningPlayerID, RoomData);
                if (HasGame)
                {
                    NewFightingZone.Rules = new BattleGameRules(BattleRoom, NewFightingZone);
                }
                NewRoom = BattleRoom;
                BattleSelect NewScreen = new BattleSelect(Owner, BattleRoom);
                DicNewScript.Add(CreateGameMissionScriptClient.ScriptName, new CreateGameBattleScriptClient(Owner, ScreenOwner.ListGameScreen, BattleRoom));
                DicNewScript.Add(ChangeRoomExtrasBattleScriptClient.ScriptName, new ChangeRoomExtrasBattleScriptClient(BattleRoom, NewScreen));
                NewMissionSelectScreen = NewScreen;
                ScreenOwner.PushScreen(NewScreen);
            }

            if (HasGame)
            {
                ScreenOwner.PushScreen(new LoadingScreen(NewFightingZone, Owner));
            }

            DicNewScript.Add(PlayerJoinedScriptClient.ScriptName, new PlayerJoinedScriptClient(NewMissionSelectScreen));
            DicNewScript.Add(PlayerLeftScriptClient.ScriptName, new PlayerLeftScriptClient(NewRoom, Owner, NewMissionSelectScreen));
            DicNewScript.Add(ChangeCharacterScriptClient.ScriptName, new ChangeCharacterScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewScript.Add(ChangePlayerTypeScriptClient.ScriptName, new ChangePlayerTypeScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewScript.Add(ChangeTeamScriptClient.ScriptName, new ChangeTeamScriptClient(NewRoom));
            DicNewScript.Add(ChangeMapScriptClient.ScriptName, new ChangeMapScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewScript.Add(ChangeRoomSubtypeScriptClient.ScriptName, new ChangeRoomSubtypeScriptClient(NewMissionSelectScreen));

            Host.AddOrReplaceScripts(DicNewScript);
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
