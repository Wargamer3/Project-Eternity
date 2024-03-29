﻿using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class PlayerJoinedScriptServer : OnlineScript
    {
        public const string ScriptName = "Player Joined";

        private readonly List<OnlinePlayerBase> ListJoiningPlayerInfo;

        public PlayerJoinedScriptServer(List<OnlinePlayerBase> ListJoiningPlayerInfo)
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
            foreach (BattleMapPlayer JoiningPlayer in ListJoiningPlayerInfo)
            {
                WriteBuffer.AppendString(JoiningPlayer.ConnectionID);
                WriteBuffer.AppendString(JoiningPlayer.Name);
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
