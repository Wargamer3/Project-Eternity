using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class PlayerJoinedScriptClient : OnlineScript
    {
        public const string ScriptName = "Player Joined";

        private readonly GamePreparationScreen NewMissionSelectScreen;

        private List<BattleMapPlayer> ListJoiningPlayer;

        public PlayerJoinedScriptClient(GamePreparationScreen NewMissionSelectScreen)
            : base(ScriptName)
        {
            this.NewMissionSelectScreen = NewMissionSelectScreen;
            ListJoiningPlayer = new List<BattleMapPlayer>();
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
            foreach (BattleMapPlayer JoiningPlayer in ListJoiningPlayer)
            {
                NewMissionSelectScreen.AddPlayer(JoiningPlayer);
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListJoiningPlayerInfoCount = Sender.ReadInt32();

            for (int P = 0; P < ListJoiningPlayerInfoCount; P++)
            {
                BattleMapPlayer JoiningPlayer = new BattleMapPlayer(Sender.ReadString(), Sender.ReadString(), OnlinePlayerBase.PlayerTypes.Player, true, 0, true, Color.Blue);

                ListJoiningPlayer.Add(JoiningPlayer);
            }
        }
    }
}
