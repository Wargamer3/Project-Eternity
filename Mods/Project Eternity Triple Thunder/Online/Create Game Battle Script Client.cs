using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class CreateGameBattleScriptClient : OnlineScript
    {
        public const string ScriptName = "Create Game";

        private readonly TripleThunderOnlineClient Owner;
        private readonly List<GameScreen> ListGameScreen;
        private readonly BattleRoomInformations Room;

        public CreateGameBattleScriptClient(TripleThunderOnlineClient Owner, List<GameScreen> ListGameScreen, BattleRoomInformations Room)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ListGameScreen = ListGameScreen;
            this.Room = Room;
        }

        public override OnlineScript Copy()
        {
            return new CreateGameBattleScriptClient(Owner, ListGameScreen, Room);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            FightingZone NewGame = new FightingZone(Owner);
            GameRules Rules = new BattleGameRules(Room, NewGame);
            NewGame.Rules = Rules;

            NewGame.ListGameScreen = ListGameScreen;
            NewGame.PushScreen(new LoadingScreen(NewGame, Owner));

            Dictionary<string, OnlineScript> DicNewScript = OnlineHelper.GetTripleThunderScriptsClient(Owner);

            Host.AddOrReplaceScripts(DicNewScript);
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
