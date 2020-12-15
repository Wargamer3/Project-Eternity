﻿using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class CreateGameMissionScriptClient : OnlineScript
    {
        public const string ScriptName = "Create Game";

        private readonly TripleThunderOnlineClient Owner;
        private readonly List<GameScreen> ListGameScreen;
        private readonly MissionRoomInformations Room;

        public CreateGameMissionScriptClient(TripleThunderOnlineClient Owner, List<GameScreen> ListGameScreen, MissionRoomInformations Room)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ListGameScreen = ListGameScreen;
            this.Room = Room;
        }

        public override OnlineScript Copy()
        {
            return new CreateGameMissionScriptClient(Owner, ListGameScreen, Room);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            FightingZone NewGame = new FightingZone(Owner);
            GameRules Rules = new MissionGameRules(Room, NewGame);
            NewGame.Rules = Rules;

            NewGame.ListGameScreen = ListGameScreen;
            NewGame.PushScreen(new LoadingScreen(NewGame, Owner));

            Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();
            DicNewScript.Add(SendPlayerUpdateScriptClient.ScriptName, new SendPlayerUpdateScriptClient(Owner));
            DicNewScript.Add(SendPlayerRespawnScriptClient.ScriptName, new SendPlayerRespawnScriptClient(Owner));
            DicNewScript.Add(SendPlayerDamageScriptClient.ScriptName, new SendPlayerDamageScriptClient(Owner));
            DicNewScript.Add(ShootBulletScriptClient.ScriptName, new ShootBulletScriptClient(Owner));
            DicNewScript.Add(ReceiveGameDataScriptClient.ScriptName, new ReceiveGameDataScriptClient(Owner));
            DicNewScript.Add(GoToNextMapScriptClient.ScriptName, new GoToNextMapScriptClient(Owner));
            DicNewScript.Add(GameEndedScriptClient.ScriptName, new GameEndedScriptClient(Owner));

            Host.AddOrReplaceScripts(DicNewScript);
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
