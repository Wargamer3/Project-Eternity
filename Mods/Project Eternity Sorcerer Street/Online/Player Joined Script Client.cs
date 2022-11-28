using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Online
{
    public class PlayerJoinedScriptClient : OnlineScript
    {
        public const string ScriptName = "Player Joined";

        private readonly GamePreparationScreen PreparationScreen;

        private List<Player> ListJoiningPlayer;

        public PlayerJoinedScriptClient(GamePreparationScreen NewMissionSelectScreen)
            : base(ScriptName)
        {
            this.PreparationScreen = NewMissionSelectScreen;
            ListJoiningPlayer = new List<Player>();
        }

        public override OnlineScript Copy()
        {
            return new PlayerJoinedScriptClient(PreparationScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            foreach (Player JoiningPlayer in ListJoiningPlayer)
            {
                PreparationScreen.AddPlayer(JoiningPlayer);
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListJoiningPlayerInfoCount = Sender.ReadInt32();

            for (int P = 0; P < ListJoiningPlayerInfoCount; P++)
            {
                Player JoiningPlayer = new Player(Sender.ReadString(), Sender.ReadString(), OnlinePlayerBase.PlayerTypes.Player, true, 0, true, Color.Blue);

                ListJoiningPlayer.Add(JoiningPlayer);
            }
        }
    }
}
