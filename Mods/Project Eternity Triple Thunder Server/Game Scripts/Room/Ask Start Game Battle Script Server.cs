﻿using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class AskStartGameBattleScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Start Game";

        private readonly BattleRoomInformations Owner;
        private readonly TripleThunderClientGroup CreatedGroup;
        private readonly GameServer OnlineServer;

        public AskStartGameBattleScriptServer(BattleRoomInformations Owner, TripleThunderClientGroup CreatedGroup, GameServer OnlineServer)
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
            FightingZone NewGame = new FightingZone(Owner.MapPath, Owner.UseTeams, OnlineServer, CreatedGroup);

            GameRules Rules = new BattleGameRules(Owner, NewGame);
            NewGame.Rules = Rules;

            CreatedGroup.SetGame(NewGame);

            for (int P = 0; P < CreatedGroup.Room.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = CreatedGroup.Room.ListOnlinePlayer[P];
                Player ActivePlayer = Owner.ListRoomPlayer[P];
                ActivePlayer.OnlineClient = ActiveOnlinePlayer;

                NewGame.AddLocalCharacter(ActivePlayer);

                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetTripleThunderScriptsServer(CreatedGroup, ActivePlayer);
                ActiveOnlinePlayer.AddOrReplaceScripts(DicNewScript);
            }

            foreach (IOnlineConnection ActiveOnlinePlayer in CreatedGroup.Room.ListUniqueOnlineConnection)
            {
                ActiveOnlinePlayer.Send(new CreateGameScriptServer());
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
