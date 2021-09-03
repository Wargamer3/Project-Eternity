using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class SendRoomIDScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Send Room ID";

        private readonly TripleThunderOnlineClient Owner;
        private readonly GameScreen ScreenOwner;
        private readonly string RoomName;
        private readonly string RoomType;
        private readonly string RoomSubtype;
        private readonly int MaxNumberOfPlayer;

        private string RoomID;
        private GameScreen NewScreen;

        public SendRoomIDScriptClient(TripleThunderOnlineClient Owner, GameScreen ScreenOwner, string RoomName, string RoomType, string RoomSubtype, int MaxNumberOfPlayer)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ScreenOwner = ScreenOwner;
            this.RoomName = RoomName;
            this.RoomType = RoomType;
            this.RoomSubtype = RoomSubtype;
            this.MaxNumberOfPlayer = MaxNumberOfPlayer;
        }

        public override OnlineScript Copy()
        {
            return new SendRoomIDScriptClient(Owner, ScreenOwner, RoomName, RoomType, RoomSubtype, MaxNumberOfPlayer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.RoomID = RoomID;
            RoomInformations NewRoom;

            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();
            IMissionSelect NewMissionSelectScreen;

            if (RoomType == RoomInformations.RoomTypeMission)
            {
                MissionRoomInformations MissionRoom = new MissionRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, MaxNumberOfPlayer);
                NewRoom = MissionRoom;

                MissionSelect NewMissionSelect = new MissionSelect(Owner, MissionRoom);
                NewScreen = NewMissionSelect;
                NewMissionSelectScreen = NewMissionSelect;

                DicNewScript.Add(CreateGameMissionScriptClient.ScriptName, new CreateGameMissionScriptClient(Owner, ScreenOwner.ListGameScreen, MissionRoom));
                DicNewScript.Add(ChangeRoomExtrasMissionScriptClient.ScriptName, new ChangeRoomExtrasMissionScriptClient(MissionRoom, NewMissionSelectScreen));
            }
            else
            {
                BattleRoomInformations BattleRoom = new BattleRoomInformations(RoomID, RoomName, RoomType, RoomSubtype, MaxNumberOfPlayer);
                NewRoom = BattleRoom;

                BattleSelect NewBattleSelect = new BattleSelect(Owner, BattleRoom);
                NewScreen = NewBattleSelect;
                NewMissionSelectScreen = NewBattleSelect;

                DicNewScript.Add(CreateGameMissionScriptClient.ScriptName, new CreateGameBattleScriptClient(Owner, ScreenOwner.ListGameScreen, BattleRoom));
                DicNewScript.Add(ChangeRoomExtrasBattleScriptClient.ScriptName, new ChangeRoomExtrasBattleScriptClient(BattleRoom, NewMissionSelectScreen));
            }

            DicNewScript.Add(PlayerJoinedScriptClient.ScriptName, new PlayerJoinedScriptClient(NewMissionSelectScreen));
            DicNewScript.Add(PlayerLeftScriptClient.ScriptName, new PlayerLeftScriptClient(NewRoom, Owner, NewMissionSelectScreen));
            DicNewScript.Add(ChangeCharacterScriptClient.ScriptName, new ChangeCharacterScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewScript.Add(ChangePlayerTypeScriptClient.ScriptName, new ChangePlayerTypeScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewScript.Add(ChangeTeamScriptClient.ScriptName, new ChangeTeamScriptClient(NewRoom));
            DicNewScript.Add(ChangeMapScriptClient.ScriptName, new ChangeMapScriptClient(NewRoom, NewMissionSelectScreen));
            DicNewScript.Add(ChangeRoomSubtypeScriptClient.ScriptName, new ChangeRoomSubtypeScriptClient(NewMissionSelectScreen));

            Host.AddOrReplaceScripts(DicNewScript);

            ScreenOwner.RemoveScreen(ScreenOwner);

            Owner.DelayOnlineScript(this);
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
