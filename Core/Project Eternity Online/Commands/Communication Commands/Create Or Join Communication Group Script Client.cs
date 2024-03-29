﻿using System;

namespace ProjectEternity.Core.Online
{
    public class CreateOrJoinCommunicationGroupScriptClient : OnlineScript
    {
        private readonly string GroupID;
        private readonly bool SaveLogs;

        public CreateOrJoinCommunicationGroupScriptClient(string GroupID, bool SaveLogs)
            : base("Create Communication Group")
        {
            this.GroupID = GroupID;
            this.SaveLogs = SaveLogs;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
            WriteBuffer.AppendBoolean(SaveLogs);
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
