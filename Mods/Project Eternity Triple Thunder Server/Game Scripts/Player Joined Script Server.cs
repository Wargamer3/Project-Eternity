using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class PlayerJoinedScriptServer : OnlineScript
    {
        public const string ScriptName = "Player Joined";

        private readonly List<Player> ListJoiningPlayerInfo;

        public PlayerJoinedScriptServer(List<Player> ListJoiningPlayerInfo)
            : base(ScriptName)
        {
            this.ListJoiningPlayerInfo = ListJoiningPlayerInfo;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ListJoiningPlayerInfo.Count);
            foreach (Player JoiningPlayer in ListJoiningPlayerInfo)
            {
                WriteBuffer.AppendString(JoiningPlayer.ConnectionID);
                WriteBuffer.AppendString(JoiningPlayer.Name);
                WriteBuffer.AppendString(JoiningPlayer.Equipment.CharacterType);
            }
        }

        protected override void Execute(IOnlineConnection ActivePlayer)
        {
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
