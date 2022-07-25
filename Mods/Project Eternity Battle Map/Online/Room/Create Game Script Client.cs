using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class CreateGameScriptClient : OnlineScript
    {
        public const string ScriptName = "Create Game";

        private readonly BattleMapOnlineClient Owner;
        private readonly List<GameScreen> ListGameScreen;
        private readonly PVPRoomInformations Room;

        public CreateGameScriptClient(BattleMapOnlineClient Owner, List<GameScreen> ListGameScreen, PVPRoomInformations Room)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ListGameScreen = ListGameScreen;
            this.Room = Room;
        }

        public override OnlineScript Copy()
        {
            return new CreateGameScriptClient(Owner, ListGameScreen, Room);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            BattleMap NewMap = BattleMap.DicBattmeMapType[Room.MapType].GetNewMap(Room.RoomType, string.Empty);

            NewMap.InitOnlineClient(Owner);

            NewMap.ListGameScreen = ListGameScreen;
            NewMap.PushScreen(new LoadingScreen(NewMap, Owner));

            Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetBattleMapScriptsClient(Owner);

            DicNewScript.Add(OpenMenuScriptClient.ScriptName, new OpenMenuScriptClient(Owner, NewMap.GetOnlineActionPanel()));

            Host.AddOrReplaceScripts(DicNewScript);
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
