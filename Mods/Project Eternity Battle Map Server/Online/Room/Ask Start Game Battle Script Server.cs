using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using ProjectEternity.GameScreens.VisualNovelScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskStartGameBattleScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Start Game";

        private readonly RoomInformations Owner;
        private readonly BattleMapClientGroup CreatedGroup;
        private readonly GameServer OnlineServer;

        public AskStartGameBattleScriptServer(RoomInformations Owner, BattleMapClientGroup CreatedGroup, GameServer OnlineServer)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.CreatedGroup = CreatedGroup;
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new AskStartGameBattleScriptServer(Owner, CreatedGroup, OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            BattleMap NewMap;

            if (CreatedGroup.Room.MapPath == "Random")
            {
                NewMap = BattleMap.DicBattmeMapType[Owner.MapModName].GetNewMap(Owner.GameInfo, Owner.RoomID);
            }
            else
            {
                NewMap = BattleMap.DicBattmeMapType[Owner.MapModName].GetNewMap(Owner.GameInfo, Owner.RoomID);
            }

            NewMap.ListGameScreen = new List<GameScreen>();
            NewMap.BattleMapPath = Owner.MapPath;
            NewMap.InitOnlineServer(OnlineServer, CreatedGroup);
            CreatedGroup.SetGame(NewMap);

            for (int P = 0; P < CreatedGroup.Room.ListUniqueOnlineConnection.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = CreatedGroup.Room.ListUniqueOnlineConnection[P];
                OnlinePlayerBase ActivePlayer = Owner.ListRoomPlayer[P];
                ActivePlayer.OnlineClient = ActiveOnlinePlayer;

                NewMap.AddLocalPlayer(ActivePlayer);

                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetBattleMapScriptsServer(CreatedGroup);
                DicNewScript.Add(ProceedVisualNovelChoiceScriptServer.ScriptName, new ProceedVisualNovelChoiceScriptServer(NewMap, CreatedGroup));
                DicNewScript.Add(ConfirmChoiceVisualNovelScriptServer.ScriptName, new ConfirmChoiceVisualNovelScriptServer(NewMap, CreatedGroup));
                DicNewScript.Add(MoveUnitScriptServer.ScriptName, new MoveUnitScriptServer(CreatedGroup));
                ActiveOnlinePlayer.AddOrReplaceScripts(DicNewScript);
            }

            for (int P = 0; P < CreatedGroup.Room.ListUniqueOnlineConnection.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = CreatedGroup.Room.ListUniqueOnlineConnection[P];
                ActiveOnlinePlayer.Send(new CreateGameScriptServer());
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
