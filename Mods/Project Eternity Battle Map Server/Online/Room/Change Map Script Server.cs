﻿using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class ChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Map";

        private readonly string MapName;
        private readonly string MapType;
        private readonly string MapPath;

        public ChangeMapScriptServer(string MapName, string MapType, string MapPath)
            : base(ScriptName)
        {
            this.MapName = MapName;
            this.MapType = MapType;
            this.MapPath = MapPath;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(MapName);
            WriteBuffer.AppendString(MapType);
            WriteBuffer.AppendString(MapPath);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
