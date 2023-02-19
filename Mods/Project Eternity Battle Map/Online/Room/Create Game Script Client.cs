using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.VisualNovelScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class CreateGameScriptClient : OnlineScript
    {
        public const string ScriptName = "Create Game";

        private readonly BattleMapOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private readonly List<GameScreen> ListGameScreen;
        private readonly RoomInformations Room;

        public CreateGameScriptClient(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, List<GameScreen> ListGameScreen, RoomInformations Room)
            : base(ScriptName)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.ListGameScreen = ListGameScreen;
            this.Room = Room;
        }

        public override OnlineScript Copy()
        {
            return new CreateGameScriptClient(OnlineGameClient, OnlineCommunicationClient, ListGameScreen, Room);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            BattleMap NewMap = BattleMap.DicBattmeMapType[Room.MapModName].GetNewMap(Room.GameInfo, string.Empty);

            NewMap.InitOnlineClient(OnlineGameClient, OnlineCommunicationClient, Room);

            NewMap.ListGameScreen = ListGameScreen;
            NewMap.PushScreen(new LoadingScreen(NewMap, OnlineGameClient));

            Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetBattleMapScriptsClient(OnlineGameClient);

            DicNewScript.Add(OpenMenuScriptClient.ScriptName, new OpenMenuScriptClient(OnlineGameClient, NewMap.GetOnlineActionPanel()));
            DicNewScript.Add(UpdateMenuScriptClient.ScriptName, new UpdateMenuScriptClient(OnlineGameClient));
            DicNewScript.Add(ProceedVisualNovelChoiceScriptClient.ScriptName, new ProceedVisualNovelChoiceScriptClient(NewMap));
            DicNewScript.Add(ConfirmChoiceVisualNovelScriptClient.ScriptName, new ConfirmChoiceVisualNovelScriptClient(NewMap));

            Host.AddOrReplaceScripts(DicNewScript);
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
