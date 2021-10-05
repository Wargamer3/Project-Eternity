using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

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
            Dictionary<string, List<Squad>> DicSpawnSquadByPlayer = new Dictionary<string, List<Squad>>();
            for (int P = 0; P < Room.ListRoomPlayer.Count; ++P)
            {
                DicSpawnSquadByPlayer.Add(Room.ListRoomPlayer[P].Name, Room.ListRoomPlayer[P].ListSquadToSpawn);
            }

            BattleMap NewMap;

            if (Room.MapPath == "Random")
            {
                NewMap = BattleMap.DicBattmeMapType[Room.MapType].GetNewMap(Room.MapPath, 1, DicSpawnSquadByPlayer);
            }
            else
            {
                NewMap = BattleMap.DicBattmeMapType[Room.MapType].GetNewMap(Room.MapPath, 1, DicSpawnSquadByPlayer);
            }

            NewMap.InitOnlineClient(Owner);

            NewMap.ListGameScreen = ListGameScreen;
            NewMap.PushScreen(new LoadingScreen(NewMap, Owner));

            Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetTripleThunderScriptsClient(Owner);

            Host.AddOrReplaceScripts(DicNewScript);
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
