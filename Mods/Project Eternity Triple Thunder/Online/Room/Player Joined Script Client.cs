using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class PlayerJoinedScriptClient : OnlineScript
    {
        public const string ScriptName = "Player Joined";

        private readonly IMissionSelect NewMissionSelectScreen;

        private List<Player> ListJoiningPlayer;

        public PlayerJoinedScriptClient(IMissionSelect NewMissionSelectScreen)
            : base(ScriptName)
        {
            this.NewMissionSelectScreen = NewMissionSelectScreen;
            ListJoiningPlayer = new List<Player>();
        }

        public override OnlineScript Copy()
        {
            return new PlayerJoinedScriptClient(NewMissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            foreach (Player JoiningPlayer in ListJoiningPlayer)
            {
                NewMissionSelectScreen.AddPlayer(JoiningPlayer);
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListJoiningPlayerInfoCount = Sender.ReadInt32();

            for (int P = 0; P < ListJoiningPlayerInfoCount; P++)
            {
                Player JoiningPlayer = new Player(Sender.ReadString(), Sender.ReadString(), Player.PlayerTypes.Player, true, 0);
                JoiningPlayer.Equipment.CharacterType = Sender.ReadString();
                ListJoiningPlayer.Add(JoiningPlayer);
            }
        }
    }
}
