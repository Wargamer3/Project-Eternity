using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class AskStartGameMissionScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Start Game";

        private readonly MissionRoomInformations Owner;
        private readonly TripleThunderClientGroup CreatedGroup;
        private readonly Server OnlineServer;

        public AskStartGameMissionScriptServer(MissionRoomInformations Owner, TripleThunderClientGroup CreatedGroup, Server OnlineServer)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.CreatedGroup = CreatedGroup;
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new AskStartGameMissionScriptServer(Owner, CreatedGroup, OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            FightingZone NewGame = new FightingZone(Owner.MapPath, Owner.UseTeams, OnlineServer, CreatedGroup);

            GameRules Rules = new MissionGameRules(Owner, NewGame);
            NewGame.Rules = Rules;

            CreatedGroup.SetGame(NewGame);

            for (int P = 0; P < CreatedGroup.Room.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = CreatedGroup.Room.ListOnlinePlayer[P];
                Player ActivePlayer = Owner.ListRoomPlayer[P];
                ActivePlayer.OnlineClient = ActiveOnlinePlayer;

                NewGame.AddLocalCharacter(ActivePlayer);

                //Add Game Specific scripts
                Dictionary<string, OnlineScript> DicNewScript = new Dictionary<string, OnlineScript>();
                DicNewScript.Add(FinishedLoadingScriptServer.ScriptName, new FinishedLoadingScriptServer(CreatedGroup));
                DicNewScript.Add(SendPlayerUpdateScriptServer.ScriptName, new SendPlayerUpdateScriptServer(CreatedGroup, ActivePlayer));
                DicNewScript.Add(ShootBulletScriptServer.ScriptName, new ShootBulletScriptServer(CreatedGroup, ActivePlayer));
                DicNewScript.Add(AskTripleThunderGameDataScriptServer.ScriptName, new AskTripleThunderGameDataScriptServer(CreatedGroup));
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
