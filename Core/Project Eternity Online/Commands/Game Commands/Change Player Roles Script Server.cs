using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class ChangePlayerRolesScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Player Roles";

        private readonly string PlayerID;
        private readonly List<string> ListRole;

        public ChangePlayerRolesScriptServer(string PlayerID, List<string> ListRole)
            : base(ScriptName)
        {
            this.PlayerID = PlayerID;
            this.ListRole = ListRole;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(PlayerID);
            WriteBuffer.AppendByte((byte)ListRole.Count);

            for (int R = 0; R < ListRole.Count; ++R)
            {
                WriteBuffer.AppendString(ListRole[R]);
            }
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
