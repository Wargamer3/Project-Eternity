﻿using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class AskChangeMapScriptClient : OnlineScript
    {
        private readonly string CurrentDifficulty;
        private readonly string NewMapName;

        public AskChangeMapScriptClient(string CurrentDifficulty, string NewMapName)
            : base("Ask Change Map")
        {
            this.CurrentDifficulty = CurrentDifficulty;
            this.NewMapName = NewMapName;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(CurrentDifficulty);
            WriteBuffer.AppendString(NewMapName);
        }

        protected override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
