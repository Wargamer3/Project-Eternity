using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskStartGameBattleScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Start Game";

        private readonly PVPRoomInformations Owner;
        private readonly BattleMapClientGroup CreatedGroup;
        private readonly GameServer OnlineServer;

        public AskStartGameBattleScriptServer(PVPRoomInformations Owner, BattleMapClientGroup CreatedGroup, GameServer OnlineServer)
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
            Dictionary<string, List<Squad>> DicSpawnSquadByPlayer = new Dictionary<string, List<Squad>>();
            for (int P = 0; P < Owner.ListRoomPlayer.Count; ++P)
            {
                DicSpawnSquadByPlayer.Add(Owner.ListRoomPlayer[P].Name, Owner.ListRoomPlayer[P].ListSquadToSpawn);
            }

            BattleMap NewMap;

            if (CreatedGroup.Room.MapPath == "Random")
            {
                NewMap = BattleMap.DicBattmeMapType[Owner.MapType].GetNewMap(Owner.MapPath, 1, DicSpawnSquadByPlayer);
            }
            else
            {
                NewMap = BattleMap.DicBattmeMapType[Owner.MapType].GetNewMap(Owner.MapPath, 1, DicSpawnSquadByPlayer);
            }

            NewMap.InitOnlineServer(OnlineServer, CreatedGroup);
            CreatedGroup.SetGame(NewMap);

            for (int P = 0; P < CreatedGroup.Room.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = CreatedGroup.Room.ListOnlinePlayer[P];
                OnlinePlayer ActivePlayer = Owner.ListRoomPlayer[P];
                ActivePlayer.OnlineClient = ActiveOnlinePlayer;

                //NewGame.AddLocalCharacter(ActivePlayer);

                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetBattleMapScriptsServer(CreatedGroup, ActivePlayer);
                ActiveOnlinePlayer.AddOrReplaceScripts(DicNewScript);
            }

            for (int P = 0; P < CreatedGroup.Room.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = CreatedGroup.Room.ListOnlinePlayer[P];
                ActiveOnlinePlayer.Send(new CreateGameScriptServer());
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
